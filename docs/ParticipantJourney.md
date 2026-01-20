# Participant Journey

```mermaid
flowchart LR
    %% Pools / Lanes

    subgraph SupportWorker["Support Worker"]
        SW_A[Identifies Participant]
        SW_B[Consent Added]
        SW_D[Returned]
        SW_F[Approved]

        SW_A --> SW_B
        SW_B --> PQA_C[Submits To PQA]
        SW_D --> PQA_C
        SW_F --> SW_G[Continue working with]
    end

    subgraph ProviderQa["Provider QA"]
        PQA_C[Submitted To PQA]
        PQA_C --> SW_D
        PQA_C --> AUTH_E[Approved]
    end

    subgraph Authority["Authority"]
        AUTH_E[Submitted To Authority]
        AUTH_E --> PQA_C
        AUTH_E --> SW_F
    end
