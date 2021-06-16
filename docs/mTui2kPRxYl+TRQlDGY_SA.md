#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.GradiusOption](mfb9nYomeqOwYy2EkL_v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](Vui7fzQ6K+_c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')
## GradiusOption.LoopAllMinions(CharacterMaster, Action&lt;GameObject&gt;) Method
Loops through the all the minions of the owner.  
```csharp
public void LoopAllMinions(RoR2.CharacterMaster ownerMaster, System.Action<UnityEngine.GameObject> actionToRun);
```
#### Parameters
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_LoopAllMinions(RoR2_CharacterMaster_System_Action_UnityEngine_GameObject_)_ownerMaster'></a>
`ownerMaster` [RoR2.CharacterMaster](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterMaster 'RoR2.CharacterMaster')  
The owner of the minions.
  
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_LoopAllMinions(RoR2_CharacterMaster_System_Action_UnityEngine_GameObject_)_actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')  
An action to execute for each minion. The minion's CharacterBody GameObject is given as the input.
  
