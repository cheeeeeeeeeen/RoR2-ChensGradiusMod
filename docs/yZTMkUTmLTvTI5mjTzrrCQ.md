#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[DroneCatalog](qPWMsXW14ySl71rXQaL2KQ 'Chen.GradiusMod.Drones.DroneCatalog')
## DroneCatalog.Initialize(string, ConfigFile) Method
Generates a list of data containing the custom drones of the mod that called this method.  
```csharp
public static System.Collections.Generic.List<Chen.GradiusMod.Drones.DroneInfo> Initialize(string modGuid, ConfigFile configFile);
```
#### Parameters
<a name='Chen_GradiusMod_Drones_DroneCatalog_Initialize(string_ConfigFile)_modGuid'></a>
`modGuid` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The mod GUID
  
<a name='Chen_GradiusMod_Drones_DroneCatalog_Initialize(string_ConfigFile)_configFile'></a>
`configFile` [BepInEx.Configuration.ConfigFile](https://docs.microsoft.com/en-us/dotnet/api/BepInEx.Configuration.ConfigFile 'BepInEx.Configuration.ConfigFile')  
The file where the mod's custom drones will bind their configs
  
#### Returns
[System.Collections.Generic.List&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')[DroneInfo](HgBDP9HfqsUu394_FlkKCg 'Chen.GradiusMod.Drones.DroneInfo')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')  
A list of DroneInfos from the mod that called this method.
