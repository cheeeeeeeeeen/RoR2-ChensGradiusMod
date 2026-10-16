#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Artifacts](0OL_C+BEo2tLyEgqqKvXxQ 'Chen.GradiusMod.Artifacts')
## ArtifactBase Class
The base class of the artifacts of this mod.  
```csharp
public abstract class ArtifactBase : Chen.GradiusMod.Modules.ModuleBase
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase') &#129106; ArtifactBase  

Derived  
&#8627; [ArtifactBase&lt;T&gt;](mnk9amYhJthJPbRV9qBcAQ 'Chen.GradiusMod.Artifacts.ArtifactBase&lt;T&gt;')  

| Properties | |
| :--- | :--- |
| [artifactDef](15jas8Wn9H8fva86QBqtxg 'Chen.GradiusMod.Artifacts.ArtifactBase.artifactDef') | The ArtifactDef of this artifact. Available after SetupAttributes.<br/> |
| [configCategoryPrefix](vsFiN3_fJdckp63N9582RQ 'Chen.GradiusMod.Artifacts.ArtifactBase.configCategoryPrefix') | The prefix of the config section that is used by the module, e.g. "Items" or "Artifacts".<br/> |
| [displayName](7a4421vDsE3Ac_1ep3v0Fw 'Chen.GradiusMod.Artifacts.ArtifactBase.displayName') | The display name of the artifact.<br/> |
| [iconResource](M6jOFdI5XYi9KhlJxiw7Bg 'Chen.GradiusMod.Artifacts.ArtifactBase.iconResource') | The icon of the artifact when it is enabled.<br/> |
| [iconResourceDisabled](G4dPZGinx2h35g42UEydWg 'Chen.GradiusMod.Artifacts.ArtifactBase.iconResourceDisabled') | The icon of the artifact when it is disabled.<br/> |
| [tokenPrefix](skXi95CyAOp6Ew8riBXnZA 'Chen.GradiusMod.Artifacts.ArtifactBase.tokenPrefix') | The prefix of the language tokens of the artifact.<br/> |

| Methods | |
| :--- | :--- |
| [IsActiveAndEnabled()](p4_AHPdO2fm8v6rQoqMJqQ 'Chen.GradiusMod.Artifacts.ArtifactBase.IsActiveAndEnabled()') | Checks if the artifact is enabled in the current run.<br/> |
| [SetupAttributes()](8Rko_h5YvYTofoDm8XzKNg 'Chen.GradiusMod.Artifacts.ArtifactBase.SetupAttributes()') | Creates and registers the definitions of the module to the game, such as the ItemDef or the ArtifactDef.<br/> |
