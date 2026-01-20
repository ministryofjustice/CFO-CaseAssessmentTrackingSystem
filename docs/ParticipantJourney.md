# Participant Journey

```mermaid
flowchart LR
    %% Pools / Lanes

    subgraph SupportWorker["Support Worker"]
        A[Identifies Participant]
        B[Consent Added]
        D[Returned]
        F[Approved]
        A --> B[Adds Consent]
        B --> C[Submits To PQA]
        D --> C[Resubmitted To PQA]
        F --> G[Continue working with]
    end

    subgraph ProviderQa["Provider QA"]
        C[Submitted To PQA]
        C --> D[Returned]
        C --> E[Approved]
    end

    subgraph Authority["Authority"]
        E[Submitted To Authority]
        E --> C[Returned]
        E --> F[Approved]
    end
