
#### [ChensGradiusMod](index 'index')

### [Chen.GradiusMod.Items.GradiusOption.Components](3b19l5ocTqQsEH2QAbTnXQ 'Chen.GradiusMod.Items.GradiusOption.Components')

## OptionTracker Class
A component attached to a Character Body that may own Options/Multiples.  
The mod handles attaching the component when necessary.  
```csharp
public class OptionTracker : UnityEngine.MonoBehaviour
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [UnityEngine.Object](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Object 'UnityEngine.Object') &#129106; [UnityEngine.Component](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Component 'UnityEngine.Component') &#129106; [UnityEngine.Behaviour](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Behaviour 'UnityEngine.Behaviour') &#129106; [UnityEngine.MonoBehaviour](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.MonoBehaviour 'UnityEngine.MonoBehaviour') &#129106; OptionTracker  

| Properties | |
| :--- | :--- |
| [characterBody](YON+f77XHm6ZtibeGm2vsQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker.characterBody') | Character Body of this Game Object.<br/> |
| [characterMaster](wT7ZMA0Ime9q3lFe5ekRQQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker.characterMaster') | Character Master of this Game Object.<br/> |
| [currentOptionAngle](yfr0dgT7IBo6Yq1pGcYWpg 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker.currentOptionAngle') | Property that stores the current positional angle from the owner.<br/>Useful for determining patterns relative to the Option's angle.<br/> |
| [masterCharacterMaster](dV+IKPvJP74L86etItJcPw 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker.masterCharacterMaster') | The Character Master of this Game Object's owner through Minion Ownership component.<br/> |

| Methods | |
| :--- | :--- |
| [GetRotateMultiplier()](kE17beTi6EX2Fk0BNbzRJQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker.GetRotateMultiplier()') | Fetches the rotational distance and speed multiplier for this object's Options.<br/> |
| [GetRotateOffset()](6wV_orjNtphNFyfHpvgQpQ 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker.GetRotateOffset()') | Fetches the rotational central offset for this object's Options.<br/> |
