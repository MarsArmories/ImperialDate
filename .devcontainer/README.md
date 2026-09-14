# ImperialDate devcontainer

This container provides a clean Linux environment for validating the repository with
the SDK selected by `global.json` (`10.0.401`). The image is based on Ubuntu 24.04
(Noble) and runs as the non-root `vscode` user.

From the repository root, run:

```sh
dotnet test --no-restore
```

The .NET 11 SDK should be added to the `dotnet` Feature's `additionalVersions`
option once the desired .NET 11 SDK is published and the project needs to target it.
Keeping .NET 10 as the selected SDK preserves the exact version required by this
repository today.
