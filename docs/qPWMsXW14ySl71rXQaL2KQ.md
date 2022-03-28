#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')
## DroneCatalog Class
A static class that caters initializing and registering custom drones.  
```csharp
public static class DroneCatalog
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; DroneCatalog  

| Methods | |
| :--- | :--- |
| [EfficientSetupAll(List&lt;DroneInfo&gt;)](nsDLaVCWj_uLXs8tRatw9A 'Chen.GradiusMod.Drones.DroneCatalog.EfficientSetupAll(System.Collections.Generic.List&lt;Chen.GradiusMod.Drones.DroneInfo&gt;)') | Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.<br/>This flavor comes with an efficient setup by taking advantage of the boolean return values of each phase.<br/> |
| [Initialize(string, ConfigFile)](yZTMkUTmLTvTI5mjTzrrCQ 'Chen.GradiusMod.Drones.DroneCatalog.Initialize(string, ConfigFile)') | Generates a list of data containing the custom drones of the mod that called this method.<br/> |
| [ScopedSetupAll(List&lt;DroneInfo&gt;)](y1ztxVUTme4S4grql6JqSA 'Chen.GradiusMod.Drones.DroneCatalog.ScopedSetupAll(System.Collections.Generic.List&lt;Chen.GradiusMod.Drones.DroneInfo&gt;)') | Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.<br/>This flavor does a scoped setup, effective if the custom drones are coded in such a way they have no dependent/shared components/behaviors from one another.<br/> |
| [SetupAll(List&lt;DroneInfo&gt;)](bw5E6naIW2BBCzLDh5Nw6Q 'Chen.GradiusMod.Drones.DroneCatalog.SetupAll(System.Collections.Generic.List&lt;Chen.GradiusMod.Drones.DroneInfo&gt;)') | Sets all the custom drones contained in the list up. Mod creators may instantiate the drones their own if they have a sophisticated logic.<br/>This flavor shows the normal way of setting up the drone instances.<br/>The phases will still capture generic flags such as if the drone is enabled or not, or if the drone was already set up or not.<br/> |
