#### [ChensGradiusMod](index 'index')
## Chen.GradiusMod.Drones Namespace

| Classes | |
| :--- | :--- |
| [BodyRotation](cavriQuBntm0cE4AZ1RD+w 'Chen.GradiusMod.Drones.BodyRotation') | A component that allows the model to be rotated along the Z-axis.<br/>May be useful to certain custom drones and some behavioral effects.<br/> |
| [Drone](o+an11PxrqGB40HSHXgvpQ 'Chen.GradiusMod.Drones.Drone') | The drone class where mod creators should inherit from to ease up development.<br/> |
| [Drone&lt;T&gt;](UWAul_yMUbN+3325jv26NQ 'Chen.GradiusMod.Drones.Drone&lt;T&gt;') | Allows for making drone classes into singleton classes.<br/> |
| [DroneCatalog](qPWMsXW14ySl71rXQaL2KQ 'Chen.GradiusMod.Drones.DroneCatalog') | A static class that caters initializing and registering custom drones.<br/> |
| [DroneDeathState](8ui+PJgGZL18czsU0lHbsw 'Chen.GradiusMod.Drones.DroneDeathState') | An Entity State that inherits from the original EntityStates.Drone.DeathState.<br/>It spawns the drone's broken interactable upon death the same way vanilla drones do, including drone upgrades from the Drone Combiner,<br/>while letting custom drones decide which spawn card to use and if the interactable should spawn at all.<br/>Do not use this class directly. Always inherit from this class and implement the interactable property.<br/> |
| [SineFlicker](gODJKXdVL5Utx1IRTLZr6w 'Chen.GradiusMod.Drones.SineFlicker') | An abstract component that needs to be implemented.<br/>It allows the Light component to flicker in a sine math function pattern.<br/> |

| Structs | |
| :--- | :--- |
| [DroneInfo](HgBDP9HfqsUu394_FlkKCg 'Chen.GradiusMod.Drones.DroneInfo') | A structure that stores data of custom drones as well as where they originated from.<br/> |
