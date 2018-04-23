Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports DevExpress.Skins
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.Utils
Imports DevExpress.XtraGrid.Columns
Imports System.Drawing
Imports System.Collections
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid.Drawing
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors.Drawing
Imports DevExpress.XtraEditors.ViewInfo

Namespace DXSample
	Public Class GroupEditProvider
		Private view As GridView
		Private editors As Dictionary(Of Type, BaseEdit)
		Private painters As Dictionary(Of Type, BaseEditPainter)
		Private info As Dictionary(Of Type, BaseEditViewInfo)
		Private hotTrackRectangle_Renamed As Rectangle
		Private groupEdit As BaseEdit
		Private showGroupEditorOnMouseHover_Renamed As Boolean = False
		Private singleClick_Renamed As Boolean = False

		Public Sub New(ByVal view As GridView)
			Me.view = view
			painters = New Dictionary(Of Type, BaseEditPainter)()
			editors = New Dictionary(Of Type, BaseEdit)()
			info = New Dictionary(Of Type, BaseEditViewInfo)()
			hotTrackRectangle_Renamed = Rectangle.Empty
		End Sub

		Public Property ShowGroupEditorOnMouseHover() As Boolean
			Get
				Return showGroupEditorOnMouseHover_Renamed
			End Get
			Set(ByVal value As Boolean)
				If showGroupEditorOnMouseHover_Renamed <> value Then
					showGroupEditorOnMouseHover_Renamed = value
				End If
			End Set
		End Property

		Public Property SingleClick() As Boolean
			Get
				Return singleClick_Renamed
			End Get
			Set(ByVal value As Boolean)
				If singleClick_Renamed <> value Then
					singleClick_Renamed = value
				End If
			End Set
		End Property

		Private Property HotTrackRectangle() As Rectangle
			Get
				Return hotTrackRectangle_Renamed
			End Get
			Set(ByVal value As Rectangle)
				If hotTrackRectangle_Renamed = value Then
					Return
				End If
				view.InvalidateRect(hotTrackRectangle_Renamed)
				view.InvalidateRect(value)
				hotTrackRectangle_Renamed = value
			End Set
		End Property

		Public Sub EnableGroupEditing()
			AddHandler view.CustomDrawGroupRow, AddressOf OnCustomDrawGroupRow
			AddHandler view.MouseDown, AddressOf OnMouseDown
			AddHandler view.MouseMove, AddressOf OnMouseMove
			AddHandler view.TopRowChanged, AddressOf OnTopRowChanged
			AddHandler view.GridControl.Resize, AddressOf OnGridControlResize
		End Sub

		Private Sub OnGridControlResize(ByVal sender As Object, ByVal e As EventArgs)
			HideGroupEditor(groupEdit, False)
		End Sub

		Private Sub OnTopRowChanged(ByVal sender As Object, ByVal e As EventArgs)
			HideGroupEditor(groupEdit, False)
		End Sub


		Private Sub OnMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			Dim hitInfo As GridHitInfo = view.CalcHitInfo(e.Location)
			If hitInfo.HitTest = GridHitTest.Row AndAlso view.IsGroupRow(hitInfo.RowHandle) Then
				Dim viewInfo As GridViewInfo = TryCast(view.GetViewInfo(), GridViewInfo)
				Dim rowInfo As GridGroupRowInfo = TryCast(viewInfo.GetGridRowInfo(hitInfo.RowHandle), GridGroupRowInfo)
				Dim col As GridColumn = viewInfo.GetNearestColumn(e.Location)
				Dim args As GridColumnInfoArgs = viewInfo.ColumnsInfo(col)
				If col.VisibleIndex = 0 Then
					HotTrackRectangle = New Rectangle(rowInfo.ButtonBounds.Right + 2, rowInfo.Bounds.Y, args.Bounds.Width + args.Bounds.X - rowInfo.ButtonBounds.Right - 2, rowInfo.Bounds.Height)
				Else
					HotTrackRectangle = New Rectangle(args.Bounds.X + 2, rowInfo.Bounds.Y, args.Bounds.Width - 2, rowInfo.Bounds.Height)
				End If
			Else
				HotTrackRectangle = Rectangle.Empty
			End If
		End Sub

		Private Sub OnMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
			Dim hitInfo As GridHitInfo = view.CalcHitInfo(e.Location)
			If e.Button = MouseButtons.Left AndAlso HotTrackRectangle.Contains(e.Location) Then
				If (SingleClick AndAlso e.Clicks = 1) OrElse ((Not SingleClick) AndAlso e.Clicks = 2) Then
					view.FocusedRowHandle = hitInfo.RowHandle
					Dim viewInfo As GridViewInfo = TryCast(view.GetViewInfo(), GridViewInfo)
					Dim col As GridColumn = viewInfo.GetNearestColumn(e.Location)
					ShowGroupEditor(col)
					DXMouseEventArgs.GetMouseArgs(e).Handled = True
				End If
			End If
		End Sub

		Private Sub ShowGroupEditor(ByVal column As GridColumn)
			groupEdit = GetGroupEditor(column)
			groupEdit.Visible = True
			groupEdit.Bounds = HotTrackRectangle
			groupEdit.Tag = column.FieldName
			groupEdit.Select()
		End Sub

		Private Function GetGroupEditor(ByVal column As GridColumn) As BaseEdit
			Dim editorType As Type = column.RealColumnEdit.GetType()
			If editors.ContainsKey(editorType) Then
				Return editors(editorType)
			End If
			Dim groupEdit As BaseEdit = column.RealColumnEdit.CreateEditor()
			groupEdit.Parent = view.GridControl
			If TypeOf groupEdit Is CheckEdit Then
				TryCast(groupEdit, CheckEdit).Properties.GlyphAlignment = HorzAlignment.Center
			End If
			AddHandler groupEdit.KeyDown, AddressOf OnGroupEditKeyDown
			AddHandler groupEdit.Validated, AddressOf OnGroupEditValidated
			editors.Add(editorType, groupEdit)
			Return groupEdit
		End Function

		Private Sub OnGroupEditKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			Dim edit As BaseEdit = TryCast(sender, BaseEdit)
			If e.KeyData = Keys.Enter Then
				HideGroupEditor(edit, True)
				e.Handled = True
			End If
			If e.KeyData = Keys.Escape Then
				HideGroupEditor(edit, False)
				e.Handled = True
			End If
		End Sub

		Private Sub HideGroupEditor(ByVal edit As BaseEdit, ByVal updateValues As Boolean)
			If edit Is Nothing OrElse (Not edit.Visible) Then
				Return
			End If
			If edit.IsModified AndAlso edit.EditValue IsNot Nothing AndAlso updateValues Then
				SetChildRowValues(edit)
			End If
			edit.EditValue = Nothing
			edit.Visible = False
		End Sub

		Private Sub OnGroupEditValidated(ByVal sender As Object, ByVal e As EventArgs)
			Dim edit As BaseEdit = TryCast(sender, BaseEdit)
			HideGroupEditor(edit, True)
		End Sub

		Private Sub SetChildRowValues(ByVal edit As BaseEdit)
			Dim groupRowHandle As Integer = view.FocusedRowHandle
			Dim childRowCount As Integer = view.GetChildRowCount(groupRowHandle)
			Dim fieldName As String = edit.Tag.ToString()
			For i As Integer = 0 To childRowCount - 1
				Dim childRowHandle As Integer = view.GetChildRowHandle(groupRowHandle, i)
				view.SetRowCellValue(childRowHandle, fieldName, edit.EditValue)
			Next i
		End Sub

		Private Sub OnCustomDrawGroupRow(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs)
			If ShowGroupEditorOnMouseHover AndAlso HotTrackRectangle <> Rectangle.Empty AndAlso e.Info.Bounds.Contains(HotTrackRectangle.Location) Then
				DrawGroupRow(e)
				DrawGroupEditor(e)
				e.Handled = True
			End If
		End Sub

		Private Sub DrawGroupEditor(ByVal e As DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs)
			Dim viewInfo As GridViewInfo = TryCast(view.GetViewInfo(), GridViewInfo)
			Dim col As GridColumn = viewInfo.GetNearestColumn(HotTrackRectangle.Location)
			Dim groupEditPainter As BaseEditPainter = GetGroupEditPainter(col)
			Dim editViewInfo As BaseEditViewInfo = GetGroupEditViewInfo(col)
			editViewInfo.Bounds = HotTrackRectangle
			editViewInfo.CalcViewInfo(e.Graphics)
			e.Graphics.FillRectangle(viewInfo.PaintAppearance.Row.GetBackBrush(e.Cache), HotTrackRectangle)
			groupEditPainter.Draw(New ControlGraphicsInfoArgs(editViewInfo, e.Cache, HotTrackRectangle))
		End Sub

		Private Sub DrawGroupRow(ByVal e As DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs)
			e.Appearance.FillRectangle(e.Cache, e.Bounds)
			e.Painter.DrawObject(e.Info)
		End Sub

		Private Function GetGroupEditPainter(ByVal col As GridColumn) As BaseEditPainter
			Dim editorType As Type = col.RealColumnEdit.GetType()
			If painters.ContainsKey(editorType) Then
				Return painters(editorType)
			End If
			painters.Add(editorType, col.RealColumnEdit.CreatePainter())
			Return painters(editorType)
		End Function

		Private Function GetGroupEditViewInfo(ByVal col As GridColumn) As BaseEditViewInfo
			Dim editorType As Type = col.RealColumnEdit.GetType()
			If info.ContainsKey(editorType) Then
				Return info(editorType)
			End If
			info.Add(editorType, col.RealColumnEdit.CreateViewInfo())
			Return info(editorType)
		End Function

		Public Sub DisableGroupEditing()
			For Each edit As BaseEdit In editors.Values
				edit.Parent = Nothing
				RemoveHandler edit.Validated, AddressOf OnGroupEditValidated
				RemoveHandler edit.KeyDown, AddressOf OnGroupEditKeyDown
				edit.Dispose()
			Next edit
			hotTrackRectangle_Renamed = Rectangle.Empty
			editors.Clear()
			painters.Clear()
			info.Clear()
			RemoveHandler view.CustomDrawGroupRow, AddressOf OnCustomDrawGroupRow
			RemoveHandler view.MouseDown, AddressOf OnMouseDown
			RemoveHandler view.MouseMove, AddressOf OnMouseMove
			RemoveHandler view.TopRowChanged, AddressOf OnTopRowChanged
			RemoveHandler view.GridControl.Resize, AddressOf OnGridControlResize
		End Sub
	End Class
End Namespace