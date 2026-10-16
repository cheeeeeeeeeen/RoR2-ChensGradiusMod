#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items](GmlPr_MhgpjgIAqS+VeJzw 'Chen.GradiusMod.Items')
## ItemBase Class
The base class of the items of this mod.  
```csharp
public abstract class ItemBase : Chen.GradiusMod.Modules.ModuleBase
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase') &#129106; ItemBase  

Derived  
&#8627; [ItemBase&lt;T&gt;](prSDiI69fBU5yrl6O+1Pxw 'Chen.GradiusMod.Items.ItemBase&lt;T&gt;')  

| Properties | |
| :--- | :--- |
| [configCategoryPrefix](l_7Dry9AKTq+K0PJxEIl6w 'Chen.GradiusMod.Items.ItemBase.configCategoryPrefix') | The prefix of the config section that is used by the module, e.g. "Items" or "Artifacts".<br/> |
| [displayName](FXkeWjtNSHKAXB2P0giLsw 'Chen.GradiusMod.Items.ItemBase.displayName') | The display name of the item.<br/> |
| [iconResource](iADDVhu3bpeAndYvMvprWA 'Chen.GradiusMod.Items.ItemBase.iconResource') | The icon of the item.<br/> |
| [itemDef](rWZ2tU2YYSRzRaIwpWGhjw 'Chen.GradiusMod.Items.ItemBase.itemDef') | The ItemDef of this item. Available after SetupAttributes.<br/> |
| [itemIsAIBlacklisted](wH8LE+vadWYauIibL+vUew 'Chen.GradiusMod.Items.ItemBase.itemIsAIBlacklisted') | If true, AI will not receive this item.<br/> |
| [itemTags](0s0UuSm45ZMYwk7E9tLq3Q 'Chen.GradiusMod.Items.ItemBase.itemTags') | The tags of the item.<br/> |
| [itemTier](aLmi+EvQZUegSuKCKXEh2g 'Chen.GradiusMod.Items.ItemBase.itemTier') | The tier of the item.<br/> |
| [modelResource](S31vDgnouVl1UCAkh7NvYQ 'Chen.GradiusMod.Items.ItemBase.modelResource') | The pickup model of the item.<br/> |
| [tokenPrefix](UjHNJOBnSxCWBSQlVLZcDQ 'Chen.GradiusMod.Items.ItemBase.tokenPrefix') | The prefix of the language tokens of the item.<br/> |

| Methods | |
| :--- | :--- |
| [GetCount(CharacterBody)](iWNTtpQJc8OtWN1ICwyTSA 'Chen.GradiusMod.Items.ItemBase.GetCount(CharacterBody)') | Gets the number of this item the body has.<br/> |
| [GetCount(CharacterMaster)](pWqjSH11CNhpnq6nk_YqIw 'Chen.GradiusMod.Items.ItemBase.GetCount(CharacterMaster)') | Gets the number of this item the master has.<br/> |
| [GetCountSpecific(CharacterBody, ItemDef)](Hi8u5bNLbpfOFTcClQuGMA 'Chen.GradiusMod.Items.ItemBase.GetCountSpecific(CharacterBody, ItemDef)') | Gets the number of a specific item the body has.<br/> |
| [SetupAttributes()](oxS+SrBK3NaB9UCNHnAo8g 'Chen.GradiusMod.Items.ItemBase.SetupAttributes()') | Creates and registers the definitions of the module to the game, such as the ItemDef or the ArtifactDef.<br/> |
