
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Items.GradiusOption](./mfb9nYomeqOwYy2EkL-v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](./Vui7fzQ6K+-c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')

## GradiusOption.OptionSync(RoR2.CharacterBody, System.Action&lt;UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker&gt;, bool) Method
Method that provides the Network Identity and Option Tracker for easier syncing. Sync logic should be provided in actionToRun.  
```csharp
public void OptionSync(RoR2.CharacterBody optionOwner, System.Action<UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker> actionToRun, bool queryTracker=true);
```

#### Parameters
<a name='kWf5AXoL9fM2nJm7mAfxRA'></a>
`optionOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the option.  
  
<a name='Tbms7eohBgY+AcC6mXLCag'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')[UnityEngine.Networking.NetworkIdentity](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.Networking.NetworkIdentity 'UnityEngine.Networking.NetworkIdentity')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')[OptionTracker](./u7j6jwd4UkMG2C3FwVR27w 'Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-2 'System.Action`2')  
The sync action to perform. Inputs are as follows: NetworkIdentity optionIdentity, OptionTracker tracker.  
  
<a name='CaEgvNfrLl9Yug0s4sicMQ'></a>
`queryTracker` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
If true, the Option tracker is automatically queried. If false, the Option tracker will not be queried.  
  
