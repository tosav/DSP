using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class SignalInfo : Form
    {
        Dispatcher disp = Dispatcher.getInstance();

        public SignalInfo()
        {
            InitializeComponent();
        }

        private void Print_information(object sender, PaintEventArgs e)
        {
            Kan.Text = Convert.ToString(disp.getK());
            Count.Text = Convert.ToString(disp.getN());
            frdis.Text = Convert.ToString(disp.getFD()) + " Гц";
            begin.Text = disp.getDateBegin().ToString("dd-MM-yyyy HH:mm:ss");
            disp.setDateFin();
            end.Text = disp.getDateFin().ToString("dd-MM-yyyy HH:mm:ss");
            Len.Text = Convert.ToString(disp.getDateFin().Subtract(disp.getDateBegin()).Days) + " суток, " + Convert.ToString(disp.getDateFin().Subtract(disp.getDateBegin()).Hours) + " часов, " + "\n" + Convert.ToString(disp.getDateFin().Subtract(disp.getDateBegin()).Minutes) + " минут, " + Convert.ToString(disp.getDateFin().Subtract(disp.getDateBegin()).Seconds) + " секунд";
            listView1.Items.Clear();
            for (int i = 0; i < disp.getK(); i++)
            {
                listView1.Items.Add(Convert.ToString(i + 1));
                listView1.Items[i].SubItems.Add(Convert.ToString(disp.getNames()[i]));
                listView1.Items[i].SubItems.Add("Файл: " + Convert.ToString(Path.GetFileName(disp.getPath())));
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.TopMost = true;
        }

    }
}
