using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;


namespace DXSample {
    public partial class Main: XtraForm {
        public Main() {
            InitializeComponent();
        }

        GroupEditProvider provider;
        private void OnLoad(object sender, EventArgs e) {
            // TODO: This line of code loads data into the 'nwindDataSet.Products' table. You can move, or remove it, as needed.
            this.productsTableAdapter.Fill(this.nwindDataSet.Products);
            provider = new GroupEditProvider(gridView1);
            provider.ShowGroupEditorOnMouseHover = true;
            provider.SingleClick = true;
            provider.EnableGroupEditing();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            provider.DisableGroupEditing();
            base.OnFormClosing(e);
        }
    }
}
