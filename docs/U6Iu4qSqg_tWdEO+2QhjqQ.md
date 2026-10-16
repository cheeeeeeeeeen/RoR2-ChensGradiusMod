#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.OptionSeed](be1vnC2Vgp_vVFpwRHLjUQ 'Chen.GradiusMod.Items.OptionSeed')
## OptionSeed Class
An item class which provides the main API related to the Option Seeds.  
```csharp
public class OptionSeed : Chen.GradiusMod.Items.ItemBase<Chen.GradiusMod.Items.OptionSeed.OptionSeed>,
Chen.GradiusMod.Mirroring.IMirrorProvider
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase') &#129106; [ItemBase](PTlTj9QMQE5cU3yAjyFa7A 'Chen.GradiusMod.Items.ItemBase') &#129106; [Chen.GradiusMod.Items.ItemBase&lt;](prSDiI69fBU5yrl6O+1Pxw 'Chen.GradiusMod.Items.ItemBase&lt;T&gt;')[OptionSeed](U6Iu4qSqg_tWdEO+2QhjqQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed')[&gt;](prSDiI69fBU5yrl6O+1Pxw 'Chen.GradiusMod.Items.ItemBase&lt;T&gt;') &#129106; OptionSeed  

Implements [IMirrorProvider](xwI8B2mplEEVQtA9e1AvSg 'Chen.GradiusMod.Mirroring.IMirrorProvider')  

| Methods | |
| :--- | :--- |
| [FireForSeeds(CharacterBody, Action&lt;GameObject,SeedBehavior,SeedTracker,float&gt;)](nQoXb7zZwSsac2Pp2fXzig 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.FireForSeeds(CharacterBody, System.Action&lt;GameObject,Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior,Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker,float&gt;)') | Loops through all the Option Seeds of the item wielder. The action has 4 useful parameters to use.<br/>The first parameter refers to the Option Seed itself. It is a GameObject.<br/>The second parameter refers to the SeedBehavior component of the Option Seed.<br/>The third parameter refers to the SeedTracker component of the item wielder.<br/>The last parameter is the computed damage multiplier based on configuration and item count of the owner.<br/> |
| [SetAutoMirroring(string, bool)](NI71NvU0PzjbNZUTbDXu9w 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetAutoMirroring(string, bool)') | Sets if the Option Seeds of the survivor copy its attacks automatically, by copying the bullets, projectiles, melee hits and explosions it creates.<br/>Use this for modded survivors. Set to false if the survivor's skills are supported manually with FireForSeeds instead.<br/> |
| [SetHorizontalOffsetMultiplier(string, float)](MLFf+wYauT+b8xnJ7TpSlQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetHorizontalOffsetMultiplier(string, float)') | Sets the horizontal offset multiplier for certain characters since some characters have a larger width than the default value. Horizontal offset default is 0.8f.<br/> |
| [SetRotationalRadius(string, float)](2pZk6KaQ7ZNE7X_O5uvM4w 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetRotationalRadius(string, float)') | Sets the rotational radius of Option Seeds for certain characters since some characters have a larger sizes just for the sake of matching. Radius default is .3f.<br/> |
| [SetVerticalOffsetMultiplier(string, float)](kA9cXMcoE80jabrUycIRmg 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetVerticalOffsetMultiplier(string, float)') | Sets the vertical offset multiplier for certain characters since some Option Seeds are positioned awkwardly. Vertical offset default is 0.4f.<br/> |
| [UsesAutoMirroring(CharacterBody)](smymkv1b70UyxBuM9CGFIQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.UsesAutoMirroring(CharacterBody)') | Checks if the Option Seeds of the character copy its attacks automatically.<br/> |
