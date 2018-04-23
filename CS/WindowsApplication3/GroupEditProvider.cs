using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using System.Drawing;
using System.Collections;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;

namespace DXSample {
    public class GroupEditProvider
    {
        GridView view;
        Dictionary<Type, BaseEdit> editors;
        Dictionary<Type, BaseEditPainter> painters;
        Dictionary<Type, BaseEditViewInfo> info;
        Rectangle hotTrackRectangle;
        BaseEdit groupEdit;
        bool showGroupEditorOnMouseHover = false;
        bool singleClick = false;

        public GroupEditProvider(GridView view)
        {
            this.view = view;
            painters = new Dictionary<Type, BaseEditPainter>();
            editors = new Dictionary<Type, BaseEdit>();
            info = new Dictionary<Type, BaseEditViewInfo>();
            hotTrackRectangle = Rectangle.Empty;
        }

        public bool ShowGroupEditorOnMouseHover
        {
            get { return showGroupEditorOnMouseHover; }
            set
            {
                if (showGroupEditorOnMouseHover != value)
                    showGroupEditorOnMouseHover = value;
            }
        }

        public bool SingleClick
        {
            get { return singleClick; }
            set
            {
                if (singleClick != value)
                    singleClick = value;
            }
        }
           
        Rectangle HotTrackRectangle
        {
            get { return hotTrackRectangle; }
            set
            {
                if (hotTrackRectangle == value)
                    return;
                view.InvalidateRect(hotTrackRectangle);
                view.InvalidateRect(value);
                hotTrackRectangle = value;
            }
        }

        public void EnableGroupEditing()
        {
            view.CustomDrawGroupRow += OnCustomDrawGroupRow;
            view.MouseDown += OnMouseDown;
            view.MouseMove += OnMouseMove;
            view.TopRowChanged += OnTopRowChanged;
            view.GridControl.Resize += OnGridControlResize;
        }

        void OnGridControlResize(object sender, EventArgs e)
        {
            HideGroupEditor(groupEdit, false);
        }

        void OnTopRowChanged(object sender, EventArgs e)
        {
            HideGroupEditor(groupEdit, false);
        }

       
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (hitInfo.HitTest == GridHitTest.Row && view.IsGroupRow(hitInfo.RowHandle))
            {
                GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                GridGroupRowInfo rowInfo = viewInfo.GetGridRowInfo(hitInfo.RowHandle) as GridGroupRowInfo;
                GridColumn col = viewInfo.GetNearestColumn(e.Location);
                GridColumnInfoArgs args = viewInfo.ColumnsInfo[col];
                if (col.VisibleIndex == 0)
                    HotTrackRectangle = new Rectangle(rowInfo.ButtonBounds.Right + 2, rowInfo.Bounds.Y,
                        args.Bounds.Width + args.Bounds.X - rowInfo.ButtonBounds.Right - 2, rowInfo.Bounds.Height);
                else
                    HotTrackRectangle = new Rectangle(args.Bounds.X + 2, rowInfo.Bounds.Y,
                        args.Bounds.Width - 2, rowInfo.Bounds.Height);
            }
            else
                HotTrackRectangle = Rectangle.Empty;
        }

        void OnMouseDown(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (e.Button == MouseButtons.Left && HotTrackRectangle.Contains(e.Location))
            {
                if ((SingleClick && e.Clicks == 1) || (!SingleClick && e.Clicks == 2))
                {
                    view.FocusedRowHandle = hitInfo.RowHandle;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridColumn col = viewInfo.GetNearestColumn(e.Location);
                    ShowGroupEditor(col);
                    DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                }
            }
        }

        private void ShowGroupEditor(GridColumn column)
        {
            groupEdit = GetGroupEditor(column);
            groupEdit.Visible = true;
            groupEdit.Bounds = HotTrackRectangle;
            groupEdit.Tag = column.FieldName;
            groupEdit.Select();
        }

        private BaseEdit GetGroupEditor(GridColumn column)
        {
            Type editorType = column.RealColumnEdit.GetType();
            if (editors.ContainsKey(editorType)) return editors[editorType];
            BaseEdit groupEdit = column.RealColumnEdit.CreateEditor();
            groupEdit.Parent = view.GridControl;
            if (groupEdit is CheckEdit)
                (groupEdit as CheckEdit).Properties.GlyphAlignment = HorzAlignment.Center;
            groupEdit.KeyDown += OnGroupEditKeyDown;
            groupEdit.Validated += OnGroupEditValidated;
            editors.Add(editorType, groupEdit);
            return groupEdit;
        }

        void OnGroupEditKeyDown(object sender, KeyEventArgs e)
        {
            BaseEdit edit = sender as BaseEdit;
            if (e.KeyData == Keys.Enter)
            {
                HideGroupEditor(edit, true);
                e.Handled = true;
            }
            if (e.KeyData == Keys.Escape)
            {
                HideGroupEditor(edit, false);
                e.Handled = true;
            }
        }

        private void HideGroupEditor(BaseEdit edit, bool updateValues)
        {
            if (edit == null || !edit.Visible) return;
            if (edit.IsModified && edit.EditValue != null && updateValues)
                SetChildRowValues(edit);
            edit.EditValue = null;
            edit.Visible = false;
        }

        void OnGroupEditValidated(object sender, EventArgs e)
        {
            BaseEdit edit = sender as BaseEdit;
            HideGroupEditor(edit, true);
        }

        private void SetChildRowValues(BaseEdit edit)
        {
            int groupRowHandle = view.FocusedRowHandle;
            int childRowCount = view.GetChildRowCount(groupRowHandle);
            string fieldName = edit.Tag.ToString();
            for (int i = 0; i < childRowCount; i++)
            {
                int childRowHandle = view.GetChildRowHandle(groupRowHandle, i);
                view.SetRowCellValue(childRowHandle, fieldName, edit.EditValue);
            }
        }

        void OnCustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            if (ShowGroupEditorOnMouseHover && HotTrackRectangle != Rectangle.Empty 
                && e.Info.Bounds.Contains(HotTrackRectangle.Location)) {
                DrawGroupRow(e);
                DrawGroupEditor(e); 
                e.Handled = true;
            }
        }

        private void DrawGroupEditor(DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            GridColumn col = viewInfo.GetNearestColumn(HotTrackRectangle.Location);
            BaseEditPainter groupEditPainter = GetGroupEditPainter(col);
            BaseEditViewInfo editViewInfo = GetGroupEditViewInfo(col);
            editViewInfo.Bounds = HotTrackRectangle;
            editViewInfo.CalcViewInfo(e.Graphics);
            e.Cache.FillRectangle(viewInfo.PaintAppearance.Row.GetBackBrush(e.Cache), HotTrackRectangle);
            groupEditPainter.Draw(new ControlGraphicsInfoArgs(editViewInfo, e.Cache, HotTrackRectangle));
        }

        private void DrawGroupRow(DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Painter.DrawObject(e.Info);
        }

        private BaseEditPainter GetGroupEditPainter(GridColumn col)
        {
            Type editorType = col.RealColumnEdit.GetType();
            if (painters.ContainsKey(editorType)) return painters[editorType];
            painters.Add(editorType, col.RealColumnEdit.CreatePainter());
            return painters[editorType];
        }

        private BaseEditViewInfo GetGroupEditViewInfo(GridColumn col)
        {
            Type editorType = col.RealColumnEdit.GetType();
            if (info.ContainsKey(editorType)) return info[editorType];
            info.Add(editorType, col.RealColumnEdit.CreateViewInfo());
            return info[editorType]; 
        }

        public void DisableGroupEditing(){
            foreach (BaseEdit edit in editors.Values)
            {
                edit.Parent = null;
                edit.Validated -= OnGroupEditValidated;
                edit.KeyDown -= OnGroupEditKeyDown;
                edit.Dispose();
            }
            hotTrackRectangle = Rectangle.Empty;
            editors.Clear();
            painters.Clear();
            info.Clear();
            view.CustomDrawGroupRow -= OnCustomDrawGroupRow;
            view.MouseDown -= OnMouseDown;
            view.MouseMove -= OnMouseMove;
            view.TopRowChanged -= OnTopRowChanged;
            view.GridControl.Resize -= OnGridControlResize;
        }
    }
}