# CATS Helm deployment

Helm deploys the entire Case Assessment and Tracking System (CATS) to the MoJ Cloud
Platform from **one chart**. There is one templating mechanism at the deploy layer — Helm —
so there is no `envsubst` or `kubectl apply` of raw manifests in the pipeline.

The CI pipeline installs **three releases from this single chart**, run in order so each
stage has isolated logs and its own timeout. Which slice of the chart renders is chosen by
two `--set` selectors — `application.enabled` (the long-running stack) and `job` (the one-off
Job to run) — so there are no per-mode values files:

| Release        | Contains                                   | Selectors                                  |
|----------------|--------------------------------------------|--------------------------------------------|
| `cats-migrate` | DB migrator Job                            | `application.enabled=false`, `job=migrate` |
| `cats-seed`    | DB seeder Job                              | `application.enabled=false`, `job=seed`    |
| `cats`         | web tier, worker, ephemeral RabbitMQ/Redis | *(defaults: `application.enabled=true`, `job=""`)* |

## What the `cats` release contains

| Resource                      | Source                                            |
|-------------------------------|---------------------------------------------------|
| Web tier (Blazor Server UI)   | `generic-service` dependency, alias `app`         |
| Worker (Quartz jobs)          | `generic-service` dependency, alias `worker`      |
| RabbitMQ (ephemeral)          | local template `templates/rabbitmq.yaml`          |
| Redis (ephemeral backplane)   | local template `templates/redis.yaml`             |
| Prometheus alerts             | `generic-prometheus-alerts` dependency (off by default) |

The web tier reaches the worker at `http://cats-worker:8080` and the in-cluster broker/cache
at `rabbitmq-service:5672` / `redis-service:6379`.

## Migrator / seeder Jobs

The migrator and seeder live in this same chart but render **only when selected**
(`--set job=migrate` / `--set job=seed`); the default `job=""` renders neither. The pipeline
runs one at a time — with `--set application.enabled=false` so the app, worker and ephemeral
deps are skipped — installing each as its own release **before** the `cats` application
release. Each Job is named per release revision (`cats-migrator-<rev>`), so every deploy
runs a fresh Job — Job pod templates are immutable, so a stable name could not be re-applied
— and `helm upgrade --wait` blocks until it completes. A failed migration therefore fails
its own step (with the Job left in place for log inspection) and the app is never rolled out.

> RabbitMQ and Redis are **ephemeral** (no persistence): RabbitMQ carries only the
> transient Rebus message flow and Redis is purely a SignalR backplane / Fusion cache.

> **Prometheus alerts are disabled by default** (`generic-prometheus-alerts.enabled: false`).
> The rules only reach a human once an Alertmanager receiver is configured for the
> `alertSeverity` (a separate `cloud-platform-environments` change). To turn them on,
> set `generic-prometheus-alerts.enabled: true` in the relevant `values-<env>.yaml`.

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
    migrator-job.yaml
    seeder-job.yaml
```

`infra/` now contains only `port-forward-deployment.yml`, a manual developer convenience
for reaching the RDS instance (applied ad hoc, not part of the pipeline).

## Per-deploy values (supplied by CI)

The image registry/tags, app version, and service account are passed at deploy time. Note
the four images share one ECR repository but use different tag prefixes
(`cats-`, `worker-`, `migrator-`, `seeder-`). The pipeline runs three releases in order:

```bash
helm dependency build ./helm_deploy/cats

# 1. Migrate
helm upgrade --install cats-migrate ./helm_deploy/cats \
  --namespace "$KUBE_NAMESPACE" \
  --values ./helm_deploy/cats/values-$ENV.yaml \
  --set application.enabled=false --set job=migrate \
  --set serviceAccountName="$KUBE_NAMESPACE" \
  --set migrator.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set migrator.image.tag="migrator-$SHA" \
  --wait --timeout 5m

# 2. Seed
helm upgrade --install cats-seed ./helm_deploy/cats \
  --namespace "$KUBE_NAMESPACE" \
  --values ./helm_deploy/cats/values-$ENV.yaml \
  --set application.enabled=false --set job=seed \
  --set serviceAccountName="$KUBE_NAMESPACE" \
  --set seeder.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set seeder.image.tag="seeder-$SHA" \
  --wait --timeout 5m

# 3. Deploy the application
helm upgrade --install cats ./helm_deploy/cats \
  --namespace "$KUBE_NAMESPACE" \
  --values ./helm_deploy/cats/values-$ENV.yaml \
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
helm dependency build ./helm_deploy/cats
helm lint ./helm_deploy/cats -f ./helm_deploy/cats/values-dev.yaml
# app release
helm template cats ./helm_deploy/cats \
  --namespace cfocats-dev --values ./helm_deploy/cats/values-dev.yaml
# migrate / seed releases
helm template cats-migrate ./helm_deploy/cats --namespace cfocats-dev \
  -f ./helm_deploy/cats/values-dev.yaml --set application.enabled=false --set job=migrate
helm template cats-seed ./helm_deploy/cats --namespace cfocats-dev \
  -f ./helm_deploy/cats/values-dev.yaml --set application.enabled=false --set job=seed
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
