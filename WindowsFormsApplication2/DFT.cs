﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplication2
{
    public partial class DFT : Form
    {
        bool loc = true;//локальный масштаб
        bool sharp = true;//решетка
        bool dots = true;
        Dispatcher disp = Dispatcher.getInstance();
        public List<Chart[]> order = new List<Chart[]>();//порядок графиков
        Chart chart;//текущий график
        public List<int> kol = new List<int>();
        private int X1;
        private int Y1;
        private int X2;
        private int Y2;
        private bool logX = false, logY=false;
        double[] Re;
        double[] Im;
        int prob = 30;
        private Interval inter;
        private int W = 700; // стандартная длина графика
        private int H = 200; // стандартная высота графика + отступ с названием канала

        private void resize(object sender, EventArgs e)//нужно изменять размер таба
        {
           /* if (order.Count > 0)
            {
                //Console.WriteLine();
                W = this.Width;
                H = (this.Height - 40 - prob) / order.Count;
                location();
            }*/
        }
        public DFT(MainForm ParrentForm)
        {
            InitializeComponent();
        }
        public void DDFT(int nk)
        {
            double[] x = new double[disp.getN()];
            int N = disp.getN();
            for (int i = 0; i < N; i++)
            {
                x[i] = disp.getData()[nk, i].Y;
            }
            Re = new double[N];
            Im = new double[N];
            for (int k = 0; k < N; k++)
            {
                Re[k] = 0; Im[k] = 0 ;
                for (int n = 0; n < N; n++)
                {
                    Re[k] += x[n] * Math.Cos(2 * Math.PI * n * k / N);
                    Im[k] += x[n] * -(Math.Sin(2 * Math.PI * n * k / N));
                }
            }
        }
        //создание и добавление нового чарта на форму
        public void SetData(int n, double mini, double maxi)
        {

            if (!kol.Contains(n))
            {
                Chart[] ch = new Chart[4];
                DDFT(n);
                TabControl tabControl1 = new TabControl();
                tabControl1.Location = new Point(0,27);
                tabControl1.Name = "tabControl1";
                tabControl1.SelectedIndex = 0;
                tabControl1.Size = new Size(W, H+ prob);
                tabControl1.TabIndex = 1;
                tabControl1.BackColor = Color.Blue;

                TabPage tabPage1 = new TabPage();
                tabPage1.Location = new System.Drawing.Point(0, 0);
                tabPage1.Name = "tabPage1";
                tabPage1.TabIndex = 0;
                tabPage1.Text = "|X(k)|";
                tabPage1.BackColor = Color.Black;
                chart = CreateChart(n);
                ch[0]=chart; //добавление чарта в общий список
                for (int i = 0; i < disp.getN(); i++)
                {
                    if (i > 0)
                    {
                        chart.Series[0].Points.AddXY((double)i / disp.getN(), Math.Sqrt(Re[i] * Re[i] + Im[i] * Im[i]));
                    }
                }
                tabPage1.Controls.Add(chart);
                chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
                this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
                this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);

                TabPage tabPage2 = new TabPage();
                tabPage2.Location = new System.Drawing.Point(0, 0);
                tabPage2.Name = "tabPage2";
                tabPage2.TabIndex = 1;
                tabPage2.Text = "|ArgX(k)|";
                tabPage2.UseVisualStyleBackColor = true;
                chart = CreateChart(n);
                ch[1] = chart; ; //добавление чарта в общий список
                for (int i = 0; i < disp.getN(); i++)
                {
                    if (i > 0)
                    {
                        chart.Series[0].Points.AddXY((double)i / disp.getN(), Math.Atan2(Im[i], Re[i]));
                    }
                }
                tabPage2.Controls.Add(chart);
                chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
                this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
                this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);


                TabPage tabPage3 = new TabPage();
                tabPage3.Location = new System.Drawing.Point(0,0);
                tabPage3.Name = "tabPage3";
                tabPage3.TabIndex = 2;
                tabPage3.Text = "ReX(k)";
                tabPage3.UseVisualStyleBackColor = true;
                chart = CreateChart(n);
                ch[2] = chart; //добавление чарта в общий список
                for (int i = 0; i < disp.getN(); i++)
                {
                    if (i > 0)
                    {
                        chart.Series[0].Points.AddXY((double)i / disp.getN(), Re[i]);
                    }
                }
                tabPage3.Controls.Add(chart);
                chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
                this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
                this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);


                TabPage tabPage4 = new TabPage();
                tabPage4.Location = new System.Drawing.Point(0,0);
                tabPage4.Name = "tabPage4";
                tabPage4.TabIndex = 3;
                tabPage4.Text = "ImX(k)";
                tabPage4.UseVisualStyleBackColor = true;
                chart = CreateChart(n);
                ch[3] = chart; //добавление чарта в общий список
                for (int i = 0; i < disp.getN(); i++)
                {
                    if (i > 0)
                    {
                        chart.Series[0].Points.AddXY((double)i / disp.getN(), Im[i]);
                    }
                }
                tabPage4.Controls.Add(chart);
                order.Add(ch);
                chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
                this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
                this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);


                tabControl1.Controls.Add(tabPage1);
                tabControl1.Controls.Add(tabPage2);
                tabControl1.Controls.Add(tabPage3);
                tabControl1.Controls.Add(tabPage4);

                //изменение размеров окна

                this.BackColor = Color.Red;
                this.Width = this.W;
                tabPage1.Size = new System.Drawing.Size(W, H * order.Count + prob);
                tabPage2.Size = new System.Drawing.Size(W, H * order.Count + prob);
                tabPage3.Size = new System.Drawing.Size(W, H * order.Count + prob);
                tabPage4.Size = new System.Drawing.Size(W, H * order.Count + prob);
                this.Height = prob*3 + this.H * order.Count;
                this.Controls.Add(tabControl1);
            }
        }

        private Chart CreateChart(int n)
        {
            Chart chart = new Chart();
            kol.Add(n);
            disp.getMf().CheckItemDPF(n);
            // Создаём новый элемент управления Chart
            chart = new Chart();
            // Помещаем его на форму
            chart.Parent = this;
            // Задаём размеры элемента
            chart.Size= new Size(W, H);
            chart.Location = new Point(0, this.H * order.Count);

            ChartArea area = new ChartArea();

            area.Name = "myGraph";
            area.AxisY.LabelStyle.Format = "N2";
            area.AxisX.LabelStyle.Format = "N10";
            area.AxisX.ScrollBar.Enabled = true;
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.ScaleView.Zoomable = true;
            area.AxisX.ScrollBar.IsPositionedInside = true;
            area.BorderDashStyle = ChartDashStyle.Solid;
            area.BorderColor = Color.Black;
            area.BorderWidth = 1;
            area.AxisX.MajorGrid.Enabled = sharp;
            area.AxisY.MajorGrid.Enabled = sharp;
            area.AxisY.MajorGrid.LineColor = Color.Black;
            area.AxisX.MajorGrid.LineColor = Color.Black;
            area.AxisX.IsLogarithmic = logX;
            area.AxisY.IsLogarithmic = logY;
            area.AxisX.ScaleView.Zoom(disp.getStart()/disp.getN(), disp.getFinish() / disp.getN());
            chart.ChartAreas.Add(area);
            Series series = new Series();
            series.ChartType = SeriesChartType.Line;

            if (dots)
                series.MarkerStyle = MarkerStyle.None;
            else
                series.MarkerStyle = MarkerStyle.Circle;
            series.ChartType = SeriesChartType.Line;

            chart.Series.Clear();
            chart.Series.Add(series);
            chart.Series[0].ChartArea = "myGraph";

            series.LegendText = disp.getNames()[n];
            chart.Legends.Add(disp.getNames()[n]);
            return chart;
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
        }
        public void area(int x1, int x2) //изменяет интервал отображения
        {
            foreach (Chart[] ch in order)
                for (int i = 0; i <= 3; i++)
                    ch[i].ChartAreas["myGraph"].AxisX.ScaleView.Zoom(x1 / disp.getN(), x2 / disp.getN());
            location();
        }

        private void location()//тут нужно изменять размер таба
        {
            for (int i = 0; i < order.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    order[i][j].Bounds = new Rectangle(0, prob + H * i, W, H);
                    if (loc)
                    {
                        order[i][j].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(order[i][j].Tag), disp.getData(), (int)disp.getStart(), (int)disp.getFinish());
                        order[i][j].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(order[i][j].Tag), disp.getData(), (int)disp.getStart(), (int)disp.getFinish());
                    }
                    else
                    {
                        order[i][j].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(order[i][j].Tag), disp.getData(), 0, disp.getN());
                        order[i][j].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(order[i][j].Tag), disp.getData(), 0, disp.getN());
                    }
                }
            }
            this.Width = this.W;
            this.Height = this.H * order.Count + 40 + prob;
        }
        private void scroller(object sender, System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs e)
        {
            double round = disp.getFinish() - disp.getStart();
            disp.setStart(e.ChartArea.AxisX.ScaleView.Position * disp.getN());
            disp.setFinish((e.ChartArea.AxisX.ScaleView.Position + e.ChartArea.AxisX.ScaleView.Size) * disp.getN());
        }
        private void viewchanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e)
        {
            disp.setStart(e.ChartArea.AxisX.ScaleView.Position * disp.getN());
            disp.setFinish((e.ChartArea.AxisX.ScaleView.Position + e.ChartArea.AxisX.ScaleView.Size) * disp.getN());
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
                for (int j = 0; j < 4; j++)
                {
                    order[k][j].Visible = false; //костыль-костылюшка //плохой костыль
                    order[k][j].Dispose();
                }
                order.RemoveAt(k);//должно удалять заданный chart
                location();
                disp.getMf().UnCheckItemDPF(kol[k]);
                kol.RemoveAt(k);
            }
        }
        //при закрытии удаляет осцилограмму в диспетчере
        public void close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            disp.getMf().UnCheckItemDPF();
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
                    for (int j = 0; j < 4; j++)
                    {
                        order[i][j].ChartAreas["myGraph"].AxisX.MajorGrid.Enabled = sharp;
                        order[i][j].ChartAreas["myGraph"].AxisY.MajorGrid.Enabled = sharp;
                    }
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
                    for (int j = 0; j < 4; j++)
                    {
                        if (dots)
                            order[i][j].Series[0].MarkerStyle = MarkerStyle.None;
                        else
                            order[i][j].Series[0].MarkerStyle = MarkerStyle.Circle;
                    }
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            inter = new Interval();
            inter.Hide();
            inter.Show();
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            if (order.Count > 0)
            {
                logX = !logX;
                toolStripButton5.Checked = logX;
                for (int i = 0; i < order.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        order[i][j].ChartAreas["myGraph"].AxisX.IsLogarithmic = logX;
                    }
                }
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            if (order.Count > 0)
            {
                logY = !logY;
                toolStripButton5.Checked = logY;
                for (int i = 0; i < order.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        order[i][j].ChartAreas["myGraph"].AxisY.IsLogarithmic = logY;
                    }
                }
            }
        }
    }
}
