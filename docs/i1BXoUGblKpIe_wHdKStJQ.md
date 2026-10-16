#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items.GradiusOption](mfb9nYomeqOwYy2EkL_v0Q 'Chen.GradiusMod.Items.GradiusOption').[GradiusOption](Vui7fzQ6K+_c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')
## GradiusOption.SetAutoMirroring(string, bool) Method
Sets if a minion type gains Options that copy its attacks automatically, by copying the bullets, projectiles, melee hits and explosions it creates.  
Use this for modded drones and minions. Minions supported with SupportMinionType rely on manual support with FireForAllOptions instead.  
```csharp
public void SetAutoMirroring(string masterName, bool enabled);
```
#### Parameters
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_SetAutoMirroring(string_bool)_masterName'></a>
`masterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The CharacterMaster name of the minion. Partial names are matched, the same way as SupportMinionType.
  
<a name='Chen_GradiusMod_Items_GradiusOption_GradiusOption_SetAutoMirroring(string_bool)_enabled'></a>
`enabled` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True to give the minion Options that copy attacks automatically, false to disable it.
  
