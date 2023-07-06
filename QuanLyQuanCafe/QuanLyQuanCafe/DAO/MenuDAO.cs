using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        public static MenuDAO Instance {
            get { 
                if (instance == null)
                    instance = new MenuDAO();
                return MenuDAO.instance;
            }
            private set => instance = value; 
        }

        private MenuDAO() { }
        public List<Menu> getListMenuByTable(int id)
        {
            List<Menu> listMenu = new List<Menu>();
            string query = "Select f.name, bi.count, f.price, f.price* bi.count AS totalPrice " +
                "From BillInfo AS bi, Bill AS b, Food AS f " +
                "Where bi.idBill = b.id AND bi.idFood = f.id AND b.status = 0 AND b.idTable = " + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach(DataRow item in data.Rows)
            {
                Menu menu = new Menu(item);
                listMenu.Add(menu);
            }


            return listMenu;
        }
    }
}
