#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[Drone](o+an11PxrqGB40HSHXgvpQ 'Chen.GradiusMod.Drones.Drone')
## Drone.FindBrokenDamagePoint(GameObject) Method
Finds the damage point effect in the broken drone effects carried over by ReplaceBrokenModel.  
```csharp
protected static Transform FindBrokenDamagePoint(GameObject customBrokenModel);
```
#### Parameters
<a name='Chen_GradiusMod_Drones_Drone_FindBrokenDamagePoint(GameObject)_customBrokenModel'></a>
`customBrokenModel` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The model passed to ReplaceBrokenModel.
  
#### Returns
[UnityEngine.Transform](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Transform 'UnityEngine.Transform')  
The transform of the damage point effect, or null if it was not found.
