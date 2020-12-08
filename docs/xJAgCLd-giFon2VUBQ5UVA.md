
### [Chen.GradiusMod](./neHTXX+yFsk1RpXqjkv9zg 'Chen.GradiusMod')

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
- [FixedUpdate()](./HSAiwKH6CzYwXUH6L8nWEg 'Chen.GradiusMod.DroneDeathState.FixedUpdate()')
- [GetInteractableSpawnCard()](./j5rhbILFIzGleFsftDOWXw 'Chen.GradiusMod.DroneDeathState.GetInteractableSpawnCard()')
- [OnEnter()](./za-CHMT7u9CxZHElgoy0sw 'Chen.GradiusMod.DroneDeathState.OnEnter()')
- [OnExit()](./c6qAlnPLdOqE-x1GTcWZNg 'Chen.GradiusMod.DroneDeathState.OnExit()')
- [OnImpactServer(UnityEngine.Vector3)](./vFgk0XiUfnVfl9QLwI5D5A 'Chen.GradiusMod.DroneDeathState.OnImpactServer(UnityEngine.Vector3)')
- [OnInteractableSpawn(UnityEngine.GameObject)](./7Qd0ZmHjohD7FLDzdC9AIw 'Chen.GradiusMod.DroneDeathState.OnInteractableSpawn(UnityEngine.GameObject)')
