
[![Run Tests](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/run-tests.yml/badge.svg)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/actions/workflows/run-tests.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Issues](https://img.shields.io/github/issues/ministryofjustice/CFO-CaseAssessmentTrackingSystem)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/issues)
[![GitHub Repo stars](https://img.shields.io/github/stars/ministryofjustice/CFO-CaseAssessmentTrackingSystem?color=594ae2&style=flat-square&logo=github)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/stargazers)
[![Contributors](https://img.shields.io/github/contributors/ministryofjustice/CFO-CaseAssessmentTrackingSystem?color=594ae2&style=flat-square&logo=github)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/graphs/contributors)
[![Pull Requests](https://img.shields.io/github/issues-pr/ministryofjustice/CFO-CaseAssessmentTrackingSystem)](https://github.com/ministryofjustice/CFO-CaseAssessmentTrackingSystem/pulls)
[![Ministry of Justice Repository Compliance Badge](https://github-community.service.justice.gov.uk/repository-standards/api/CFO-CaseAssessmentTrackingSystem/badge)](https://github-community.service.justice.gov.uk/repository-standards/CFO-CaseAssessmentTrackingSystem)


# Overview

HMPPS Creating Future Opportunities (CFO) utilise the Case Assessment and Tracking System (CATS) to support delivery of [CFO Evolution](https://www.CreatingFutureOpportunities.gov.uk) . The programme utilises external funding to perform rehabilitative services with offenders in custody and the community. Approx. 600 users from non-government organisations use CATS to record work performed with offenders creating an evidence base that supports performance management, payments to providers, ongoing research and audits from external bodies.

# Interfaces/Systems (Backend interface for surfacing the data to the front end)

* [.NET 10](https://dotnet.microsoft.com/en-us/download)
* [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)

# Mechanism (How does it communicate with other systems? Frequency of data pull/push, reporting, events etc) 

CATS relies on the external data from Nomis and Delius. This is aggregate and managed by the [CFO External Data Integration System](https://github.com/ministryofjustice/CFO-ExternalDataIntegrationSystem).


# Technology (What's the technology that drives the product? i.e. Azure, java script etc) 

* ASP.NET Core (Blazor)
* C#
* LINQ
* Entity Framework
* SQL Server
* HTML, CSS, Javascript
* Dotnet Aspire

# No. of users 

600 (approx. 100 concurrent)

# Development Environment

This has been developed on Windows 11 using Visual Studio 2022, Visual Studio Code and JetBrains Rider

# Development Setup and Execution Guide
## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Visual Studio Code users**:
    - [C# Dev Kit Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
    - [Aspire Extension (& CLI)](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode)

## Running the apps
The recommended way to run and debug these apps is using .NET Aspire.
- **Using Visual Studio Code**: open the project and press `F5`, selecting the *Default Configuration*.
- **Using Visual Studio or other IDEs**: From the debug configuration dropdown, select `Cats.AppHost` and start the application.
---

## Publishing (preview)

This repository uses [Aspire]("https://github.com/dotnet/aspire") for service composition (dependency injection, service discovery, and configuration management). 

Aspire is also used to provide a Kubernetes publishing workflow that is currently in preview: container image build & push, and generation/packaging of Kubernetes manifests & Helm charts.

Prerequisites
- Docker (for image builds)
- Access to a container registry (credentials configured)
- kubectl and a valid kubeconfig with cluster access
- Helm (for chart-based deployments)

### Build and publish image
```
IMAGE_NAME=hmpps-cfo/cats
TAG=latest
REGISTRY=registry.mycorp.com:1234

# Locally
dotnet publish src/Server.UI/Server.UI.csproj \
    --configuration Release \
    --os linux \
    --arch x64 \
    --target:PublishContainer \
    --property:ContainerRepository=$IMAGE_NAME \
    --property:ContainerImageTag=$TAG

# or to a remote registry (with the ContainerRegistry property)
dotnet publish src/Server.UI/Server.UI.csproj \
    --configuration Release \
    --os linux \
    --arch x64 \
    --target:PublishContainer \
    --property:ContainerRegistry=$REGISTRY \
    --property:ContainerRepository=$IMAGE_NAME \
    --property:ContainerImageTag=$TAG
```

### Generate Kubernetes manifests & Helm charts
```
dotnet aspire publish -o aspire-output
```

### Deploy (using Helm upgrade)
```
helm upgrade --install aspire ./aspire-output --namespace default \
    --set parameters.cats.cats_image=$IMAGE
```
