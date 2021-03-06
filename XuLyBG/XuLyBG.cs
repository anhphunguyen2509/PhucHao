using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using CDTLib;
using System.Data;
using System.Windows.Forms;
using CDTDatabase;
using DevExpress.XtraLayout.Utils;
using System.Drawing;

namespace XuLyBG
{
    public class XuLyBG : ICControl
    { 
        List<string> lstNL = new List<string>(new string[] { "Mat_", "SB_", "MB_", "SC_", "MC_", "SE_", "ME_" });
        DataTable dtNL;
        bool focusing = false;
        bool doikho = false;
        GridView gvMain;
        string dkThung = "";
        string dkTam = "";
        Database db = Database.NewDataDatabase();
        CheckEdit ceKCT;
        CheckEdit ceXa;
        CheckEdit ceCL;
        GridLookUpEdit gluKH;
        LayoutControl lcMain; 
        ComboBoxEdit cbeLoai;
        ComboBoxEdit cbeLoaiThung;
        CalcEdit ceRong;
        SpinEdit ceLop;
        CalcEdit ceXX;
        SpinEdit seDaoX;
        SpinEdit seDaoCL;
        CalcEdit ceDCL;
        CalcEdit ceRCL;
        CalcEdit ceCCL;
        GridLookUpEdit gluTHS;
        DateEdit deNgayCT;
        List<GridLookUpEdit> lstGiay = new List<GridLookUpEdit>();
        private List<string> visibleCls = new List<string>();
        private List<string> captionCls = new List<string>();
        private DataCustomFormControl _data;
        private InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        string tableName;

        #region ICControl Members

        public void AddEvent()
        {
            tableName = _data.DrTableMaster["TableName"].ToString();
            List<string> lstTB = new List<string>(new string[] { "MTBaoGia", "MTDonHang", "MTLSX" });
            List<string> lstSo = new List<string>(new string[] { "SoBG", "SoDH", "SoDH" });
            if (!lstTB.Contains(tableName))
                return;
            deNgayCT = _data.FrmMain.Controls.Find("NgayCT", true)[0] as DateEdit;
            lcMain = _data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            if (tableName == "MTDonHang")
            {
                SimpleButton btnKho = new SimpleButton();
                btnKho.Name = "btnKho";
                btnKho.Text = "Lấy khổ";
                LayoutControlItem lci = lcMain.AddItem("", btnKho);
                lci.Name = "cusKho";
                btnKho.Click += new EventHandler(btnKho_Click);
            }
            gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            gvMain.OptionsBehavior.Editable = false;
            
            gvMain.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(gvMain_BeforeLeaveRow);
            gvMain.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(gvMain_FocusedRowChanged);
            gluKH = _data.FrmMain.Controls.Find("MaKH", true)[0] as GridLookUpEdit;
            dtNL = db.GetDataTable("select Ma, Ten, MaPhu, Kho, GiaBan, GiaBanMoi from wDMNL");
            dtNL.PrimaryKey = new DataColumn[] { dtNL.Columns["Ma"] };
            if (tableName == "MTBaoGia")
            {
                var ceGiaBanMoi = _data.FrmMain.Controls.Find("isGiaMoi", true)[0] as CheckEdit;
                ceGiaBanMoi.CheckedChanged += CeGiaBanMoi_CheckedChanged;

                gluKH.EditValueChanged += new EventHandler(gluKH_EditValueChanged);
                LayoutControlItem lciDH = lcMain.Items.FindByName("lciTaoDH") as LayoutControlItem;
                lciDH.AppearanceItemCaption.ForeColor = Color.Red;
                lciDH.AppearanceItemCaption.Options.UseForeColor = true;
            }
            ceKCT = _data.FrmMain.Controls.Find("KCT", true)[0] as CheckEdit;
            if (tableName == "MTDonHang")
            {
                ceXa = _data.FrmMain.Controls.Find("isXa", true)[0] as CheckEdit;
                ceCL = _data.FrmMain.Controls.Find("isCL", true)[0] as CheckEdit;
                CalcEdit ceDT = _data.FrmMain.Controls.Find("DienTich", true)[0] as CalcEdit;
                ceDT.Enter += new EventHandler(ceDT_Enter);
                ceXX = _data.FrmMain.Controls.Find("XaX", true)[0] as CalcEdit;
                ceXX.Enter += new EventHandler(ceXX_Enter);
                ceXX.EditValueChanged += new EventHandler(ceXX_EditValueChanged);
                seDaoX = _data.FrmMain.Controls.Find("DaoX", true)[0] as SpinEdit;
                seDaoX.Enter += new EventHandler(seDao_Enter);
                seDaoCL = _data.FrmMain.Controls.Find("DaoCL", true)[0] as SpinEdit;
                ceDCL = _data.FrmMain.Controls.Find("DaiCL", true)[0] as CalcEdit;
                ceDCL.Enter += new EventHandler(ceDCL_Enter);
                ceRCL = _data.FrmMain.Controls.Find("RongCL", true)[0] as CalcEdit;
                ceRCL.Enter += new EventHandler(ceDCL_Enter);
                ceRCL.EditValueChanged += new EventHandler(ceRCL_EditValueChanged);
                ceCCL = _data.FrmMain.Controls.Find("CaoCL", true)[0] as CalcEdit;
                ceCCL.Enter += new EventHandler(ceDCL_Enter);
                ceCCL.EditValueChanged += new EventHandler(ceCCL_EditValueChanged);
                SpinEdit seLCL = _data.FrmMain.Controls.Find("LopCL", true)[0] as SpinEdit;
                seLCL.Enter += new EventHandler(seLCL_Enter);
            }
            CalcEdit ceGB = _data.FrmMain.Controls.Find("GiaBan", true)[0] as CalcEdit;
            ceGB.Enter += new EventHandler(ceGB_Enter);
            CalcEdit ceKTT = _data.FrmMain.Controls.Find("KhoTT", true)[0] as CalcEdit;
            ceKTT.Enter += new EventHandler(ceKTT_Enter);
            CalcEdit ceDao = _data.FrmMain.Controls.Find("Dao", true)[0] as CalcEdit;
            ceDao.Enter += new EventHandler(ceDao_Enter);
            cbeLoaiThung = _data.FrmMain.Controls.Find("LoaiThung", true)[0] as ComboBoxEdit;
            cbeLoai = _data.FrmMain.Controls.Find("Loai", true)[0] as ComboBoxEdit;
            cbeLoai.EditValueChanged += new EventHandler(cbeLoai_EditValueChanged);
            CalcEdit ceDai = _data.FrmMain.Controls.Find("Dai", true)[0] as CalcEdit;
            ceRong = _data.FrmMain.Controls.Find("Rong", true)[0] as CalcEdit;

            ceLop = _data.FrmMain.Controls.Find("Lop", true)[0] as SpinEdit;
            ceLop.EditValueChanged += new EventHandler(ceLop_EditValueChanged);

            if (tableName == "MTDonHang" || tableName == "MTLSX")
            {
                gluTHS = _data.FrmMain.Controls.Find("THS", true)[0] as GridLookUpEdit;
                gluTHS.Popup += new EventHandler(gluTHS_Popup);
            }
       
            GridLookUpEdit gluMat = _data.FrmMain.Controls.Find("Mat_Giay", true)[0] as GridLookUpEdit;
            GridLookUpEdit gluSB = _data.FrmMain.Controls.Find("SB_Giay", true)[0] as GridLookUpEdit;
            GridLookUpEdit gluMB = _data.FrmMain.Controls.Find("MB_Giay", true)[0] as GridLookUpEdit;
            GridLookUpEdit gluSC = _data.FrmMain.Controls.Find("SC_Giay", true)[0] as GridLookUpEdit;
            GridLookUpEdit gluMC = _data.FrmMain.Controls.Find("MC_Giay", true)[0] as GridLookUpEdit;
            GridLookUpEdit gluSE = _data.FrmMain.Controls.Find("SE_Giay", true)[0] as GridLookUpEdit;
            GridLookUpEdit gluME = _data.FrmMain.Controls.Find("ME_Giay", true)[0] as GridLookUpEdit;
            lstGiay.AddRange(new GridLookUpEdit[] { gluMat, gluSB, gluMB, gluSC, gluMC, gluSE, gluME });
            SetDMGiay();
            foreach (GridLookUpEdit glu in lstGiay)
            {
                glu.Popup += new EventHandler(gluGiay_Popup);
                glu.KeyDown += new KeyEventHandler(glu_KeyDown);
                glu.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(glu_CloseUp);
            }
            
            if (tableName == "MTDonHang" || tableName == "MTLSX")
            {
                ceDai.Enter += new EventHandler(ceDai_Enter);
                ceRong.Enter += new EventHandler(ceRong_Enter);
                gvMain.OptionsView.EnableAppearanceEvenRow = false;
                gvMain.OptionsView.EnableAppearanceOddRow = false;
                gvMain.Appearance.FocusedRow.BackColor = Color.Transparent;
                gvMain.RowStyle += new RowStyleEventHandler(gvMain_RowStyle);
            }

            if (tableName == "MTDonHang")
            {
                ceRong.EditValueChanged += new EventHandler(ceRong_EditValueChanged);
                ceRong.Leave += new EventHandler(ceRong_Leave);
            }
            gvMain.ShownEditor += new EventHandler(gvMain_ShownEditor);

            _data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
            BsMain_DataSourceChanged(_data.BsMain, new EventArgs());
        }

        void ceCCL_EditValueChanged(object sender, EventArgs e)
        {
            TinhDaoCL();
        }

        void ceRCL_EditValueChanged(object sender, EventArgs e)
        {
            TinhDaoCL();
        }

        void ceXX_EditValueChanged(object sender, EventArgs e)
        {
            TinhDaoX();
        }

        private void TinhDaoX()
        {
            if (ceRong == null || ceRong.Properties.ReadOnly
                || ceXX == null || ceXX.Properties.ReadOnly)
                return;
            if (ceRong.EditValue == null || String.IsNullOrEmpty(ceRong.EditValue.ToString())
                || ceXX.EditValue == null || String.IsNullOrEmpty(ceXX.EditValue.ToString()))
                return;
            if (Convert.ToDecimal(ceXX.EditValue) == 0)
                seDaoX.EditValue = 0;
            else
                seDaoX.EditValue = Math.Min(5, Math.Round(Convert.ToDecimal(ceRong.EditValue) / Convert.ToDecimal(ceXX.EditValue), 0, MidpointRounding.ToEven));
        }

        private void TinhDaoCL()
        {
            if (ceRong == null || ceRong.Properties.ReadOnly
                || ceRCL == null || ceRCL.Properties.ReadOnly
                || ceCCL == null || ceCCL.Properties.ReadOnly)
                return;
            if (ceRong.EditValue == null || String.IsNullOrEmpty(ceRong.EditValue.ToString())
                || ceRCL.EditValue == null || String.IsNullOrEmpty(ceRCL.EditValue.ToString())
                || ceCCL.EditValue == null || String.IsNullOrEmpty(ceCCL.EditValue.ToString()))
                return;
            if (ceLop.EditValue == null || String.IsNullOrEmpty(ceLop.EditValue.ToString()))
                return;
            decimal t;
            if (Convert.ToInt32(ceLop.EditValue) == 3)
                t = 0.2M;
            else
                if (Convert.ToInt32(ceLop.EditValue) == 5)
                    t = 0.4M;
                else
                    t = 0.6M;
            if (Convert.ToDecimal(ceRCL.EditValue) == 0 || Convert.ToDecimal(ceCCL.EditValue) == 0)
                seDaoCL.EditValue = 0;
            else
                seDaoCL.EditValue = Math.Min(5, Math.Round(Convert.ToDecimal(ceRong.EditValue) / (Convert.ToDecimal(ceRCL.EditValue) +
                    Convert.ToDecimal(ceCCL.EditValue) + t), 0, MidpointRounding.ToEven));
        }

        void seLCL_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceCL.Checked);
        }

        void ceDCL_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceCL.Checked);
        }

        void seDao_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceXa.Checked);
        }

        void ceXX_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceXa.Checked);
        }

        void gluTHS_Popup(object sender, EventArgs e)
        {
            //if (ceLop.Properties.ReadOnly)
            //    return;
            int soLop = Convert.ToInt32(ceLop.EditValue);
            GridLookUpEdit glu = sender as GridLookUpEdit;
            glu.Properties.View.ActiveFilterString = "SoLop=" + soLop.ToString();
        }

        void ceLop_EditValueChanged(object sender, EventArgs e)
        {
            
            //if (ceLop.Properties.ReadOnly)
                //return;
            if (ceLop.EditValue == null || ceLop.EditValue.ToString() == "")
                return;
            SpinEdit glu = sender as SpinEdit;
         
            switch (Convert.ToInt32(glu.EditValue))
            {
                case 3:
                    foreach (GridLookUpEdit gl in lstGiay)

                    {
                        if (gl.Name == "Mat_Giay" || gl.Name == "SB_Giay" || gl.Name == "MB_Giay")
                            gl.Enabled = true;
                        else
                            gl.Enabled = false;
                    }
                    break;
                case 5:
                    foreach (GridLookUpEdit gl in lstGiay)
                    {
                        if (gl.Name == "Mat_Giay" || gl.Name == "SB_Giay" || gl.Name == "MB_Giay" || gl.Name == "SC_Giay" || gl.Name == "MC_Giay")
                            gl.Enabled = true;
                        else
                            gl.Enabled = false;
                    }
                    break;
                case 7:
                    foreach (GridLookUpEdit gl in lstGiay)
                    {
                        gl.Enabled = true;
                    }
                    break;
                default:
                    foreach (GridLookUpEdit gl in lstGiay)
                    {
                        gl.Enabled = false;
                    }
                    XtraMessageBox.Show("Số lớp không hợp lệ!");
                    break;
            }
            TinhDaoCL();
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

        void ceRong_EditValueChanged(object sender, EventArgs e)
        {
            if (ceRong.Properties.ReadOnly || focusing)
                return;
            doikho = true;
            TinhDaoX();
            TinhDaoCL();
        }

        void ceRong_Leave(object sender, EventArgs e)
        {
            if (ceRong.Properties.ReadOnly || focusing || !doikho)
                return;
            gvMain.UpdateCurrentRow();
            gvMain.SetFocusedRowCellValue(gvMain.Columns["Mat_Kho"], DBNull.Value);
            gvMain.UpdateCurrentRow();
            doikho = false;
        }

        private void CeGiaBanMoi_CheckedChanged(object sender, EventArgs e)
        {
            var ceGiaBanMoi = sender as CheckEdit;
            if (ceGiaBanMoi.Properties.ReadOnly)
            {
                return;
            }
            var giaFieldName = ceGiaBanMoi.Checked ? "GiaBanMoi" : "GiaBan";
            DataRow drCur = (_data.BsMain.Current as DataRowView).Row;
            gvMain.UpdateCurrentRow();
            DataRow dr = gvMain.GetDataRow(gvMain.FocusedRowHandle);
            foreach (string s in lstNL)
            {
                string ma = dr[s + "Giay"].ToString();
                if (ma == "")
                    continue;
                DataRow drNL = dtNL.Rows.Find(ma);
                if (drNL != null)
                {
                    gvMain.SetFocusedRowCellValue(gvMain.Columns[s + "DG"], drNL[giaFieldName]);
                    gvMain.UpdateCurrentRow();
                }
            }
        }

        void btnKho_Click(object sender, EventArgs e)
        {
            if (ceRong.Properties.ReadOnly)
            {
                XtraMessageBox.Show("Vui lòng thực hiện khi thêm/sửa đơn hàng",
                    Config.GetValue("PackageName").ToString());
                return;
            }
            
            DataRow drCur = (_data.BsMain.Current as DataRowView).Row;
            gvMain.UpdateCurrentRow();
            DataRow dr = gvMain.GetDataRow(gvMain.FocusedRowHandle);
            if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                return;
            if (dr.RowState == DataRowState.Modified
                && dr["Rong", DataRowVersion.Original].ToString() == dr["Rong", DataRowVersion.Current].ToString())
                return;

            int khoMoi = Convert.ToInt32(dr["Rong"]);
            if (Convert.ToInt32(Config.GetValue("KhoMax").ToString()) / Convert.ToInt32(dr["Rong"].ToString()) >= 2)
            {
                XtraForm1 frmKho = new XtraForm1();
                frmKho.Text = Config.GetValue("PackageName").ToString();
                frmKho.StartPosition = FormStartPosition.CenterScreen;
                frmKho.ShowDialog();
                khoMoi = Convert.ToInt32(frmKho.Tag);
            }

            string msg = "";
            string ssKho = "";
            foreach (string s in lstNL)
            {
                string ma = dr[s + "Giay"].ToString();
                if (ma == "")
                    continue;
                DataRow drNL = dtNL.Rows.Find(ma);
                string maphu = drNL["MaPhu"].ToString();
                DataRow[] drs = dtNL.Select("MaPhu = '" + maphu + "' and Kho >= " + khoMoi.ToString(), "Kho");
                
                if (drs.Length > 0)
                {
                    gvMain.SetFocusedRowCellValue(gvMain.Columns[s + "Giay"], drs[0]["Ma"]);
                    gvMain.SetFocusedRowCellValue(gvMain.Columns[s + "Kho"], drs[0]["Kho"]);
                    if(ssKho == "")
                        ssKho = drs[0]["Kho"].ToString();
                    if (ssKho != "" && ssKho != drs[0]["Kho"].ToString() && msg == "")
                        msg = "Khổ giấy mặt/sóng không giống nhau";
                }
                else
                {
                    XtraMessageBox.Show(dr["TenHang"].ToString() + " không có khổ giấy phù hợp!",
                        Config.GetValue("PackageName").ToString());
                }
            }

            if(msg != "")
                XtraMessageBox.Show(msg,Config.GetValue("PackageName").ToString());
            SetDMGiay();

            //tinh kho max
            //decimal tmp = 0;
            //foreach (string s in lstNL)
            //{
            //    decimal sk = gvMain.GetFocusedRowCellValue(s + "Kho").ToString() == "" ? 0 : decimal.Parse(gvMain.GetFocusedRowCellValue(s + "Kho").ToString());
            //    if (sk > tmp)
            //        tmp = sk;
            //}
            //gvMain.SetFocusedRowCellValue(gvMain.Columns["KhoMax"], tmp);

            //Tính dao
            decimal dao=0;
            decimal rong = dr["Rong"].ToString() == "" ? 0 : decimal.Parse(dr["Rong"].ToString());
            decimal t = Math.Round(Decimal.Parse(Config.GetValue("KhoMax").ToString()) / rong, 1);
            if (t >= 1 && Convert.ToDecimal(t)  <= Convert.ToDecimal(1.9))
                dao = 1;
            if (t >= 2 && Convert.ToDecimal(t) <= Convert.ToDecimal(2.9))
                dao = 2;
            if (t >= 3 && Convert.ToDecimal(t) <= Convert.ToDecimal(3.9))
                dao = 3;
            if (t >= 4 && Convert.ToDecimal(t) <= Convert.ToDecimal(4.9))
                dao = 4;
            if (t >= 5)// && Convert.ToDecimal(t) <= Convert.ToDecimal(5.9))
                dao = 5;
            //if (t >= 6 && Convert.ToDecimal(t) <= Convert.ToDecimal(6.9))
            //    dao = 6;
            //if (t >= 7 && Convert.ToDecimal(t) <= Convert.ToDecimal(7.9))
            //    dao = 7;
            //if (t >= 8 && Convert.ToDecimal(t) <= Convert.ToDecimal(8.9))
            //    dao = 8;
            //if (t >= 9 && Convert.ToDecimal(t) <= Convert.ToDecimal(9.9))
            //    dao = 9;
            //if (t >= 10 && Convert.ToDecimal(t) <= Convert.ToDecimal(10.9))
            //    dao = 10;
            gvMain.SetFocusedRowCellValue(gvMain.Columns["Dao"], dao);
            gvMain.UpdateCurrentRow();

        }
        void glu_KeyDown(object sender, KeyEventArgs e)
        {
            GridLookUpEdit glu = sender as GridLookUpEdit;
            if (!glu.Properties.ReadOnly && e.KeyCode == Keys.Delete)
            {
                string fn1 = glu.Name.Replace("_Giay", "_Kho");
                gvMain.SetFocusedRowCellValue(gvMain.Columns[fn1], DBNull.Value);
                string fn2 = glu.Name.Replace("_Giay", "_DL");
                gvMain.SetFocusedRowCellValue(gvMain.Columns[fn2], DBNull.Value);
                string fn3 = glu.Name.Replace("_Giay", "_DG");
                gvMain.SetFocusedRowCellValue(gvMain.Columns[fn3], DBNull.Value);
            }
        }
        void glu_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            if (cbeLoai.EditValue == null || cbeLoai.EditValue.ToString() == "")
                return;
            GridLookUpEdit glu = sender as GridLookUpEdit;
            if (cbeLoai.EditValue.ToString() == "Thùng")
            {
                string fn = glu.Name.Replace("_Giay", "_Kho");
                gvMain.SetFocusedRowCellValue(gvMain.Columns[fn], glu.Properties.View.GetFocusedRowCellValue("Kho"));
            }
        }

        void gluGiay_Popup(object sender, EventArgs e)
        {
            if (cbeLoai.EditValue == null || cbeLoai.EditValue.ToString() == "")
                return;
            GridLookUpEdit glu = sender as GridLookUpEdit;
            if (cbeLoai.EditValue.ToString() == "Tấm")
            {
                glu.Properties.View.Columns["Ma"].Visible = false;
                glu.Properties.View.Columns["MaPhu"].VisibleIndex = 0;
                glu.Properties.View.Columns["Kho"].Visible = false;
            }
            else
            {
                glu.Properties.View.Columns["Ma"].VisibleIndex = 0;
                glu.Properties.View.Columns["MaPhu"].Visible = false;
                glu.Properties.View.Columns["Kho"].VisibleIndex = 9;
            }
        }

        void SetDMGiay()
        {
            if (cbeLoai.EditValue == null || cbeLoai.EditValue.ToString() == "")
                return;
            string dm;
            if (tableName == "MTBaoGia")
            {
                dm = (cbeLoai.EditValue.ToString() == "Tấm") ? "MaPhu" : "Ma";
                if (cbeLoai.EditValue.ToString() == "Tấm")
                    cbeLoaiThung.EditValue = null;
                else
                    cbeLoaiThung.EditValue = "Thường";
            } 
            else
                if (ceRong.Properties.ReadOnly || cbeLoai.EditValue.ToString() == "Thùng")
                    dm = "Ma";
                else
                    dm = (ceRong.EditValue == null || ceRong.EditValue.ToString() == "") ? "MaPhu" : "Ma";
            foreach (GridLookUpEdit glu in lstGiay)
                glu.Properties.DisplayMember = dm;
        }

        void gvMain_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0 && gvMain.IsDataRow(e.RowHandle))
            {
                object slbg = gvMain.GetRowCellValue(e.RowHandle, "SLBG");
                object sldh = gvMain.GetRowCellValue(e.RowHandle, "SoLuong");
                if (slbg == DBNull.Value || sldh == DBNull.Value)
                    e.Appearance.BackColor = Color.Transparent;
                else
                {
                    if (decimal.Parse(slbg.ToString()) > decimal.Parse(sldh.ToString()))
                        e.Appearance.BackColor = Color.Red;
                    else
                        e.Appearance.BackColor = Color.Transparent;
                }
            }
        }

        void gluKH_EditValueChanged(object sender, EventArgs e)
        {
            if (gluKH.Properties.ReadOnly)
                return;
            if (gvMain.DataRowCount == 0)    //grid chi tiet chua co dong nao
            {
                gvMain.UpdateCurrentRow();
                gvMain.AddNewRow();
                gvMain.UpdateCurrentRow();
            }
            else
            {
                DataTable dt = (_data.BsMain.DataSource as DataSet).Tables[1];
                for (int i = 0; i < gvMain.DataRowCount; i++)
                {
                    DataRow dr = gvMain.GetDataRow(i);
                    dtBaoGia_ColumnChanged(dt, new DataColumnChangeEventArgs(dr, dt.Columns["Loai"], dr["Loai"]));
                }
            }
        }

        void cbeLoai_EditValueChanged(object sender, EventArgs e)
        {
            if (cbeLoai.EditValue == null)
                return;
            string loai = cbeLoai.EditValue.ToString();
            if (loai == "")
                return;
            SetDMGiay();
            if (tableName == "MTDonHang")
            {
                gvMain.Columns["Dai"].OptionsColumn.AllowFocus = (loai == "Tấm");
                gvMain.Columns["Rong"].OptionsColumn.AllowFocus = (loai == "Tấm");
                LayoutControlItem lci = lcMain.Items.FindByName("cusKho") as LayoutControlItem;
                if (lci != null)
                    //lci.Visibility = LayoutVisibility.Always;
                    lci.Visibility = (loai == "Thùng") ? LayoutVisibility.Never : LayoutVisibility.Always;
            }
            if (gluKH.EditValue == DBNull.Value)
                XtraMessageBox.Show("Vui lòng chọn khách hàng trước", Config.GetValue("PackageName").ToString());
            bool isThung = loai == "Thùng";
            LayoutVisibility lv = isThung ? LayoutVisibility.Always : LayoutVisibility.Never;
            foreach (BaseLayoutItem li in lcMain.Items)
            {
                if (li.Text == "Loại thùng")
                    li.Visibility = lv;
                if (li.GetType() != typeof(LayoutControlGroup))
                    continue;
                if (li.Text == "Thung")
                    li.Visibility = lv;
                if (li.Text == "Tấm")
                    //li.Visibility = LayoutVisibility.Always;
                    li.Visibility = isThung ? LayoutVisibility.Never : LayoutVisibility.Always;
                if (tableName == "MTBaoGia" && li.Text == "Khổ")
                    li.Visibility = lv;
            }
        }

        void ceDao_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceKCT.Checked);
        }

        void ceKTT_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceKCT.Checked);
        }

        void ceGB_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceKCT.Checked);
        }

        void ceDT_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, !ceKCT.Checked);
        }

        void ceRong_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, cbeLoai.Text == "Thùng");
        }

        void ceDai_Enter(object sender, EventArgs e)
        {
            KhongChoChon(sender, cbeLoai.Text == "Thùng");
        }

        private void KhongChoChon(object sender, bool condition)
        {
            if (condition)
                lcMain.FocusHelper.GetNextControl(sender).Focus();
        }

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataTable dtBaoGia = (_data.BsMain.DataSource as DataSet).Tables[1];
            dtBaoGia.ColumnChanged += new DataColumnChangeEventHandler(dtBaoGia_ColumnChanged);
        }

        void CapNhatDieuKhoan(DataRow dr)
        {
            string loai = dr["Loai"].ToString();
            if (loai == "")
                dr["DieuKhoan"] = DBNull.Value;
            if (loai == "Tấm")
            {
                if (dkTam == "")
                {
                    object o = db.GetValue("select top 1 DkTam from CHDKBG");
                    dkTam = o == null ? "" : o.ToString();
                }
                if (dr["DieuKhoan"].ToString() != dkTam)
                    dr["DieuKhoan"] = dkTam;
            }
            if (loai == "Thùng")
            {
                if (dkThung == "")
                {
                    object o = db.GetValue("select top 1 DkThung from CHDKBG");
                    dkThung = o == null ? "" : o.ToString();
                }
                if (dr["DieuKhoan"].ToString() != dkThung)
                    dr["DieuKhoan"] = dkThung;
            }
            dr.EndEdit();
        }

        void gvMain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (!ceRong.Properties.ReadOnly)
            {
                focusing = false;
                if (e.PrevFocusedRowHandle >= 0)
                {
                    object opr = gvMain.GetRowCellValue(e.PrevFocusedRowHandle, "Rong");
                    object or = gvMain.GetRowCellValue(e.FocusedRowHandle, "Rong");
                    string pr = (opr == null || opr.ToString() == "") ? "" : opr.ToString();
                    string r = (or == null || or.ToString() == "") ? "" : or.ToString();
                    if ((pr != "" && r == "") || (pr == "" && r != ""))
                        SetDMGiay();
                }
            }
        }

        void gvMain_BeforeLeaveRow(object sender, DevExpress.XtraGrid.Views.Base.RowAllowEventArgs e)
        {
            if (!ceRong.Properties.ReadOnly)
                focusing = true;
        }

        private void XuLyXaCanLan(DataRow dr)
        {
            if (!Convert.ToBoolean(dr["isXa"]))
            {
                dr["XaX"] = DBNull.Value;
                dr["DaoX"] = DBNull.Value;
            }
            if (!Convert.ToBoolean(dr["isCL"]))
            {
                dr["DaiCL"] = DBNull.Value;
                dr["RongCL"] = DBNull.Value;
                dr["CaoCL"] = DBNull.Value;
                dr["LopCL"] = DBNull.Value;
            }
            else
                dr["LopCL"] = dr["Lop"];
            dr.EndEdit();
        }

        void dtBaoGia_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {

            if (!_data.FrmMain.Visible || focusing)     //form chua hien thi, hoac dang change focus cua grid mat hang -> khong chay cong thuc
                return;
            if (tableName == "MTBaoGia" && e.Column.ColumnName == "Loai")
                CapNhatDieuKhoan(e.Row);
            string f = e.Column.ColumnName.ToUpper();
            if (f.EndsWith("GIAY") && gluKH.EditValue == DBNull.Value)
                XtraMessageBox.Show("Vui lòng chọn khách hàng", Config.GetValue("PackageName").ToString());
            if (f.EndsWith("GIAY") && e.Row["Lop"] == DBNull.Value)
                XtraMessageBox.Show("Vui lòng chọn số lớp", Config.GetValue("PackageName").ToString());
            if (e.Row["Loai"].ToString() == "" || e.Row["Lop"].ToString() == "")
                return;

            if (tableName == "MTBaoGia" && f.EndsWith("GIAY") && e.ProposedValue != null)
            {
                var isGiaMoi = Convert.ToBoolean(e.Row["isGiaMoi"]);
                var drNL = dtNL.Rows.Find(e.ProposedValue);
                if (drNL != null)
                {
                    e.Row[f.Replace("GIAY", "DG")] = isGiaMoi ? drNL["GiaBanMoi"] : drNL["GiaBan"];
                    e.Row.EndEdit();
                }
            }

            bool isTam = e.Row["Loai"].ToString() == "Tấm";

            //tinh thanh tien cua DonHang
            if (tableName == "MTDonHang")
            {
                if (f == "ISXA" || f == "ISCL")
                    XuLyXaCanLan(e.Row);
                if (f == "GIANBAN" || f == "SOLUONG" || f == "DAI" || f == "RONG")
                {
                    decimal gb = e.Row["GiaBan"].ToString() == "" ? 0 : decimal.Parse(e.Row["GiaBan"].ToString());
                    decimal sl = e.Row["SoLuong"].ToString() == "" ? 0 : decimal.Parse(e.Row["SoLuong"].ToString());
                    decimal d = e.Row["Dai"].ToString() == "" ? 0 : decimal.Parse(e.Row["Dai"].ToString());
                    decimal r = e.Row["Rong"].ToString() == "" ? 0 : decimal.Parse(e.Row["Rong"].ToString());
                    if (isTam)
                    {
                        e.Row["SL2"] = Math.Round(sl * d * r / 10000, 0);
                        //e.Row["ThanhTien"] = Math.Round(sl * gb * d * r / 10000, 0);
                        e.Row["ThanhTien"] = Math.Round(sl * d * r / 10000, 0) * Math.Round(gb, 0);
                    }
                    else
                        e.Row["ThanhTien"] = sl * gb;
                }
            }

            if (Boolean.Parse(e.Row["KCT"].ToString()))     //neu khong co cong thuc -> khong tinh gia ban
                return;
            int lop = Int32.Parse(e.Row["Lop"].ToString());

            //dinh luong 7 lop
            decimal mdl = e.Row["Mat_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["Mat_DL"].ToString());
            decimal sbdl = e.Row["SB_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["SB_DL"].ToString());
            decimal mbdl = e.Row["MB_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["MB_DL"].ToString());
            decimal scdl = e.Row["SC_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["SC_DL"].ToString());
            decimal mcdl = e.Row["MC_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["MC_DL"].ToString());
            decimal sedl = e.Row["SE_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["SE_DL"].ToString());
            decimal medl = e.Row["ME_DL"].ToString() == "" ? 0 : decimal.Parse(e.Row["ME_DL"].ToString());

            //kho 7 lop
            decimal mk = e.Row["Mat_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["Mat_Kho"].ToString());
            decimal sbk = e.Row["SB_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["SB_Kho"].ToString());
            decimal mbk = e.Row["MB_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["MB_Kho"].ToString());
            decimal sck = e.Row["SC_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["SC_Kho"].ToString());
            decimal mck = e.Row["MC_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["MC_Kho"].ToString());
            decimal sek = e.Row["SE_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["SE_Kho"].ToString());
            decimal mek = e.Row["ME_Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row["ME_Kho"].ToString());

            //don gia kg 7 lop
            decimal mdg = e.Row["Mat_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["Mat_DG"].ToString());
            decimal sbdg = e.Row["SB_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["SB_DG"].ToString());
            decimal mbdg = e.Row["MB_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["MB_DG"].ToString());
            decimal scdg = e.Row["SC_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["SC_DG"].ToString());
            decimal mcdg = e.Row["MC_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["MC_DG"].ToString());
            decimal sedg = e.Row["SE_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["SE_DG"].ToString());
            decimal medg = e.Row["ME_DG"].ToString() == "" ? 0 : decimal.Parse(e.Row["ME_DG"].ToString());

            if (isTam)
            {
                //công sửa ngày 20/4/2015: chỉ nhảy đơn giá tấm đối với báo giá
                if (tableName == "MTBaoGia" && ((f.EndsWith("_DL") || f.EndsWith("_DG") || f == "KCT" || f == "CPK" || f == "CK" || f == "LOAI" || f == "LOP" || f == "MAKH")))
                    //|| f == "SOLUONG" || f == "DAI" || f == "DAO")
                {
                    //tinh tong don gia giay
                    decimal g = (mdl * mdg) / 1000
                                + 1.5M * (sbdl * sbdg) / 1000 + (mbdl * mbdg) / 1000
                                + 1.5M * (scdl * scdg) / 1000 + (mcdl * mcdg) / 1000
                                + 1.5M * (sedl * sedg) / 1000 + (medl * medg) / 1000;

                    //tinh chi phi gian tiep
                    object o = db.GetValue("select sum(Tien * HSLop) from CHChiPhi where Lop = '" + lop.ToString() + "'");
                    decimal gt = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());

                    //ty le hao hut
                    o = Config.GetValue("HH" + lop.ToString());
                    decimal hh = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());

                    //ty le loi nhuan
                    o = Config.GetValue("LN" + lop.ToString());
                    decimal ln = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());

                    //chi phi khac
                    decimal cpk = (e.Row["CPK"].ToString() == "") ? 0 : decimal.Parse(e.Row["CPK"].ToString());
                    //chiet khau
                    decimal ck = (e.Row["CK"].ToString() == "") ? 0 : decimal.Parse(e.Row["CK"].ToString());

                    //tong gia ban
                    decimal gb = (g + gt) * (100 + hh + ln) / 100 + cpk - ck;
                    decimal hoahong = LayHoaHong();
                    gb = gb / ((100 - hoahong) / 100);
                    e.Row["GiaBan"] = gb;
                    e.Row.EndEdit();
                }
            }
            else
            {
                //tinh kho max
                decimal tmp = 0;
                if (f.EndsWith("_KHO"))
                {
                    foreach (string s in lstNL)
                    {
                        decimal sk = e.Row[s + "Kho"].ToString() == "" ? 0 : decimal.Parse(e.Row[s + "Kho"].ToString());
                        if (sk > tmp)
                            tmp = sk;
                    }
                    e.Row["KhoMax"] = tmp;
                    e.Row.EndEdit();
                }
                //tinh gia ban
                if (f == "LOAITHUNG" || f.EndsWith("_DL") || f.EndsWith("_KHO") || f.EndsWith("_DG") || f == "KCT" || f == "CPK" || f == "CK" || f == "LOAI" || f == "LOP" || f == "MAKH"
                    || (f == "SOLUONG" && tableName == "MTBaoGia") || f == "SOMAU" || f == "DAI" || f == "RONG" || f == "CAO" || f == "DOKHO" || f == "DOPHU")
                {
                    //tinh dien tich
                    decimal dai = e.Row["Dai"].ToString() == "" ? 0 : decimal.Parse(e.Row["Dai"].ToString());
                    decimal rong = e.Row["Rong"].ToString() == "" ? 0 : decimal.Parse(e.Row["Rong"].ToString());
                    decimal cao = e.Row["Cao"].ToString() == "" ? 0 : decimal.Parse(e.Row["Cao"].ToString());
                    string lt = e.Row["LoaiThung"].ToString();
                    if (lt == "")
                        return;
                    decimal k = 0, d = 0;
                    if (lt == "Thường")
                    {
                        decimal t1 = lop == 3 ? 2 : 3;
                        decimal t2 = lop == 3 ? 5 : 6;
                        k = rong + cao + t1;
                        d = (dai + rong) * 2 + t2;
                    }
                    else
                        if (lt == "Âm dương")
                        {
                            decimal t1 = lop == 3 ? 2 : 3;
                            decimal t2 = 2;
                            k = rong + cao + t1 + cao;
                            d = dai + 2 * cao + t2;
                        } 
                        else
                            if (lt == "Nắp chồm")
                            {
                                decimal t1 = lop == 3 ? 2 : 3;
                                decimal t2 = lop == 3 ? 5 : 6;
                                k = rong + cao + t1 + rong;
                                d = (dai + rong) * 2 + t2;
                            }
                    decimal t3 = lop == 3 ? 5 : 6;
                    decimal dt = k >= 200 ? (d + t3) * k / 10000 : d * k / 10000;
                    e.Row["DienTich"] = dt;
                    //tinh kho tt
                    object okm = Config.GetValue("KhoMax");
                    //object okm = e.Row["KhoMax"];
                    decimal kmax = okm == DBNull.Value ? 0 : decimal.Parse(okm.ToString());
                    decimal t = Math.Round(kmax / k, 1);
                    decimal ktt = 0;
                    if (lop == 3)
                    {
                        if (t > 1 && t <= 2)
                            ktt = k;
                        if (t > 2 && t <= 3)
                            ktt = 2 * k - 2;
                        if (t > 3 && t <= 4)
                            ktt = 3 * k - 4;
                        if (t > 4 && t <= 5)
                            ktt = 4 * k - 5;
                        if (t > 5)
                            ktt = 5 * k - 7;
                    }
                    else
                    {
                        if (t > 1 && t <= 2)
                            ktt = k;
                        if (t > 2 && t <= 3)
                            ktt = 2 * k - 3;
                        if (t > 3 && t <= 4)
                            ktt = 3 * k - 6;
                        if (t > 4 && t <= 5)
                            ktt = 4 * k - 9;
                        if (t > 5)
                            ktt = 5 * k - 12;
                    }
                    e.Row["KhoTT"] = ktt;

                    //tinh dao
                    decimal dao = 0;
                    if (t > 1 && t <= 2)
                        dao = 1;
                    if (t > 2 && t <= 3)
                        dao = 2;
                    if (t > 3 && t <= 4)
                        dao = 3;
                    if (t > 4 && t <= 5)
                        dao = 4;
                    if (t > 5)// && t<= 6)
                        dao = 5;
                    //if (t > 6 && t <= 7)
                    //    dao = 6;
                    //if (t > 7 && t <= 8)
                    //    dao = 7;
                    //if (t > 8 && t <= 9)
                    //    dao = 8;
                    //if (t > 9 && t <= 10)
                    //    dao = 9;
                    //if (t > 10 && t <= 11)
                    //    dao = 10;
                    e.Row["Dao"] = dao;

                    //tinh tong don gia giay
                    decimal g = (mdl * mdg) * dt / 1000
                                + 1.5M * (sbdl * sbdg) * dt / 1000 + (mbdl * mbdg) * dt / 1000
                                + 1.5M * (scdl * scdg) * dt / 1000 + (mcdl * mcdg) * dt / 1000
                                + 1.5M * (sedl * sedg) * dt / 1000 + (medl * medg) * dt / 1000;

                    //tinh chi phi gian tiep
                    object o = db.GetValue("select sum(Tien) from CHChiPhi where Lop = '" + lop.ToString() + "'");
                    decimal gt = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());
                    gt = d >= 200 ? dt * gt * 1.2M : dt * gt;

                    decimal sl = e.Row["SoLuong"].ToString() == "" ? 0 : decimal.Parse(e.Row["SoLuong"].ToString());
                    //tinh chi phi hao hut
                    o = db.GetValue("select HaoHut from CHHaoHut where Lop = '" + lop.ToString() + "' and " + sl.ToString().Replace(",", ".") + " between TuSL and DenSL");
                    decimal tlhh = (o == null || o.ToString() == "") ? 0 : decimal.Parse(o.ToString());
                    decimal hh = tlhh * (g + gt);

                    //tinh lai
                    decimal l = (gt + g) * 0.06M;
                    //tinh so mau
                    decimal m = e.Row["SoMau"].ToString() == "" ? 0 : decimal.Parse(e.Row["SoMau"].ToString());
                    m = (m == 0) ? 0 : (dt * 200) + ((m - 1) * 50 * dt);
                    //tinh do phu
                    decimal dp = Boolean.Parse(e.Row["DoPhu"].ToString()) ? 100 : 0;
                    //tinh do kho
                    decimal dk = Boolean.Parse(e.Row["DoKho"].ToString()) ? (g + hh) * 0.02M : 0;

                    //gia ban chua co lai lo
                    decimal gb = g + hh + gt + l + m + dp + dk;

                    //tinh lai lo
                    if (dao == 0)
                        return;
                    decimal ld = (d >= 200) ? d + 6 : d;
                    decimal glo = ((mk - ktt) * ld / 10000) * ((mdl * mdg) / 1000)
                                + 1.5M * ((sbk - ktt) * ld / 10000) * (sbdl * sbdg) / 1000 + ((mbk - ktt) * ld / 10000) * (mbdl * mbdg) / 1000
                                + 1.5M * ((sck - ktt) * ld / 10000) * (scdl * scdg) / 1000 + ((mck - ktt) * ld / 10000) * (mcdl * mcdg) / 1000
                                + 1.5M * ((sek - ktt) * ld / 10000) * (sedl * sedg) / 1000 + ((mek - ktt) * ld / 10000) * (medl * medg) / 1000;
                    glo = glo / dao;

                    decimal lk = 0;
                    if (t >= 1 && t < 2)
                        lk = d * 0;
                    if (t >= 2 && t < 3)
                        lk = d * 1.6M;
                    if (t >= 3 && t < 4)
                        lk = d * 3.2M;
                    if (t >= 4)
                        lk = d * 4.8M;
                    lk = lk / 10000 / dao;
                    decimal glai = lk * ((mdl * mdg) / 1000)
                                + 1.5M * lk * (sbdl * sbdg) / 1000 + lk * (mbdl * mbdg) / 1000
                                + 1.5M * lk * (scdl * scdg) / 1000 + lk * (mcdl * mcdg) / 1000
                                + 1.5M * lk * (sedl * sedg) / 1000 + lk * (medl * medg) / 1000;
                    glai = glai / dao;
                    decimal lailo = glo - glai;
                    gb = lailo > 0 ? gb + lailo : gb;

                    //chi phi khac
                    decimal cpk = (e.Row["CPK"].ToString() == "") ? 0 : decimal.Parse(e.Row["CPK"].ToString());
                    //chiet khau
                    decimal ck = (e.Row["CK"].ToString() == "") ? 0 : decimal.Parse(e.Row["CK"].ToString());

                    //tong gia ban
                    gb = gb + cpk - ck;
                    decimal hoahong = LayHoaHong();
                    gb = gb / ((100 - hoahong) / 100);
                    e.Row["GiaBan"] = gb;
                    e.Row.EndEdit();
                }
            }
        }

        decimal LayHoaHong()
        {
            if (gluKH.EditValue == null || gluKH.EditValue.ToString() == "")
                return 0;
            DataTable dtKH = (gluKH.Properties.DataSource as BindingSource).DataSource as DataTable;
            DataRow[] drs = dtKH.Select("MaKH = '" + gluKH.EditValue.ToString() + "'");
            if (drs.Length == 0)
                return 0;
            decimal hhpt = drs[0]["NVPT_HH"].ToString() == "" ? 0 : decimal.Parse(drs[0]["NVPT_HH"].ToString());
            decimal hhtm = drs[0]["NVTM_HH"].ToString() == "" ? 0 : decimal.Parse(drs[0]["NVTM_HH"].ToString());
            return (hhtm + hhpt);
        }

        void gvMain_ShownEditor(object sender, EventArgs e)
        {
            if (deNgayCT.Properties.ReadOnly)
                return;

            GridView gv = sender as GridView;

            if (tableName == "MTLSX" || tableName == "MTDonHang" || tableName == "MTBaoGia")
            {
                gvMain.OptionsBehavior.Editable = true;
                //List<string> lstCol = new List<string>(new string[] { "MSong", "SLTT", "SLKD", "SLDat", "LSong", "SLDC", "MTLSXID" });
                foreach (GridColumn i in gvMain.Columns)
                {
                    //if (lstCol.Contains(i.FieldName))
                    //    i.OptionsColumn.AllowEdit = true;
                    //else
                    i.OptionsColumn.AllowEdit = false;
                }
            }
            else
                gvMain.OptionsBehavior.Editable = false;
        }
    }
}
