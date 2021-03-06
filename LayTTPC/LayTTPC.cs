using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System.Data;
using CDTDatabase;
using CDTLib;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
using DevExpress.XtraGrid.Columns;
using FormFactory;
using System.Windows.Forms;

namespace LayTTPC
{
    public class LayTTPC : ICControl
    {
        DataRow drCur;
        GridView gvMain;
        DataCustomFormControl _data;
        InfoCustomControl _info = new InfoCustomControl(IDataType.MasterDetailDt);
        #region ICControl Members

        public void AddEvent()
        {
            gvMain = (_data.FrmMain.Controls.Find("gcMain", true)[0] as GridControl).MainView as GridView;
            //thêm nút chọn DH
            LayoutControl lcMain = _data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            // thêm nút lấy số phiếu cân
            SimpleButton btnLayTTCan = new SimpleButton();
            btnLayTTCan.Name = "btnLayTTCan";
            btnLayTTCan.Text = "Lấy thông tin cân xe";
            LayoutControlItem lci2 = lcMain.AddItem("", btnLayTTCan);
            lci2.Name = "cusLayTTCan";
            btnLayTTCan.Click += new EventHandler(btnLayTTCan_Click);
        }
        void btnLayTTCan_Click(object sender, EventArgs e)
        {
            drCur = (_data.BsMain.Current as DataRowView).Row;
            if (!gvMain.Editable)
            {
                XtraMessageBox.Show("Vui lòng chọn chế độ thêm hoặc sửa phiếu",
                    Config.GetValue("PackageName").ToString());
                return;
            }
            if (drCur["SoPC"] == DBNull.Value)
            {
                XtraMessageBox.Show("Vui lòng nhập số phiếu cân!",
                    Config.GetValue("PackageName").ToString());
                return;
            }
            else
            {
                string vitrukconn = "Server = 113.161.95.123,1436\\HOATIEU2K8R2; database = VitruckWeigh302_20160909V1; user = sa; pwd = Makiut123";
                Database vitruk = Database.NewCustomDatabase(vitrukconn);
                string sophieu = drCur["SoPC"].ToString();
                XtraMessageBox.Show("Số phiếu cân là: " + sophieu.ToString());
                object weight1 = vitruk.GetValue(string.Format("select top 1 Weight1 from WeighVoucher where WvId = '{0}'", sophieu));
                object weight2 = vitruk.GetValue(string.Format("select top 1 Weight2 from WeighVoucher where WvId = '{0}'", sophieu));
                object weight_fin = vitruk.GetValue(string.Format("select top 1 Weight from WeighVoucher where WvId = '{0}'", sophieu));
                object carplate1 = vitruk.GetValue(string.Format("select top 1 PlateNumber1 from WeighVoucher where WvId = '{0}'", sophieu));
                object carplate2 = vitruk.GetValue(string.Format("select top 1 PlateNumber2 from WeighVoucher where WvId = '{0}'", sophieu));
                object time1 = vitruk.GetValue(string.Format("select top 1 convert(varchar, DateTime1, 103) + ' '+ convert(varchar, DateTime1, 108) from WeighVoucher where WvId = '{0}'", sophieu));
                object time2 = vitruk.GetValue(string.Format("select top 1 convert(varchar, DateTime2, 103) + ' '+ convert(varchar, DateTime2, 108) from WeighVoucher where WvId = '{0}'", sophieu));
                XtraMessageBox.Show("Số phiếu cân là: " + sophieu.ToString() + "\n" +
                                    "Biển số 1: " + carplate1.ToString() + "\n" +
                                    "Biển số 2: " + carplate2.ToString() + "\n" +
                                    "Cân lần 1: " + weight1.ToString() + "\n" +
                                    "Thời gian: " + time1.ToString() + "\n" +
                                    "Cân lần 2: " + weight2.ToString() + "\n" +
                                    "Thời gian: " + time2.ToString() + "\n" +
                                    "Trọng lượng hàng: " + weight_fin.ToString() 
                                    );
                drCur["SoCV"] = weight1.ToString();
                drCur["SoCR"] = weight2.ToString();
                drCur["SokgTN"] = weight_fin.ToString();
                drCur["SoXe"] = carplate1.ToString();
                
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
