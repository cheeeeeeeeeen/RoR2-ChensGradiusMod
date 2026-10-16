#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[Drone](o+an11PxrqGB40HSHXgvpQ 'Chen.GradiusMod.Drones.Drone')
## Drone.CreatePickupDisplay(GameObject, float) Method
Creates a model that is displayed when the drone is dropped as a pickup.  
Call this before the original model is used for the body, as replacing the body model changes the original object.  
```csharp
protected GameObject CreatePickupDisplay(GameObject model, float scale=1f);
```
#### Parameters
<a name='Chen_GradiusMod_Drones_Drone_CreatePickupDisplay(GameObject_float)_model'></a>
`model` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The model of the drone.
  
<a name='Chen_GradiusMod_Drones_Drone_CreatePickupDisplay(GameObject_float)_scale'></a>
`scale` [System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single')  
The scale of the display model.
  
#### Returns
[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The display prefab
