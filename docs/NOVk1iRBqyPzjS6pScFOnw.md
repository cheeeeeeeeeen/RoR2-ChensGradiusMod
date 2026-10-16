#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Artifacts.Machines](ayrCd5wE1fGIQOox6GFHYA 'Chen.GradiusMod.Artifacts.Machines').[Machines](06BKrroboYsdkfWNwbWj1A 'Chen.GradiusMod.Artifacts.Machines.Machines')
## Machines.ShouldSuppressBrokenDrone(CharacterBody) Method
Checks if the drone should not leave a repurchasable broken drone behind upon death.  
Drones owned by enemies, and drones that were given by this artifact, are never repurchasable.  
```csharp
public bool ShouldSuppressBrokenDrone(CharacterBody droneBody);
```
#### Parameters
<a name='Chen_GradiusMod_Artifacts_Machines_Machines_ShouldSuppressBrokenDrone(CharacterBody)_droneBody'></a>
`droneBody` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The body of the drone that died.
  
#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if the broken drone should not spawn.
