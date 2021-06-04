
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Items.OptionSeed](./be1vnC2Vgp-vVFpwRHLjUQ 'Chen.GradiusMod.Items.OptionSeed').[OptionSeed](./U6Iu4qSqg-tWdEO+2QhjqQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed')

## OptionSeed.FireForSeeds(RoR2.CharacterBody, System.Action&lt;UnityEngine.GameObject,Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior,Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker,float&gt;) Method
Loops through all the Option Seeds of the item wielder. The action has 4 useful parameters to use.  
The first parameter refers to the Option Seed itself. It is a GameObject.  
The second parameter refers to the SeedBehavior component of the Option Seed.  
The third parameter refers to the SeedTracker component of the item wielder.  
The last parameter is the computed damage multiplier based on configuration and item count of the owner.  
```csharp
public void FireForSeeds(RoR2.CharacterBody optionSeedOwner, System.Action<UnityEngine.GameObject,Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior,Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker,float> actionToRun);
```

#### Parameters
<a name='Ks8qndcI53GB0Y-WTJNjIw'></a>
`optionSeedOwner` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The owner of the Option Seed.  
  
<a name='eEnwaymjp47aahuLL5FyWw'></a>
`actionToRun` [System.Action&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[UnityEngine.GameObject](https://docs.microsoft.com/en-us/dotnet/api/UnityEngine.GameObject 'UnityEngine.GameObject')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[SeedBehavior](./DzDEYY3b5XN15kC+ypLh7A 'Chen.GradiusMod.Items.OptionSeed.Components.SeedBehavior')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[SeedTracker](./MLJxQ-Rdea9IQ2pGcFrbCQ 'Chen.GradiusMod.Items.OptionSeed.Components.SeedTracker')[,](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')[System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Action-4 'System.Action`4')  
An action to execute for each Option. The inputs are as follows:  
            GameObject seed, SeedBehavior behavior, SeedTracker tracker, float multiplier.  
  
