#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Artifacts.Machines](ayrCd5wE1fGIQOox6GFHYA 'Chen.GradiusMod.Artifacts.Machines')
## Machines Class
An artifact class which provides the main API related to controlling which drones the enemy may spawn with.  
```csharp
public class Machines : Chen.GradiusMod.Artifacts.ArtifactBase<Chen.GradiusMod.Artifacts.Machines.Machines>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase') &#129106; [ArtifactBase](WBEZFsK1dXU87InG+LPPlg 'Chen.GradiusMod.Artifacts.ArtifactBase') &#129106; [Chen.GradiusMod.Artifacts.ArtifactBase&lt;](mnk9amYhJthJPbRV9qBcAQ 'Chen.GradiusMod.Artifacts.ArtifactBase&lt;T&gt;')[Machines](06BKrroboYsdkfWNwbWj1A 'Chen.GradiusMod.Artifacts.Machines.Machines')[&gt;](mnk9amYhJthJPbRV9qBcAQ 'Chen.GradiusMod.Artifacts.ArtifactBase&lt;T&gt;') &#129106; Machines  

| Methods | |
| :--- | :--- |
| [AddEnemyDroneType(GameObject, int)](rBlf05VZ6caiITwwVUBbzA 'Chen.GradiusMod.Artifacts.Machines.Machines.AddEnemyDroneType(GameObject, int)') | Adds a drone to the spawn pool for the enemies.<br/> |
| [AssignEnemyDroneAsGrounded(string)](ujVsLUGxMgrFCppqzzg4zQ 'Chen.GradiusMod.Artifacts.Machines.Machines.AssignEnemyDroneAsGrounded(string)') | Assigns a drone as grounded. This will be used to determine if the drone should be repositioned to the ground to avoid floating stationary drones.<br/> |
| [RemoveEnemyDroneType(GameObject)](1LqF8kU18SL6Omn8Y+dMoA 'Chen.GradiusMod.Artifacts.Machines.Machines.RemoveEnemyDroneType(GameObject)') | Removes a drone from the spawn pool for the enemies.<br/> |
| [ShouldSuppressBrokenDrone(CharacterBody)](NOVk1iRBqyPzjS6pScFOnw 'Chen.GradiusMod.Artifacts.Machines.Machines.ShouldSuppressBrokenDrone(CharacterBody)') | Checks if the drone should not leave a repurchasable broken drone behind upon death.<br/>Drones owned by enemies, and drones that were given by this artifact, are never repurchasable.<br/> |
| [UnassignEnemyDroneAsGrounded(string)](XJdp1Eubm6eDxnF6IddfZw 'Chen.GradiusMod.Artifacts.Machines.Machines.UnassignEnemyDroneAsGrounded(string)') | Unassigns a drone from being grounded.<br/> |
