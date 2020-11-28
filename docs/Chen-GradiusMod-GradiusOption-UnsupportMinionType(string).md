
### [Chen.GradiusMod](./Chen-GradiusMod 'Chen.GradiusMod').[GradiusOption](./Chen-GradiusMod-GradiusOption 'Chen.GradiusMod.GradiusOption')

## GradiusOption.UnsupportMinionType(string) Method
Removes support for a minion so that they do not acquire Options.  
```csharp
public bool UnsupportMinionType(string masterName);
```

#### Parameters
<a name='Chen-GradiusMod-GradiusOption-UnsupportMinionType(string)-masterName'></a>
`masterName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The CharacterMaster name of the minion.  
  

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
True if the minion is not supported anymore. False if it is already unsupported.  
