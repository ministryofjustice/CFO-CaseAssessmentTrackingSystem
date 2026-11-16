# Run CATS locally for training

This guide starts a local development instance for practising with the supplied seed data. It is not the deployed training environment: the normal Aspire launch uses the **Development** configuration.

> **Do not enter real or live personal data.** A local instance is not necessarily isolated from external services: features such as DMS lookups, document storage and notifications may require external configuration and may be unavailable or contact configured services. Do not configure production services or credentials for training.

## Limitations

- **Enrolments and transfers:** there is no mock Data Management System (DMS), so these workflows are not supported without a suitable DMS integration.
- **Notifications and two-factor authentication:** GOV.UK Notify requires a configured account, API key and templates. Without these, notification delivery and two-factor codes sent through Notify will not work.
- **Documents:** uploading documents, including those used in activities, requires a configured Amazon S3 bucket and suitable access. Downloads also require the expected malware-scan tag on the object.

## Before you start

- Install the [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) version specified in [global.json](../global.json). Roll-forward is disabled, so the exact version is required.
- Install and start [Docker](https://docs.docker.com/engine/install/) with Linux containers enabled.
- Install [Visual Studio Code](https://code.visualstudio.com/), the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit), and the [Aspire extension and CLI](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode).
- Obtain a local training account and its sign-in details from your trainer or the project team. Do not use production credentials.
- Ensure you have internet access to restore packages and download container images on first startup.

## Configure local integrations

Store local integration settings in [.NET user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets), not in tracked configuration files. User secrets are stored on your machine outside the repository but are **not encrypted**. Use only approved non-production services and credentials.

For S3, edit the user secrets for the [Cats.AppHost project](../src/Aspire/Cats.AppHost/Cats.AppHost.csproj). Aspire passes these settings to CATS and its worker and consumer services:

| Key | Value |
| --- | --- |
| `AWS:Region` | AWS region for the bucket |
| `AWS:Bucket` | Non-production bucket name |
| `AWS:RootFolder` | Folder prefix for CATS documents |
| `AWS:AccessKey` | Access key, if using explicit AWS credentials |
| `AWS:SecretKey` | Secret key, if using explicit AWS credentials |

Set both access and secret keys together, or neither when using the AWS SDK's normal credential chain. Obtain the bucket, permissions and configuration from the project team. Do not commit credentials or enter them in commands saved in shell history.

The AppHost also passes its `DMS:ApplicationUrl`, `DMS:ApiKey`, `DMS:ClientCertBase64` and `DMS:ClientCertPassword` settings to these services when configured. GOV.UK Notify settings are not forwarded by the AppHost: for sign-in notifications, configure `Notify:ApiKey` and the relevant `Notify:Templates` in the [Server.UI project's](../src/Server.UI/Server.UI.csproj) user secrets instead. Obtain approved non-production settings from the project team.

## Start the application

1. Clone or download this repository and open its root folder in Visual Studio Code.
2. Make sure Docker is running.
3. Press **F5** and select **Aspire: Launch AppHost** if prompted. Alternatively, in Visual Studio select `Cats.AppHost` as the startup project and start debugging.
4. Open the Aspire dashboard from the extension if it does not open automatically. The first startup can take a while while images and packages are downloaded.
5. In the dashboard, wait for the `cats` resource to start, then open its endpoint. Sign in using the local training account provided by your trainer.

Aspire starts the supporting services, applies the [CatsDb SQL Project](../src/Database/CatsDb/) schema and runs [database seeding](../src/DatabaseSeeding/) before starting CATS. The seed scripts include users and participant records as well as reference data. Depending on the account and its permissions, different records and functions will be visible.

## Stop and restart

Stop debugging in your IDE when finished. Aspire uses persistent containers, which may remain running, and data volumes, including the SQL Server volume, so local changes can remain between sessions. Restart using the same steps; do not assume the seed scripts reset existing data. Ask the project team for help if you need a fresh training dataset.

## Troubleshooting

- **The app does not start:** check that Docker is running, the required SDK is installed, and the Aspire dashboard shows the database deployment and seeding have completed. Open the failing resource's logs in the dashboard.
- **A port is already in use:** check for another local service using the port reported in the logs. The AppHost configures SQL Server on port `61744` and RabbitMQ on ports `5672` and `15672`.
- **You cannot sign in:** check the local training account details with your trainer or the project team. Seeded accounts and credentials may differ from those in other environments.
- **A feature is unavailable:** some features depend on external services or credentials which are not supplied by this local setup. Check with the project team before configuring an integration.
