using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Length_of_the_segment : Form
    {
        private Spectral sp;
        public Length_of_the_segment()
        {
            InitializeComponent();
            if (Dispatcher.getInstance().L != 0)
                textBox1.Text = Dispatcher.getInstance().L.ToString();
            else
                textBox1.Text = Math.Round(Math.Sqrt((double)Dispatcher.getInstance().getN())).ToString();
        }

        private bool check(string s) {
            long number;
            if (Int64.TryParse(s, out number) && Convert.ToInt64(s) > 4 && Convert.ToInt64(s) <= Dispatcher.getInstance().getN())
                return true;
            else
                return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!check(textBox1.Text))
            {
                MessageBox.Show("Значение должно быть целочисленным, больше 4 и меньше либо равно " + Dispatcher.getInstance().getN().ToString() + " (количеству отсчётов в сигнале)", "Ошибка!");
            }
            else
            {
                sp.kek = true;
                sp.setL(Convert.ToInt32(textBox1.Text));
                Dispatcher.getInstance().L = Convert.ToInt32(textBox1.Text);
            }
        }

        public void Sp(Spectral SP)
        {
            sp = SP;
        }
    }
}
