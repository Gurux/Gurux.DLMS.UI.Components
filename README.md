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

## COSEM contextual help

Register the COSEM `IGXHelpProvider` implementation and the shared help service:

```csharp
using Gurux.DLMS.UI.Components.Help;

builder.Services.AddGXDLMSHelp();
```

`AddGXDLMSHelp` can be called more than once. It preserves an existing
`GXHelpService` registration and adds `GXDLMSObjectHelpProvider` alongside other
help providers. The provider returns Markdown for known COSEM topics and null for
unrelated or unknown topics, allowing the service to try the next provider.

Object views register their class topic and current object version with
`GXHelpContext`. A host's help presenter can inject
`Gurux.UI.Components.Help.GXHelpService` and load the active object's help:

```csharp
var content = await helpService.GetHelpAsync(cancellationToken);
```

To request help for an object directly, without changing the active context:

```csharp
var request = GXDLMSObjectHelpCatalog.GetRequest(target);
var content = await helpService.GetHelpAsync(request, cancellationToken);
```

The request includes a stable topic such as `cosem.GXDLMSPushSetup` and the
object's COSEM version, formatted independently of the current culture. The
provider returns only the attributes and descriptions applicable to that COSEM
version. For example, Push setup versions 0, 1 and 2 show attributes 1-7, 1-10
and 1-13 respectively, and version 2 gets the structured retry-delay description.
An invalid or unsupported version returns null rather than showing another
version's instructions. Requests without a version return the complete class help
with all version alternatives and notes. Other requested languages fall back to
English. Render the returned Markdown with
`GXMarkdownView` in the host's presenter. The common `GXHelp` documentation-link
control requires the host to connect provider content to its help presentation.

The Help button in each object view uses the same provider directly and works
without this registration. It loads on demand, preserves the dialog during value
refreshes and closes it when the selected class or object version changes.

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
