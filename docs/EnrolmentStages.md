# Participant Journey

```mermaid
stateDiagram-v2

 classDef archiveClass fill:#F00
 classDef approvedClass fill:#008000
 classDef qaClass fill:#FFD700

 [*] --> Identified
 Identified --> Enrolling
 Enrolling --> PQA
 PQA --> Enrolling
 PQA --> Authority
 Authority --> PQA
 Authority --> Approved
 Approved --> Archived
 Enrolling --> Archived
 Identified --> Archived

 class Archived archiveClass
 class Approved approvedClass
 class PQA qaClass
 class Authority qaClass
```