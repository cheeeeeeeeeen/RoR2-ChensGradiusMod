#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.OptionSeed](be1vnC2Vgp_vVFpwRHLjUQ 'Chen.GradiusMod.Items.OptionSeed')
## OptionSeed Class
An item class which provides the main API related to the Option Seeds. It is powered by TILER2.  
```csharp
public class OptionSeed : TILER2.Item<Chen.GradiusMod.Items.OptionSeed.OptionSeed>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [TILER2.AutoConfigContainer](https://docs.microsoft.com/en-us/dotnet/api/TILER2.AutoConfigContainer 'TILER2.AutoConfigContainer') &#129106; [TILER2.T2Module](https://docs.microsoft.com/en-us/dotnet/api/TILER2.T2Module 'TILER2.T2Module') &#129106; [TILER2.CatalogBoilerplate](https://docs.microsoft.com/en-us/dotnet/api/TILER2.CatalogBoilerplate 'TILER2.CatalogBoilerplate') &#129106; [TILER2.Item](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Item 'TILER2.Item') &#129106; [TILER2.Item&lt;](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Item-1 'TILER2.Item`1')[OptionSeed](U6Iu4qSqg_tWdEO+2QhjqQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/TILER2.Item-1 'TILER2.Item`1') &#129106; OptionSeed  

| Methods | |
| :--- | :--- |
| [FireForSeeds(CharacterBody, Action&lt;GameObject,SeedBehavior,SeedTracker,float&gt;)](SjLSn1N_vnVWRnAgi56XBw 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.FireForSeeds(RoR2.CharacterBody, System.Action&lt;UnityEngine.GameObject,Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior,Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker,float&gt;)') | Loops through all the Option Seeds of the item wielder. The action has 4 useful parameters to use.<br/>The first parameter refers to the Option Seed itself. It is a GameObject.<br/>The second parameter refers to the SeedBehavior component of the Option Seed.<br/>The third parameter refers to the SeedTracker component of the item wielder.<br/>The last parameter is the computed damage multiplier based on configuration and item count of the owner.<br/> |
| [SetHorizontalOffsetMultiplier(string, float)](MLFf+wYauT+b8xnJ7TpSlQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetHorizontalOffsetMultiplier(string, float)') | Sets the horizontal offset multiplier for certain characters since some characters have a larger width than the default value. Horizontal offset default is 0.8f.<br/> |
| [SetRotationalRadius(string, float)](2pZk6KaQ7ZNE7X_O5uvM4w 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetRotationalRadius(string, float)') | Sets the rotational radius of Option Seeds for certain characters since some characters have a larger sizes just for the sake of matching. Radius default is .2f.<br/> |
| [SetVerticalOffsetMultiplier(string, float)](kA9cXMcoE80jabrUycIRmg 'Chen.GradiusMod.Items.OptionSeed.OptionSeed.SetVerticalOffsetMultiplier(string, float)') | Sets the vertical offset multiplier for certain characters since some Option Seeds are positioned awkwardly. Vertical offset default is 0.4f.<br/> |
