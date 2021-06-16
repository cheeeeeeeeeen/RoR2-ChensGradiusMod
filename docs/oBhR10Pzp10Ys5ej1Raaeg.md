#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.GradiusOption](mfb9nYomeqOwYy2EkL_v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](Vui7fzQ6K+_c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')
## GradiusOption.OptionSync(CharacterBody, Action&lt;NetworkIdentity,OptionTracker&gt;, bool) Method
Method that provides the Network Identity and Option Tracker for easier syncing. Sync logic should be provided in actionToRun.  
```csharp
public void OptionSync(RoR2.CharacterBody optionOwner, System.Action<UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker> actionToRun, bool queryTracker=true);
```
#### Parameters
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_OptionSync(RoR2_CharacterBody_System_Action_UnityEngine_Networking_NetworkIdentity_Chen_GradiusMod_Items_GradiusOption_Components_OptionTracker__bool)_optionOwner'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the option.
  
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_OptionSync(RoR2_CharacterBody_System_Action_UnityEngine_Networking_NetworkIdentity_Chen_GradiusMod_Items_GradiusOption_Components_OptionTracker__bool)_actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')[UnityEngine.Networking.NetworkIdentity](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Networking.NetworkIdentity 'UnityEngine.Networking.NetworkIdentity')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')[OptionTracker](u7j6jwd4UkMG2C3FwVR27w 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')  
The sync action to perform. Inputs are as follows: NetworkIdentity optionIdentity, OptionTracker tracker.
  
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_OptionSync(RoR2_CharacterBody_System_Action_UnityEngine_Networking_NetworkIdentity_Chen_GradiusMod_Items_GradiusOption_Components_OptionTracker__bool)_queryTracker'></a>
`queryTracker` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
If true, the Option tracker is automatically queried. If false, the Option tracker will not be queried.
  
