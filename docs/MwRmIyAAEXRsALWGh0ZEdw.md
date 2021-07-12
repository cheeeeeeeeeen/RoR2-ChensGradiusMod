#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod](neHTXX+yFsk1RpXqjkv9zg 'Chen.GradiusMod')
## Extensions Class
Helpful extensions for objects that will be recurring in the mod.  
```csharp
public static class Extensions
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; Extensions  

| Methods | |
| :--- | :--- |
| [AssignDeathBehavior(GameObject, Type)](lm5QfeIEC4A80PITFu34bQ 'Chen.GradiusMod.Extensions.AssignDeathBehavior(UnityEngine.GameObject, System.Type)') | Assigns the Death Behavior of the CharacterMaster GameObject.<br/> |
| [FilterOutOwnerFromAttack(BulletAttack)](Tqr3eIr8BwWGwm6rgpylvQ 'Chen.GradiusMod.Extensions.FilterOutOwnerFromAttack(RoR2.BulletAttack)') | Filters the owner out from the attack so that they do not hit themselves with their own attack.<br/>Useful for Option Seeds' behavior to avoid hitting the owner.<br/> |
| [InitializeDroneModelComponents(GameObject, CharacterBody, float)](E47faf7DqMG6BLdiWCQOHQ 'Chen.GradiusMod.Extensions.InitializeDroneModelComponents(UnityEngine.GameObject, RoR2.CharacterBody, float)') | Shortcut for initializing a custom drone model. Only applies when work flow is followed the same as this mod's drones.<br/> |
| [MuzzleEffect(GameObject, GameObject, bool)](7roQPfGASRtMvS48E57GWQ 'Chen.GradiusMod.Extensions.MuzzleEffect(UnityEngine.GameObject, UnityEngine.GameObject, bool)') | Method that provides an easy way of displaying effect prefabs for muzzle effects.<br/>Mainly used for Options and Option Seeds.<br/> |
| [SafeCheck(Dictionary&lt;string,Object&gt;, string)](UcBLHVdDDZkETVaa0moaRw 'Chen.GradiusMod.Extensions.SafeCheck(System.Collections.Generic.Dictionary&lt;string,UnityEngine.Object&gt;, string)') | Safely checks if the dictionary has the key and if they key has an existing object.<br/> |
| [SetAllDriversToAimTowardsEnemies(AISkillDriver[])](gnPOJV62YUPZiTf4ltA7QA 'Chen.GradiusMod.Extensions.SetAllDriversToAimTowardsEnemies(RoR2.CharacterAI.AISkillDriver[])') | Sets all Skill Drivers within the array to aim towards the enemy.<br/> |
| [SetAllDriversToAimTowardsEnemies(GameObject)](dJhiosGXV8eLrawB7h83Bg 'Chen.GradiusMod.Extensions.SetAllDriversToAimTowardsEnemies(UnityEngine.GameObject)') | Sets all Skill Drivers of the drone to aim towards the enemy.<br/> |
