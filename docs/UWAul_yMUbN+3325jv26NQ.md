
#### [ChensGradiusMod](index 'index')

### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')

## Drone&lt;T&gt; Class
Allows for making drone classes into singleton classes.  
```csharp
public abstract class Drone<T> : Chen.GradiusMod.Drones.Drone
    where T : Chen.GradiusMod.Drones.Drone<T>
```

#### Type parameters
<a name='Chen_GradiusMod_Drones_Drone_T__T'></a>
`T`  
The drone class name
  

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [Drone](o+an11PxrqGB40HSHXgvpQ 'Chen.GradiusMod.Drones.Drone') &#129106; Drone&lt;T&gt;  

| Constructors | |
| :--- | :--- |
| [Drone()](sXfQkXRO1ML6lPAL8AIK_g 'Chen.GradiusMod.Drones.Drone&lt;T&gt;.Drone()') | Constructor that creates the instance of the singleton class.<br/> |

| Properties | |
| :--- | :--- |
| [instance](JiNCVRg0sq3LIHTN0Xwc_A 'Chen.GradiusMod.Drones.Drone&lt;T&gt;.instance') | The instance of the singleton class.<br/> |
