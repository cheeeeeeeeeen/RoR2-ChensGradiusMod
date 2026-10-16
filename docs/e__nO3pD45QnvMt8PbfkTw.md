#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.GradiusOption](mfb9nYomeqOwYy2EkL_v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](Vui7fzQ6K+_c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')
## GradiusOption.ForEachOption(CharacterBody, Action&lt;GameObject,OptionBehavior,GameObject,Vector3&gt;) Method
Same as FireForAllOptions, but also runs for minions that use automatic mirroring.  
Only use this for attacks that the automatic mirroring cannot copy, such as orbs.  
```csharp
internal void ForEachOption(CharacterBody optionOwner, System.Action<GameObject,Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior,GameObject,Vector3> actionToRun);
```
#### Parameters
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_ForEachOption(CharacterBody_System_Action_GameObject_Chen_GradiusMod_Items_GradiusOption_Components_OptionBehavior_GameObject_Vector3_)_optionOwner'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
  
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_ForEachOption(CharacterBody_System_Action_GameObject_Chen_GradiusMod_Items_GradiusOption_Components_OptionBehavior_GameObject_Vector3_)_actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[OptionBehavior](cwz_G2wxzba4Id7zOi0Rig 'Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.Vector3](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Vector3 'UnityEngine.Vector3')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')  
  
