
#### [ChensGradiusMod](./index 'index')

### [Chen.GradiusMod.Drones](./Y-iPobZkdIiJ9feSuBjDaQ 'Chen.GradiusMod.Drones').[DroneDeathState](./8ui+PJgGZL18czsU0lHbsw 'Chen.GradiusMod.Drones.DroneDeathState')

## DroneDeathState.OnEnter() Method
Overrideable OnEnter method from the original state. Always call base.OnEnter. Initialize values at runtime here.  
To perform the death behavior specified in OnImpactServer, destroyOnImpact must be set to true.  
This method already does that so long as base.OnEnter is invoked.  
```csharp
public override void OnEnter();
```
