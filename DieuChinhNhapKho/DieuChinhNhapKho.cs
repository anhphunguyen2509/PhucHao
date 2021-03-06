using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System.Data;
using CDTLib;
using DevExpress.XtraGrid;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;

namespace DieuChinhNhapKho
{
    public class DieuChinhNhapKho : ICReport
    {
        private DataCustomReport _data;
        private InfoCustomReport _info = new InfoCustomReport(IDataType.Report);
        private Database db = Database.NewDataDatabase();
        List<string> listEdited = new List<string>();
        GridView gvMain;

        #region ICReport Members

        public DataCustomReport Data
        {
            set { _data = value; }

        }

        public InfoCustomReport Info
        {
            get { return _info; }
        }

        #endregion

        public void Execute()
        {
            gvMain = (_data.FrmMain.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            gvMain.DataSourceChanged += new EventHandler(gvMain_DataSourceChanged);
            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvMain_CellValueChanged);
            SimpleButton btnXL = _data.FrmMain.Controls.Find("btnXuLy", true)[0] as SimpleButton;
            btnXL.Text = "F4-Điều chỉnh kho";
            btnXL.Click += new EventHandler(btnXL_Click);

            gvMain.DoubleClick += gridView1_DoubleClick;

            GetListEdited();
            gvMain.RowStyle += GvMain_RowStyle;
            gvMain.RowCountChanged += GvMain_DataSourceChanged;
        }

        private void GvMain_DataSourceChanged(object sender, EventArgs e)
        {
            GetListEdited();
        }

        private void GvMain_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView View = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string id = View.GetRowCellDisplayText(e.RowHandle, View.Columns["ID"]).Trim();
                if (listEdited.Contains(id))
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.BackColor2 = Color.Red;
                }
            }
        }

        private void GetListEdited()
        {
            listEdited.Clear();
            DataTable data = db.GetDataTable("SELECT DISTINCT IDNKSX FROM POResultChangeLog");
            foreach (DataRow item in data.Rows)
            {
                listEdited.Add(item["IDNKSX"].ToString());
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(Control.MousePosition);
            DoRowDoubleClick(view, pt);
        }

        private static void DoRowDoubleClick(GridView view, Point pt)
        {
            GridHitInfo info = view.CalcHitInfo(pt);
            if (info.InRow || info.InRowCell)
            {
                string colCaption = info.Column == null ? "N/A" : info.Column.ToString();

                DataRow dr = view.GetDataRow(info.RowHandle);
                string id = dr["ID"].ToString();

                Database db = Database.NewDataDatabase();
                DataTable data = db.GetDataTable(string.Format("SELECT * FROM POResultChangeLog WHERE IDNKSX = '{0}' ORDER BY Date", id));

                if (data.Rows.Count > 0)
                {
                    HistoryForm form = new HistoryForm(data);
                    form.FormBorderStyle = FormBorderStyle.Fixed3D;
                    form.ShowDialog();
                } else
                {
                    XtraMessageBox.Show("Không có nhật ký chỉnh sửa của dữ liệu này.", Config.GetValue("PackageName").ToString());
                }
            }
        }
        void gvMain_DataSourceChanged(object sender, EventArgs e)
        {
            gvMain.Columns["AdjustQTY"].DisplayFormat.FormatString = "###,##0";
            Font f = new Font(gvMain.Columns["AdjustQTY"].AppearanceCell.Font, FontStyle.Bold);
            gvMain.Columns["AdjustQTY"].AppearanceCell.Font = f;
            gvMain.Columns["TotalQTY"].AppearanceCell.Font = f;
            gvMain.Columns["TotalQTY"].AppearanceCell.BackColor = Color.Red;
        }

        void gvMain_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AdjustQTY")
                gvMain.SetFocusedRowCellValue(gvMain.Columns["TotalQTY"],
                    Convert.ToDecimal(e.Value) + Convert.ToDecimal(gvMain.GetFocusedRowCellValue("FinishJobQTY")));
        }

        void btnXL_Click(object sender, EventArgs e)
        {
            //kiem tra quyen su dung
            bool admin = Convert.ToBoolean(Config.GetValue("Admin"));
            bool hasRight = admin ||
                (_data.DrReport.Table.Columns.Contains("sInsert") && Convert.ToBoolean(_data.DrReport["sInsert"])) ||
                (_data.DrReport.Table.Columns.Contains("sUpdate") && Convert.ToBoolean(_data.DrReport["sUpdate"])) ||
                (_data.DrReport.Table.Columns.Contains("sDelete") && Convert.ToBoolean(_data.DrReport["sDelete"]));
            if (!hasRight)
                XtraMessageBox.Show("Người dùng không có quyền thực hiện chức năng này\nVui lòng liên hệ quản trị hệ thống!",
                    Config.GetValue("PackageName").ToString());

            DataView dv = gvMain.DataSource as DataView;
            dv.Table.AcceptChanges();
            dv.RowFilter = "[Chon] = 1";
            if (dv.Count == 0)
            {
                dv.RowFilter = "";
                XtraMessageBox.Show("Vui lòng đánh dấu chọn đơn hàng cần điều chỉnh kho", Config.GetValue("PackageName").ToString());
                return;
            }

            bool rs = true;

            Database dbstruct = Database.NewStructDatabase();
            string sysUserID = Config.GetValue("sysUserID").ToString();
            DataTable dtDb = dbstruct.GetDataTable(string.Format("SELECT * FROM sysUser WHERE sysUserID = '{0}'", sysUserID));
            string username = !string.IsNullOrEmpty(dtDb.Rows[0]["FullName"].ToString()) ? dtDb.Rows[0]["FullName"].ToString() : dtDb.Rows[0]["UserName"].ToString();

            foreach (DataRowView drv in dv)
            {
                string id = drv["ID"].ToString();
                string newValue = drv["AdjustQTY"].ToString();

                object oldData = db.GetValue(string.Format("SELECT AdjustQTY FROM POResultLog WHERE ID = '{0}'", id));
                string oldValue = "";
                if (oldData != null)
                {
                   oldValue = oldData.ToString();
                }

                string total = drv["TotalQTY"].ToString();

                rs = db.UpdateDatabyStore("AdjustPOResult", new string[] { "ID", "AdjustQTY", "TotalQTY" },
                 new object[] { id, newValue, total });

                if (dtDb.Rows.Count > 0)
                {
                    string sql = string.Format("INSERT INTO POResultChangeLog (IDNKSX, UserName, Date, OLDVALUE, NEWVALUE) VALUES({0},'{1}',{2},{3},{4});", id, username, "GETDATE()", oldValue, newValue);
                    var result = db.UpdateByNonQuery(sql);
                }

                if (rs)
                    drv.Row.Delete();
                else
                    break;
            }

            dv.Table.AcceptChanges();

            dv.RowFilter = "";//Bỏ fillter

            if (rs)
                XtraMessageBox.Show("Cập nhật dữ liệu thành công", Config.GetValue("PackageName").ToString());
        }
    }
}
