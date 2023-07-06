using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class BillDAO
    {
        private static BillDAO instance;
        public static BillDAO Instance
        {
            get {
                if (instance == null)
                    instance = new BillDAO();
                return instance; }
            private set { instance = value; }
        }
        private BillDAO() { }
        /// <summary>
        /// Thành công: bill ID
        /// Thất bại : -1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int getUnCheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * From Bill Where idTable="+id+" AND status = 0");
            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            return -1;
        }

        public void InSertBill(int idTable)
        {
            DataProvider.Instance.ExecuteNonQuery("exec USP_InsertBill @idTable", new object[] { idTable });
        }

        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("Select MAX(id) From Bill");
            }
            catch
            {
                return 1;
            }
        }

        public void CheckOut(int id, int discount, float totalPrice)
        {
            string query = "Update Bill Set status = 1,dateCheckOut=GetDate(),discount="+discount+",totalPrice="+totalPrice+" Where id = "+id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("EXEC USP_GetListBillByDate @checkIn , @checkOut",new object[] {checkIn,checkOut});
        }
        public DataTable GetBillListByDateAndPage(DateTime checkIn, DateTime checkOut, int pageNum)
        {
            return DataProvider.Instance.ExecuteQuery("EXEC USP_GetListBillByDateAndPage @checkIn , @checkOut , @page", new object[] { checkIn, checkOut,pageNum });
        }
        public int GetNumListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            return (int)DataProvider.Instance.ExecuteScalar("EXEC USP_GetNumBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }
    }
}
