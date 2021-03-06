using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;

namespace KTSua
{
    public class KTSua : ICControl
    {
        DataCustomFormControl _data;
        InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        Database data = Database.NewDataDatabase();

        #region ICControl Members
        GridView gvMain;
        string tableName = "";
        public void AddEvent()
        {
            tableName = _data.DrTableMaster["TableName"].ToString();
            //_data.DrTable.Table.Columns.Contains("ExtraSql")
            if(tableName == "MT22" || tableName == "MT23" 
                || (tableName == "MT32" && _data.DrTable.Table.Columns.Contains("ExtraSql") && _data.DrTable["ExtraSql"].ToString().Contains("XK = 1")))
            {
                gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
                _data.BsMain.DataSourceChanged+=new EventHandler(BsMain_DataSourceChanged);
                BsMain_DataSourceChanged(_data.BsMain, new EventArgs());
            }
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = _data.BsMain.DataSource as DataSet;
            if (ds == null) return;
            ds.Tables[1].ColumnChanged += new DataColumnChangeEventHandler(KTSua_ColumnChanged);
        }
        bool setSL = false;
        void KTSua_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            e.Row.EndEdit();
            if (e.Row.RowState == DataRowState.Modified && e.Column.ColumnName.ToUpper().Equals("SOLUONG"))
            {
                string slMoi = e.Row["SoLuong", DataRowVersion.Current] == DBNull.Value ? "":e.Row["SoLuong", DataRowVersion.Current].ToString();
                string slCu = e.Row["SoLuong", DataRowVersion.Original] == DBNull.Value ? "" :e.Row["SoLuong", DataRowVersion.Original].ToString();
                if (slMoi == slCu)
                    return;
                string sql = @" select isnull(sum(cast(isGP as int)),0) from dt32 
                                where dtdhid = '" + e.Row["DTDHID"].ToString() + "' and tenhang = N'" + e.Row["TenHang"].ToString() + "'";
                object obj = data.GetValue(sql);
                if (obj == null)
                    return;
                if (Convert.ToInt32(obj) > 0)
                {
                    string sql1 = @"select m.soct from dt32 d inner join mt32 m on d.mt32id = m.mt32id 
                                    where d.dtdhid = '" + e.Row["DTDHID"].ToString() + "' and d.isGP = 1 and tenhang = '" + e.Row["TenHang"].ToString() + "'";
                    string phieubh = "";
                    using(DataTable dtable = data.GetDataTable(sql1))
                    {
                        foreach(DataRow dr in dtable.Rows)
                        {
                            phieubh += dr["soct"].ToString() + ", ";
                        }
                    }
                    if (setSL == false)
                    {
                        XtraMessageBox.Show(string.Format("Phiếu bán hàng {0}đã xuất giấy phế không sửa được!", phieubh)
                            , Config.GetValue("PackageName").ToString());
                        setSL = true;
                        gvMain.SetFocusedRowCellValue(gvMain.Columns.ColumnByFieldName("SoLuong"), slCu);
                        setSL = false;
                    }

                }
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
