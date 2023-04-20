using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ВыборДаты
{
    public partial class Form2 : Form
    {
        private string dateTime;

        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "Выбранная дата: " + StaticData.DataBufferDATETIME + ' ' + StaticData.DataBufferTIME;
            label1.Text = StaticData.DataBufferDATETIME + ' ' + StaticData.DataBufferTIME;
            dateTime = StaticData.DataBufferDATE + ' ' + StaticData.DataBufferTIME;

            Console.WriteLine("Дата и время для базы: {0}", dateTime);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlQuery = "INSERT " +
                                "INTO taskManager ('date', 'notes') " +
                              "VALUES ('" + dateTime + "', '" + textBox1.Text + "')";

            using (SQLiteConnection con = new SQLiteConnection("Data Source=TestSQLite.sqlite"))
            {
                con.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sqlQuery, con))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Ошибка добавления данных: " + ex.Message);
                    }
                }
                con.Close();
            }
            this.Close();
        }
    }
}
