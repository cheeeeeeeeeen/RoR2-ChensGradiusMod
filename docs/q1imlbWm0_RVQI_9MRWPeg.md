#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Modules](vpFhjnRUIa36IfVm3haMEw 'Chen.GradiusMod.Modules')
## ModuleBase Class
The base class of the items and artifacts of this mod. It replaces the module system that TILER2 used to provide.  
```csharp
public abstract class ModuleBase
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; ModuleBase  

Derived  
&#8627; [ArtifactBase](WBEZFsK1dXU87InG+LPPlg 'Chen.GradiusMod.Artifacts.ArtifactBase')  
&#8627; [ItemBase](PTlTj9QMQE5cU3yAjyFa7A 'Chen.GradiusMod.Items.ItemBase')  

| Properties | |
| :--- | :--- |
| [config](WO5tUWKxBMK_t88dw15kJQ 'Chen.GradiusMod.Modules.ModuleBase.config') | The config file where the module binds its config options.<br/> |
| [configCategory](14XaKHmMAUoG8FJHcb2rAA 'Chen.GradiusMod.Modules.ModuleBase.configCategory') | The config section used by the module. This follows the same format TILER2 used so old config files are preserved.<br/> |
| [configCategoryPrefix](aHruCNUsJ7wOJ14e7USanQ 'Chen.GradiusMod.Modules.ModuleBase.configCategoryPrefix') | The prefix of the config section that is used by the module, e.g. "Items" or "Artifacts".<br/> |
| [enabled](ltPWBLRgbtY1dh8aIH9S3Q 'Chen.GradiusMod.Modules.ModuleBase.enabled') | Determines if the module is enabled. Disabled modules are not registered to the game.<br/> |
| [name](CWvU+lJFA6xcH2zxmGMrYQ 'Chen.GradiusMod.Modules.ModuleBase.name') | The internal name of the module, which is the class name.<br/> |

| Methods | |
| :--- | :--- |
| [Bind&lt;T&gt;(string, T, string)](v+driiV5hTMWFwCwP+Pa+A 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string)') | Binds a config option under the module's config section.<br/> |
| [Bind&lt;T&gt;(string, T, string, T, T)](2ptroXlvhRY80yqGn9vQFA 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string, T, T)') | Binds a config option under the module's config section, clamped within a range.<br/> |
| [Install()](1M5SBpHvdzFUbX7uwJqWnQ 'Chen.GradiusMod.Modules.ModuleBase.Install()') | Applies the hooks of the module.<br/> |
| [NPlur(int)](XKgjAe+Y3j4h49rzupMjCg 'Chen.GradiusMod.Modules.ModuleBase.NPlur(int)') | Returns the plural suffix "s" when the number is not 1.<br/> |
| [Pct(float, uint)](3orgcfe2asGDCmQd375PAA 'Chen.GradiusMod.Modules.ModuleBase.Pct(float, uint)') | Formats a number into a percentage string, e.g. 0.25 becomes "25%".<br/> |
| [SetupAttributes()](klP2FCAjgqPXhVdhaojttA 'Chen.GradiusMod.Modules.ModuleBase.SetupAttributes()') | Creates and registers the definitions of the module to the game, such as the ItemDef or the ArtifactDef.<br/> |
| [SetupBehavior()](n1UpuZeOG+H08REqQRG2Pg 'Chen.GradiusMod.Modules.ModuleBase.SetupBehavior()') | Sets up the behavior of the module such as language tokens and assets.<br/> |
| [SetupConfig()](9vtnRw9xUjz8zXHkb3f8hw 'Chen.GradiusMod.Modules.ModuleBase.SetupConfig()') | Binds the config options of the module. Always call base.SetupConfig when overriding.<br/> |
| [Uninstall()](HQdZy2Jj7+29asx726L7hA 'Chen.GradiusMod.Modules.ModuleBase.Uninstall()') | Removes the hooks of the module.<br/> |
