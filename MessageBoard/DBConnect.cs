using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows;

namespace MessageBoard
{
    /// <summary>
    /// Connect SQL Server Database
    /// </summary>
    class DBConnect
    {
        public SqlConnection con { get; set; }
        private String conString { get; set; }

        public void connect()
        {
            conString = "Data Source=localhost;Initial Catalog=dbMessage;Integrated Security=True;";
            con = new SqlConnection(conString);
        }
        public void conOpen()
        {
            try
            {
                con.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("DB Connection error. " + e.Message);
            }
        }

        public void conClose()
        {
            con.Close();
        }
    }
}
