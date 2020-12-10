
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Items.GradiusOption](./mfb9nYomeqOwYy2EkL-v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](./Vui7fzQ6K+-c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')

## GradiusOption.FireForAllOptions(RoR2.CharacterBody, System.Action&lt;UnityEngine.GameObject,Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior,UnityEngine.GameObject&gt;) Method
Loops through all the Options of the minion. Always do a null check on the target parameter of actionToRun.  
```csharp
public void FireForAllOptions(RoR2.CharacterBody optionOwner, System.Action<UnityEngine.GameObject,Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior,UnityEngine.GameObject> actionToRun);
```

#### Parameters
<a name='nGnip6lvEd-ovDRIx4u86w'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the option.  
  
<a name='UX0RILKpMZ8d93+74LkJ2g'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[OptionBehavior](./cwz-G2wxzba4Id7zOi0Rig 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')  
An action to execute for each Option. The inputs are as follows: GameObject option, OptionBehavior behavior, GameObject target.  
  
