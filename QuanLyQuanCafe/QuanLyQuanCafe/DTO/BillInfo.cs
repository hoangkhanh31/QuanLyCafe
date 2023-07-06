using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class BillInfo
    {
        private int iD;
        private int iDBill;
        private int iDFood;
        private int count;

        public BillInfo(int id, int idBill, int idFood, int count)
        {
            this.iD = id;
            this.iDBill = idBill;
            this.iDFood = idFood;
            this.count = count;
        }

        public BillInfo(DataRow row)
        {
            this.iD = (int)row["id"];
            this.iDBill = (int)row["idBill"];
            this.iDFood = (int)row["idFood"];
            this.count = (int)row["count"];
        }

        public int ID { get => iD; set => iD = value; }
        public int IDBill { get => iDBill; set => iDBill = value; }
        public int IDFood { get => iDFood; set => iDFood = value; }
        public int Count { get => count; set => count = value; }
    }
}
