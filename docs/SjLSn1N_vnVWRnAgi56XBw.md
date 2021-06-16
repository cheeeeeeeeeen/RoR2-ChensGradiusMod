#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.OptionSeed](be1vnC2Vgp_vVFpwRHLjUQ 'Chen.GradiusMod.Items.OptionSeed').[OptionSeed](U6Iu4qSqg_tWdEO+2QhjqQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed')
## OptionSeed.FireForSeeds(CharacterBody, Action&lt;GameObject,SeedBehavior,SeedTracker,float&gt;) Method
Loops through all the Option Seeds of the item wielder. The action has 4 useful parameters to use.  
The first parameter refers to the Option Seed itself. It is a GameObject.  
The second parameter refers to the SeedBehavior component of the Option Seed.  
The third parameter refers to the SeedTracker component of the item wielder.  
The last parameter is the computed damage multiplier based on configuration and item count of the owner.  
```csharp
public void FireForSeeds(RoR2.CharacterBody optionSeedOwner, System.Action<UnityEngine.GameObject,Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior,Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker,float> actionToRun);
```
#### Parameters
<a name='Chen_GradiusMod_Items_OptionSeed_OptionSeed_FireForSeeds(RoR2_CharacterBody_System_Action_UnityEngine_GameObject_Chen_GradiusMod_Items_OptionSeed_Components_SeedBehavior_Chen_GradiusMod_Items_OptionSeed_Components_SeedTracker_float_)_optionSeedOwner'></a>
`optionSeedOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the Option Seed.
  
<a name='Chen_GradiusMod_Items_OptionSeed_OptionSeed_FireForSeeds(RoR2_CharacterBody_System_Action_UnityEngine_GameObject_Chen_GradiusMod_Items_OptionSeed_Components_SeedBehavior_Chen_GradiusMod_Items_OptionSeed_Components_SeedTracker_float_)_actionToRun'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[SeedBehavior](DzDEYY3b5XN15kC+ypLh7A 'Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[SeedTracker](MLJxQ_Rdea9IQ2pGcFrbCQ 'Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')  
An action to execute for each Option. The inputs are as follows:  
            GameObject seed, SeedBehavior behavior, SeedTracker tracker, float multiplier.
  
