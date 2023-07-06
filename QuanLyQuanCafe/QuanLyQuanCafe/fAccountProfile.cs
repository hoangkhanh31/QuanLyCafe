﻿using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
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
    public partial class fAccountProfile : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get => loginAccount;
            set
            {
                loginAccount = value;
                ChangAccount(LoginAccount);
            }
        }
        public fAccountProfile(Account acc)
        {
            InitializeComponent();
            LoginAccount = acc;
        }
        void ChangAccount(Account acc)
        {
            txbUserName.Text = acc.UserName;
            txbDisplayName.Text = acc.DisplayName;

        }
        void UpdateAccountInfo()
        {
            string displayName = txbDisplayName.Text;
            string password = txbPassWord.Text;
            string newpass = txbNewPass.Text;
            string reEnterPass = txbReEnterPass.Text;
            string userName = txbUserName.Text;

            if (!newpass.Equals(reEnterPass))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu đúng với mật khẩu mới!");
            }
            else
            {
                if (AccountDAO.Instance.UpdateAccount(userName, displayName, password, newpass))
                {
                    MessageBox.Show("Cập nhật thành công");
                    if (updateAccount != null)
                    {
                        updateAccount(this,new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)));
                    }
                }
                else
                    MessageBox.Show("Vui lòng điền đúng mật khẩu");
            }
        }
        private event EventHandler<AccountEvent> updateAccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount += value; } 
            remove { updateAccount -= value; }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }
    }

    public class AccountEvent : EventArgs
    {
        private Account acc;
        public Account Acc
        {
            get { return acc; } set {  acc = value; }
        }
        public AccountEvent(Account acc)    
        {
            this.Acc = acc;
        }
    }
}
