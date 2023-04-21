using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


// Программа выбора нужной даты
namespace DateSelection
{
    public partial class Form1 : Form
    {
        private string strDateFormat;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Планировщик задач";

            // Обработка события загрузки формы:     
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            strDateFormat = dateTimePicker1.Text;
            dateTimePicker1.CustomFormat = "ddd, dd MMMM, yyyy";
            button1.Text = "Создать задачу";
            button2.Text = "Свернуть планировщик";
            button3.Text = "Список всех задач";
            label1.Text = String.Format("Сегодня: {0} {1}", dateTimePicker1.Text, dateTimePicker2.Text);

            notifyIcon1.Icon = SystemIcons.Asterisk;
            notifyIcon1.Text = "Планировщик задач работает...";
            notifyIcon1.Visible = false;
            createSQLiteDBANDTable();
            taskTimer();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Обработка события изменения даты:          
            label1.Text = String.Format("Выбранная дата: {0} {1}", dateTimePicker1.Text, dateTimePicker2.Text);
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            strDateFormat = dateTimePicker1.Text;
            dateTimePicker1.CustomFormat = "ddd, dd MMMM, yyyy";
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            // Обработка события изменения времени:
            label1.Text = String.Format("Выбранная дата: {0} {1}", dateTimePicker1.Text, dateTimePicker2.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string addZero;

            if (dateTimePicker2.Text.Length < 8)
            {
                addZero = '\u0030' + dateTimePicker2.Text;
            }
            else
            {
                addZero = dateTimePicker2.Text;
            }

            StaticData.DataBufferDATETIME = dateTimePicker1.Text;
            StaticData.DataBufferDATE = strDateFormat;
            StaticData.DataBufferTIME = addZero;
            Form2 form2 = new Form2();
            form2.FormBorderStyle = FormBorderStyle.Fixed3D;
            form2.MaximizeBox = false;
            form2.MinimizeBox = false;
            form2.ShowDialog();
        }

        private void createSQLiteDBANDTable()
        {
            string dbFileName = "TaskSQLite.sqlite";

            if (!File.Exists(dbFileName))
            {
                SQLiteConnection.CreateFile(dbFileName);
            }

            string sqlQuery = "SELECT count(*) " +
                                "FROM sqlite_master " +
                               "WHERE type = 'table' AND name = 'taskManager';";

            string sqlQueryCreate = "CREATE TABLE IF NOT EXISTS taskManager (" +
                                  "id INTEGER  PRIMARY KEY AUTOINCREMENT, " +
                                "date DATETIME, " +
                               "notes TEXT);";

            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + dbFileName + ""))
            {
                con.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        cmd.Dispose();

                        if (rdr.StepCount != 0)
                        {
                            rdr.Read();
                            try
                            {
                                //Console.WriteLine(rdr.GetValue(0).ToString());
                                if (Convert.ToInt16(rdr.GetValue(0)) != 1)
                                {
                                    try
                                    {
                                        cmd.CommandText = sqlQueryCreate;
                                        cmd.ExecuteNonQuery();
                                        Console.WriteLine("Таблица успешно создана!");
                                        cmd.Dispose();
                                    }
                                    catch (SQLiteException ex)
                                    {
                                        Console.WriteLine("Ошибка создания таблицы: " + ex.Message);
                                    }
                                }
                            }
                            catch (SQLiteException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        rdr.Close();
                    }
                }
                con.Close();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false; this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide(); notifyIcon1.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Form4().ShowDialog();
        }

        private void showTask()
        {
            Form3 form3 = new Form3();
            form3.MaximizeBox = false;
            form3.MinimizeBox = false;
            form3.FormBorderStyle = FormBorderStyle.Fixed3D;
            form3.StartPosition = FormStartPosition.CenterScreen;
            form3.BringToFront();
            form3.ShowDialog();
        }
        private void taskTimer()
        {
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Start();
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //DateTime timeNow = DateTime.Now;
            //Console.WriteLine(timeNow);
            //dateTimePicker2.Value = timeNow;
            //Console.WriteLine(timeOut);
            string sqlQueryValidate = "SELECT date, notes FROM taskManager WHERE date = '" + timeOut + "'";

            using (SQLiteConnection con = new SQLiteConnection("Data Source=TaskSQLite.sqlite"))
            {
                con.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sqlQueryValidate, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        cmd.Dispose();

                        if (rdr.StepCount != 0)
                        {
                            rdr.Read();
                            try
                            {
                                DateTime dt = DateTime.Parse(rdr.GetValue(0).ToString());
                                StaticData.DataBufferDATETIME = String.Format("{0:ddd, dd MMMM, yyyy HH:mm:ss}", dt);
                                StaticData.DataBufferNOTES = rdr.GetValue(1).ToString();
                                showTask();
                            }
                            catch (InvalidOperationException ie)
                            {
                                Console.WriteLine(ie.Message);
                            }
                        }
                        rdr.Close();
                    }
                }
                con.Close();
            }
            //Console.WriteLine(StaticData.DataBufferDATETIME + ' ' + StaticData.DataBufferNOTES);
        }
    }

    public static class StaticData
    {
        // Буфер данных
        public static String DataBufferDATE = String.Empty;
        public static String DataBufferTIME = String.Empty;
        public static String DataBufferDATETIME = String.Empty;
        public static String DataBufferNOTES = String.Empty;
    }
}
