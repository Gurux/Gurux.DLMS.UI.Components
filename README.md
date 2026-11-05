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
