using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Table
    {
        private int id; 
        private string name;
        private string status;

        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Status { get { return status; } set { status = value; } }
        public Table(int id, string name,string status)
        {
            this.id = id;
            this.name = name;
            this.status = status;
        }
        
        public Table(DataRow row)
        {
            this.id = (int)row["ID"];
            this.name = (string)row["Name"];
            this.status = row["status"].ToString();
        }
    }
}
