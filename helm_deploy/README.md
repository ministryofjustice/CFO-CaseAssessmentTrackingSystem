# CATS Helm deployment

Helm deploys the entire Case Assessment and Tracking System (CATS) to the MoJ Cloud
Platform from **one chart**. There is one templating mechanism at the deploy layer — Helm —
so there is no `envsubst` or `kubectl apply` of raw manifests in the pipeline.

The CI pipeline installs **three releases from this single chart**, run in order so each
stage has isolated logs and its own timeout. Every workload is **off by default** and each
release opts in to exactly the components it needs via per-component `--set X.enabled=true`
flags — so there are no per-mode values files:

| Release        | Contains                                   | Enabled components                                       |
|----------------|--------------------------------------------|----------------------------------------------------------|
| `cats-migrate` | DB migrator Job                            | `migrator.enabled=true`                                  |
| `cats-seed`    | DB seeder Job                              | `seeder.enabled=true`                                    |
| `cats`         | web tier, worker, ephemeral RabbitMQ/Redis | `app.enabled`, `worker.enabled`, `rabbitmq.enabled`, `redis.enabled` = `true` |

## What the `cats` release contains

| Resource                      | Source                                            |
|-------------------------------|---------------------------------------------------|
| Web tier (Blazor Server UI)   | `generic-service` dependency, alias `app`         |
| Worker (Quartz jobs)          | `generic-service` dependency, alias `worker`      |
| RabbitMQ (ephemeral)          | local template `templates/rabbitmq.yaml`          |
| Redis (ephemeral backplane)   | local template `templates/redis.yaml`             |

The web tier reaches the worker at `http://cats-worker:8080` and the in-cluster broker/cache
at `rabbitmq-service:5672` / `redis-service:6379`.

## Migrator / seeder Pods

The migrator and seeder live in this same chart but render **only when enabled**
(`--set migrator.enabled=true` / `--set seeder.enabled=true`); by default every component is
off. The pipeline runs one at a time — enabling only that component, so the app, worker and
ephemeral deps are skipped — installing each as its own release **before** the `cats`
application release. Each is named per release revision (`cats-migrator-<rev>`), so every
deploy runs a fresh Pod (a Pod spec is immutable, so a stable name could not be re-applied)
and Helm prunes the previous revision's Pod on upgrade. `restartPolicy: OnFailure` retries a
transient failure in place.

They are plain **Pods**, not Jobs, deliberately. Measured on Cloud Platform, the migrate/seed
*work* takes 2–15s, but a **Job** object takes a further **~70–90s** to be marked `Complete`
after its pod has already reached `Succeeded` — control-plane (kube-controller-manager)
latency that no chart/Job setting can influence. A bare Pod's `Succeeded` phase is set by the
kubelet ~1s after the container exits, so the pipeline waits on that instead.

Because they run to completion, the pipeline does **not** use `helm --wait` (Helm judges a Pod
ready via its `Ready` condition, which is never true for a `Succeeded` pod, so `--wait` would
hang until timeout even on success). Each step runs `helm upgrade` (no wait) then
`kubectl wait --for=jsonpath='{.status.phase}'=Succeeded -l app=<migrator|seeder>`. A failed
migration therefore fails its own step (Pod left in place, with `describe`/`logs` dumped for
inspection) and the app is never rolled out.

> RabbitMQ and Redis are **ephemeral** (no persistence): RabbitMQ carries only the
> transient Rebus message flow and Redis is purely a SignalR backplane / Fusion cache.

> **Prometheus alerts are commented out** for now. The `generic-prometheus-alerts`
> dependency (Chart.yaml) and its values blocks (`values.yaml` + each `values-<env>.yaml`)
> are left in place but commented, because the rules only reach a human once an Alertmanager
> receiver is configured for the `alertSeverity` (a separate `cloud-platform-environments`
> change). To re-enable: uncomment those blocks and run
> `helm dependency update ./helm_deploy/cats` to fetch the new dependency.

## ModSecurity WAF

The web tier's ingress runs behind Cloud Platform's ModSecurity ingress controllers
(OWASP Core Rule Set, anomaly scoring). Config lives entirely in values — the
`generic-service` subchart renders the annotations from `app.ingress`:

| Setting | Where | Value |
| --- | --- | --- |
| `className` | `values.yaml` / `values-production.yaml` | `modsec-non-prod` (dev/staging), `modsec` (production) |
| `modsecurity_enabled` | `values.yaml` | `true` |
| `modsecurity_github_team` | `values.yaml` | `hmpps-creating-future-opportunities-devs` (controls who can read the logs) |
| `modsecurity_mode` | `values.yaml` (+ overlays) | `DetectionOnly` (current) → `On` (block) |

`modsecurity_mode` is a local knob fed into the snippet's `SecRuleEngine`. The snippet
also pins Paranoia Level 1 and tags every event with the GitHub team + namespace so the
logs are reachable in OpenSearch (`live_kubernetes_ingress*`, search `ModSecurity`).

> **Class default is non-prod by design.** The subchart hardcodes the *production*
> `modsec` class whenever `modsecurity_enabled` is true and no `className` is set, so the
> base `values.yaml` pins `modsec-non-prod` and only `values-production.yaml` overrides it
> to `modsec`.

### Phased rollout (monitor → block)

Everything currently runs in **`DetectionOnly`** (logs, never blocks). To promote:

1. Exercise CATS in dev/staging — uploads, long forms, SignalR/WebSocket sessions.
2. Review WAF hits in OpenSearch; add `SecRuleRemoveById <id>` lines to the snippet for
   any false positives.
3. Flip dev/staging to blocking: set `modsecurity_mode: On` in `values-dev.yaml` /
   `values-staging.yaml`.
4. Once staging is clean, set `modsecurity_mode: On` in `values-production.yaml`.

## Layout

```
helm_deploy/cats/
  Chart.yaml                 # app + worker (aliased generic-service) + alerts
  values.yaml                # shared defaults (Jobs disabled)
  values-dev.yaml            # namespace: cfocats-dev
  values-staging.yaml
  values-production.yaml
  templates/
    _helpers.tpl
    rabbitmq.yaml
    redis.yaml
    migrator-pod.yaml
    seeder-pod.yaml
```

`infra/` now contains only `port-forward-deployment.yml`, a manual developer convenience
for reaching the RDS instance (applied ad hoc, not part of the pipeline).

## Per-deploy values (supplied by CI)

The image registry/tags, app version, and service account are passed at deploy time. Note
the four images share one ECR repository but use different tag prefixes
(`cats-`, `worker-`, `migrator-`, `seeder-`). The pipeline runs three releases in order:

```bash
helm dependency update ./helm_deploy/cats

# 1. Migrate
helm upgrade --install cats-migrate ./helm_deploy/cats \
  --namespace "$KUBE_NAMESPACE" \
  --values ./helm_deploy/cats/values-$ENV.yaml \
  --set migrator.enabled=true \
  --set serviceAccountName="$KUBE_NAMESPACE" \
  --set migrator.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set migrator.image.tag="migrator-$SHA" \
  --timeout 5m
kubectl -n "$KUBE_NAMESPACE" wait --for=jsonpath='{.status.phase}'=Succeeded --timeout=300s pod -l app=migrator

# 2. Seed
helm upgrade --install cats-seed ./helm_deploy/cats \
  --namespace "$KUBE_NAMESPACE" \
  --values ./helm_deploy/cats/values-$ENV.yaml \
  --set seeder.enabled=true \
  --set serviceAccountName="$KUBE_NAMESPACE" \
  --set seeder.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set seeder.image.tag="seeder-$SHA" \
  --timeout 5m
kubectl -n "$KUBE_NAMESPACE" wait --for=jsonpath='{.status.phase}'=Succeeded --timeout=300s pod -l app=seeder

# 3. Deploy the application
helm upgrade --install cats ./helm_deploy/cats \
  --namespace "$KUBE_NAMESPACE" \
  --values ./helm_deploy/cats/values-$ENV.yaml \
  --set app.enabled=true --set worker.enabled=true \
  --set rabbitmq.enabled=true --set redis.enabled=true \
  --set serviceAccountName="$KUBE_NAMESPACE" \
  --set app.serviceAccountName="$KUBE_NAMESPACE" \
  --set app.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set app.image.tag="cats-$SHA" \
  --set app.env.Sentry__Release="$APP_VERSION" \
  --set app.env.AppConfigurationSettings__Version="$APP_VERSION" \
  --set worker.serviceAccountName="$KUBE_NAMESPACE" \
  --set worker.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set worker.image.tag="worker-$SHA" \
  --set worker.env.Sentry__Release="$APP_VERSION" \
  --set worker.env.AppConfigurationSettings__Version="$APP_VERSION" \
  --atomic --wait --timeout 10m
```

## Local validation

```bash
helm dependency update ./helm_deploy/cats
helm lint ./helm_deploy/cats -f ./helm_deploy/cats/values-dev.yaml
# app release
helm template cats ./helm_deploy/cats \
  --namespace cfocats-dev --values ./helm_deploy/cats/values-dev.yaml \
  --set app.enabled=true --set worker.enabled=true \
  --set rabbitmq.enabled=true --set redis.enabled=true
# migrate / seed releases
helm template cats-migrate ./helm_deploy/cats --namespace cfocats-dev \
  -f ./helm_deploy/cats/values-dev.yaml --set migrator.enabled=true
helm template cats-seed ./helm_deploy/cats --namespace cfocats-dev \
  -f ./helm_deploy/cats/values-dev.yaml --set seeder.enabled=true
```

## First-time migration from the previous (kubectl) deploy

The previous pipeline created differently-named objects. On the **first** Helm deploy to a
namespace that already ran the old pipeline, delete the legacy resources once so they don't
collide (notably the old ingress vs the new `cats-v1-2` ingress on the same host):

```bash
kubectl -n <ns> delete deploy cats-deployment cats-worker-deployment \
  rabbitmq-deployment redis-deployment
kubectl -n <ns> delete svc cats-service cats-worker-service \
  rabbitmq-service redis-service
kubectl -n <ns> delete ingress cats-ingress
```

Helm then owns `cats`, `cats-worker`, `rabbitmq-*`, `redis-*` and `cats-v1-2` in the `cats`
release, with the migrator/seeder Jobs owned by the separate `cats-migrate` / `cats-seed`
releases.
