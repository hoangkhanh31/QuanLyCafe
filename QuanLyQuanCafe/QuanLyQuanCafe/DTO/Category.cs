using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Category
    {
        private int iD;
        private string name;
        public Category(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public Category(DataRow row)
        {
            ID = (int)row["id"];
            Name = (string)row["name"];
        }

        public Category()
        {
        }

        public int ID { get => iD; set => iD = value; }
        public string Name { get => name; set => name = value; }
    }
}
