Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraEditors


Namespace DXSample
	Partial Public Class Main
		Inherits XtraForm

		Public Sub New()
			InitializeComponent()
		End Sub

		Private provider As GroupEditProvider
		Private Overloads Sub OnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			' TODO: This line of code loads data into the 'nwindDataSet.Products' table. You can move, or remove it, as needed.
			Me.productsTableAdapter.Fill(Me.nwindDataSet.Products)
			provider = New GroupEditProvider(gridView1)
			provider.ShowGroupEditorOnMouseHover = True
			provider.SingleClick = True
			provider.EnableGroupEditing()
		End Sub

		Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
			provider.DisableGroupEditing()
			MyBase.OnFormClosing(e)
		End Sub
	End Class
End Namespace
