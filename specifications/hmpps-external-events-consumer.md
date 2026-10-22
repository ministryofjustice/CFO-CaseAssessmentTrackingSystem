# HMPPS External Events Consumer Architecture

## Objective

Implement support for consuming HMPPS External API events from AWS SQS using temporary AWS credentials obtained from the HMPPS token endpoint.

The implementation must follow the existing CATS consumer architecture and support running in either:

- The CATS Blazor application (in-process)
- The CATS Worker Service (out-of-process)

Host selection must be configuration-driven and require no code changes.

---

# Architectural Principles

## Host Agnostic Design

The HMPPS consumer must follow the same pattern as existing RabbitMQ consumers.

The consumer implementation must not depend on where it is hosted.

```text
                    Configuration
                           │
                           ▼
              ┌────────────────────┐
              │ AddHmppsConsumer() │
              └──────────┬─────────┘
                         │
           ┌─────────────┴─────────────┐
           │                           │
           ▼                           ▼

    CATS Blazor Host          CATS Worker Host
      (In Process)            (Out Of Process)
           │                           │
           └─────────────┬─────────────┘
                         │
                         ▼
                 HMPPS Consumer
```

The same implementation should be reusable by both hosts.

---

# Existing Consumer Model

The HMPPS consumer should follow the same pattern currently used by RabbitMQ integration event consumers.

```text
Message Source
       │
       ▼
Consumer
       │
       ▼
Dispatcher
       │
       ▼
Handler
       │
       ▼
Domain Logic
```

The only difference should be the transport mechanism.

RabbitMQ consumers:

```text
RabbitMQ
   │
   ▼
Consumer
```

HMPPS consumers:

```text
AWS SQS
   │
   ▼
Consumer
```

All downstream processing should follow existing conventions.

---

# Solution Components

## HMPPS Consumer Module

Create a reusable module that can be registered by either host.

Suggested project:

```text
CATS.ExternalEvents
```

Responsibilities:

- AWS credential management
- SQS client management
- Message consumption
- Message deserialization
- Event dispatch
- Event processing

---

# Service Registration

Expose a single registration method.

```csharp
services.AddHmppsExternalEvents(configuration);
```

Both hosts use the same registration.

---

# Blazor Host Registration

When consumers are configured to run in-process:

```csharp
if (hostingOptions.RunConsumersInProcess)
{
    services.AddHmppsExternalEvents(configuration);
}
```

---

# Worker Host Registration

When consumers are configured to run separately:

```csharp
services.AddHmppsExternalEvents(configuration);
```

No implementation changes should be required.

---

# AWS Credential Provider

## Purpose

Manage temporary AWS credentials obtained from the HMPPS token endpoint.

The rest of the application should not be aware that temporary credentials are being used.

### Interface

```csharp
public interface IAwsCredentialProvider
{
    Task<SessionAWSCredentials> GetCredentialsAsync(
        CancellationToken cancellationToken);
}
```

### Responsibilities

- Call the token endpoint
- Cache credentials
- Refresh credentials before expiry
- Hide credential lifecycle management

### Refresh Behaviour

```text
Credentials Missing?
      │
      └─ Fetch Credentials

Credentials Expiring Soon?
      │
      └─ Refresh Credentials

Otherwise
      │
      └─ Return Cached Credentials
```

Recommended refresh window:

```text
5 minutes before expiry
```

---

# SQS Client Provider

Create an abstraction responsible for creating and refreshing SQS clients.

### Interface

```csharp
public interface ISqsClientProvider
{
    Task<IAmazonSQS> GetClientAsync(
        CancellationToken cancellationToken);
}
```

### Responsibilities

- Create clients using current temporary credentials
- Recreate clients when credentials change
- Hide AWS SDK lifecycle concerns

---

# HMPPS Event Consumer

Implement as a hosted service following the same pattern used by existing consumers.

### Interface

```csharp
public interface IHmppsEventConsumer
{
}
```

### Hosted Service

```csharp
public class HmppsEventConsumer : BackgroundService
{
}
```

Responsibilities:

- Poll SQS
- Read messages
- Deserialize payloads
- Dispatch to handlers
- Delete successfully processed messages

---

# Queue Consumption

Use SQS Long Polling.

Recommended settings:

```csharp
MaxNumberOfMessages = 10;
WaitTimeSeconds = 20;
```

Benefits:

- Reduced API calls
- Reduced empty responses
- Improved scalability
- Lower AWS cost

---

# Message Flow

HMPPS uses an SNS-to-SQS pattern.

Queue messages arrive wrapped in a notification envelope.

Example:

```json
{
  "Message": "{...actual event payload...}"
}
```

Consumer responsibilities:

```text
Receive Queue Message
        │
        ▼
Deserialize SNS Wrapper
        │
        ▼
Deserialize Event Payload
        │
        ▼
Dispatch Event
```

---

# Event Processing Model

The HMPPS notification should be treated as a trigger.

Example notification:

```json
{
  "eventId": 123456,
  "hmppsId": "X688624",
  "eventType": "PERSON_NAME_CHANGED",
  "url": "https://api/events/person-name-changed/X688624"
}
```

The notification itself is not the source of truth.

The authoritative data should be retrieved from the event URL.

### Processing Flow

```text
Receive Notification
          │
          ▼
Call Event Endpoint
          │
          ▼
Retrieve Event Data
          │
          ▼
Map To Internal Event
          │
          ▼
Publish Internally
```

---

# Event Handlers

Use dedicated handlers per event type.

Suggested interface:

```csharp
public interface IHmppsEventHandler
{
    Task Handle(
        HmppsNotification notification,
        CancellationToken cancellationToken);
}
```

Examples:

```text
PersonNameChangedHandler

PersonRiskChangedHandler

PersonStatusChangedHandler
```

The consumer should not contain business logic.

---

# Internal Event Publishing

After mapping, publish internal integration events using existing patterns.

```text
HMPPS Event
      │
      ▼
Event Handler
      │
      ▼
Internal Integration Event
      │
      ▼
Existing Event Bus
      │
      ▼
Existing Consumers
```

This keeps the rest of the application unaware of:

- AWS
- SQS
- Certificates
- HMPPS-specific infrastructure

---

# Idempotency

External events may be delivered more than once.

All processing must be idempotent.

Store:

```text
EventId
ProcessedDateTime
```

Processing logic:

```text
Already Processed?
      │
  ┌───┴───┐
  │       │
 Yes      No
  │       │
 Ignore Process
```

---

# Failure Handling

Messages must only be deleted from SQS after successful processing.

```text
Receive Message
       │
       ▼
Process Message
       │
       ▼
Success?
   │
 ┌─┴─────┐
 │       │
Yes      No
 │       │
Delete   Leave In Queue
```

Failed messages will become available again after the SQS visibility timeout expires.

---

# Configuration

Example:

```json
{
  "Consumers": {
    "RunInProcess": true
  }
}
```

Or:

```json
{
  "Consumers": {
    "RunInProcess": false
  }
}
```

Behaviour:

```text
true
 └─ Consumers run within Blazor

false
 └─ Consumers run within Worker Service
```

No code changes should be required when switching modes.

---

# Non-Functional Requirements

- Follow existing CATS consumer architecture.
- Support both hosting models.
- Configuration-driven activation.
- Automatic credential refresh.
- Long polling enabled.
- Graceful shutdown support.
- Idempotent processing.
- Horizontal scalability when hosted in Worker Service.
- No message loss.
- No AWS-specific concerns leaking into domain logic.
- No certificate handling outside infrastructure components.
- Message processing failures must not stop queue consumption.

---

# Implementation Summary

Build the HMPPS External Events Consumer as a reusable consumer module that can run in either the Blazor application or the Worker Service using the same configuration-driven hosting model already used by existing RabbitMQ consumers.

The consumer should:

1. Obtain temporary AWS credentials from the HMPPS token endpoint.
2. Refresh credentials automatically before expiry.
3. Consume messages from SQS using long polling.
4. Extract and deserialize HMPPS notifications.
5. Retrieve authoritative event data from the supplied URL.
6. Dispatch to event-specific handlers.
7. Publish internal integration events using existing application patterns.
8. Support idempotent processing and safe retries.
9. Remain completely independent of the hosting process.


Be aware some of the configuration settings already exist in CONFIG as the blazor site already consumes an API endpoint with these similar credentials
