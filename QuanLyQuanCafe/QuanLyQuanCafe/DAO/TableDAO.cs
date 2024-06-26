﻿using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class TableDAO
    {

        public static int TableWidth = 100;
        public static int TableHeight = 100;
        private static TableDAO instance;
        
        public static TableDAO Instance { 
            get
            {
                if (instance == null)
                    instance = new TableDAO();
                return TableDAO.instance;
            } 
            private set => instance = value; 
        }

        private TableDAO() { }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");
            
            foreach (DataRow item in data.Rows) 
            {
                Table table = new Table(item);
                tableList.Add(table);
            }
            return tableList;
        }
        
        public void SwitchTable(int id1, int id2)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2",new object[] {id1,id2});
        }
    }
}
