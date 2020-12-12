
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Drones](./Y-iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[DroneCatalog](./qPWMsXW14ySl71rXQaL2KQ 'Chen.GradiusMod.Drones.DroneCatalog')

## DroneCatalog.Initialize(string, BepInEx.Configuration.ConfigFile) Method
Generates a list of data containing the custom drones of the mod that called this method.  
```csharp
public static System.Collections.Generic.List<Chen.GradiusMod.Drones.DroneInfo> Initialize(string modGuid, BepInEx.Configuration.ConfigFile configFile);
```

#### Parameters
<a name='S35pHLHgq+TkpQFrDgn6-A'></a>
`modGuid` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The mod GUID  
  
<a name='k+p80ldDi3mIjL5zDA6ilA'></a>
`configFile` [BepInEx.Configuration.ConfigFile](https://docs.microsoft.com/en-us/dotnet/api/BepInEx.Configuration.ConfigFile 'BepInEx.Configuration.ConfigFile')  
The file where the mod's custom drones will bind their configs  
  

#### Returns
[System.Collections.Generic.List&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')[DroneInfo](./HgBDP9HfqsUu394-FlkKCg 'Chen.GradiusMod.Drones.DroneInfo')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')  
A list of DroneInfos from the mod that called this method.  