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
    public partial class MainForm : Form
    {
        About about = new About();//справка, которая не дублируется
        /*int K; //число каналов в многоканальном сигнале
        int N;// количество отсчетов в сигнале  - N
        double fd;// частота дискретизации в Герцах: fd = 1/T, где T – шаг между отсчетами в секундах
        DateTime date;// дата начала сигнала в формате ДД-ММ-ГГГГ
        DateTime time;// время начала сигнала в формате ЧЧ:MM:CC.CCC (секунды могут быть указаны с точностью до 3-го десятичного знака, на практике чаще всего задаются с точностью до единиц, т.е.применяется форма ЧЧ:MM:CC)
        String[] names;  //имена всех каналов, разделенные знаком “;” (в конце последнего имени может быть или не быть “;”, программа в любом должна правильно прочитать это имя, при записи TXT рекомендуем писать этот разделительный знак в конце строки)
        */

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.chm");
        }

        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            about.Hide();
            about.Show();//пока пойдет
        }

        private void File(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";

            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {//тут будет инциализация данных
                            /*string[] lines = File.ReadAllLines().Take(10).ToArray();
                            string filename = theDialog.FileName;
                            string[] filelines = File.ReadAllLines(filename);
                            MessageBox.Show("Прочтено");*/
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }
    }
}
