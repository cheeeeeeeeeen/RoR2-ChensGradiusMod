
#### [ChensGradiusMod](index 'index')

### [Chen.GradiusMod.Artifacts.Machines](ayrCd5wE1fGIQOox6GFHYA 'Chen.GradiusMod.Artifacts.Machines')

## Machines Class
As artifact class which provides the main API related to the controlling which drones the enemy may spawn with. It is powered by TILER2.  
```csharp
public class Machines : TILER2.Artifact<Chen.GradiusMod.Artifacts.Machines.Machines>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [TILER2.AutoConfigContainer](https://docs.microsoft.com/en-us/dotnet/api/TILER2.AutoConfigContainer 'TILER2.AutoConfigContainer') &#129106; [TILER2.T2Module](https://docs.microsoft.com/en-us/dotnet/api/TILER2.T2Module 'TILER2.T2Module') &#129106; [TILER2.CatalogBoilerplate](https://docs.microsoft.com/en-us/dotnet/api/TILER2.CatalogBoilerplate 'TILER2.CatalogBoilerplate') &#129106; [TILER2.Artifact](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Artifact 'TILER2.Artifact') &#129106; [TILER2.Artifact&lt;](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Artifact-1 'TILER2.Artifact`1')[Machines](06BKrroboYsdkfWNwbWj1A 'Chen.GradiusMod.Artifacts.Machines.Machines')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Artifact-1 'TILER2.Artifact`1') &#129106; Machines  

| Methods | |
| :--- | :--- |
| [AddEnemyDroneType(GameObject, int)](uwkirow3FE0V6ozwq7FSSQ 'Chen.GradiusMod.Artifacts.Machines.Machines.AddEnemyDroneType(UnityEngine.GameObject, int)') | Adds a drone to the spawn pool for the enemies.<br/> |
| [AssignEnemyDroneAsGrounded(string)](ujVsLUGxMgrFCppqzzg4zQ 'Chen.GradiusMod.Artifacts.Machines.Machines.AssignEnemyDroneAsGrounded(string)') | Assigns a drone as grounded. This will be used to determine if the drone should be repositioned to the ground to avoid floating stationary drones.<br/> |
| [RemoveEnemyDroneType(GameObject)](Jb22Pbm943Sl+FSDV7KCiA 'Chen.GradiusMod.Artifacts.Machines.Machines.RemoveEnemyDroneType(UnityEngine.GameObject)') | Removes a drone from the spawn pool for the enemies.<br/> |
| [UnassignEnemyDroneAsGrounded(string)](XJdp1Eubm6eDxnF6IddfZw 'Chen.GradiusMod.Artifacts.Machines.Machines.UnassignEnemyDroneAsGrounded(string)') | Unassigns a drone from being grounded.<br/> |
