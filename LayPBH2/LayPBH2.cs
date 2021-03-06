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

namespace LayPBH2
{
    public class LayPBH2 : ICControl
    {
        DataRow drCur;
        GridView gvMain;
        ReportPreview frmDS;
        GridView gvDS;
        DataCustomFormControl _data;
        InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        #region ICControl Members

        public void AddEvent()
        {
            gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvMain_CellValueChanged);
            //thêm nút chọn DH
            LayoutControl lcMain = _data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            SimpleButton btnChon = new SimpleButton();
            btnChon.Name = "btnChon";
            btnChon.Text = "Chọn phiếu BH";
            LayoutControlItem lci = lcMain.AddItem("", btnChon);
            lci.Name = "cusChon";
            btnChon.Click += new EventHandler(btnChon_Click);
        }

        void gvMain_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "SoLuong")
            {
                object osl = gvMain.GetFocusedRowCellValue("SoLuong");
                object odg = gvMain.GetFocusedRowCellValue("DonGia");
                object od = gvMain.GetFocusedRowCellValue("Dai");
                object or = gvMain.GetFocusedRowCellValue("Rong");
                decimal sl = (osl == null || osl.ToString() == "") ? 0 : decimal.Parse(osl.ToString());
                decimal dg = (odg == null || odg.ToString() == "") ? 0 : decimal.Parse(odg.ToString());
                decimal d = (od == null || od.ToString() == "") ? 0 : decimal.Parse(od.ToString());
                decimal r = (or == null || or.ToString() == "") ? 0 : decimal.Parse(or.ToString());
                object l = gvMain.GetFocusedRowCellValue("Loai");
                if (l != null && l.ToString() == "Tấm")
                {
                    decimal tt = Math.Round(sl * d * r / 10000, 0) * Math.Round(dg, 0);
                    gvMain.SetFocusedRowCellValue(gvMain.Columns["ThanhTien"], tt - (tt % 10));
                }
                else
                    gvMain.SetFocusedRowCellValue(gvMain.Columns["ThanhTien"], sl * dg);
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
            if (drCur["MaKH"] == DBNull.Value)
            {
                XtraMessageBox.Show("Vui lòng chọn khách hàng trước!",
                    Config.GetValue("PackageName").ToString());
                return;
            }
            //dùng cách này để truyền tham số vào report
            Config.NewKeyValue("@MaKH", drCur["MaKH"]);
            //dùng report 1514 trong sysReport
            frmDS = FormFactory.FormFactory.Create(FormType.Report, "1540") as ReportPreview;
            gvDS = (frmDS.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            //viết xử lý cho nút F4-Xử lý trong report
            SimpleButton btnXuLy = (frmDS.Controls.Find("btnXuLy", true)[0] as SimpleButton);
            btnXuLy.Text = "Chọn phiếu BH";
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
                XtraMessageBox.Show("Bạn chưa chọn phiếu để nhập hàng trả", Config.GetValue("PackageName").ToString());
                return;
            }
            frmDS.Close();
            //add du lieu vao danh sach
            DataTable dtDTKH = (_data.BsMain.DataSource as DataSet).Tables[1];
            foreach (DataRow dr in drs)
            {
                if (dtDTKH.Select(string.Format("MT23ID = '{0}' and DT32ID = '{1}'", drCur["MT23ID"], dr["DT32ID"])).Length > 0)
                    continue;
                gvMain.AddNewRow();
                gvMain.UpdateCurrentRow();
                gvMain.SetFocusedRowCellValue(gvMain.Columns["DTDHID"], dr["DTDHID"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["SoPBH"], dr["SoCT"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["NgayPBH"], dr["NgayCT"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["MaHH"], dr["MaHH"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["TenHang"], dr["TenHang"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["DVT"], dr["DVT"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Loi"],dr["Loi"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["DonGia"], dr["DonGia"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["DT32ID"], dr["DT32ID"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Loai"], dr["Loai"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Dai"], dr["Dai"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Rong"], dr["Rong"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["SoLSX"], dr["SoLSX"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["NgaySXCD1"], dr["NgaySXCD1"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["NgaySXCD2"], dr["NgaySXCD2"]);
                gvMain.SetFocusedRowCellValue(gvMain.Columns["CASX"], dr["CASX"]);
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
