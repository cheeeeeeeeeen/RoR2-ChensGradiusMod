#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.OptionSeed](be1vnC2Vgp_vVFpwRHLjUQ 'Chen.GradiusMod.Items.OptionSeed').[OptionSeed](U6Iu4qSqg_tWdEO+2QhjqQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed')
## OptionSeed.SetAutoMirroring(string, bool) Method
Sets if the Option Seeds of the survivor copy its attacks automatically, by copying the bullets, projectiles, melee hits and explosions it creates.  
Use this for modded survivors. Set to false if the survivor's skills are supported manually with FireForSeeds instead.  
```csharp
public void SetAutoMirroring(string bodyName, bool enabled);
```
#### Parameters
<a name='Chen_GradiusMod_Items_OptionSeed_OptionSeed_SetAutoMirroring(string_bool)_bodyName'></a>
`bodyName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The CharacterBody name of the survivor, e.g. "CommandoBody".
  
<a name='Chen_GradiusMod_Items_OptionSeed_OptionSeed_SetAutoMirroring(string_bool)_enabled'></a>
`enabled` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True to copy attacks automatically, false to only use manual support.
  
