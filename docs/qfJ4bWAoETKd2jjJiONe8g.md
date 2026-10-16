#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Mirroring](lkYXZXA6XUB40+EnhohmlA 'Chen.GradiusMod.Mirroring')
## AttackMirror Class
Copies attacks automatically by hooking into the basic attack types of the game: bullets, projectiles, melee hitboxes and explosions.  
This is how Option Seeds and Options support characters that do not have dedicated support, such as modded survivors and drones.  
Only attacks that come from skills are copied. Item procs, equipment and attacks that are spawned by projectiles are ignored.  
```csharp
public static class AttackMirror
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; AttackMirror  

| Methods | |
| :--- | :--- |
| [ExcludeEntityState(Type)](hd2LdWcCpJ9AlTj3+_tXYg 'Chen.GradiusMod.Mirroring.AttackMirror.ExcludeEntityState(System.Type)') | Prevents attacks from being copied while the character is in the given state. Useful for skills that should not be duplicated,<br/>such as skills that throw something unique.<br/> |
| [ExcludeProjectile(GameObject)](yXmlbU1oagsy3zoufXqp1A 'Chen.GradiusMod.Mirroring.AttackMirror.ExcludeProjectile(GameObject)') | Prevents a projectile from being copied.<br/> |
