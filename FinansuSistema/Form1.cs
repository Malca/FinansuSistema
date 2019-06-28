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
        int numb = 0;
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
                MySqlConnection con = new MySqlConnection(conn_info);
                con.Open();
                PopulateFields(con);
                con.Close();
                tabControl1.SizeMode = TabSizeMode.Fixed;
                tabControl1.ItemSize = new Size((tabControl1.Width - 10) / tabControl1.TabCount, 0);
                tabControl1.SelectedTab = tabPage2;
                dateTimePicker2.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

        }

        private void Done()
        {
            tabControl1.Visible = false;
            button1.Visible = true;
            label3.Visible = true;
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

        private void button2_Click_1(object sender, EventArgs e)
        {

            string cat = comboBox1.SelectedItem == null ? String.Empty : comboBox1.SelectedItem.ToString();
            int catid = comboBox1.SelectedIndex;
            string item = textBox1.Text.ToString();
            float price = (float)numericUpDown1.Value;
            int amount = (int)numericUpDown2.Value;
            string date = dateTimePicker1.Value.ToString();

            if (cat == "" || item == null || price == 0)
            {
                textBox3.Visible = true;
                timer1.Start();
                textBox3.Text = "Užpildykite visus laukus!";
                return;

            }
            else
            {
                MySqlConnection con = new MySqlConnection(conn_info);
                con.Open();
                MySqlCommand command = con.CreateCommand();
                command.CommandText = "INSERT INTO entries(name, price, date, categoryid, amount) VALUES (@name, @price, @date, @categoryid, @amount)";
                command.Parameters.AddWithValue("@name", item);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@categoryid", catid + 1);
                command.Parameters.AddWithValue("@amount", amount);
                command.ExecuteNonQuery();
                con.Close();


                numb++;
                textBox2.Visible = true;
                textBox2.AppendText("#" + numb + " Kategorija: " + cat + " - " + "Pirkinys: " + item + " " + amount + " x " + price + " = " + string.Format("{0:0.00}", (float)amount * (float)price) + " €    " + date + "\n");
                addSourceToDB(item);
            }
        }

        private void addSourceToDB(string item)
        {
            AutoCompleteStringCollection complete = textBox1.AutoCompleteCustomSource;
            if (!complete.Contains(item))
            {
                MySqlConnection con = new MySqlConnection(conn_info);
                con.Open();
                MySqlCommand com = con.CreateCommand();
                com.CommandText = "INSERT INTO autocomplete(text) VALUES (@text)";
                com.Parameters.AddWithValue("text", item);
                com.ExecuteNonQuery();
                con.Close();
                textBox1.AutoCompleteCustomSource.Add(item);
            }
        }

        private void PopulateFields(MySqlConnection con)
        {


            MySqlCommand dbb = con.CreateCommand();
            dbb.CommandText = "SELECT * FROM categories";
            MySqlDataReader record = dbb.ExecuteReader();
            while (record.Read())
            // int count = product_cat.Length;
            //for (int i = 0; i <= count - 1; i++)
            {
                comboBox1.Items.Add(new ComboBoxItem(record["idCategories"].ToString(), record["categoryname"].ToString()));
                comboBox2.Items.Add(new ComboBoxItem(record["idCategories"].ToString(), record["categoryname"].ToString()));

            }
            record.Close();

            MySqlCommand auto = con.CreateCommand();
            auto.CommandText = "SELECT text FROM autocomplete";
            MySqlDataReader item = auto.ExecuteReader();
            while (item.Read())
            {
                textBox1.AutoCompleteCustomSource.Add(item["text"].ToString());
                textBox4.AutoCompleteCustomSource.Add(item["text"].ToString());
            }
            item.Close();
        }


        public static string FirstCharToUpper(string input)
        {
            if (input != "")
            {
                return input.First().ToString().ToUpper() + input.Substring(1);
            }
            else
            {
                return "";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox3.Visible = false;
            timer1.Stop();
        }


        private void textBox1_Leave(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            textBox1.Text = FirstCharToUpper(text);
        }

        private void ShowHideReportControls(object sender, EventArgs e)
        {

            if (radioButton1.Checked)
            {
                label12.Visible = false;
                comboBox2.Visible = false;

                label13.Visible = false;
                textBox4.Visible = false;

            }
            else if (radioButton2.Checked)
            {
                label13.Visible = false;
                textBox4.Visible = false;

                label12.Visible = true;
                comboBox2.Visible = true;


            }

            else if (radioButton3.Checked)
            {
                label13.Visible = true;
                textBox4.Visible = true;

                label12.Visible = false;
                comboBox2.Visible = false;

            }
        }

        private void AddDates(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string name = ((LinkLabel)sender).Name;

            if (name == linkLabel1.Name.ToString())
            {

                DayOfWeek day = DateTime.Now.DayOfWeek;

                if (day == DayOfWeek.Sunday)
                {

                    day = day + 1;

                    int days = day - DayOfWeek.Monday;
                    DateTime start = DateTime.Now.AddDays(-days);
                    DateTime end = start.AddDays(-6);
                    dateTimePicker2.Value = end;
                    dateTimePicker3.Value = start;
                }
                else
                {
                    int days = day - DayOfWeek.Monday;
                    DateTime start = DateTime.Now.AddDays(-days);
                    DateTime end = start.AddDays(6);
                    dateTimePicker2.Value = start;
                    dateTimePicker3.Value = end;
                }
            }

            else if (name == linkLabel2.Name.ToString())
            {

                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);
                dateTimePicker2.Value = start;
                dateTimePicker3.Value = end;

            }

            else if (name == linkLabel3.Name.ToString())
            {
                DateTime start = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime end = new DateTime(DateTime.Now.Year, 12, 31);
                dateTimePicker2.Value = start;
                dateTimePicker3.Value = end;
            }
        }

        private void GenerateReport(object sender, EventArgs e)
        {
            int count = 0;
            float suma = 0;
            DateTime nuo = dateTimePicker2.Value;
            DateTime iki = dateTimePicker3.Value;
            if (nuo > iki)
            {
                MessageBox.Show("Pradžios data didesnė nei pabaigos data.");
                return;
            }
            MySqlConnection con = new MySqlConnection(conn_info);
            con.Open();
            MySqlCommand record = con.CreateCommand();

            if (radioButton1.Checked)
            {
                record.CommandText = "SELECT * FROM entries WHERE date BETWEEN @nuo AND @iki";
                record.Parameters.AddWithValue("@nuo", nuo);
                record.Parameters.AddWithValue("@iki", iki);
            }
            if (radioButton2.Checked)
            {
                int catid = comboBox2.SelectedIndex;
                string cat = comboBox2.SelectedItem == null ? String.Empty : comboBox2.SelectedItem.ToString();
                if (cat == "")
                {
                    MessageBox.Show("Nepasirinkta kategorija.");
                    return;
                }

                record.CommandText = "SELECT * FROM entries WHERE categoryid = @catid AND date BETWEEN @nuo AND @iki";
                record.Parameters.AddWithValue("@catid", catid + 1);
                record.Parameters.AddWithValue("@nuo", nuo);
                record.Parameters.AddWithValue("@iki", iki);
            }
            if (radioButton3.Checked)
            {
                string name = textBox4.Text;
                if (name == "")
                {
                    MessageBox.Show("Neįvestas produkto/paslaugos pavadinimas.");
                    return;
                }

                record.CommandText = "SELECT * FROM entries WHERE name = @name AND date BETWEEN @nuo AND @iki";
                record.Parameters.AddWithValue("@name", name);
                record.Parameters.AddWithValue("@nuo", nuo);
                record.Parameters.AddWithValue("@iki", iki);
            }

            MySqlDataReader item = record.ExecuteReader();
            if (item.HasRows)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                while (item.Read())
                {
                    comboBox1.SelectedItem = item["categoryid"];
                    string cat = comboBox1.Items[(int)item["categoryid"] - 1].ToString();
                    int amount = (int)item["amount"];
                    float price = (float)item["price"];
                    float sum = amount * price;
                    string data = string.Empty;
                    data = DateTime.Parse(item["date"].ToString()).ToShortDateString();
                    suma += sum;
                    dataGridView1.Visible = true;
                    label16.Visible = false;
                    count++;
                    dataGridView1.Rows.Add(count, cat, item["name"], item["amount"], string.Format("{0:0.00}", (float)item["price"]) + " €", data, string.Format("{0:0.00}", (float)sum) + " €");

                }
                label17.Visible = true;
                label17.Text = "Bendra prekių/paslaugų suma: " + suma + " €";
                // label17.Text = "Laikotarpiu > " + dateTimePicker2.Value.ToShortDateString() +" - " + dateTimePicker3.Value.ToShortDateString() + " > Bendra prekių/paslaugų suma: " + suma + " €";
            }
            else
            {
                dataGridView1.Visible = false;
                label17.Visible = false;
                label16.Visible = true;
                label16.Text = "Pagal šiuos kriterijus, rezultatų nerasta.";

            }
            item.Close();
            con.Close();



        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                tabControl1.SizeMode = TabSizeMode.FillToRight;
                // tabControl1.ItemSize = new Size((tabControl1.Width - 10) / tabControl1.TabCount, 0);
            }
        }

        //private float FormatCurrency(float number)
        //{
        //    number = float.Parse(string.Format("{0:0.00}", (float)number));
        //    return number;

        //}
    }
}
