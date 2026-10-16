#### [ChensGradiusMod](index 'index')
### [Chen.GradiusMod.Items](GmlPr_MhgpjgIAqS+VeJzw 'Chen.GradiusMod.Items')
## ItemBase&lt;T&gt; Class
Allows for making item classes into singleton classes.  
```csharp
public abstract class ItemBase<T> : Chen.GradiusMod.Items.ItemBase
    where T : Chen.GradiusMod.Items.ItemBase<T>
```
#### Type parameters
<a name='Chen_GradiusMod_Items_ItemBase_T__T'></a>
`T`  
The item class name
  

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [ModuleBase](q1imlbWm0_RVQI_9MRWPeg 'Chen.GradiusMod.Modules.ModuleBase') &#129106; [ItemBase](PTlTj9QMQE5cU3yAjyFa7A 'Chen.GradiusMod.Items.ItemBase') &#129106; ItemBase&lt;T&gt;  

Derived  
&#8627; [GradiusOption](Vui7fzQ6K+_c8O4kYLP8Wg 'Chen.GradiusMod.Items.GradiusOption.GradiusOption')  
&#8627; [OptionSeed](U6Iu4qSqg_tWdEO+2QhjqQ 'Chen.GradiusMod.Items.OptionSeed.OptionSeed')  

| Constructors | |
| :--- | :--- |
| [ItemBase()](iT2QlCT6ZhKOk+nZfwf+2g 'Chen.GradiusMod.Items.ItemBase&lt;T&gt;.ItemBase()') | Constructor that creates the instance of the singleton class.<br/> |

| Properties | |
| :--- | :--- |
| [instance](Ft1kDNeTnni9ng+x531PmQ 'Chen.GradiusMod.Items.ItemBase&lt;T&gt;.instance') | The instance of the singleton class.<br/> |
