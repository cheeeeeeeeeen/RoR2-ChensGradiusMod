#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')
## Drone Class
The drone class where mod creators should inherit from to ease up development.  
```csharp
public abstract class Drone
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; Drone  

Derived  
&#8627; [Drone&lt;T&gt;](UWAul_yMUbN+3325jv26NQ 'Chen.GradiusMod.Drones.Drone&lt;T&gt;')  

| Fields | |
| :--- | :--- |
| [config](4Wedup2526hIGF2cJs8kUA 'Chen.GradiusMod.Drones.Drone.config') | The config file assigned to this custom drone. Use this to bind config options.<br/> |

| Properties | |
| :--- | :--- |
| [affectedByDroneRepairKit](LtK+HDvGfEOAosyKriPOxA 'Chen.GradiusMod.Drones.Drone.affectedByDroneRepairKit') | Chen's Classic Items Compatibility: Determines if this drone can be healed by Drone Repair Kit.<br/> |
| [alreadySetup](N9oYEvtp1qDnSIojdZai9Q 'Chen.GradiusMod.Drones.Drone.alreadySetup') | Used to determine if the custom drone was already set up.<br/> |
| [canBeInspired](LxERhX2G1JKf7yIDtI0HSg 'Chen.GradiusMod.Drones.Drone.canBeInspired') | Aetherium Compatibility: Determines if this drone can be inspired by the Inspiring Drone.<br/> |
| [canHaveOptions](XkBw6JYh+1Iu4w4pFJ9EXg 'Chen.GradiusMod.Drones.Drone.canHaveOptions') | Determines if the drone can be spawned with Gradius' Options. Required to explicitly implement.<br/> |
| [configCategory](oCCPZRl2yRxO1ooRx17R8g 'Chen.GradiusMod.Drones.Drone.configCategory') | The category that will be used in the config file that contains the custom drone's config options.<br/> |
| [DroneCharacterMasterObject](A1tlRZLu0v_MEW2ubLUQRQ 'Chen.GradiusMod.Drones.Drone.DroneCharacterMasterObject') | This refers to the CharacterMaster GameObject of the drone.<br/>Implement this method in the drone class and have it return the CharacterMaster GameObject.<br/> |
| [enabled](xPmiKVc3dVangaNc4oESVw 'Chen.GradiusMod.Drones.Drone.enabled') | Determines if the drone should be enabled/disabled. Disabled drones will not be set up.<br/> |
| [name](g7Gy6uLkkvXY1NMU+razzw 'Chen.GradiusMod.Drones.Drone.name') | Fetches the custom drone's class name.<br/> |
| [spawnWeightWithMachinesArtifact](vLBa8EK1Y++L9uxr5rvwjw 'Chen.GradiusMod.Drones.Drone.spawnWeightWithMachinesArtifact') | Determines if the drone can be spawned in the enemy drone spawn pool with Artifact of Machines.<br/> |

| Methods | |
| :--- | :--- |
| [PostSetup()](KUFSuWDwAMIaslnupDsZ6A 'Chen.GradiusMod.Drones.Drone.PostSetup()') | The fifth step in the setup process. Place here the code for cleanup, or for finalization.<br/>This will still be performed whether the drone is enabled or disabled.<br/>This will still also be performed if the drone was already set up or not.<br/> |
| [PreSetup()](3zKKz0n2lFUXR+_amkFWnQ 'Chen.GradiusMod.Drones.Drone.PreSetup()') | The first step in the setup process. Place here the logic needed before any processing begins.<br/> |
| [SetupBehavior()](V5iY9ZIU3NkhftRxvU7CZw 'Chen.GradiusMod.Drones.Drone.SetupBehavior()') | The fourth step in the setup process. Place here the code related to the drone's behavior.<br/>One may place here mod compatibility code. Hooks should also go here.<br/> |
| [SetupComponents()](yhN8rCGbqdXblfim0mCg1w 'Chen.GradiusMod.Drones.Drone.SetupComponents()') | The third step in the setup process. Place here all initialization of components, assets, textures, sounds, etc.<br/> |
| [SetupConfig()](7ib30zyZZcXZBiDl7uyslg 'Chen.GradiusMod.Drones.Drone.SetupConfig()') | The second step in the setup process. Place here all the code related to adding configurations for the custom drone.<br/> |
| [SetupFirstPhase()](YiZhTbgkH2NOfoJMti_spg 'Chen.GradiusMod.Drones.Drone.SetupFirstPhase()') | First phase of the setup process along with required logic. This phase invokes SetupConfig.<br/>This method is exposed for usage outside of this class.<br/> |
| [SetupSecondPhase()](aUDSadqxuQxXUVi+QYZnZg 'Chen.GradiusMod.Drones.Drone.SetupSecondPhase()') | Second phase of the setup process along with required logic. This method invokes SetupComponents.<br/>This method is exposed for usage outside of this class.<br/> |
| [SetupThirdPhase()](I_gdruxMKT+FtcN6OrqEGQ 'Chen.GradiusMod.Drones.Drone.SetupThirdPhase()') | Third phase of the setup process along with required logic. This method invokes SetupBehavior.<br/>This method is exposed for usage outside of this class.<br/> |
