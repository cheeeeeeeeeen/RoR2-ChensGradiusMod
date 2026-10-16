#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Modules](vpFhjnRUIa36IfVm3haMEw 'Chen.GradiusMod.Modules').[ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase')
## ModuleBase.Bind&lt;T&gt;(string, T, string) Method
Binds a config option under the module's config section.  
```csharp
protected T Bind<T>(string propertyName, T defaultValue, string description);
```
#### Type parameters
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string)_T'></a>
`T`  
Type of the config option
  
#### Parameters
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string)_propertyName'></a>
`propertyName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Name of the property that holds the value. It is turned to PascalCase for the config key.
  
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string)_defaultValue'></a>
`defaultValue` [T](v+driiV5hTMWFwCwP+Pa+A#Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string)_T 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string).T')  
Default value of the config option
  
<a name='Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string)_description'></a>
`description` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Description of the config option
  
#### Returns
[T](v+driiV5hTMWFwCwP+Pa+A#Chen_GradiusMod_Modules_ModuleBase_Bind_T_(string_T_string)_T 'Chen.GradiusMod.Modules.ModuleBase.Bind&lt;T&gt;(string, T, string).T')  
The value of the config option
