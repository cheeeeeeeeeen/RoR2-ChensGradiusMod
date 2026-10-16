#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[Drone](o+an11PxrqGB40HSHXgvpQ 'Chen.GradiusMod.Drones.Drone')
## Drone.CreateDroneDef(DroneDef, GameObject, GameObject, InteractableSpawnCard, string, Texture, GameObject, DroneDef) Method
Creates and registers the DroneDef of the custom drone, and links it to the broken interactable so it behaves like a vanilla drone.  
The language tokens used are {tokenPrefix}_NAME, {tokenPrefix}_PICKUP, {tokenPrefix}_DESCRIPTION and {tokenPrefix}_SKILL_DESCRIPTION.  
```csharp
protected DroneDef CreateDroneDef(DroneDef tierTemplate, GameObject bodyPrefab, GameObject brokenObject, InteractableSpawnCard spawnCard, string tokenPrefix, Texture icon, GameObject displayPrefab, DroneDef commandTemplate=null);
```
#### Parameters
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_tierTemplate'></a>
`tierTemplate` [RoR2.DroneDef](https://docs.microsoft.com/en-us/dotnet/api/RoR2.DroneDef 'RoR2.DroneDef')  
A vanilla DroneDef where the tier, type and other gameplay flags are copied from.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_bodyPrefab'></a>
`bodyPrefab` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The CharacterBody prefab of the drone.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_brokenObject'></a>
`brokenObject` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The broken interactable prefab of the drone.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_spawnCard'></a>
`spawnCard` [RoR2.InteractableSpawnCard](https://docs.microsoft.com/en-us/dotnet/api/RoR2.InteractableSpawnCard 'RoR2.InteractableSpawnCard')  
The spawn card of the broken interactable.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_tokenPrefix'></a>
`tokenPrefix` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The prefix of the language tokens of the drone.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_icon'></a>
`icon` [UnityEngine.Texture](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Texture 'UnityEngine.Texture')  
The icon of the drone.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_displayPrefab'></a>
`displayPrefab` [UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')  
The model displayed when the drone is a pickup. Use CreatePickupDisplay to make one.
  
<a name='Chen_GradiusMod_Drones_Drone_CreateDroneDef(DroneDef_GameObject_GameObject_InteractableSpawnCard_string_Texture_GameObject_DroneDef)_commandTemplate'></a>
`commandTemplate` [RoR2.DroneDef](https://docs.microsoft.com/en-us/dotnet/api/RoR2.DroneDef 'RoR2.DroneDef')  
The vanilla DroneDef the drone body was cloned from. Its Drone Tech command text is reused since the command behavior is inherited.
  
#### Returns
[RoR2.DroneDef](https://docs.microsoft.com/en-us/dotnet/api/RoR2.DroneDef 'RoR2.DroneDef')  
The registered DroneDef
