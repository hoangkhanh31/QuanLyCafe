using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Menu = QuanLyQuanCafe.DTO.Menu;

namespace QuanLyQuanCafe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get => loginAccount;
            set
            {
                loginAccount = value;
                ChangeAccount(loginAccount.Type);
            }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc;
            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbSwitchTable);
        }
        #region Methods
        void ChangeAccount(int type)
        {
            //if (type != 1)
            //{
            //    adminToolStripMenuItem.Visible = false;
            //}
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
        }
        void LoadTable()
        {
            flpTable.Controls.Clear();

            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight};
                btn.Text = item.Name + "\n" + item.Status;
                flpTable.Controls.Add(btn);
                btn.Click += Btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.LightPink;
                        break;
                }
            }
        }
        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<Menu> listBillInfo = MenuDAO.Instance.getListMenuByTable(id);
            float totalPrice = 0;

            foreach (Menu item in listBillInfo) 
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());

                lsvBill.Items.Add(lsvItem);
                totalPrice += item.TotalPrice;
            }
            CultureInfo culture = new CultureInfo("vi-VN");
            
            //Thread.CurrentThread.CurrentCulture = culture;    //setting lại cho ứng dụng sang ngôn ngữ = culture
            
            txbTotalPrice.Text = totalPrice.ToString("c", new CultureInfo("vi"));
        }
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";

        }
        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }
        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }
        #endregion

        #region Events
        private void Btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).Id;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinTàiKhoảnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += F_EventUpdateAccount;
            f.ShowDialog();
        }

        private void F_EventUpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.InsertFood += F_InsertFood;
            f.DeleteFood += F_DeleteFood;
            f.UpdateFood += F_UpdateFood;
            f.ShowDialog();
        }

        private void F_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).Id);
        }

        private void F_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).Id);
            LoadTable();
        }

        private void F_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if(lsvBill.Tag !=null)
                ShowBill((lsvBill.Tag as Table).Id);
        }
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb?.SelectedIndex == null)
            {
                return;
            }
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);

        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Vui lòng chọn bàn");
                return;
            }

            int idBill = BillDAO.Instance.getUnCheckBillIDByTableID(table.Id);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InSertBill(table.Id);
                BillInfoDAO.Instance.InSertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InSertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.Id);
            LoadTable();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.getUnCheckBillIDByTableID(table.Id);
            int discount = (int)nmDiscount.Value;

            string priceText = txbTotalPrice.Text.Replace(".", "");
            priceText = priceText.Split(',')[0];
            double totalPrice = Convert.ToDouble(priceText);
            double finalTotalPrice = totalPrice - totalPrice * discount / 100;

            if (idBill != -1)
            {
                if (MessageBox.Show(String.Format("Bạn có chắc thanh toán hóa đơn cho Bàn {0}\nTổng tiền - (Tổng tiền / 100) x Giảm giá\n=> {1} - ({1}/100 x {2}) = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.Id);
                    LoadTable();
                }
            }
        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).Id;
            int id2 = (cbSwitchTable.SelectedItem as Table).Id;
            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                LoadTable();
            }

        }
        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this,new EventArgs());
        }

        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(this,new EventArgs()); 
        }
        #endregion
    }
}
