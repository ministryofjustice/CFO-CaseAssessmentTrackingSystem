# CATS Helm deployment

A **single Helm release** deploys the entire Case Assessment and Tracking System (CATS)
to the MoJ Cloud Platform. There is one templating mechanism at the deploy layer — Helm —
so there is no `envsubst` or `kubectl apply` of raw manifests in the pipeline.

## What the release contains

| Resource                      | Source                                            |
|-------------------------------|---------------------------------------------------|
| Web tier (Blazor Server UI)   | `generic-service` dependency, alias `app`         |
| Worker (Quartz jobs)          | `generic-service` dependency, alias `worker`      |
| RabbitMQ (ephemeral)          | local template `templates/rabbitmq.yaml`          |
| Redis (ephemeral backplane)   | local template `templates/redis.yaml`             |
| DB migrator (run-once)        | `templates/migrator-job.yaml` — pre-upgrade hook  |
| DB seeder (run-once)          | `templates/seeder-job.yaml` — pre-upgrade hook    |
| Prometheus alerts             | `generic-prometheus-alerts` dependency            |

The migrator (hook-weight `-5`) and seeder (hook-weight `0`) run as **`pre-install`/
`pre-upgrade` Helm hooks**: Helm runs them in weight order and waits for each to succeed
before rolling out the app — so a failed migration fails the deploy. The web tier reaches
the worker at `http://cats-worker:8080` and the in-cluster broker/cache at
`rabbitmq-service:5672` / `redis-service:6379`.

> RabbitMQ and Redis are **ephemeral** (no persistence): RabbitMQ carries only the
> transient Rebus message flow and Redis is purely a SignalR backplane / Fusion cache.

## Layout

```
helm_deploy/cats/
  Chart.yaml                 # app + worker (aliased generic-service) + alerts
  values.yaml                # shared defaults
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
(`cats-`, `worker-`, `migrator-`, `seeder-`):

```bash
helm dependency build ./helm_deploy/cats

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
  --set migrator.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set migrator.image.tag="migrator-$SHA" \
  --set seeder.image.repository="$REGISTRY/$ECR_REPOSITORY" \
  --set seeder.image.tag="seeder-$SHA" \
  --wait --timeout 10m
```

## Local validation

```bash
helm dependency build ./helm_deploy/cats
helm lint ./helm_deploy/cats -f ./helm_deploy/cats/values-dev.yaml
helm template cats ./helm_deploy/cats \
  --namespace cfocats-dev --values ./helm_deploy/cats/values-dev.yaml
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

Helm then owns `cats`, `cats-worker`, `rabbitmq-*`, `redis-*`, `cats-v1-2` and the hook Jobs.
