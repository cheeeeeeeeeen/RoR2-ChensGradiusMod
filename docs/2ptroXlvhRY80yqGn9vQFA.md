#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Modules](vpFhjnRUIa36IfVm3haMEw 'Chen.GradiusMod.Modules').[ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase')
## ModuleBase.Bind&lt;T&gt;(string, T, string, T, T) Method
Binds a config option under the module's config section, clamped within a range.  
```csharp
protected T Bind<T>(string propertyName, T defaultValue, string description, T min, T max)
    where T : System.IComparable;
```
#### Type parameters
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_T'></a>
`T`  
Type of the config option
  
#### Parameters
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_propertyName'></a>
`propertyName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Name of the property that holds the value. It is turned to PascalCase for the config key.
  
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_defaultValue'></a>
`defaultValue` [T](2ptroXlvhRY80yqGn9vQFA#Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_T 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string, T, T).T')  
Default value of the config option
  
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_description'></a>
`description` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Description of the config option
  
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_min'></a>
`min` [T](2ptroXlvhRY80yqGn9vQFA#Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_T 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string, T, T).T')  
Minimum value of the config option
  
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_max'></a>
`max` [T](2ptroXlvhRY80yqGn9vQFA#Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_T 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string, T, T).T')  
Maximum value of the config option
  
#### Returns
[T](2ptroXlvhRY80yqGn9vQFA#Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string_T_T)_T 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string, T, T).T')  
The value of the config option
