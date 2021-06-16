#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Drones](Y_iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones')
## BodyRotation Class
A component that allows the model to be rotated along the Z-axis.  
May be useful to certain custom drones and some behavioral effects.  
```csharp
public class BodyRotation : UnityEngine.MonoBehaviour
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [UnityEngine.Object](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Object 'UnityEngine.Object') &#129106; [UnityEngine.Component](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Component 'UnityEngine.Component') &#129106; [UnityEngine.Behaviour](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Behaviour 'UnityEngine.Behaviour') &#129106; [UnityEngine.MonoBehaviour](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.MonoBehaviour 'UnityEngine.MonoBehaviour') &#129106; BodyRotation  

| Fields | |
| :--- | :--- |
| [accelerate](E86bvyPfyMQb26x7+Ww8Lw 'Chen.GradiusMod.Drones.BodyRotation.accelerate') | A flag to toggle if the model should accelerate in rotating.<br/> |
| [acceleration](9t7lCfOBBQkBEr1PEtSEQQ 'Chen.GradiusMod.Drones.BodyRotation.acceleration') | The rate at which the rotation speed will accelerate.<br/> |
| [maxRotationSpeed](GgJv_FXaWaYhKXPLZOGQ9Q 'Chen.GradiusMod.Drones.BodyRotation.maxRotationSpeed') | The maximum rotation speed that the model can achieve.<br/> |
| [rotationDirection](WddzZTbWJ3bSJM_AceNdPQ 'Chen.GradiusMod.Drones.BodyRotation.rotationDirection') | The direction of the rotation. This should only be 1, 0 or -1.<br/>Anything less or greater will cause faster rotation. 0 will not let it rotate.<br/> |
| [rotationSpeed](Szw5dfT3w5+yTFvtBvIS3A 'Chen.GradiusMod.Drones.BodyRotation.rotationSpeed') | The constant base speed of which the object will rotate around the Z-axis.<br/> |
