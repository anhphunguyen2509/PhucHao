using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;

namespace KTraNHTra
{
    public class KTraNHTra:ICData
    {
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        Database dbCDT = Database.NewStructDatabase();

         #region ICData Members
  
        public KTraNHTra()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            
        }
        
        public void ExecuteBefore()
        {
            if (_data.CurMasterIndex < 0)
                return;

            DataRow drCur = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            DataTable dt = _data.DsData.Tables[1].GetChanges(DataRowState.Added | DataRowState.Modified);
            if (dt == null)
                return;
            string sql = @" select sum(d.soluong) from dt32 d inner join mt32 m on m.mt32id = d.mt32id
                            where d.dtdhid = '{0}' and m.soct='{1}' and d.tenhang = N'{2}'";
            foreach (DataRow dr in dt.Rows)
            {
                object obj = _data.DbData.GetValue(string.Format(sql,dr["DTDHID"],dr["SoPBH"],dr["TenHang"]));
                if (Convert.ToInt32(dr["SoLuong"]) > Convert.ToInt32(obj))
                {
                    XtraMessageBox.Show(string.Format("Mặt hàng '{0}' có số lượng xuất bán: {1}, số lượng trả vượt quá số lượng xuất bán!",
                                        dr["TenHang"],Convert.ToInt32(obj)));
                    _info.Result = false;
                    break;
                }
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

         #endregion
    }
}
