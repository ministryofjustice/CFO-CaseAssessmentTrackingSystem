# Participant Journey

```mermaid
flowchart LR
    %% Pools / Lanes
    subgraph Participant["Participant"]
        A[Start Event\nParticipant Action]
        A --> B[Send Command\nExample: Change Status]
    end

    subgraph Application["Application Service"]
        B --> C[Invoke Aggregate\nTransition Status]
    end

    subgraph Domain["Domain Model / Aggregate"]
        C --> D[Domain Event Raised\nStatus Transitioned]
        D --> E[End of Transaction\nCommit]
    end

    subgraph Integration["Integration Layer"]
        E --> F[Publish Integration Event]
    end

    subgraph External["External Event Handlers"]
        F --> G[Handler A Updates State]
        F --> H[Handler B Updates State]
        G --> I[End]
        H --> I
    end