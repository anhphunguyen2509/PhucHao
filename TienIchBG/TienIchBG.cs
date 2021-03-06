using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using CDTDatabase;
using System.Data;
using DevExpress.XtraEditors;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;

namespace TienIchBG
{
    public class TienIchBG : ICForm
    {
        Database db = Database.NewDataDatabase();
        DataCustomFormControl _data;
        List<InfoCustomForm> _info = new List<InfoCustomForm>();
        public TienIchBG()
        {
            InfoCustomForm icfHuy = new InfoCustomForm(IDataType.MasterDetailDt, 1, "Hủy báo giá", "", "MTBaoGia");
            InfoCustomForm icfCNG = new InfoCustomForm(IDataType.MasterDetailDt, 2, "Cập nhật đơn giá", "", "MTBaoGia");
            InfoCustomForm icfGBG = new InfoCustomForm(IDataType.MasterDetailDt, 3, "Ghép báo giá", "", "MTBaoGia");
            _info.Add(icfHuy);
            _info.Add(icfCNG);
            _info.Add(icfGBG);
        }
        #region ICForm Members

        public DataCustomFormControl Data
        {
            set { _data = value; }
        }

        void CapNhatHuy()
        {
            if (_data.BsMain.Current == null)
                return;
            DataRow drCur = (_data.BsMain.Current as DataRowView).Row;
            if (drCur.RowState != DataRowState.Unchanged)
                return;
            Boolean huy = Boolean.Parse(drCur["Huy"].ToString());
            if (!huy)
                XtraMessageBox.Show("Báo giá này sẽ chuyển về tình trạng hủy", Config.GetValue("PackageName").ToString());
            else
                XtraMessageBox.Show("Báo giá này sẽ bỏ tình trạng hủy", Config.GetValue("PackageName").ToString());
            string mtbgid = drCur["MTBGID"].ToString();
            DataTable dt = (_data.BsMain.DataSource as DataSet).Tables[0];
            DataRow[] drs = dt.Select("MTBGID = '" + mtbgid + "'");
            string s = "update MTBaoGia set Huy = {0} where MTBGID = '{1}'";
            int h = huy ? 0 : 1;
            db.UpdateByNonQuery(string.Format(s, h, drCur["MTBGID"]));
            foreach (DataRow dr in drs)
            {
                dr["Huy"] = !huy;
                dr.AcceptChanges();
            }
        }

        DataTable LayDSNL(DataRow drCur)
        {
            DataTable dtNL = db.GetDataTable("select Ma, Ten from DMNL");
            dtNL.PrimaryKey = new DataColumn[] {dtNL.Columns["Ma"]};

            DataTable dtGia = new DataTable();
            dtGia.Columns.Add("Ma", typeof(String));
            dtGia.Columns.Add("Ten", typeof(String));
            dtGia.Columns.Add("Gia", typeof(Decimal));

            string mtbgid = drCur["MTBGID"].ToString();
            DataTable dt = (_data.BsMain.DataSource as DataSet).Tables[1];
            DataRow[] drs = dt.Select("MTBGID = '" + mtbgid + "'");

            List<string> lstMa = new List<string>();
            foreach (DataRow dr in drs)
            {
                string m = dr["Mat_Giay"].ToString();
                string sb = dr["SB_Giay"].ToString();
                string mb = dr["MB_Giay"].ToString();
                string sc = dr["SC_Giay"].ToString();
                string mc = dr["MC_Giay"].ToString();
                string se = dr["SE_Giay"].ToString();
                string me = dr["ME_Giay"].ToString();
                if (m != "" && !lstMa.Contains(m))
                {
                    string ten = dtNL.Rows.Find(m)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { m, ten });
                    lstMa.Add(m);
                }
                if (sb != "" && !lstMa.Contains(sb))
                {
                    string ten = dtNL.Rows.Find(sb)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { sb, ten });
                    lstMa.Add(sb);
                }
                if (mb != "" && !lstMa.Contains(mb))
                {
                    string ten = dtNL.Rows.Find(mb)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { mb, ten });
                    lstMa.Add(mb);
                }
                if (sc != "" && !lstMa.Contains(sc))
                {
                    string ten = dtNL.Rows.Find(sc)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { sc, ten });
                    lstMa.Add(sc);
                }
                if (mc != "" && !lstMa.Contains(mc))
                {
                    string ten = dtNL.Rows.Find(mc)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { mc, ten });
                    lstMa.Add(mc);
                }
                if (se != "" && !lstMa.Contains(se))
                {
                    string ten = dtNL.Rows.Find(se)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { se, ten });
                    lstMa.Add(se);
                }
                if (me != "" && !lstMa.Contains(me))
                {
                    string ten = dtNL.Rows.Find(me)["Ten"].ToString();
                    dtGia.Rows.Add(new object[] { me, ten });
                    lstMa.Add(me);
                }
            }

            return dtGia;
        }

        void CapNhatGia()
        {
            if (_data.BsMain.Current == null)
                return;
            DataRow drCur = (_data.BsMain.Current as DataRowView).Row;
            Boolean duyet = Boolean.Parse(drCur["Duyet"].ToString());
            if (duyet)
            {
                XtraMessageBox.Show("Không được sửa giá của báo giá đã duyệt", Config.GetValue("PackageName").ToString());
                return;
            }
            DataTable dtGia = LayDSNL(drCur);
            if (dtGia.Rows.Count == 0)
            {
                XtraMessageBox.Show("Báo giá này chưa sử dụng nguyên liệu nào", Config.GetValue("PackageName").ToString());
                return;
            }

            FrmDSNL frm = new FrmDSNL(dtGia);
            if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            DataView dv = new DataView(dtGia);
            dv.RowFilter = "Gia is not null and Gia > 0";
            if (dv.Count == 0)
                return;

            DataTable dtGia2 = dv.ToTable();
            dtGia2.PrimaryKey = new DataColumn[] { dtGia2.Columns["Ma"] };

            string mtbgid = drCur["MTBGID"].ToString();
            DataTable dt = (_data.BsMain.DataSource as DataSet).Tables[1];
            DataRow[] drs = dt.Select("MTBGID = '" + mtbgid + "'");
            foreach (DataRow dr in drs)
            {
                decimal mdg = 0, sbdg = 0, mbdg = 0, scdg = 0, mcdg = 0, sedg = 0, medg = 0;
                string m = dr["Mat_Giay"].ToString();
                string sb = dr["SB_Giay"].ToString();
                string mb = dr["MB_Giay"].ToString();
                string sc = dr["SC_Giay"].ToString();
                string mc = dr["MC_Giay"].ToString();
                string se = dr["SE_Giay"].ToString();
                string me = dr["ME_Giay"].ToString();
                DataRow drGia = dtGia2.Rows.Find(m);
                if (drGia != null)
                    mdg = decimal.Parse(drGia["Gia"].ToString());

                drGia = dtGia2.Rows.Find(sb);
                if (drGia != null)
                    sbdg = decimal.Parse(drGia["Gia"].ToString());
                drGia = dtGia2.Rows.Find(mb);
                if (drGia != null)
                    mbdg = decimal.Parse(drGia["Gia"].ToString());

                drGia = dtGia2.Rows.Find(sc);
                if (drGia != null)
                    scdg = decimal.Parse(drGia["Gia"].ToString());
                drGia = dtGia2.Rows.Find(mc);
                if (drGia != null)
                    mcdg = decimal.Parse(drGia["Gia"].ToString());

                drGia = dtGia2.Rows.Find(se);
                if (drGia != null)
                    sedg = decimal.Parse(drGia["Gia"].ToString());
                drGia = dtGia2.Rows.Find(me);
                if (drGia != null)
                    medg = decimal.Parse(drGia["Gia"].ToString());

                string s = "update DTBaoGia set GiaBan = {0}";
                if (mdg != 0)
                {
                    dr["Mat_DG"] = mdg;
                    s += string.Format(", Mat_DG = {0}", mdg);
                }
                if (sbdg != 0)
                {
                    dr["SB_DG"] = sbdg;
                    s += string.Format(", SB_DG = {0}", sbdg);
                }
                if (mbdg != 0)
                {
                    dr["MB_DG"] = mbdg;
                    s += string.Format(", MB_DG = {0}", mbdg);
                }
                if (scdg != 0)
                {
                    dr["SC_DG"] = scdg;
                    s += string.Format(", SC_DG = {0}", scdg);
                }
                if (mcdg != 0)
                {
                    dr["MC_DG"] = mcdg;
                    s += string.Format(", MC_DG = {0}", mcdg);
                }
                if (sedg != 0)
                {
                    dr["SE_DG"] = sedg;
                    s += string.Format(", SE_DG = {0}", sedg);
                }
                if (medg != 0)
                {
                    dr["ME_DG"] = medg;
                    s += string.Format(", ME_DG = {0}", medg);
                }
                s += " where DTBGID = '{1}'";
                db.UpdateByNonQuery(string.Format(s, dr["GiaBan"], dr["DTBGID"]));
                dr.AcceptChanges();
            }
        }

        private void GhepBaoGia()
        {
            if (_data.BsMain.Current == null)
                return;
            DataRow drCur = (_data.BsMain.Current as DataRowView).Row;
            if (drCur.RowState != DataRowState.Added)
            {
                XtraMessageBox.Show("Vui lòng thực hiện khi tạo báo giá mới", Config.GetValue("PackageName").ToString());
                return;
            }
            FrmChonBG frm = new FrmChonBG();
            frm.ShowDialog();
            DataTable dtDSBG = frm.dtDSBG;
            DataRow[] drs = dtDSBG.Select("Chon = 1");
            if (drs.Length == 0)
            {
                XtraMessageBox.Show("Không có báo giá nào được chọn", Config.GetValue("PackageName").ToString());
                return;
            }
            GridView gv = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            for (int i = gv.DataRowCount - 1; i >= 0; i--)
                if (gv.GetDataRow(i)["TenHang"] == DBNull.Value)
                    gv.DeleteRow(i);
            string mtbgid = drCur["MTBGID"].ToString();
            DataTable dtBaoGia = (_data.BsMain.DataSource as DataSet).Tables[1];
            DataTable tmp = dtBaoGia.Clone();
            foreach (DataRow dr in drs)
                tmp.ImportRow(dr);
            foreach (DataRow dr in tmp.Rows)
            {
                dr["MTBGID"] = mtbgid;
                dr["DTBGID"] = Guid.NewGuid();
                DataRow drNew = dtBaoGia.NewRow();
                drNew.ItemArray = (object[])dr.ItemArray.Clone();
                dtBaoGia.Rows.Add(drNew);
            }
            gv.RefreshData();
        }

        public void Execute(int menuID)
        {
            switch (menuID)
            {
                case 1:
                    CapNhatHuy();
                    break;
                case 2:
                    CapNhatGia();
                    break;
                case 3:
                    GhepBaoGia();
                    break;
            }
        }

        public List<InfoCustomForm> LstInfo
        {
            get { return _info; }
        }

        #endregion
    }
}
