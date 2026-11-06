# Gurux.DLMS.UI.Components library

Reusable Razor components for editing DLMS COSEM objects in Gurux applications.
Targets .NET 10.0 and supports Blazor applications.

See the [main README](../README.md) for installation, service registration,
and confirmed meter value handling.

Build from this directory:

```shell
dotnet build Gurux.DLMS.UI.Components.csproj
```

## COSEM object help

Every object view includes a Help button in its shared header. It opens an English
Markdown description of the class, an example use and an explanation of each
numbered attribute. Version-specific attributes are identified in the text. The
same help works in a dedicated editor and in the generic object view.

Help is embedded in the library under `Help/Topics`. It works without a web server,
an external documentation site or access to the application's working directory.
The first request loads the resource asynchronously; later requests reuse the
cached text. No help is read while simply opening an object view.

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


For a new COSEM class, add `Help/Topics/<type-name>.md` with an overview,
`## When to use it` and `## Attributes`, using `### 1. Logical name` style
headings for each attribute. The file is automatically embedded. Keep these
English descriptions in plain language and include older version differences.

Each class topic declares its supported attribute counts in an authoring comment,
with one entry for version 0, version 1, and so on:

```markdown
# Push setup

<!-- cosem-attributes: 7,10,13 -->
```

The version renderer automatically omits numbered attribute sections above that
version's count. It also hides the `## Versions` history section in a filtered
request. Without a version, a version/attribute summary is generated when no
history section has been authored. The tests compare the metadata and rendered
headings against real
`IGXDLMSBase.GetAttributeCount()` results for every reported version, including a
newer constructor default when `GetMaxSupportedVersion()` underreports it.

When an attribute's meaning, heading or structure changes, author alternative
sections explicitly. Use the same numbered heading with a version suffix so that
the unversioned help remains understandable:

```markdown
<!-- versions: 0-1 -->
### 7. Repetition delay (versions 0-1)

Sets a single delay in seconds between attempts.
<!-- /versions -->

<!-- versions: 2 -->
### 7. Repetition delay (version 2)

Defines the minimum delay, exponential growth factor and maximum delay.
<!-- /versions -->
```

Ranges are inclusive; comma-separated ranges are also supported. The directives
and heading suffixes are removed in selected-version output. Directives are
removed but all alternatives are retained when no version is requested. The
rendered cache keeps different topic/version combinations separate; the current
resources use the same English fallback for every culture. No runtime reflection
or COSEM object construction is required to render help. The generic Object topic
deliberately has no class-version limits.


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
