<!-- default file list -->
*Files to look at*:

* [GroupEditProvider.cs](./CS/WindowsApplication3/GroupEditProvider.cs) (VB: [GroupEditProvider.vb](./VB/WindowsApplication3/GroupEditProvider.vb))
* [Main.cs](./CS/WindowsApplication3/Main.cs) (VB: [Main.vb](./VB/WindowsApplication3/Main.vb))
* [Program.cs](./CS/WindowsApplication3/Program.cs) (VB: [Program.vb](./VB/WindowsApplication3/Program.vb))
<!-- default file list end -->
# How to enable editing in a group row so it is possible to change child cell values 


<p>This example illustrates how to show an editor in a group row under a corresponding column. The editor corresponds to an in-place editor used in the column. </p><p>To invoke the group editor click the group row once or twice based upon the <strong>GroupEditProvider.SingleClick property</strong>. Then, you can enter any value in this editor. To apply this value to child cells and close the editor you should either press the <strong>Enter key</strong> or force the editor to lose focus. To discard changes press the<strong> Esc key</strong> when the group editor is active. <br />
Also, you can provide  end-users with a visual effect pointing that he/she can invoke the group editor for a group row located under the mouse pointer. For this, enable the  <strong>GroupEditProvider.ShowGroupEditorOnMouseHover property</strong>.</p>

<br/>


