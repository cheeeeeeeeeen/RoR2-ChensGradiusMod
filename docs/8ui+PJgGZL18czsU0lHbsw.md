
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Drones](./Y-iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')

## DroneDeathState Class
An Entity State that should inherit from the original EntityStates.Drone.DeathState.  
The original code does not support custom spawn cards to be detected when dying so that the interactable can spawn again.  
This state will cater to custom drones so they are also able to spawn interactables upon death.  
Do not use this class directly. Always inherit from this class and implement the interactable property.  
```csharp
public class DroneDeathState : DeathState
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [EntityStates.EntityState](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.EntityState 'EntityStates.EntityState') &#129106; [EntityStates.BaseState](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.BaseState 'EntityStates.BaseState') &#129106; [EntityStates.GenericCharacterDeath](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.GenericCharacterDeath 'EntityStates.GenericCharacterDeath') &#129106; [EntityStates.Drone.DeathState](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.Drone.DeathState 'EntityStates.Drone.DeathState') &#129106; DroneDeathState  

### Methods
- [FixedUpdate()](./2AqJyYy-iWUfEyKdaA+9Rw 'Chen.GradiusMod.Drones.DroneDeathState.FixedUpdate()')
- [GetInteractableSpawnCard()](./esfQyw8WQL8EuByjF63uyQ 'Chen.GradiusMod.Drones.DroneDeathState.GetInteractableSpawnCard()')
- [OnEnter()](./FPuV2SVv3WwBJucxoS5Gmg 'Chen.GradiusMod.Drones.DroneDeathState.OnEnter()')
- [OnExit()](./nPq+dCp76qxAoa7RhXmUOQ 'Chen.GradiusMod.Drones.DroneDeathState.OnExit()')
- [OnImpactServer(UnityEngine.Vector3)](./DqMybqvd0GvG24TnEiDQOw 'Chen.GradiusMod.Drones.DroneDeathState.OnImpactServer(UnityEngine.Vector3)')
- [OnInteractableSpawn(UnityEngine.GameObject)](./8rmrVlQPh8iJSQwNPDpGMw 'Chen.GradiusMod.Drones.DroneDeathState.OnInteractableSpawn(UnityEngine.GameObject)')
