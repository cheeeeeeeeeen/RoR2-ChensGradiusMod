#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')
## DroneDeathState Class
An Entity State that should inherit from the original EntityStates.Drone.DeathState.  
The original code does not support custom spawn cards to be detected when dying so that the interactable can spawn again.  
This state will cater to custom drones so they are also able to spawn interactables upon death.  
Do not use this class directly. Always inherit from this class and implement the interactable property.  
```csharp
public class DroneDeathState : EntityStates.Drone.DeathState
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [EntityStates.EntityState](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.EntityState 'EntityStates.EntityState') &#129106; [EntityStates.BaseState](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.BaseState 'EntityStates.BaseState') &#129106; [EntityStates.GenericCharacterDeath](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.GenericCharacterDeath 'EntityStates.GenericCharacterDeath') &#129106; [EntityStates.Drone.DeathState](https://docs.microsoft.com/en-us/dotnet/api/EntityStates.Drone.DeathState 'EntityStates.Drone.DeathState') &#129106; DroneDeathState  

| Methods | |
| :--- | :--- |
| [FixedUpdate()](2AqJyYy_iWUfEyKdaA+9Rw 'Chen.GradiusMod.Drones.DroneDeathState.FixedUpdate()') | Overrideable OnEnter method from the original state. Always call base.FixedUpdate.<br/> |
| [GetInteractableSpawnCard()](esfQyw8WQL8EuByjF63uyQ 'Chen.GradiusMod.Drones.DroneDeathState.GetInteractableSpawnCard()') | A method that should be implemented by the child class. This will be the Spawn Card that will be used to spawn when the drone is destroyed.<br/> |
| [OnEnter()](FPuV2SVv3WwBJucxoS5Gmg 'Chen.GradiusMod.Drones.DroneDeathState.OnEnter()') | Overrideable OnEnter method from the original state. Always call base.OnEnter. Initialize values at runtime here.<br/>To perform the death behavior specified in OnImpactServer, destroyOnImpact must be set to true.<br/>This method already does that so long as base.OnEnter is invoked.<br/> |
| [OnExit()](nPq+dCp76qxAoa7RhXmUOQ 'Chen.GradiusMod.Drones.DroneDeathState.OnExit()') | Overrideable OnEnter method from the original state. Always call base.OnExit.<br/> |
| [OnImpactServer(Vector3)](DqMybqvd0GvG24TnEiDQOw 'Chen.GradiusMod.Drones.DroneDeathState.OnImpactServer(UnityEngine.Vector3)') | Overridden method from the original state so that it would instead spawn the specified interactable's spawn card.<br/>There is no need to override this unless special behavior is needed.<br/> |
| [OnInteractableSpawn(GameObject)](8rmrVlQPh8iJSQwNPDpGMw 'Chen.GradiusMod.Drones.DroneDeathState.OnInteractableSpawn(UnityEngine.GameObject)') | A method that can be overridden to add or change the logic when the interactable is spawned.<br/>Default logic is to compute for the scaled cost of the drone.<br/> |
