using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System.Data;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
using DevExpress.XtraGrid.Columns;
using FormFactory;
using System.Windows.Forms;
using DevExpress.XtraLayout.Utils;

namespace LayCNNCC
{
    public class LayCNNCC : ICControl
    {
        LayoutControl lcMain;
        DataRow drCur;
        GridView gvMain;
        ReportPreview frmDS;
        GridView gvDS;
        ComboBoxEdit cbePX;
        DataCustomFormControl _data;
        InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        bool isVay;
        bool isFilteringID;

        #region ICControl Members

        public void AddEvent()
        {
            isVay = _data.DrTable.Table.Columns.Contains("ExtraSql") && _data.DrTable["ExtraSql"].ToString().ToLower().Contains("isvay = 1");
            gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            RepositoryItemGridLookUpEdit gluLC = gvMain.GridControl.RepositoryItems["LoaiChi"] as RepositoryItemGridLookUpEdit;
            gluLC.Popup += new EventHandler(gluLC_Popup);
            gluLC.View.ColumnFilterChanged += new EventHandler(View_ColumnFilterChanged);
            cbePX = (_data.FrmMain.Controls.Find("PhanXuong", true)[0] as ComboBoxEdit);
            cbePX.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(cbePX_EditValueChanging);
            //thêm nút chọn DH
            lcMain = _data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            if (!isVay)
            {
                SimpleButton btnChon = new SimpleButton();
                btnChon.Name = "btnChon";
                btnChon.Text = "Chọn NCC";
                LayoutControlItem lci = lcMain.AddItem("", btnChon);
                lci.Name = "cusChon";
                btnChon.Click += new EventHandler(btnChon_Click);
            }
            _data.FrmMain.Shown += new EventHandler(FrmMain_Shown);
            _data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(_data.BsMain, new EventArgs());
        }

        void View_ColumnFilterChanged(object sender, EventArgs e)
        {
            if (isFilteringID)
                return;
            isFilteringID = true;
            GridView gv = sender as GridView;
            if (gv.ActiveFilterString != "")
                gv.ActiveFilterString += " and (ID != 71)";
            else
                gv.ActiveFilterString = "ID != 71";
            isFilteringID = false;
        }

        void gluLC_Popup(object sender, EventArgs e)
        {
            if (isVay)
                return;
            GridLookUpEdit glu = sender as GridLookUpEdit;
            if (glu.Properties.View.ActiveFilterString != "")
                glu.Properties.View.ActiveFilterString += " and (ID != 71)";
            else
                glu.Properties.View.ActiveFilterString = "ID != 71";
        }

        private void CustomizeUI()
        {
            lcMain.Items.FindByName("lciPhanXuong").Visibility = isVay ? LayoutVisibility.Never : LayoutVisibility.Always;
            gvMain.Columns.ColumnByName("clLoaiChi").Visible = !isVay;
            gvMain.Columns.ColumnByName("clMaNCC").Visible = !isVay;
            gvMain.Columns.ColumnByName("clMaNCCex").Visible = !isVay;

            drCur = (_data.BsMain.Current as DataRowView).Row;
            if (drCur["HinhThucTT"].ToString().Equals("Tiền mặt"))
            {
                gvMain.Columns.ColumnByFieldName("TaiKhoan").Visible = false;
                gvMain.Columns.ColumnByFieldName("SoTaiKhoan").Visible = false;
            }
            else
            {
                gvMain.Columns.ColumnByFieldName("TaiKhoan").Visible = true;
                gvMain.Columns.ColumnByFieldName("SoTaiKhoan").Visible = true;
            }
        }

        void cbePX_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (gvMain.Editable)
                for (int i = 0; i < gvMain.DataRowCount; i++)
                    gvMain.SetRowCellValue(i, "LoaiChi", null);
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            if (_data.BsMain.Current == null)
                return;
            CustomizeUI();
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = _data.BsMain.DataSource as DataSet;
            if (ds == null)
                return;
            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(LayCNKH_ColumnChanged);
            ds.Tables[0].TableNewRow += new DataTableNewRowEventHandler(Table0_TableNewRow);
            ds.Tables[1].TableNewRow += new DataTableNewRowEventHandler(Table1_TableNewRow);
            if (_data.BsMain.Current != null)
            {
                DataRow dr = (_data.BsMain.Current as DataRowView).Row;
                if (dr.RowState == DataRowState.Detached || dr.RowState == DataRowState.Added)
                    Table0_TableNewRow(ds.Tables[0], new DataTableNewRowEventArgs(dr));
            }
        }

        void Table0_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            if (isVay)
            {
                e.Row["IsVay"] = isVay;
                e.Row["PhanXuong"] = "Bao bì";
            }
        }

        void Table1_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            if (isVay)
                e.Row["LoaiChi"] = 71;  //id của trả nợ vay
        }

        void LayCNKH_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName.Equals("HinhThucTT"))
            {
                if (e.Row["HinhThucTT"].ToString().Equals("Tiền mặt"))
                {
                    gvMain.Columns.ColumnByFieldName("TaiKhoan").Visible = false;
                    gvMain.Columns.ColumnByFieldName("SoTaiKhoan").Visible = false;
                }
                else
                {
                    gvMain.Columns.ColumnByFieldName("TaiKhoan").Visible = true;
                    gvMain.Columns.ColumnByFieldName("SoTaiKhoan").Visible = true;
                }
            }
        }

        void btnChon_Click(object sender, EventArgs e)
        {
            if (!gvMain.Editable)
            {
                XtraMessageBox.Show("Vui lòng chọn chế độ thêm hoặc sửa phiếu",
                    Config.GetValue("PackageName").ToString());
                return;
            }
            drCur = (_data.BsMain.Current as DataRowView).Row;
            //dùng report 1514 trong sysReport
            frmDS = FormFactory.FormFactory.Create(FormType.Report, "1530") as ReportPreview;
            gvDS = (frmDS.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            //viết xử lý cho nút F4-Xử lý trong report
            SimpleButton btnXuLy = (frmDS.Controls.Find("btnXuLy", true)[0] as SimpleButton);
            btnXuLy.Text = "Chọn NCC";
            btnXuLy.Click += new EventHandler(btnXuLy_Click);
            frmDS.WindowState = FormWindowState.Maximized;
            frmDS.ShowDialog();
        }

        void btnXuLy_Click(object sender, EventArgs e)
        {
            DataTable dtDS = (gvDS.DataSource as DataView).Table;
            dtDS.AcceptChanges();
            DataRow[] drs = dtDS.Select("Chọn = 1");
            if (drs.Length == 0)
            {
                XtraMessageBox.Show("Bạn chưa chọn nhà cung cấp để thanh toán", Config.GetValue("PackageName").ToString());
                return;
            }
            frmDS.Close();
            //add du lieu vao danh sach
            DataTable dtDTKH = (_data.BsMain.DataSource as DataSet).Tables[1];
            foreach (DataRow dr in drs)
            {
                if (dtDTKH.Select(string.Format("MT12ID = '{0}' and MaNCC = '{1}'", drCur["MT12ID"], dr["MaNCC"])).Length > 0)
                    continue;
                gvMain.AddNewRow();
                gvMain.UpdateCurrentRow();
                gvMain.SetFocusedRowCellValue(gvMain.Columns["MaNCC"], dr["MaNCC"]);
            }
        }

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        public InfoCustomControl Info
        {
            get { return _info; }
        }

        #endregion
    }
}
