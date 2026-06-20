{{/*
Service account name. Cloud Platform creates a service account named after the
namespace (with IRSA role bindings for S3/RDS), so default to the release namespace.
*/}}
{{- define "cats.serviceAccountName" -}}
{{- .Values.serviceAccountName | default .Release.Namespace -}}
{{- end -}}

{{/*
Pod-level security context shared by the local (non generic-service) workloads.
*/}}
{{- define "cats.podSecurityContext" -}}
seccompProfile:
  type: RuntimeDefault
runAsUser: 1001
runAsGroup: 1001
runAsNonRoot: true
{{- end -}}

{{/*
Environment variables that expose the MSSQL connection details from the
rds-mssql-instance-output namespace secret, plus the composed connection string.
Used by the migrator and seeder Jobs.
*/}}
{{- define "cats.databaseEnv" -}}
- name: DATABASE_ADDRESS
  valueFrom:
    secretKeyRef:
      name: rds-mssql-instance-output
      key: rds_instance_address
- name: DATABASE_USERNAME
  valueFrom:
    secretKeyRef:
      name: rds-mssql-instance-output
      key: database_username
- name: DATABASE_PASSWORD
  valueFrom:
    secretKeyRef:
      name: rds-mssql-instance-output
      key: database_password
- name: ConnectionStrings__CatsDb
  value: "Server=$(DATABASE_ADDRESS);Database=CatsDb;User Id=$(DATABASE_USERNAME);Password=$(DATABASE_PASSWORD);TrustServerCertificate=True;"
{{- end -}}
