{{/*
Environment variables that expose the MSSQL connection details from the
rds-mssql-instance-output namespace secret, plus the composed connection string.
Used by the migrator and seeder hook Jobs.
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
  value: {{ .Values.connectionStrings.catsDb | quote }}
{{- end -}}

{{/*
Renders secretKeyRef env entries from a namespace_secrets map, mirroring the
generic-service chart's own `namespace_secrets` convention (secretName -> {envVar: secretKey}),
so bespoke templates (e.g. worker.yaml) can reuse the same values shape/pattern as `generic-service:`.
Pass the map itself, e.g. {{ include "cats.namespaceSecretsEnv" .Values.worker.namespace_secrets }}
*/}}
{{- define "cats.namespaceSecretsEnv" -}}
{{- range $secret, $envs := . }}
{{- range $key, $val := $envs }}
- name: {{ $key }}
  valueFrom:
    secretKeyRef:
      name: {{ $secret }}
      key: {{ trimSuffix "?" $val }}
      {{- if hasSuffix "?" $val }}
      optional: true
      {{- end }}
{{- end }}
{{- end }}
{{- end -}}
