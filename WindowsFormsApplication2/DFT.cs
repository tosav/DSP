﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ComplexConsole;

namespace WindowsFormsApplication2
{
    public partial class DFT : Form
    {
        bool loc = true;//локальный масштаб
        bool sharp = true;//решетка
        bool dots = true;
        Dispatcher disp = Dispatcher.getInstance();
        public List<Chart> order = new List<Chart>();//порядок графиков
        Chart chart;//текущий график
        public List<int> kol = new List<int>();
        private int X1;
        private int Y1;
        private int X2;
        private int Y2;
        double[] Re;
        double[] Im;
        int prob = 30;
        private Interval inter;
        private int W = 700; // стандартная длина графика
        private int H = 200; // стандартная высота графика + отступ с названием канала
        Complex[] Furie;

        private void resize(object sender, EventArgs e)
        {
            if (order.Count > 0)
            {
                //Console.WriteLine();
                W = this.Width;
                H = (this.Height - 40 - prob) / order.Count;
                location();
            }
        }
        public DFT(MainForm ParrentForm)
        {
            InitializeComponent();
            Furie = new Complex[disp.getN()];
        }
        public void DDFT(int n)
        {
            Re = new double[disp.getN()];
            Im = new double[disp.getN()];
            for (int i = 0; i < disp.getN(); i++)
            {
                Re[i] = 0; Im[i] = 0 ;
                Furie[i] = new Complex();
                for (int j = 0; j < disp.getN(); j++)
                {
                    Re[i] += disp.getData()[n, j].Y * Math.Cos(2 * Math.PI * i * j / disp.getFD());
                    Im[i] += disp.getData()[n, j].Y * -(Math.Sin(2 * Math.PI * i * j / disp.getFD()));
                }
            }
        }
        //создание и добавление нового чарта на форму
        public void SetData(int n, double mini, double maxi)
        {

            if (!kol.Contains(n))
                {
                Series series = new Series("DFT");
                DDFT(n);
                
                for (int i = 0; i < disp.getN(); i++)
                {
                    if (i > 0)
                    {
                        series.Points.AddXY((double)i / (disp.getFD() / 180), Math.Sqrt(Re[i] * Re[i] + Im[i] * Im[i]));
                    }
                }

                kol.Add(n);
                disp.getMf().CheckItem(n);
                // Создаём новый элемент управления Chart
                chart = new Chart();
                // Помещаем его на форму
                chart.Parent = this;
                // Задаём размеры элемента
                chart.SetBounds(0, prob + H * order.Count, W, H);

                series.ChartType = SeriesChartType.Line;

                chart.Series.Clear();
                chart.Series.Add(series);


                ChartArea area = new ChartArea();
                Axis areaX = area.AxisX;
                areaX.MajorGrid.LineColor = Color.LightGray;
                areaX.IsLogarithmic = true;
                Axis areaY = area.AxisY;
                areaY.MajorGrid.LineColor = Color.LightGray;
                areaY.IsLogarithmic = true;
                chart.ChartAreas.Add(area);
            }
        }

        private void CreateChart(int n)
        {
            
            kol.Add(n);
            disp.getMf().CheckItem(n);
            // Создаём новый элемент управления Chart
            chart = new Chart();
            // Помещаем его на форму
            chart.Parent = this;
            // Задаём размеры элемента
            chart.SetBounds(0, prob + H * order.Count, W, H);

            // Создаём новую область для построения графика
            ChartArea area = new ChartArea();
            // Даём ей имя (чтобы потом добавлять графики)
            area.Name = "myGraph";
            area.AxisY.LabelStyle.Format = "N2";
            area.AxisX.LabelStyle.Format = "N0";
            area.AxisX.ScrollBar.Enabled = true;
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.ScaleView.Zoomable = true;
            area.AxisX.ScrollBar.IsPositionedInside = true;
            area.BorderDashStyle = ChartDashStyle.Solid;
            area.BorderColor = Color.Black;
            area.BorderWidth = 1;
            area.AxisX.MajorGrid.Enabled = sharp;
            area.AxisX.IsLogarithmic = true;
            area.AxisY.IsLogarithmic = true;
            area.AxisY.MajorGrid.Enabled = sharp;
            area.AxisY.MajorGrid.LineColor = Color.Black;
            area.AxisX.MajorGrid.LineColor = Color.Black;
            
            //area
            // Добавляем область в диаграмму
            chart.ChartAreas.Add(area);
            // Создаём объект для первого графика
            Series series1 = new Series();
            // Ссылаемся на область для построения графика
            series1.ChartArea = "myGraph";
            if (dots)
                series1.MarkerStyle = MarkerStyle.None;
            else
                series1.MarkerStyle = MarkerStyle.Circle;
            // Задаём тип графика - сплайны
            //series1.XValueType = ChartValueType.DateTime;
            series1.ChartType = SeriesChartType.Line;
            series1.LegendText = disp.getNames()[n];
            chart.Legends.Add(disp.getNames()[n]);
            // Добавляем в список графиков диаграммы
            chart.Series.Add(series1);
            area.AxisX.ScaleView.Zoom(disp.getStart(), disp.getFinish());
        }


        // для определения графика для удаления
        private void position1(object sender, MouseEventArgs e)
        {
            X1 = e.X;
            Y1 = e.Y;
            if (e.Button == MouseButtons.Right)
            {
                chart = (Chart)sender;
            }
        }

        private void position2(object sender, MouseEventArgs e)
        {
            X2 = e.X;
            Y2 = e.Y;
            //disp.setFinish(X2);
        }
        public void area(int x1, int x2) //изменяет интервал отображения
        {
            foreach (Chart ch in order)
                ch.ChartAreas["myGraph"].AxisX.ScaleView.Zoom(x1, x2);
            /*ch.ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(ch.Tag), disp.getData(), x1, x2);
            ch.ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(ch.Tag), disp.getData(), x1, x2);*/
            location();
        }

        private void location()//изменяет местоположение графиков при удалении и задает нужный размер окна
        {
            for (int i = 0; i < order.Count; i++)
            {
                order[i].Bounds = new Rectangle(0, prob + H * i, W, H);
                if (loc)
                {
                    order[i].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(order[i].Tag), disp.getData(), (int)disp.getStart(), (int)disp.getFinish());
                    order[i].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(order[i].Tag), disp.getData(), (int)disp.getStart(), (int)disp.getFinish());
                }
                else
                {
                    order[i].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(order[i].Tag), disp.getData(), 0, disp.getN());
                    order[i].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(order[i].Tag), disp.getData(), 0, disp.getN());
                }
            }
            this.Width = this.W;
            this.Height = this.H * order.Count + 40 + prob;
        }
        private void scroller(object sender, System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs e)
        {
            double round = disp.getFinish() - disp.getStart();
            disp.setStart(e.ChartArea.AxisX.ScaleView.Position);
            disp.setFinish(e.ChartArea.AxisX.ScaleView.Position + e.ChartArea.AxisX.ScaleView.Size);
        }
        private void viewchanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e)
        {
            disp.setStart(e.ChartArea.AxisX.ScaleView.Position);
            disp.setFinish(e.ChartArea.AxisX.ScaleView.Position + e.ChartArea.AxisX.ScaleView.Size);
        }
        //удаление осцилограмм по клику правой клавишей
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int level;
            for (level = 0; !chart.Equals(order[level]); level++) { }
            remove(level);
        }
        public void remove(int k)
        {
            if (order.Count == 1) //если и так одна осц, то удаляем форму вообще к хренам
                this.Close();
            else
            {
                order[k].Visible = false; //костыль-костылюшка //плохой костыль
                order[k].Dispose();
                order.RemoveAt(k);//должно удалять заданный chart
                location();
                disp.getMf().UnCheckItem(kol[k]);
                kol.RemoveAt(k);
            }
        }
        //при закрытии удаляет осцилограмму в диспетчере
        public void close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            disp.getMf().UnCheckItem();
            disp.setDPF(null);
        }

        private void локальныйМасштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!loc)
            {
                this.локальныйМасштабToolStripMenuItem.CheckState = CheckState.Checked;
                глобальныйМасштабToolStripMenuItem.CheckState = CheckState.Unchecked;
                loc = true;
                location();
            }
        }

        private void глобальныйМасштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loc)
            {
                this.локальныйМасштабToolStripMenuItem.CheckState = CheckState.Unchecked;
                глобальныйМасштабToolStripMenuItem.CheckState = CheckState.Checked;
                loc = false;
                location();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (order.Count > 0)
            {
                sharp = !sharp;
                this.toolStripButton1.Checked = sharp;
                for (int i = 0; i < order.Count; i++)
                {
                    order[i].ChartAreas["myGraph"].AxisX.MajorGrid.Enabled = sharp;
                    order[i].ChartAreas["myGraph"].AxisY.MajorGrid.Enabled = sharp;
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (order.Count > 0)
            {
                dots = !dots;
                this.toolStripButton2.Checked = dots;
                for (int i = 0; i < order.Count; i++)
                {
                    if (dots)
                        order[i].Series[0].MarkerStyle = MarkerStyle.None;
                    else
                        order[i].Series[0].MarkerStyle = MarkerStyle.Circle;
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            inter = new Interval();
            inter.Hide();
            inter.Show();
        }
    }
}
