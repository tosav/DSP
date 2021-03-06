﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApplication2
{
    public partial class Spectral : Form
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
        int prob = 30;
        Interval inter;
        int W = 700; // стандартная длина графика
        int H = 200; // стандартная высота графика + отступ с названием канала
        double[] Re;
        double[] Im;
        double[] Pm;
        double dpf_start = 0, dpf_fin = 0.5;
        int L,M;
        private bool logX = false, logY = false;
        public bool kek = false; // паблик, чобы можно было обратиться из формы для полуширины (ваще тупо ((((((( )
        DialogResult di;
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
        public void setL(int l)
        {
            L = l;
        }
        public Spectral(MainForm ParrentForm)
        {
            Length_of_the_segment l = new Length_of_the_segment();
            l.Sp(this);
            l.ShowDialog();
            if (DialogResult.OK == l.DialogResult)
            {
                //kek = true;
                InitializeComponent();
            }
            di = l.DialogResult;
        }

        //создание и добавление нового чарта на форму
        public void SetData(int n)
        {
            if (kek)
            {
                if (!kol.Contains(n))
                {
                    M = Math.Round((double)disp.getN() / L) < disp.getN() / L ? (int) Math.Round((double)disp.getN() / L) + 1 : (int) Math.Round((double)disp.getN() / L);
                    //DDFT(n);
                    kat(n);
                    chart = CreateChart(n);
                    order.Add(chart); //добавление чарта в общий список
                                      //double k = 1 / 60 / disp.getFD();
                    double min = double.MaxValue, max = double.MinValue;
                    for (int i = 0; i <L; i++)//инициализация массива
                    {
                        if (i > 0)
                        {
                            //double z =(Re[i] * Re[i] + Im[i] * Im[i])/disp.getN();//простое спректральный анализ
                            double z = Pm[i];
                            min = min > z ? z : min;
                            max = max < z ? z : max;
                            chart.Series[0].Points.AddXY((double)i / L, z);//то что показывается на графике
                            chart.Tag = n.ToString();
                            chart.ChartAreas[0].AxisY.Minimum = min;
                            chart.ChartAreas[0].AxisY.Maximum = max;
                        }
                    }
                    chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                    chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
                    //изменение размеров окна
                    this.Width = this.W;
                    this.Height = prob + this.H * order.Count + 40;
                    this.Controls.Add(chart);
                    this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
                    this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);
                }
                this.Show();
            }
            else
            {
                this.Close();
                disp.setSpF(null);
                if (DialogResult.OK == di)
                    disp.CreateSp(disp.LEVEL);
            }
        }

        public void kat(int nk)//вычисления
        {
            double[] x = new double[L * M];
            int N = disp.getN();
            for (int i = 0; i < L*M; i++)
            {
                x[i] = N > i ? disp.getData()[nk, i].Y : 0;
            }
            Re = new double[M];
            Im = new double[M];
            Pm = new double[L];
            for (int k = 0; k < L; k++)
            {
                Pm[k] = 0;
                for (int m = 0; m < M; m++)
                {
                    Re[m] = 0; Im[m] = 0; 
                    for (int n = 0; n < L; n++)
                    {
                        Re[m] += x[m * L + n] * Math.Cos(2 * Math.PI * n * k / L);
                        Im[m] += x[m * L + n] * -(Math.Sin(2 * Math.PI * n * k / L));
                    }
                    Pm[k] += (Re[m] * Re[m] + Im[m] * Im[m]) / L;
                }
                Pm[k] /= M;
            } 
        }
       /* public void smooth(int L)
        {
            int i, j, z, k1, k2, hw;
            double tmp;
            if (L% 2 == 0) L++;
            hw = (L - 1) / 2;
            int n = disp.getN();
            for (i = 1; i < n; i++)
            {
                tmp = 0;
                if (i < hw)
                {
                    k1 = 0;
                    k2 = 2 * i;
                    z = k2 + 1;
                }
                else if ((i + hw) > (n - 1))
                {
                    k1 = i - n + i + 1;
                    k2 = n - 1;
                    z = k2 - k1 + 1;
                }
                else
                {
                    k1 = i - hw;
                    k2 = i + hw;
                    z = L;
                }

                for (j = k1; j <= k2; j++)
                {
                    tmp += Pm[j];
                }
                Pm[i] = tmp / z;
            }
        }*/
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
            Pm = new double[N];
            for (int k = 0; k < N; k++)
            {
                Re[k] = 0; Im[k] = 0; Pm[k] = 0;
                for (int n = 0; n < N; n++)
                {
                    Re[k] += x[n] * Math.Cos(2 * Math.PI * n * k / N);
                    Im[k] += x[n] * -(Math.Sin(2 * Math.PI * n * k / N));
                }
                Pm[k] = Math.Sqrt(Re[k] * Re[k] + Im[k] * Im[k]);
            }
        }
        private Chart CreateChart(int n)
        {
            Chart chart = new Chart();
            kol.Add(n);
            disp.getMf().CheckItemDPF(n);
            // Создаём новый элемент управления Chart
            // Помещаем его на форму
            chart.Parent = this;
            // Задаём размеры элемента
            chart.Size = new Size(W, H);
            chart.Location = new Point(0, this.H * order.Count);

            ChartArea area = new ChartArea();

            area.Name = "myGraph";

            area.AxisY.LabelStyle.Format = "N2";
            area.AxisX.LabelStyle.Format = "N2";

            area.AxisX.ScrollBar.Enabled = true;

            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;

            area.AxisX.ScaleView.Zoomable = true;
            area.AxisX.ScaleView.Zoom(dpf_start, dpf_fin);

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

            //area.AxisY.Minimum = -Math.PI;
            //area.AxisY.Maximum = Math.PI;
            chart.ChartAreas.Add(area);

            Series series = new Series();
            series.ChartType = SeriesChartType.Line;

            if (dots)
                series.MarkerStyle = MarkerStyle.Circle;
            else
                series.MarkerStyle = MarkerStyle.None;
            series.ChartType = SeriesChartType.Line;

            chart.Series.Clear();
            chart.Series.Add(series);
            chart.Series[0].ChartArea = "myGraph";

            series.LegendText = disp.getNames()[n];
            chart.Legends.Add(disp.getNames()[n]);

            chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
            chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
            chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
            chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);
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
                    //order[i].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(order[i].Tag), disp.getData(), (int)disp.getStart(), (int)disp.getFinish());
                    //order[i].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(order[i].Tag), disp.getData(), (int)disp.getStart(), (int)disp.getFinish());
                }
                else
                {
                    //order[i].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(Convert.ToInt32(order[i].Tag), disp.getData(), 0, disp.getN());
                    //order[i].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(Convert.ToInt32(order[i].Tag), disp.getData(), 0, disp.getN());
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
                disp.getMf().UnCheckItemSp(kol[k]);
                kol.RemoveAt(k);
            }
        }
        //при закрытии удаляет осцилограмму в диспетчере
        public void close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            disp.getMf().UnCheckItemSp();
            disp.setSpF(null);
        }

        private void локальныйМасштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!loc)
            {
                this.toolStripButton3.CheckState = CheckState.Checked;
                toolStripButton3.CheckState = CheckState.Unchecked;
                loc = true;
                location();
            }
        }

        private void глобальныйМасштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loc)
            {
                this.toolStripButton4.CheckState = CheckState.Unchecked;
                toolStripButton4.CheckState = CheckState.Checked;
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
