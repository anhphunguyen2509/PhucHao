using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;
using CDTDatabase;

namespace KiemTraBangGia
{
    public class KiemTraBangGia : ICData
    {
        DataCustomData _data;
        InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
        DataTable dtBangGia;
        Database db = Database.NewDataDatabase();

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
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            string spCond = string.Format("MTID = '{0}'", drMaster["MTID"]);
            string loai = drMaster["Loai"].ToString();
            DataView dvKV = new DataView(_data.DsData.Tables["mDTBangGiaKV"]);
            DataView dvKH = new DataView(_data.DsData.Tables["mDTBangGiaKH"]);
            if (drMaster.RowState == DataRowState.Added)
            {
                dvKH.RowStateFilter = DataViewRowState.Added;
                dvKV.RowStateFilter = DataViewRowState.Added;
            }
            else
            {
                dvKV.RowFilter = spCond;
                dvKH.RowFilter = spCond;
            }
            switch (loai)
            {
                case "Báo giá vùng":
                    if (dvKV.Count == 0)
                    {
                        XtraMessageBox.Show("Vui lòng nhập khu vực áp dụng bảng giá",
                            Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }
                    if (dvKH.Count > 0)
                    {
                        XtraMessageBox.Show("Bảng giá vùng không được nhập khách hàng",
                            Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }
                    break;
                case "Báo giá khách hàng":
                    if (dvKH.Count == 0)
                    {
                        XtraMessageBox.Show("Vui lòng nhập khách hàng áp dụng bảng giá",
                            Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }
                    if (dvKV.Count > 0)
                    {
                        XtraMessageBox.Show("Bảng giá khách hàng không được nhập khu vực",
                            Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }
                    break;
                case "Bán lẻ":
                    if (dvKV.Count > 0 || dvKH.Count > 0)
                    {
                        XtraMessageBox.Show("Bảng giá bán lẻ không được nhập khu vực/khách hàng",
                            Config.GetValue("PackageName").ToString());
                        _info.Result = false;
                        return;
                    }
                    break;
            }
            DataView dvSP = new DataView(_data.DsData.Tables["mDTBangGiaSP"]);
            if (drMaster.RowState == DataRowState.Added)
                dvSP.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
            else
                dvSP.RowFilter = spCond; 
            //ktra san pham trung voi so lieu dang nhap
            foreach (DataRowView drv in dvSP)
            {
                DataRow[] drs = dvSP.Table.Select((spCond == string.Empty) ? string.Format("MTID is null and MaSP = '{0}'", drv["MaSP"]) :
                    spCond + string.Format(" and MaSP = '{0}'", drv["MaSP"]));
                if (drs.Length > 1)
                {
                    XtraMessageBox.Show(string.Format("Sản phẩm {0} trùng lặp trong chi tiết sản phẩm", drv["MaSP"]),
                        Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                    return;
                }
            }
            bool isChangeToActive = (drMaster.RowState == DataRowState.Modified &&
                !drMaster["InActive", DataRowVersion.Original].Equals(drMaster["InActive", DataRowVersion.Current]) &&
                !Convert.ToBoolean(drMaster["InActive"]));
            dvSP.RowFilter = (spCond == string.Empty) ? string.Empty : spCond;
            if (dtBangGia == null)
                dtBangGia = db.GetDataTable("select * from wBangGia");
            switch (loai)
            {
                case "Bán lẻ":
                    //kiem tra san pham trung voi so lieu da nhap
                    if (!isChangeToActive)
                        dvSP.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                    foreach (DataRowView drv in dvSP)
                    {
                        DataRow[] drs = dtBangGia.Select(string.Format("KhuVuc is null and MaKH is null and MaSP = '{0}'", drv["MaSP"]));
                        if (drs.Length > 0)
                        {
                            XtraMessageBox.Show(string.Format("Sản phẩm {0} đã khai báo giá trong bảng giá bán lẻ {1}", drv["MaSP"], drs[0]["SoBG"]),
                                Config.GetValue("PackageName").ToString());
                            _info.Result = false;
                            return;
                        }
                    }
                    break;
                case "Báo giá khách hàng":
                    if (!isChangeToActive)
                        dvKH.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                    //ktra khach hang trung voi so lieu dang nhap
                    foreach (DataRowView drv in dvKH)
                    {
                        DataRow[] drs = dvKH.Table.Select((spCond == string.Empty) ? string.Format("MTID is null and MaKH = '{0}'", drv["MaKH"]) :
                            spCond + string.Format(" and MaKH = '{0}'", drv["MaKH"]));
                        if (drs.Length > 1)
                        {
                            XtraMessageBox.Show(string.Format("Khách hàng {0} trùng lặp trong chi tiết khách hàng", drv["MaKH"]),
                                Config.GetValue("PackageName").ToString());
                            _info.Result = false;
                            return;
                        }
                    }
                    //kiem tra san pham va khach hang trung voi so lieu da nhap
                    foreach (DataRowView drvSP in dvSP)
                    {
                        foreach (DataRowView drvKH in dvKH)
                        {
                            DataRow[] drs = dtBangGia.Select(string.Format("KhuVuc is null and MaKH = '{1}' and MaSP = '{0}'", drvSP["MaSP"], drvKH["MaKH"]));
                            if (drs.Length > 0)
                            {
                                XtraMessageBox.Show(string.Format("Khách hàng {0} đã khai báo giá cho sản phẩm {1} trong bảng giá {2}", drvKH["MaKH"], drvSP["MaSP"], drs[0]["SoBG"]),
                                    Config.GetValue("PackageName").ToString());
                                _info.Result = false;
                                return;
                            }
                        }
                    }
                    break;
                case "Báo giá vùng":
                    if (!isChangeToActive)
                        dvKV.RowStateFilter = DataViewRowState.Added | DataViewRowState.ModifiedCurrent;
                    //ktra khu vuc trung voi so lieu dang nhap
                    foreach (DataRowView drv in dvKV)
                    {
                        DataRow[] drs = dvKV.Table.Select((spCond == string.Empty) ? string.Format("MTID is null and KhuVuc = '{0}'", drv["KhuVuc"]) :
                            spCond + string.Format(" and KhuVuc = '{0}'", drv["KhuVuc"]));
                        if (drs.Length > 1)
                        {
                            XtraMessageBox.Show(string.Format("Khu vực {0} trùng lặp trong chi tiết khu vực", drv["KhuVuc"]),
                                Config.GetValue("PackageName").ToString());
                            _info.Result = false;
                            return;
                        }
                    }
                    //kiem tra san pham va khu vuc trung voi so lieu da nhap
                    foreach (DataRowView drvSP in dvSP)
                    {
                        foreach (DataRowView drvKV in dvKV)
                        {
                            DataRow[] drs = dtBangGia.Select(string.Format("MaKH is null and KhuVuc = '{1}' and MaSP = '{0}'", drvSP["MaSP"], drvKV["KhuVuc"]));
                            if (drs.Length > 0)
                            {
                                XtraMessageBox.Show(string.Format("Khu vực {0} đã khai báo giá cho sản phẩm {1} trong bảng giá {2}", drvKV["KhuVuc"], drvSP["MaSP"], drs[0]["SoBG"]),
                                    Config.GetValue("PackageName").ToString());
                                _info.Result = false;
                                return;
                            }
                        }
                    }
                    break;
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        #endregion
    }
}
