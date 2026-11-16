# Build automation with Cake

This repository includes a file-based Cake script at `cake.cs`. Run it from the repository root with:

```bash
dotnet cake.cs
```

By default this runs the `Publish` target with the `Release` configuration. The full target chain is:

`Clean -> Restore -> Build -> Test -> Publish`

You can run a specific target or change the configuration by passing arguments to the script:

```bash
# Build without publishing
dotnet cake.cs --target=Build

# Run tests
dotnet cake.cs --target=Test

# Publish using Debug configuration
dotnet cake.cs --target=Publish --configuration=Debug
```

Available targets:

- `Clean`
- `Restore`
- `Build`
- `Test`
- `Publish`

The `Publish` target writes output to:

- `artifacts/Server.UI`
- `artifacts/DatabaseSeeding`
- `artifacts/CatsDb.dacpac`
