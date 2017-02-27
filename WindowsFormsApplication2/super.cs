using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class super : Form
    {
        Dispatcher disp = Dispatcher.getInstance();
        Modelling modellng;
        public super()
        {
            InitializeComponent();
            checkedListBox1.Items.AddRange(disp.getNames());
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            start.Text = "0";
            finish.Text = Convert.ToString(disp.getN()-1);
        }
        
        private string deRussianDouble(string s)
        {
            return s.Replace(",", ".");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count ==0)
            {
                MessageBox.Show("Не было выбрано ни одного канала");
            } else
            {
                disp.set("super_c", checkedListBox1.CheckedItems.Count);
                disp.set("super_ch", checkedListBox1);
                try
                {
                    disp.setStart(Convert.ToDouble(start.Text));
                    disp.setFinish(Convert.ToDouble(finish.Text));
                    if (disp.check("super_m")) {
                        modellng = (Modelling) disp.get("super_m");
                        modellng.Close();
                    }
                    modellng = new Modelling(false);
                    modellng.Show();
                    disp.set("super_m", modellng);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            start.Text = check(start.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            finish.Text = check(finish.Text);
        }

        //Проверка на граничные значения
        private String check(String s)
        {
            if (s == "")
                s = "0";
            else if (Convert.ToDouble(s) > (disp.getN()-1))
                s = (disp.getN()-1).ToString();
            return s;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            start.Text = "0";
            finish.Text = (disp.getN()).ToString();
        }
    }
}
