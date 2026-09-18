# ImperialDate devcontainer

This container provides a clean Linux environment for validating the repository with
the SDK selected by `global.json` (`10.0.401`). The image is based on Ubuntu 24.04
(Noble) and runs as the non-root `vscode` user.

From the repository root, run:

```sh
dotnet test --no-restore
```

If the project later needs to target .NET 11, update the base image tag (or add an SDK install step) so the required 11.x SDK is available alongside the version pinned by `global.json`.
