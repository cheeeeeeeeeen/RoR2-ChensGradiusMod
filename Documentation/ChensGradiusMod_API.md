<a name='assembly'></a>
# ChensGradiusMod

## Contents

- [GradiusOption](#T-Chen-GradiusMod-GradiusOption 'Chen.GradiusMod.GradiusOption')
  - [FireForAllOptions(optionOwner,actionToRun)](#M-Chen-GradiusMod-GradiusOption-FireForAllOptions-RoR2-CharacterBody,System-Action{UnityEngine-GameObject,Chen-GradiusMod-OptionBehavior,UnityEngine-GameObject}- 'Chen.GradiusMod.GradiusOption.FireForAllOptions(RoR2.CharacterBody,System.Action{UnityEngine.GameObject,Chen.GradiusMod.OptionBehavior,UnityEngine.GameObject})')
  - [LoopAllMinions(ownerMaster,actionToRun)](#M-Chen-GradiusMod-GradiusOption-LoopAllMinions-RoR2-CharacterMaster,System-Action{UnityEngine-GameObject}- 'Chen.GradiusMod.GradiusOption.LoopAllMinions(RoR2.CharacterMaster,System.Action{UnityEngine.GameObject})')
  - [OptionSync(optionOwner,actionToRun,queryTracker)](#M-Chen-GradiusMod-GradiusOption-OptionSync-RoR2-CharacterBody,System-Action{UnityEngine-Networking-NetworkIdentity,Chen-GradiusMod-OptionTracker},System-Boolean- 'Chen.GradiusMod.GradiusOption.OptionSync(RoR2.CharacterBody,System.Action{UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.OptionTracker},System.Boolean)')
  - [SetRotateOptionMultiplier(masterName,newValue)](#M-Chen-GradiusMod-GradiusOption-SetRotateOptionMultiplier-System-String,System-Single- 'Chen.GradiusMod.GradiusOption.SetRotateOptionMultiplier(System.String,System.Single)')
  - [SetRotateOptionOffset(masterName,newValue)](#M-Chen-GradiusMod-GradiusOption-SetRotateOptionOffset-System-String,UnityEngine-Vector3- 'Chen.GradiusMod.GradiusOption.SetRotateOptionOffset(System.String,UnityEngine.Vector3)')
  - [SetToRegularOptions(masterName)](#M-Chen-GradiusMod-GradiusOption-SetToRegularOptions-System-String- 'Chen.GradiusMod.GradiusOption.SetToRegularOptions(System.String)')
  - [SetToRotateOptions(masterName)](#M-Chen-GradiusMod-GradiusOption-SetToRotateOptions-System-String- 'Chen.GradiusMod.GradiusOption.SetToRotateOptions(System.String)')
  - [SupportMinionType(masterName)](#M-Chen-GradiusMod-GradiusOption-SupportMinionType-System-String- 'Chen.GradiusMod.GradiusOption.SupportMinionType(System.String)')
  - [UnsupportMinionType(masterName)](#M-Chen-GradiusMod-GradiusOption-UnsupportMinionType-System-String- 'Chen.GradiusMod.GradiusOption.UnsupportMinionType(System.String)')

<a name='T-Chen-GradiusMod-GradiusOption'></a>
## GradiusOption `type`

##### Namespace

Chen.GradiusMod

<a name='M-Chen-GradiusMod-GradiusOption-FireForAllOptions-RoR2-CharacterBody,System-Action{UnityEngine-GameObject,Chen-GradiusMod-OptionBehavior,UnityEngine-GameObject}-'></a>
### FireForAllOptions(optionOwner,actionToRun) `method`

##### Summary

Loops through all the Options of the minion.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| optionOwner | [RoR2.CharacterBody](#T-RoR2-CharacterBody 'RoR2.CharacterBody') | The owner of the option. |
| actionToRun | [System.Action{UnityEngine.GameObject,Chen.GradiusMod.OptionBehavior,UnityEngine.GameObject}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{UnityEngine.GameObject,Chen.GradiusMod.OptionBehavior,UnityEngine.GameObject}') | An action to execute for each Option. The inputs are as follows: GameObject option, OptionBehavior behavior, GameObject target. |

<a name='M-Chen-GradiusMod-GradiusOption-LoopAllMinions-RoR2-CharacterMaster,System-Action{UnityEngine-GameObject}-'></a>
### LoopAllMinions(ownerMaster,actionToRun) `method`

##### Summary

Loops through the all the minions of the owner.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ownerMaster | [RoR2.CharacterMaster](#T-RoR2-CharacterMaster 'RoR2.CharacterMaster') | The owner of the minions. |
| actionToRun | [System.Action{UnityEngine.GameObject}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{UnityEngine.GameObject}') | An action to execute for each minion. The minion's CharacterBody GameObject is given as the input. |

<a name='M-Chen-GradiusMod-GradiusOption-OptionSync-RoR2-CharacterBody,System-Action{UnityEngine-Networking-NetworkIdentity,Chen-GradiusMod-OptionTracker},System-Boolean-'></a>
### OptionSync(optionOwner,actionToRun,queryTracker) `method`

##### Summary

Syncs the Option from the server to clients. Sync logic should be provided in actionToRun. Mostly used for syncing effects and sounds.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| optionOwner | [RoR2.CharacterBody](#T-RoR2-CharacterBody 'RoR2.CharacterBody') | The owner of the option. |
| actionToRun | [System.Action{UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.OptionTracker}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{UnityEngine.Networking.NetworkIdentity,Chen.GradiusMod.OptionTracker}') | The sync action to perform. Inputs are as follows: NetworkIdentity optionIdentity, OptionTracker tracker. |
| queryTracker | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | If true, the Option tracker is automatically queried. If false, the Option tracker will not be queried. |

<a name='M-Chen-GradiusMod-GradiusOption-SetRotateOptionMultiplier-System-String,System-Single-'></a>
### SetRotateOptionMultiplier(masterName,newValue) `method`

##### Summary

Sets the rotation multiplier for a minion type. This multiplier affects the distance and speed of rotation.

##### Returns

True if the values are set. False if not.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| masterName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The CharacterMaster name of the minion. |
| newValue | [System.Single](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Single 'System.Single') | The multiplier value. |

<a name='M-Chen-GradiusMod-GradiusOption-SetRotateOptionOffset-System-String,UnityEngine-Vector3-'></a>
### SetRotateOptionOffset(masterName,newValue) `method`

##### Summary

Sets the offset center position for a minion type. Options will rotate around the offset.

##### Returns

True if the values are set. False if not.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| masterName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The CharacterMaster name of the minion. |
| newValue | [UnityEngine.Vector3](#T-UnityEngine-Vector3 'UnityEngine.Vector3') | The offset value. |

<a name='M-Chen-GradiusMod-GradiusOption-SetToRegularOptions-System-String-'></a>
### SetToRegularOptions(masterName) `method`

##### Summary

Lets the minion use Regular Options.

##### Returns

True if the minion is successfully set to use Regular Options. False if it is already using Regular Options.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| masterName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The CharacterMaster name of the minion. |

<a name='M-Chen-GradiusMod-GradiusOption-SetToRotateOptions-System-String-'></a>
### SetToRotateOptions(masterName) `method`

##### Summary

Lets the minion use Rotate Options.

##### Returns

True if the minion is successfully set to use Rotate Options. False if it is already using Rotate Options.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| masterName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The CharacterMaster name of the minion. |

<a name='M-Chen-GradiusMod-GradiusOption-SupportMinionType-System-String-'></a>
### SupportMinionType(masterName) `method`

##### Summary

Adds a support for a minion for them to gain Options.

##### Returns

True if the minion is supported. False if it is already supported.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| masterName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The CharacterMaster name of the minion. |

<a name='M-Chen-GradiusMod-GradiusOption-UnsupportMinionType-System-String-'></a>
### UnsupportMinionType(masterName) `method`

##### Summary

Removes support for a minion so that they do not acquire Options.

##### Returns

True if the minion is not supported anymore. False if it is already unsupported.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| masterName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The CharacterMaster name of the minion. |
