using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTLib;
using Plugins;
using CDTDatabase;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.Drawing;

namespace ToMau
{
    public class ToMau:ICReport
    {
        private DataCustomReport _data;
        private InfoCustomReport _info = new InfoCustomReport(IDataType.Report);
        private Database db = Database.NewDataDatabase();
        GridView gvMain;
        DataTable dtNCC;

        public void Execute()
        {
            dtNCC = db.GetDataTable("SELECT * FROM DMNCC");
            gvMain = (_data.FrmMain.Controls.Find("gridControlReport", true)[0] as GridControl).MainView as GridView;
            gvMain.RowCellStyle += new RowCellStyleEventHandler(gvMain_RowCellStyle);
        }

        void gvMain_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            e.Column.Width = 65;
            if(e.RowHandle<0 ||
               e.Column.FieldName.ToUpper().Equals("KHO") == true ||
               e.Column.FieldName.ToUpper().Equals("TỔNG SỐ CUỘN") == true ||
               e.Column.FieldName.ToUpper().Equals("TỔNG SỐ KÝ") == true)
                return;

            //string[] str = e.Column.FieldName.Trim().Split(' ');
            string[] str = e.Column.FieldName.Trim().Split('-');
            str[0] = str[0].Substring(0, str[0].Length - 3);

            DataRow[] dr = dtNCC.Select("MaNCC = '" + str[0] + "'");
            if (dr.Length == 0 || !str[1].Trim().ToUpper().Equals("CUỘN"))
                return;
            //if (dr.Length == 0 || e.Column.FieldName.Trim().Substring(e.Column.FieldName.Trim().Length - 7, 7).ToUpper().Equals("SỐ CUỘN") == false)
            //    return;

            if (Convert.ToBoolean(dr[0]["IsHangTon"]) == false)
                return;

            if (gvMain.GetRowCellValue(e.RowHandle, e.Column.FieldName) == System.DBNull.Value)
            {
                e.Appearance.BackColor = Color.Red;
                return;
            }
            if (Convert.ToInt32(gvMain.GetRowCellValue(e.RowHandle, e.Column.FieldName)) == 0)
               e.Appearance.BackColor = Color.Red;
            if (Convert.ToInt32(gvMain.GetRowCellValue(e.RowHandle, e.Column.FieldName)) > 0 &&
                Convert.ToInt32(gvMain.GetRowCellValue(e.RowHandle, e.Column.FieldName)) <= 3)
                e.Appearance.BackColor = Color.Yellow;
        }

        public DataCustomReport Data
        {
            set { _data = value; }

        }

        public InfoCustomReport Info
        {
            get { return _info; }
        }
    }
}
