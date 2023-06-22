using QuanLyQuanCafe.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class fAdmin : Form
    {
        public fAdmin()
        {
            InitializeComponent();
        }

        void LoadFoodList()
        {
            string query = "Select * From Food";
            dtgvFood.DataSource = DataProvider.Instance.ExecuteQuery(query);
        }

        void LoadAccountList()
        {
            string query = "EXEC USP_GetAccountByUserName @userName";
            dtgvAccount.DataSource = DataProvider.Instance.ExecuteQuery(query,new object[] {"admin"});
        }
    }
}
