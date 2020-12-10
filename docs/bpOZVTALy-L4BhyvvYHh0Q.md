
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Items.GradiusOption](./mfb9nYomeqOwYy2EkL-v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](./Vui7fzQ6K+-c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')

## GradiusOption.FireForAllOptions(RoR2.CharacterBody, System.Action&lt;UnityEngine.GameObject,Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior,UnityEngine.GameObject,UnityEngine.Vector3&gt;) Method
Loops through all the Options of the minion. The action has 4 useful parameters to use.  
The first parameter refers to the Option/Multiple itself. It is a GameObject.  
The second parameter refers to the OptionBehavior component in the Option/Multiple.  
The third parameter refers to the target of the Option/Multiple owner. It is also a GameObject.  
The last parameter refers to the direction from the option to the target. It is a normalized Vector3.  
```csharp
public void FireForAllOptions(RoR2.CharacterBody optionOwner, System.Action<UnityEngine.GameObject,Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior,UnityEngine.GameObject,UnityEngine.Vector3> actionToRun);
```

#### Parameters
<a name='Uyq6VrqyM4WsYKytSJ+vwg'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the option.  
  
<a name='8NINrmAeAXIePgDEMROHqA'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[OptionBehavior](./cwz-G2wxzba4Id7zOi0Rig 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.Vector3](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Vector3 'UnityEngine.Vector3')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')  
An action to execute for each Option. The inputs are as follows:  
            GameObject option, OptionBehavior behavior, GameObject target, Vector3 direction.  
  
