# Field Getters

Generate public getters for private fields in Unity with Roslyn source generation. Mark a partial class or struct with `[CommonConfig]` to generate its getters; ordinary classes and `ScriptableObject` types are supported.

## Requirements

- A Unity Editor with Roslyn 4.3 or newer. The package declares a conservative compatibility floor of Unity 2022.3.45f1; earlier patch versions are not claimed as supported. Integration is tested on Unity 6000.5.0f1.
- A .NET SDK to build the generator. Source Gen Builder can use the current Editor's bundled SDK or an SDK installed on PATH.
- [Source Gen Builder 1.0.2+](https://github.com/FatLarva/unity-sourcegen-builder) for the Unity Editor build-and-install workflow.

## Installation

In Unity, open **Window → Package Manager**, choose **Add package from git URL…**, and enter:

```text
https://github.com/FatLarva/unity-field-getters.git
```

The package ID is `com.fatlarva.fieldgetters`. You can also clone the repository and use **Add package from disk…** to select its `package.json`, or add it as a Git submodule:

```sh
git submodule add https://github.com/FatLarva/unity-field-getters.git Packages/com.fatlarva.fieldgetters
```

Install **Source Gen Builder** (`com.fatlarva.sourcegenbuilder`) separately from `https://github.com/FatLarva/unity-sourcegen-builder.git`. Field Getters contains sources; Source Gen Builder builds and installs the DLLs. A separate builder dependency is not required when building the solution directly with `dotnet`.

## Setup

1. Create a Settings asset via **Assets → Create → Source Gen Builder → Settings**.
2. Create a Profile asset via **Assets → Create → Source Gen Builder → Profile**.
3. Set the Profile's **Display Name** to `Field Getters` and **Source Root** to `Packages/com.fatlarva.fieldgetters/GetterGenerator~`. Leave **Extra Build Args** empty to use the default Release build.
4. Add one entry to **Dependencies**, and configure **Generator**, using the values below.

| Target | Csproj Path | Install Dll Path | Dll Type |
| --- | --- | --- | --- |
| Dependencies: attributes | `Attributes/CommonConfigGen.Attributes.csproj` | `Assets/GeneratorsAndAnalyzers/CommonConfigGen.Attributes.dll` | `Runtime` |
| Generator | `Generator/CommonConfigGen.Generator.csproj` | `Assets/GeneratorsAndAnalyzers/CommonConfigGen.Generator.dll` | `Analyzer` |

5. Add the Profile to the Settings asset's **Profiles** list.
6. Open **Tools → Source Gen Builder**, assign the Settings asset, and click **Build All**.

**Source Root** is relative to the Unity project root; **Csproj Path** is relative to Source Root. The same logical package path works for Git URL, local, embedded, and submodule installations: Source Gen Builder 1.0.2+ resolves the package's physical directory automatically. No cache path or commit hash needs to be entered in the profile.

The builder builds the attributes dependency before the generator and installs both DLLs at the configured asset paths. On first installation, it enables the attributes DLL for runtime use, disables all platforms for the generator DLL, and applies its `RoslynAnalyzer` label.

## Usage

```csharp
using CommonConfigGen;
using UnityEngine;

[CommonConfig]
public partial class WeaponConfig : ScriptableObject
{
    [SerializeField] private string _weaponName;
    [SerializeField] private DamageRange _damageRange; // struct → ref getter

    [Skip]
    [SerializeField] private string _internalNotes;    // excluded
}
```

Generated:
```csharp
public partial class WeaponConfig
{
    public string WeaponName { get => _weaponName; }
    public ref DamageRange DamageRange { get => ref _damageRange; }
}
```

## Rules

- `private` fields → getter generated
- `protected`/`public`/`internal` → skipped
- `[Skip]` → skipped
- Existing public property with same name → skipped
- User-defined `struct` field type in a class → `ref` getter, which allows changing the value through the returned reference; fields declared in a struct receive value getters
- `static`/`const` → skipped
- `_someField` → `SomeField` naming

## Editing the Generator

Open `GetterGenerator~/CommonConfigGen.sln` inside this package in a separate Rider/VS instance for full IntelliSense. The `~` suffix keeps Unity from importing the source files. After editing the sources, open **Tools → Source Gen Builder** and click **Build All**.

For editing, use a checkout installed as a Local package or Git submodule, so your changes are tracked in its repository. A package installed from a Git URL is a managed cached copy. Configure the IDE to use a .NET SDK and restore NuGet dependencies before editing the standalone solution; the builder's SDK detection does not configure the IDE.

The existing `CommonConfigGen` namespace, `[CommonConfig]` and `[Skip]` attributes, project names, and DLL names are preserved.

## License

[MIT](LICENSE).
