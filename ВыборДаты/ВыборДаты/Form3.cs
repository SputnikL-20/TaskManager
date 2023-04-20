using System;
using System.Windows.Forms;

namespace ВыборДаты
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = "Событие таймера: " + StaticData.DataBufferDATETIME;
            label1.Text = StaticData.DataBufferNOTES;
        }
    }
}
