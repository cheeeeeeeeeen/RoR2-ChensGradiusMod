#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.GradiusOption](mfb9nYomeqOwYy2EkL_v0Q 'Chen.GradiusMod.Items.GradiusOption')
## GradiusOption Class
An item class which provides the main API related to the Options/Multiples. It is powered by TILER2.  
```csharp
public class GradiusOption : TILER2.Item<Chen.GradiusMod.Items.GradiusOption.GradiusOption>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [TILER2.AutoConfigContainer](https://docs.microsoft.com/en-us/dotnet/api/TILER2.AutoConfigContainer 'TILER2.AutoConfigContainer') &#129106; [TILER2.T2Module](https://docs.microsoft.com/en-us/dotnet/api/TILER2.T2Module 'TILER2.T2Module') &#129106; [TILER2.CatalogBoilerplate](https://docs.microsoft.com/en-us/dotnet/api/TILER2.CatalogBoilerplate 'TILER2.CatalogBoilerplate') &#129106; [TILER2.Item](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Item 'TILER2.Item') &#129106; [TILER2.Item&lt;](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Item-1 'TILER2.Item`1')[GradiusOption](Vui7fzQ6K+_c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Item-1 'TILER2.Item`1') &#129106; GradiusOption  

| Methods | |
| :--- | :--- |
| [FireForAllOptions(CharacterBody, Action&lt;GameObject,OptionBehavior,GameObject,Vector3&gt;)](Tlsaeo+IJEAqzCDMO9dP7A 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.FireForAllOptions(CharacterBody, System.Action&lt;GameObject,Chen.GradiusMod.Items.GradiusOption.Components.OptionBehavior,GameObject,Vector3&gt;)') | Loops through all the Options of the minion. The action has 4 useful parameters to use.<br/>The first parameter refers to the Option/Multiple itself. It is a GameObject.<br/>The second parameter refers to the OptionBehavior component in the Option/Multiple.<br/>The third parameter refers to the target of the Option/Multiple owner. It is also a GameObject.<br/>The last parameter refers to the direction from the option to the target. It is a normalized Vector3.<br/> |
| [LoopAllMinions(CharacterMaster, Action&lt;GameObject&gt;)](JbohoveQoBE5qO8HuRpuUA 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.LoopAllMinions(CharacterMaster, System.Action&lt;GameObject&gt;)') | Loops through the all the minions of the owner.<br/> |
| [OptionMuzzleEffect(GameObject, GameObject, bool)](2UlZ3VdVzXfnE0FX32HJ3A 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.OptionMuzzleEffect(GameObject, GameObject, bool)') | Deprecated method that provides an easy way of displaying the effect prefab on Options.<br/> |
| [OptionSync(CharacterBody, Action&lt;NetworkIdentity,OptionTracker&gt;, bool)](VZZ1wiKCbwaUnE1iq9F8Ng 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.OptionSync(CharacterBody, System.Action&lt;NetworkIdentity,Chen.GradiusMod.Items.GradiusOption.Components.OptionTracker&gt;, bool)') | Method that provides the Network Identity and Option Tracker for easier syncing. Sync logic should be provided in actionToRun.<br/> |
| [SetRotateOptionMultiplier(string, float)](HB1dnkNbzefti8Cem4lAOA 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.SetRotateOptionMultiplier(string, float)') | Sets the rotation multiplier for a minion type. This multiplier affects the distance and speed of rotation.<br/> |
| [SetRotateOptionOffset(string, Vector3)](W5XnRwgY6t9HUEXZZTHEyA 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.SetRotateOptionOffset(string, Vector3)') | Sets the offset center position for a minion type. Options will rotate around the offset.<br/> |
| [SetToRegularOptions(string)](TACNxBmya3KGqngqHLvv0g 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.SetToRegularOptions(string)') | Lets the minion use Regular Options.<br/> |
| [SetToRotateOptions(string)](8WVJdWadeecgGmNsU0+v6A 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.SetToRotateOptions(string)') | Lets the minion use Rotate Options.<br/> |
| [SupportMinionType(string)](M4LGoyiM_1WEKxvlP2bq_A 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.SupportMinionType(string)') | Adds a support for a minion for them to gain Options.<br/> |
| [UnsupportMinionType(string)](NNjvPyiS9MHUrmUpeQYO+g 'Chen.GradiusMod.Items.GradiusOption.GradiusOption.UnsupportMinionType(string)') | Removes support for a minion so that they do not acquire Options.<br/> |
