
### [Chen.GradiusMod](./Chen-GradiusMod 'Chen.GradiusMod').[GradiusOption](./Chen-GradiusMod-GradiusOption 'Chen.GradiusMod.GradiusOption')

## GradiusOption.FireForAllOptions(RoR2.CharacterBody, System.Action&lt;UnityEngine.GameObject,Chen.GradiusMod.OptionBehavior,UnityEngine.GameObject&gt;) Method
Loops through all the Options of the minion.  
```csharp
public void FireForAllOptions(RoR2.CharacterBody optionOwner, System.Action<UnityEngine.GameObject,Chen.GradiusMod.OptionBehavior,UnityEngine.GameObject> actionToRun);
```

#### Parameters
<a name='Chen-GradiusMod-GradiusOption-FireForAllOptions(RoR2-CharacterBody_System-Action-UnityEngine-GameObject_Chen-GradiusMod-OptionBehavior_UnityEngine-GameObject-)-optionOwner'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the option.  
  
<a name='Chen-GradiusMod-GradiusOption-FireForAllOptions(RoR2-CharacterBody_System-Action-UnityEngine-GameObject_Chen-GradiusMod-OptionBehavior_UnityEngine-GameObject-)-actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[OptionBehavior](./Chen-GradiusMod-OptionBehavior 'Chen.GradiusMod.OptionBehavior')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-3 'System.Action`3')  
An action to execute for each Option. The inputs are as follows: GameObject option, OptionBehavior behavior, GameObject target.  
  
