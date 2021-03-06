#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod](neHTXX+yFsk1RpXqjkv9zg 'Chen.GradiusMod').[Extensions](MwRmIyAAEXRsALWGh0ZEdw 'Chen.GradiusMod.Extensions')
## Extensions.InitializeDroneModelComponents(GameObject, CharacterBody, float, bool) Method
Shortcut for initializing a custom drone model. Only applies when work flow is followed the same as this mod's drones.  
```csharp
public static void InitializeDroneModelComponents(this UnityEngine.GameObject droneModel, RoR2.CharacterBody droneBody, float colliderMultiplier, bool debug=false);
```
#### Parameters
<a name='Chen_GradiusMod_Extensions_InitializeDroneModelComponents(UnityEngine_GameObject_RoR2_CharacterBody_float_bool)_droneModel'></a>
`droneModel` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The custom drone model to initialize.
  
<a name='Chen_GradiusMod_Extensions_InitializeDroneModelComponents(UnityEngine_GameObject_RoR2_CharacterBody_float_bool)_droneBody'></a>
`droneBody` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The associated body of the model.
  
<a name='Chen_GradiusMod_Extensions_InitializeDroneModelComponents(UnityEngine_GameObject_RoR2_CharacterBody_float_bool)_colliderMultiplier'></a>
`colliderMultiplier` [System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single')  
Collider size multiplier to be applied to the collider in the CharacterBody object.  
The basis would be the collider found within the model. A small collider may result in clipping through the map.  
  
<a name='Chen_GradiusMod_Extensions_InitializeDroneModelComponents(UnityEngine_GameObject_RoR2_CharacterBody_float_bool)_debug'></a>
`debug` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
Set to true to attach the Material Controller for modifying materials in-game.
  
