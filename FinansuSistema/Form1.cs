using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace FinansuSistema
{
    public partial class Form1 : Form
    {
        static string conn_info = "Server=localhost;Database=testing;Uid=root;pwd=LabasAsKebabas";

        public Form1()
        {
         
            InitializeComponent();
            
if (checkDB_Conn() != true)
            {
                if (MessageBox.Show("Something went wrong. Can't connect to Database") == DialogResult.OK)
                {
                    Done();
                }
                
            }
            else
            {
           
                MySqlConnection con = new MySqlConnection(conn_info);
                con.Open();
                string[] product_cat = {
                "Pieno produktai",
                "Mėsos ir žuvies produktai",
                "Duonos gaminiai",
                "Vaisiai ir daržovės",
                "Gaivieji gėrimai",
                "Stiprieji gėrimai",
                "Saldumynai",
                "Bakalėjos prekės",
                "Buitinė chemija",
                "Higienos prekės",
                "Namų apyvokos prekės",
                "Prekės augintiniams"
            };

                MySqlCommand dbb = con.CreateCommand();
                dbb.CommandText = "SELECT * FROM categories";
                MySqlDataReader record = dbb.ExecuteReader();
                while (record.Read())
                // int count = product_cat.Length;
                //for (int i = 0; i <= count - 1; i++)
                {
                    comboBox1.Items.Add(new ComboBoxItem(record["idCategories"].ToString(), record["categoryname"].ToString()));
                }

                con.Close();
            }
        }

        private void Done()
        {
            //richTextBox1.Visible = false;
            //textBox1.Visible = false;
            // comboBox1.Visible = false;
            tabControl1.Visible = false;
            button1.Visible = true;
            label3.Visible = true;
            //button2.Visible = true;
        }

        public class ComboBoxItem
        {
            public string Value;
            public string Text;

            public ComboBoxItem(string val, string text)
            {
                Value = val;
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }
            public static bool checkDB_Conn()
        {
            
            bool isConn = false;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(conn_info);
                conn.Open();
                isConn = true;
            }
            catch (ArgumentException a_ex)
            {
                /*
                Console.WriteLine("Check the Connection String.");
                Console.WriteLine(a_ex.Message);
                Console.WriteLine(a_ex.ToString());
                */
            }
            catch (MySqlException ex)
            {
                /*string sqlErrorMessage = "Message: " + ex.Message + "\n" +
                "Source: " + ex.Source + "\n" +
                "Number: " + ex.Number;
                Console.WriteLine(sqlErrorMessage);
                */
                isConn = false;
                switch (ex.Number)
                {
                    //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            
            return isConn;
        }

        
    

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // MessageBox.Show(((ComboBoxItem)comboBox1.SelectedItem).Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
