# Gurux.DLMS.UI.Components library

Reusable Razor components for editing DLMS COSEM objects in Gurux applications.
Targets .NET 10.0 and supports Blazor applications.

See the [main README](../README.md) for installation, service registration,
and confirmed meter value handling.

Build from this directory:

```shell
dotnet build Gurux.DLMS.UI.Components.csproj
```

## COSEM contextual help

`builder.Services.AddGXDLMSHelp()` registers the shared COSEM provider and help
service. Object views also work without registration, using the default native
HTTP reader. A registered host help service is used by the object help dialog,
so browser hosts can supply an authenticated server proxy instead of requiring
cross-origin access to gurux.fi.

`GXDLMSObjectHelpCatalog.GetRequest(target)` retains the selected class, COSEM
version and current UI culture. `LoadHtmlAsync(request)` retrieves the official
HTML document. `GXDLMSObjectHelpProvider` returns `HelpKind.Html` with the
original address and article selector. `GXHelpContentView` extracts and
sanitizes the article in the browser and displays it in a sandboxed iframe.
No HTML-to-Markdown conversion or AngleSharp dependency is required.
The existing host-loader delegate constructor still accepts Markdown directly.

Finnish cultures use the /fi/ article; HTTP 404 falls back to English.
Other cultures use English. Successful documents are cached for one hour,
with request time and response size limits. Configure supplied HTTP clients
to disable automatic redirects. Browser hosts should register a same-origin
proxy rather than request gurux.fi directly from WebAssembly.

Director uses the native reader. AMI's COSEM endpoint delegates to the same
catalog; its browser provider only calls AMI. AMI release builds must update
their pinned NuGet version after this library version has been published.
Set AMI's `GuruxDlmsUiComponentsVersion` build property to that published
version. AMI deliberately refuses builds using the old 1.0.2611.902 package
when the sibling source checkout is unavailable or release mode is selected.

## License

Gurux.DLMS.UI.Components is available under either of the following licenses:

- **Open-source license:** GNU General Public License, version 2 only
  (GPL-2.0-only). See [LICENSE](../LICENSE) for the full license text.
- **Commercial license:** Available from Gurux Ltd under a separate
  written agreement. This option allows use under the terms of that
  agreement instead of GPL-2.0-only.

A valid commercial license is required to use the commercial licensing
option. See [Gurux Commercial License Notice](../LICENSES/LicenseRef-Gurux-Commercial.txt).

For commercial licensing inquiries, [contact Gurux Ltd](https://www.gurux.fi/).
