using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//задание диапазона через Меню->Инструменты
namespace WindowsFormsApplication2
{
    public partial class Interval : Form
    {
        Dispatcher disp = Dispatcher.getInstance();
        public Interval()
        {
            InitializeComponent();
            Start.Text = disp.getStart().ToString();
            Finish.Text = disp.getFinish().ToString();
        }
        //делает максимальный диапазон (ВСЕ)
        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "\u00EE";
            Start.Text = "0";
            Finish.Text = (disp.getN()-1).ToString();
        }
        //Проверка ввода текста в finish
        private void Finish_TextChanged(object sender, EventArgs e)
        {
            Finish.Text = check(Finish.Text);
        }
        //Проверка ввода текста в start
        private void Start_TextChanged(object sender, EventArgs e)
        {
            Start.Text = check(Start.Text);
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
        //а тут данные отправляются в диспетчер
        private void button1_Click(object sender, EventArgs e)
        {
            disp.setStart(Convert.ToDouble(Start.Text));
            disp.setFinish(Convert.ToDouble(Finish.Text));
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Start_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }
    }
}
