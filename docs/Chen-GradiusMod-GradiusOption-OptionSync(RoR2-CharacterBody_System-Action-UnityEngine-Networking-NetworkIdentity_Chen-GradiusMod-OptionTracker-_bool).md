
### [Chen.GradiusMod](./Chen-GradiusMod 'Chen.GradiusMod').[GradiusOption](./Chen-GradiusMod-GradiusOption 'Chen.GradiusMod.GradiusOption')

## GradiusOption.OptionSync(RoR2.CharacterBody, System.Action&lt;UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.OptionTracker&gt;, bool) Method
Method that provides the Network Identity and Option Tracker for easier syncing. Sync logic should be provided in actionToRun.  
```csharp
public void OptionSync(RoR2.CharacterBody optionOwner, System.Action<UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.OptionTracker> actionToRun, bool queryTracker=true);
```

#### Parameters
<a name='Chen-GradiusMod-GradiusOption-OptionSync(RoR2-CharacterBody_System-Action-UnityEngine-Networking-NetworkIdentity_Chen-GradiusMod-OptionTracker-_bool)-optionOwner'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the option.  
  
<a name='Chen-GradiusMod-GradiusOption-OptionSync(RoR2-CharacterBody_System-Action-UnityEngine-Networking-NetworkIdentity_Chen-GradiusMod-OptionTracker-_bool)-actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')[UnityEngine.Networking.NetworkIdentity](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Networking.NetworkIdentity 'UnityEngine.Networking.NetworkIdentity')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')[OptionTracker](./Chen-GradiusMod-OptionTracker 'Chen.GradiusMod.OptionTracker')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')  
The sync action to perform. Inputs are as follows: NetworkIdentity optionIdentity, OptionTracker tracker.  
  
<a name='Chen-GradiusMod-GradiusOption-OptionSync(RoR2-CharacterBody_System-Action-UnityEngine-Networking-NetworkIdentity_Chen-GradiusMod-OptionTracker-_bool)-queryTracker'></a>
`queryTracker` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
If true, the Option tracker is automatically queried. If false, the Option tracker will not be queried.  
  
