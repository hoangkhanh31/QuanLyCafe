using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance { 
            get {
                if (instance == null) instance = new AccountDAO();
                return instance;
                }
            private set => instance = value; 
        }

        private AccountDAO() { }

        public bool Login (string username, string password)
        {
            //mã hóa password MD5
            //byte[] temp = ASCIIEncoding.ASCII.GetBytes(password);
            //byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            //string hasPass = "";
            //foreach(byte item in hasData)
            //{
            //    hasPass += item;
            //}

            string query = "USP_Login @userName , @passWord";

            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] {username,password/*hasPass*/});
            
            return result.Rows.Count > 0;
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * From Account Where userName='"+userName+"'");
            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }

        public bool UpdateAccount(string userName, string displayName, string pass, string newPass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("EXEC USP_UpdateAccount @userName , @displayName , @password , @newPassword",new object[] { userName, displayName, pass, newPass });
            return result > 0;
        }
        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("Select userName,displayName,type From Account");
        }
        public bool InsertAccount(string username, string displayName, int type)
        {
            string query = string.Format("Insert Into Account(userName,displayName,type) VALUES (N'{0}',N'{1}',{2})", username,displayName,type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool UpdateAccount(string username, string displayName, int type)
        {
            string query = string.Format("Update Account Set displayName=N'{1}', type={2} Where userName=N'{0}'", username,displayName,type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool DeleteAccount(string userName)
        {
            string query = string.Format("Delete Account Where userName=N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool ResetPassword(string userName)
        {
            string query = string.Format("Update Account Set passWord=N'0' Where userName=N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
    }
}
