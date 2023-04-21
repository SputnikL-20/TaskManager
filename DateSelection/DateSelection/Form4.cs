using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DateSelection
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.Text = "Список всех задач";
            tasklist();
        }

        private void tasklist()
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=TaskSQLite.sqlite;");
            DataTable dTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT date, notes " +
                                    "FROM taskManager " +
                                "ORDER BY id DESC";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, con);
                adapter.Fill(dTable);

                if (dTable.Rows.Count > 0)
                {
                    dataGridView1.Rows.Clear();

                    for (int i = 0; i < dTable.Rows.Count; i++)
                    {
                        dataGridView1.Rows.Add(dTable.Rows[i].ItemArray);
                    }
                }
                else
                {
                    MessageBox.Show("Задачи отсутствуют");
                }
                adapter.Dispose();                 
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            con.Close();
        }
    }
}
