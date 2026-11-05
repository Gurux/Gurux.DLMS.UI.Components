# Gurux.DLMS.UI.Components

Reusable Razor DLMS UI components for Gurux web, Windows, iOS, and Android applications. 
The library provides UI components for DLMS COSEM objects.

## Requirements

- .NET 10.0 or later
- A Blazor application (WebAssembly, Server, or Web App)

## Install

Add the package to your application when it is available from your configured NuGet feed:

```bash
dotnet add package Gurux.DLMS.UI.Components
```

Register the local-storage service in `Program.cs`:

```csharp
using Gurux.DLMS.UI.Components;

builder.Services.AddScoped<IGXLocalStorage, GXLocalStorage>();
```

Add the component namespace to `_Imports.razor`:

```razor
@using Gurux.DLMS.UI.Components
```

## Confirmed meter values

When supplying a meter read to an object editor, keep its received value alongside
the COSEM model conversion. Some COSEM setters normalize empty values, dates or
reference types. The editor needs the confirmed value to display them faithfully:

```csharp
var edit = GXDLMSObjectEditState.For(target);
edit.SetConfirmedAttribute(index, receivedValue);
client.UpdateValue(target, index, receivedValue);
edit.RefreshConfirmedValues();
```

Call `SetConfirmedAttribute` for each admitted read or persisted value refresh.
It copies array and byte buffers and preserves pending edits. `AcceptValue`
updates the confirmed value after an acknowledged Data/Register write.
Object types without a dedicated layout use the common attribute editor.

## Tests and API documentation

Run the edit-state tests from the repository root:

```bash
dotnet test Development/Gurux.DLMS.UI.Components.sln
```

The tests use real COSEM objects without a meter connection. They cover pending
edits, acknowledgements, incoming reads, validation, and confirmed buffer ownership.
Builds generate XML API documentation alongside the library and include it in the
NuGet package for IntelliSense.

## Publishing to NuGet.org

The GitHub Actions workflow `.github/workflows/publish-dlms-ui-components.yml`
restores, builds and packs `Development/Gurux.DLMS.UI.Components.csproj` on .NET 10.
It publishes the NuGet and symbols packages after pushes to `main`, and can also
be started manually with **Actions > Publish Gurux.DLMS.UI.Components NuGet
package > Run workflow**. An optional four-part version can be supplied.
Otherwise the version follows the latest published major/minor series, using
`major.minor.yyMM.(day*100+sequence)` (initial series: `1.0`), following
[Gurux.UI.Components](https://github.com/Gurux/Gurux.UI.Components/blob/main/.github/workflows/publish-ui-components.yml).
The date is UTC. For example, the first release on 5 November 2026 is
`1.0.2611.501`, the next is `1.0.2611.502`, and the first release on 6 November
is `1.0.2611.601`. The revision is always greater than the latest revision in
the same month. A new month starts its own date-based revision series.
Published versions are
tagged `Gurux.DLMS.UI.Components/v<version>`; reruns of a tagged commit reuse its
version. Existing NuGet versions without a matching commit tag are rejected.
Package dependencies are not changed by the workflow.

Configure publishing before running the workflow:

1. Make the workflow available in the `Gurux/Gurux.DLMS.UI.Components` GitHub
   repository. An SVN working copy does not execute GitHub Actions by itself.
2. Add the repository Actions variable `NUGET_USER`, containing the NuGet.org
   **profile name**, not an email address.
3. On NuGet.org, create a Trusted Publishing policy for owner `Gurux`, repository
   `Gurux.DLMS.UI.Components`, and workflow file `publish-dlms-ui-components.yml`.
   Leave the environment field empty and allow the `Gurux.DLMS.UI.Components`
   package. No long-lived NuGet API key is stored in the repository.
4. Allow GitHub Actions to create version tags (`contents: write`). Repository
   rules must permit `Gurux.DLMS.UI.Components/v*` tags from this workflow.

The workflow retains `.nupkg` and `.snupkg` build artifacts. It builds the package
project directly; the solution's referenced `Tests` directory is currently absent.

## License

Gurux.DLMS.UI.Components is available under either of the following licenses:

- **Open-source license:** GNU General Public License, version 2 only
  (GPL-2.0-only). See [LICENSE](LICENSE) for the full license text.
- **Commercial license:** Available from Gurux Ltd under a separate
  written agreement. This option allows use under the terms of that
  agreement instead of GPL-2.0-only.

A valid commercial license is required to use the commercial licensing
option. See [Gurux Commercial License Notice](LICENSES/LicenseRef-Gurux-Commercial.txt).

For commercial licensing inquiries, [contact Gurux Ltd](https://www.gurux.fi/).

## Links

- [Gurux](https://www.gurux.fi/)
- [Project repository](https://github.com/gurux/Gurux.DLMS.UI.Components)
