<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128628456/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3036)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# WinForms Data Grid - Display editors in a group row to edit cell values in the group 

This example demonstrates how to display column editors in group rows. The user can use the editor to specify the same value for all cells in a column in a group.

![WinForms Data Grid - Display editors in a group row to edit cell values in the group](media/winforms-grid-group-row-editors.png)

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
<!-- feedback -->
## Does This Example Address Your Development Requirements/Objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=winforms-grid-enable-editing-in-group-row-to-change-cell-values&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=winforms-grid-enable-editing-in-group-row-to-change-cell-values&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->

