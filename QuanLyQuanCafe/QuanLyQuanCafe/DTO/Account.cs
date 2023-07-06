using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Account
    {
        private string userName;
        private string displayName;
        private string password;
        private int type;

        public Account(string userName, string displayName,int type, string password = null)
        {
            UserName = userName;
            DisplayName = displayName;
            Password = password;
            Type = type;
        }

        public Account (DataRow row)
        {
            UserName = row["userName"].ToString();
            DisplayName = row["displayName"].ToString();
            Password = row["password"].ToString();
            Type = (int)row["type"];
        }

        public string UserName { get => userName; set => userName = value; }
        public string Password { get => password; set => password = value; }
        public string DisplayName { get => displayName; set => displayName = value; }
        public int Type { get => type; set => type = value; }
    }
}
