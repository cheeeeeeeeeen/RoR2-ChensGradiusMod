#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items](GmlPr_MhgpjgIAqS+VeJzw 'Chen.GradiusMod.Items').[ItemBase](PTlTj9QMQE5cU3yAjyFa7A 'Chen.GradiusMod.Items.ItemBase')
## ItemBase.GetCountSpecific(CharacterBody, ItemDef) Method
Gets the number of a specific item the body has.  
```csharp
public int GetCountSpecific(CharacterBody body, ItemDef specificItemDef);
```
#### Parameters
<a name='Chen_GradiusMod_Items_ItemBase_GetCountSpecific(CharacterBody_ItemDef)_body'></a>
`body` [RoR2.CharacterBody](https://docs.microsoft.com/en-us/dotnet/api/RoR2.CharacterBody 'RoR2.CharacterBody')  
The body to check
  
<a name='Chen_GradiusMod_Items_ItemBase_GetCountSpecific(CharacterBody_ItemDef)_specificItemDef'></a>
`specificItemDef` [RoR2.ItemDef](https://docs.microsoft.com/en-us/dotnet/api/RoR2.ItemDef 'RoR2.ItemDef')  
The item to count
  
#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
The item count. 0 if the body or its inventory is missing.
