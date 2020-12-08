
### [Chen.GradiusMod](./neHTXX+yFsk1RpXqjkv9zg 'Chen.GradiusMod').[DroneCatalog](./e4Rd6FCByUnS5tZmK8HU3A 'Chen.GradiusMod.DroneCatalog')

## DroneCatalog.Initialize(string, BepInEx.Configuration.ConfigFile) Method
Generates a list of data containing the custom drones of the mod that called this method.  
```csharp
public static System.Collections.Generic.List<Chen.GradiusMod.DroneInfo> Initialize(string modGuid, BepInEx.Configuration.ConfigFile configFile);
```

#### Parameters
<a name='8uvZv+IMzCJKKDB0sR1ICQ'></a>
`modGuid` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The mod GUID  
  
<a name='X+8+Kt-LkaXq68WSwaXPsA'></a>
`configFile` [BepInEx.Configuration.ConfigFile](https://docs.microsoft.com/en-us/dotnet/api/BepInEx.Configuration.ConfigFile 'BepInEx.Configuration.ConfigFile')  
The file where the mod's custom drones will bind their configs  
  

#### Returns
[System.Collections.Generic.List&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')[DroneInfo](./6StSpaxBN5onTwyyDs-VEQ 'Chen.GradiusMod.DroneInfo')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')  
A list of DroneInfos from the mod that called this method.  
