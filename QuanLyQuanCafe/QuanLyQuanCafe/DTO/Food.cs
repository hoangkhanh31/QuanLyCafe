using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Food
    {
        private int iD;
        private string name;
        private int categoryID;
        private float price;
        public Food(int id, string name, int categoryID, float price)
        {
            ID = id;
            Name = name;
            CategoryID = categoryID;
            Price = price;
        }
        public Food(DataRow row)
        {
            ID = (int)row["ID"];
            Name = (string)row["Name"];
            CategoryID = (int)row["idCategory"];
            Price = (float)Convert.ToDouble(row["price"].ToString());
        }

        public int ID { get => iD; set => iD = value; }
        public string Name { get => name; set => name = value; }
        public int CategoryID { get => categoryID; set => categoryID = value; }
        public float Price { get => price; set => price = value; }
    }
}
