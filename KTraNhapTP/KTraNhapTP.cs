using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;

namespace KTraNhapTP
{
    public class KTraNhapTP : ICData
    {
        DataCustomData _data;
        InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
        #region ICData Members

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            SuaNhapTP();
            //DataRow drCur = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            //if (drCur.RowState == DataRowState.Deleted)
            //    return;
            //DataView dv = new DataView(_data.DsData.Tables[1]);
            //dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
            //string sql = @"select sum(SLSX) from DTKH where DTLSXID in (select DTLSXID from DTLSX where DTDHID = '{0}')";
            //string sql1 = @"select sum(SoLuong) from DT22 where DTDHID = '{0}'";
            //foreach (DataRowView drv in dv)
            //{
            //    string dtdhid = drv["DTDHID"].ToString();
            //    string mahh = drv["MaHH"].ToString();
            //    object o = _data.DbData.GetValue(string.Format(sql, dtdhid));
            //    object j = _data.DbData.GetValue(string.Format(sql1, dtdhid));
            //    decimal slt = o == DBNull.Value ? 0 : decimal.Parse(o.ToString());
            //    decimal sldn = j == DBNull.Value ? 0 : decimal.Parse(j.ToString());   
            //    if(drv.Row.RowState == DataRowState.Modified)
            //        sldn -= decimal.Parse(drv.Row["SoLuong", DataRowVersion.Original].ToString());
            //    slt -= sldn;
            //    decimal sln = decimal.Parse(drv["SoLuong"].ToString());
            //    if (sln > slt)
            //    {
            //        XtraMessageBox.Show("Không được nhập vượt quá số lượng hoàn thành\n" +
            //            mahh + ": Số lượng nhập = " + sln.ToString("###,##0") + "; Số lượng hoàn thành = " + slt.ToString("###,##0"),
            //            Config.GetValue("PackageName").ToString());
            //        _info.Result = false;
            //        return;
            //    }
            //}
            //_info.Result = true;
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion

        //Sửa phiếu nhập
        private void SuaNhapTP()
        {
            DataTable dt = _data.DsData.Tables[1].GetChanges(DataRowState.Modified);
            DataRow drCur = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if(dt == null || drCur == null)
                return;
            string sql = @" select sum(soluong - soluong_x) from BLVT 
                            where Loi = '{0}' and DTDHID = '{1}' and TenHH = N'{2}' 
                            and NgayCT <= '{3}' and MTIDDT <> '{4}'";
            foreach(DataRow dr in dt.Rows)
            {
                if (!dr["SoLuong", DataRowVersion.Original].ToString().Equals(dr["SoLuong", DataRowVersion.Current].ToString()))
                {
                    object obj = _data.DbData.GetValue(string.Format(sql, dr["Loi"], dr["DTDHID"], dr["TenHang"], drCur["NgayCT"], dr["DT22ID"]));
                    if (obj == null)
                        continue;
                    decimal soluong = Convert.ToDecimal(dr["SoLuong"]) + Convert.ToDecimal(obj);
                    if (soluong < 0)
                    {
                        XtraMessageBox.Show("Mặt hàng " + dr["TenHang"].ToString() + " có số lượng nhập nhỏ hơn số lượng xuất, không thể sửa!", Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }
                }
            }
        }
    }
}
