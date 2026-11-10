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

`builder.Services.AddGXDLMSHelp()` registers the shared COSEM provider and help
service. Object views also work without registration, using the default native
HTTP reader. A registered host help service is used by the object help dialog,
so browser hosts can supply an authenticated server proxy instead of requiring
cross-origin access to gurux.fi.

`GXDLMSObjectHelpCatalog.GetRequest(target)` retains the selected class, COSEM
version and current UI culture. `LoadAsync(request)` retrieves only the body of
`https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMS...`; navigation, login forms,
headers, footers and scripts are removed. The article is returned as Markdown
with headings, tables, images and absolute links. No local COSEM documentation
is embedded in the package.

Finnish cultures (`fi` and `fi-FI`) use the `/fi/` article. If that translation
is missing, the reader retries English. Other UI languages use English. The
online article describes its own version variants; the reader does not filter
out sections for a requested COSEM version. Successful articles are cached by
address/language for one hour. Fetching is bounded by time and response size.

`GXDLMSHelpArticleReader(HttpClient)` supports a host-owned transport for tests
or application networking. Disable automatic redirects on that client. A host
can register a `GXDLMSObjectHelpProvider` with a loader delegate; the help
service and object dialog then use that transport. Do not fetch the website
from browser WebAssembly: expose a server endpoint calling the same catalog
and return its Markdown through the application's authenticated API.

Director uses the native reader. AMI's COSEM endpoint delegates to the same
catalog; its browser provider only calls AMI. AMI release builds must update
their pinned NuGet version after this library version has been published.
Set AMI's `GuruxDlmsUiComponentsVersion` build property to that published
version. AMI deliberately refuses builds using the old 1.0.2611.902 package
when the sibling source checkout is unavailable or release mode is selected.

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
