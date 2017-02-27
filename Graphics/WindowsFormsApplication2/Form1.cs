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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MathGraph();
            label1.Text ="ЛОЛ СИНУСОЙДА";
            label2.Text = "ЛОЛ КОСИНУСОЙДА";

        }
        private void MathGraph()

        {
            const double Pi = 3.14159265;
            double shag = 0.1;
            double mashtab = 30;

            int fillHeight = 100;
            int fillWidth = 500;
            int otstup = 5;

            Graphics e = this.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.White);

            e.FillRectangle(brush, new Rectangle(otstup, otstup, fillWidth, fillHeight));
            e.FillRectangle(brush, new Rectangle(otstup, 2* otstup+fillHeight, fillWidth, fillHeight));

            int n = Convert.ToInt32(4 * Pi / shag);

            Point[] pointSin = new Point[n];
            Point[] pointCos = new Point[n];

            int p = 0;
            for (double i = 0; i < 4 * Pi; i += shag) 
            {
                pointSin[p] = new Point(Convert.ToInt32(i * mashtab + otstup), Convert.ToInt32(Math.Sin(i) * mashtab + (fillHeight / 2) + otstup));
                pointCos[p] = new Point(Convert.ToInt32(i * mashtab + otstup), Convert.ToInt32(Math.Cos(i) * mashtab + fillHeight / 2) + 2 * otstup + fillHeight);
                p++;
            }

            Pen pen = new Pen(Color.Black);

            e.DrawCurve(pen, pointSin);
            e.DrawCurve(pen, pointCos);

        }

      
    }
}
