<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128628456/13.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3036)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# WinForms Data Grid - Display editors in a group row to edit cell values in the group 

This example demonstrates how to display column editors in group rows. The user can use the editor to specify the same value for all cells in a column in a group.

![](https://raw.githubusercontent.com/DevExpress-Examples/how-to-enable-editing-in-a-group-row-so-it-is-possible-to-change-child-cell-values-e3036/13.1.4%2B/media/winforms-grid-group-row-editors.png)

You can also enable the `GroupEditProvider.ShowGroupEditorOnMouseHover` option to automatically invoke the group editor on mouse hover.

```csharp
provider = new GroupEditProvider(gridView1);
provider.ShowGroupEditorOnMouseHover = true;
provider.SingleClick = true;
provider.EnableGroupEditing();
```


## Files to Review

* [GroupEditProvider.cs](./CS/WindowsApplication3/GroupEditProvider.cs) (VB: [GroupEditProvider.vb](./VB/WindowsApplication3/GroupEditProvider.vb))
* [Main.cs](./CS/WindowsApplication3/Main.cs) (VB: [Main.vb](./VB/WindowsApplication3/Main.vb))
