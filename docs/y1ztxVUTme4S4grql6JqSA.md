#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[DroneCatalog](qPWMsXW14ySl71rXQaL2KQ 'Chen.GradiusMod.Drones.DroneCatalog')
## DroneCatalog.ScopedSetupAll(List&lt;DroneInfo&gt;) Method
Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.  
This flavor does a scoped setup, effective if the custom drones are coded in such a way they have no dependent/shared components/behaviors from one another.  
```csharp
public static void ScopedSetupAll(System.Collections.Generic.List<Chen.GradiusMod.Drones.DroneInfo> droneInfos);
```
#### Parameters
<a name='Chen_GradiusMod_Drones_DroneCatalog_ScopedSetupAll(System_Collections_Generic_List_Chen_GradiusMod_Drones_DroneInfo_)_droneInfos'></a>
`droneInfos` [System.Collections.Generic.List&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')[DroneInfo](HgBDP9HfqsUu394_FlkKCg 'Chen.GradiusMod.Drones.DroneInfo')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1 'System.Collections.Generic.List`1')  
List of DroneInfos generated by Initialize
  
