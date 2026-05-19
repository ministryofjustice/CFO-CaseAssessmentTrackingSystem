# CATS Database Repository

This repository contains the database definition, deployment tooling, and supporting utilities for the **CATS database** using a modern **.NET SQL Project (DbProj, .NET 10)** approach.

---

## 📦 Repository Structure

```
/ (root)
│
├── /src/Database/CatsDb      # SQL Project (database schema)
├── /src/DatabaseSeeding      # Data seeding & migration project
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

## 🚀 Deployment Process (sqlpackage)

The SQL project builds into a `.dacpac` file:

```
CatsDb.dacpac
```

### Build

```
dotnet build
```

---

### Deploy

```
sqlpackage /Action:Publish   /SourceFile:CatsDb.dacpac /TargetConnectionString:"<connection-string>" /p:BlockOnPossibleDataLoss=false
```

### How it Works

- Compares `.dacpac` with target database
- Generates deployment plan
- Applies only schema differences

---

### Notes

- Avoid manual DB changes (they can be overwritten)
- Use PRs for all schema updates

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
3. Deploy via `sqlpackage`
4. Run DatabaseSeeding

---

## ✅ Summary

- SQL Project → Schema definition
- sqlpackage → Deployment
- DatabaseSeeding → Data setup & migration

---

## 🛠️ Requirements

- .NET 10 SDK
- sqlpackage CLI
- SQL Server access

