#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.GradiusOption.Components](3b19l5ocTqQsEH2QAbTnXQ 'Chen.GradiusMod.Items.GradiusOption.Components')
## OptionBehavior Class
A component attached to the Options/Multiples for their behavioral functions.  
```csharp
public class OptionBehavior
```

Inheritance [UnityEngine.MonoBehaviour](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.MonoBehaviour 'UnityEngine.MonoBehaviour') &#129106; OptionBehavior  

| Fields | |
| :--- | :--- |
| [numbering](LL3U18iiScRLNCdMslZJ8A 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.numbering') | The number that represents the identification of the Option scoped under the owner.<br/> |
| [objectData](9rJODcSt3n4dc8tnvT0HLA 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.objectData') | Useful for native objects or class instances that needs to be saved from one state to another of the owner.<br/>Utilizing this means that one does not need to create and attach a component for storing these objects.<br/>Casting is required when the object is accessed.<br/> |
| [owner](iGOzPfYJ7tQzCVKdE5AWJA 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.owner') | The Character Body Game Object of this Option's owner.<br/> |
| [unityData](_zvR3eC84B0IbOC+zBCFdQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.unityData') | Useful for storing prefabs, components, scriptable objects that needs to be saved from one state to another of the owner.<br/>Utilizing this means that one does not need to create and attach a component for storing these objects.<br/>Using this dictionary allows objects here to be checked using the Unity way.<br/> |

| Properties | |
| :--- | :--- |
| [O](gIeAnpHCoZ9xekNkaktEjQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.O') | Shorthand for the Object data dictionary.<br/> |
| [U](z_8xYfioFWF0Dfu93d43mQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.U') | Shorthand for the Unity data dictionary.<br/> |

| Methods | |
| :--- | :--- |
| [DecidePosition(float)](_PwxV_eu4pTTt4GnyV7N4A 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior.DecidePosition(float)') | Computes for the actual position of the Option based on the owner's rotational variables and its numbering.<br/> |
