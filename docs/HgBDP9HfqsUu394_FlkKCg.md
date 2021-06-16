#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')
## DroneInfo Struct
A structure that stores data of custom drones as well as where they originated from.  
```csharp
public struct DroneInfo :
System.IEquatable<Chen.GradiusMod.Drones.DroneInfo>
```

Implements [System.IEquatable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')[DroneInfo](HgBDP9HfqsUu394_FlkKCg 'Chen.GradiusMod.Drones.DroneInfo')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')  

| Constructors | |
| :--- | :--- |
| [DroneInfo(string, Drone, ConfigFile)](LIAmd3gVRwFC4_RvHHGoNw 'Chen.GradiusMod.Drones.DroneInfo.DroneInfo(string, Chen.GradiusMod.Drones.Drone, BepInEx.Configuration.ConfigFile)') | Basic constructor that stores the data of a custom drone.<br/> |

| Fields | |
| :--- | :--- |
| [instance](gKlEitv9wr5HRnmDWvPRag 'Chen.GradiusMod.Drones.DroneInfo.instance') | The instance of a Drone.<br/> |
| [mod](PGy9DMpFoxYK9n1Dh99fog 'Chen.GradiusMod.Drones.DroneInfo.mod') | Mod identifier for differentiation, preferably the GUID.<br/> |

| Methods | |
| :--- | :--- |
| [Equals(DroneInfo)](VHGZaA99JSCZxKajcQg5Wg 'Chen.GradiusMod.Drones.DroneInfo.Equals(Chen.GradiusMod.Drones.DroneInfo)') | Compares this instance and the other to see if they are "equal" as defined.<br/>For equality, always use this method instead of equality operators.<br/> |
