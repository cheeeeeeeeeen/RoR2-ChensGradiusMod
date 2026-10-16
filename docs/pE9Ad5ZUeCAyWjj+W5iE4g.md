#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[Drone](o+an11PxrqGB40HSHXgvpQ 'Chen.GradiusMod.Drones.Drone')
## Drone.ReplaceBrokenModel(GameObject, GameObject, Renderer, GameObject[]) Method
Replaces the model of the broken interactable that was cloned from a vanilla broken drone, and makes the new model interactable and highlightable.  
```csharp
protected void ReplaceBrokenModel(GameObject brokenObject, GameObject customBrokenModel, Renderer highlightRenderer, params GameObject[] extraInteractableParts);
```
#### Parameters
<a name='Chen_GradiusMod_Drones_Drone_ReplaceBrokenModel(GameObject_GameObject_Renderer_GameObject__)_brokenObject'></a>
`brokenObject` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The cloned broken interactable.
  
<a name='Chen_GradiusMod_Drones_Drone_ReplaceBrokenModel(GameObject_GameObject_Renderer_GameObject__)_customBrokenModel'></a>
`customBrokenModel` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The new model of the broken interactable.
  
<a name='Chen_GradiusMod_Drones_Drone_ReplaceBrokenModel(GameObject_GameObject_Renderer_GameObject__)_highlightRenderer'></a>
`highlightRenderer` [UnityEngine.Renderer](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Renderer 'UnityEngine.Renderer')  
The renderer that is highlighted when looking at the interactable.
  
<a name='Chen_GradiusMod_Drones_Drone_ReplaceBrokenModel(GameObject_GameObject_Renderer_GameObject__)_extraInteractableParts'></a>
`extraInteractableParts` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')  
Other parts of the model with colliders that should also be interactable.
  
