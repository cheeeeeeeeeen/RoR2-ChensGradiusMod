
### [Chen.GradiusMod](./Chen-GradiusMod 'Chen.GradiusMod').[GradiusOption](./Chen-GradiusMod-GradiusOption 'Chen.GradiusMod.GradiusOption')

## GradiusOption.LoopAllMinions(RoR2.CharacterMaster, System.Action&lt;UnityEngine.GameObject&gt;) Method
Loops through the all the minions of the owner.  
```csharp
public void LoopAllMinions(RoR2.CharacterMaster ownerMaster, System.Action<UnityEngine.GameObject> actionToRun);
```

#### Parameters
<a name='Chen-GradiusMod-GradiusOption-LoopAllMinions(RoR2-CharacterMaster_System-Action-UnityEngine-GameObject-)-ownerMaster'></a>
`ownerMaster` [RoR2.CharacterMaster](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterMaster 'RoR2.CharacterMaster')  
The owner of the minions.  
  
<a name='Chen-GradiusMod-GradiusOption-LoopAllMinions(RoR2-CharacterMaster_System-Action-UnityEngine-GameObject-)-actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-1 'System.Action`1')  
An action to execute for each minion. The minion's CharacterBody GameObject is given as the input.  
  
