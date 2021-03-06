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

namespace DCKho
{
    public class DCKho : ICControl
    {
        GridView gvMain;
        DataCustomFormControl _data;
        InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        #region ICControl Members

        public void AddEvent()
        {
            gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            gvMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvMain_CellValueChanged);
        }

        void gvMain_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "QuyCach" && e.Value != DBNull.Value)
            {
                string[] s = e.Value.ToString().Split('*');
                string loai = s.Length == 2 ? "Tấm" : "Thùng";
                gvMain.SetFocusedRowCellValue(gvMain.Columns["Loai"], loai);
            }
            if ("SoLuong,Dai,Rong".Contains(e.Column.FieldName))
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
                if (l == null || l.ToString() == "")
                    return;
                if (l != null && l.ToString() == "Tấm")
                {
                    gvMain.SetFocusedRowCellValue(gvMain.Columns["SL2"], sl * d * r / 10000);
                    decimal tt = sl * dg * d * r / 10000;
                    gvMain.SetFocusedRowCellValue(gvMain.Columns["ThanhTien"], tt - (tt % 10));
                }
                else
                    gvMain.SetFocusedRowCellValue(gvMain.Columns["ThanhTien"], sl * dg);
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
