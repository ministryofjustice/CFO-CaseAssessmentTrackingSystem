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
