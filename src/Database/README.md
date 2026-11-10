# CATS Database Repository

This repository contains the database definition, deployment tooling, and supporting utilities for the **CATS database** using a modern **.NET SQL Project (DbProj, .NET 10)** approach.

---

## 📦 Repository Structure

```
/ (root)
│
├── /src/Database/CatsDb      # SQL Project (database schema)
├── /scripts/migrate-database.cs  # Schema deploy tool — .NET file-based app (deploys the DACPAC via DacFx)
├── /src/DatabaseSeeding      # Data seeding project
```

---

## 🧱 SQL Project (CatsDb)

### What is a SQL Project?

The SQL Project is a **schema-first database project** built using the .NET SQL SDK. It represents the full structure of the CATS database as source-controlled code.

This includes:

- Tables
- Views
- Stored Procedures
- Functions
- Schemas
- Constraints & indexes
- Security objects

### Key Concepts

- Declarative model – define the desired state, not the steps
- Source-controlled schema
- Repeatable builds
- Drift detection

---

## 🚀 Deployment Process (DacFx)

The SQL project builds into a `.dacpac` file:

```
CatsDb.dacpac
```

### Build

```
dotnet build src/Database/CatsDb/CatsDb.sqlproj
```

---

### Deploy

At release time the **migrate-database** app (`scripts/migrate-database.cs`, a .NET 10
file-based app) publishes the DACPAC to the target database using **DacFx**
(`DacServices.Deploy` — the same engine `sqlpackage` wraps). It runs automatically as a
pre-install/pre-upgrade Helm hook Job from the shared `hmpps-cfo-cats` image (see
`helm_deploy/cats/templates/migrator-hook.yaml`), reading the connection string from the
`ConnectionStrings__CatsDb` environment variable and loading `CatsDb.dacpac` from its own
directory (the image build copies the compiled DACPAC there).

During development you can run it straight from source with `dotnet run`. Point `DACPAC_PATH`
at a freshly built DACPAC (it otherwise looks next to the app):

```
dotnet build src/Database/CatsDb/CatsDb.sqlproj -c Release
ConnectionStrings__CatsDb="<connection-string>" \
  DACPAC_PATH=src/Database/CatsDb/bin/Release/CatsDb.dacpac \
  dotnet run scripts/migrate-database.cs
```

### How it Works

- Compares `.dacpac` with target database
- Generates deployment plan
- Applies only schema differences (`BlockOnPossibleDataLoss` disabled, matching the previous sqlpackage settings)

---

### Notes

- Avoid manual DB changes (they can be overwritten)
- Use PRs for all schema updates

### Account Deactivation Warnings

Ensure both the warning job and the outbox publisher are enabled in the deployment.

Warnings are queued once per user and last-login timestamp (or creation timestamp
for users who have never logged in). Active accounts inactive for at least 23
calendar days remain eligible until queued, including accounts overdue for deactivation.
Each warning uses a deterministic outbox ID based on the user and activity timestamp.
Retain these outbox rows to preserve deduplication; deleting them allows warnings
to be queued again. Failed warning publishes remain pending for the next publisher
run, with fresh messages prioritized over failures. Email failures use the existing
Rebus retry policy and error queue. Replay exhausted messages through the existing
error-queue process rather than deleting outbox records.

Delivery is at least once: an uncertain send outcome can produce a duplicate email.
There is no historical delivery backfill, so the first run can warn an eligible
account that received a warning before tracking was introduced. This does not
change the 30-day deactivation threshold or grant a new seven-day grace period.

---

## 🌱 Database Seeding Project

### Location

```
/src/DatabaseSeeding
```

### Purpose

This project handles **data**, not schema:

- Test data setup
- Data migrations
- Backfilling and transformation

### Why Separate?

The SQL Project is schema-only. This project handles:

- Data insertion
- Data transformation

Keeping them separate ensures clean deployments and clear responsibility.

---

### Execution

```
dotnet run --project DatabaseSeeding
```

---

### Best Practices

- Make scripts idempotent
- Keep migrations small
- Log changes clearly

---

## 🔄 Workflow

1. Update schema in SQL project
2. Build to `CatsDb.dacpac`
3. Deploy via the migrate-database (DacFx) hook
4. Run DatabaseSeeding

---

## ✅ Summary

- SQL Project → Schema definition
- migrate-database (DacFx) → Schema deployment
- DatabaseSeeding → Data setup & migration

---

## 🛠️ Requirements

- .NET 10 SDK
- SQL Server access

