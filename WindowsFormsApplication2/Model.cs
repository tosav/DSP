using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

//СДЕЛАТЬ НОРМАЛЬНУЮ РАБОТУ С ОКНОМ
//добавление новых графиков в список (а не на новое окно), удаление по требованию, сохранение в списке навигации

namespace WindowsFormsApplication2
{//тут будет что то в духе осцилограм, но только для моделирования
  //при открытие его вме остальные окна будут скрываться
  //при его закрытии они будут раскарываться
    public partial class Model : Form
    {
        bool loc = true; //флаг для локального масштаба
        bool sharp = true; //решетка
        bool dots = true; //маркеры 

        public PointF[] data;
        Random ran = new Random();
        
        List<TextBox> texts; //принимает информацию из текстбоксов при моделировании

        Dispatcher disp = Dispatcher.getInstance();

        List<Chart> order = new List<Chart>();//порядок графиков
        Chart chart;//текущий график
        List<int> kol = new List<int>();
        private int X1; //координаты, по которым находится график, по которому щёлкнули
        private int Y1;
        private int X2;
        private int Y2;
        int prob = 30;
        int W = 700; // стандартная длина графика
        int H = 150; // стандартная высота графика + отступ с названием канала
        Interval inter;

        private string RussianDouble(string s) // заменяем точку на запятую для нормального считывания вещ.чисел
        {
            return s.Replace(".", ",").Replace(",", ",");
        }

        public Model(MainForm ParrentForm)
        {
            InitializeComponent();
        }

        private void resize(object sender, EventArgs e)
        {
            if (order.Count > 0)
            {
                W = this.Width;
                H = (this.Height - 40 - prob) / order.Count;
                location();
            }
        }

        //создание и добавление нового чарта на форму
        public void SetData(int n, double mini, double maxi)
        {
            PointF[] tata = new PointF[disp.getN()]; //массив для белого шума
            data = new PointF[disp.getN()];
            
            texts =(List<TextBox>)disp.get("model"); //массив значений, введённых с формы
            CreateChart(mini, maxi, n);

            switch ((int) disp.get("model_k")) {
            case 12:
                for (int i = 0; i < Convert.ToInt64(disp.getN()); i++)
                {
                    double nu = 0; for (int j = 1; j <= 12; j++) nu += ran.NextDouble(); nu = nu / 12 - 0.5;
                    tata[i] = new PointF(i, Convert.ToSingle(nu));
                }
                break;
            }
            order.Add(chart); //добавление чарта в общий список

            for (int i = 0; i < Convert.ToInt64(disp.getN()); i++)//вычисление данных для моделей и запихивание их в чарт
            {
                switch ((int) disp.get("model_k")) { 
                    case 1: data[i] = new PointF(i, i == Convert.ToDouble(RussianDouble(texts[0].Text)) ? 1 : 0); break;
                    case 2: data[i] = new PointF(i, i < Convert.ToDouble(RussianDouble(texts[0].Text)) ? 0 : 1); break;
                    case 3: data[i] = new PointF(i, Convert.ToSingle(Math.Pow(Convert.ToDouble(RussianDouble(texts[0].Text)), i))); break;
                    case 4: data[i] = new PointF(i, Convert.ToSingle(Convert.ToDouble(RussianDouble(texts[0].Text)) * Math.Sin(i * (Convert.ToDouble(RussianDouble(texts[1].Text)) * Math.PI / 180) + Convert.ToDouble(RussianDouble(texts[2].Text)) * Math.PI / 360))); break;
                    case 5: data[i] = new PointF(i, i % Convert.ToDouble(RussianDouble(texts[0].Text)) < Convert.ToDouble(RussianDouble(texts[0].Text)) / 2 ? 1 : -1); break;
                    case 6: data[i] = new PointF(i, Convert.ToSingle((i % Convert.ToDouble(RussianDouble(texts[0].Text))) / Convert.ToDouble(RussianDouble(texts[0].Text)))); break;//до сюда дошла
                    case 7: data[i] = new PointF(i, Convert.ToSingle(Convert.ToDouble(RussianDouble(texts[0].Text)) * Math.Exp(-i / Convert.ToDouble(RussianDouble(texts[1].Text))) * Math.Cos(2 * Math.PI * Convert.ToDouble(RussianDouble(texts[2].Text)) * i + Convert.ToDouble(RussianDouble(texts[3].Text))))); break;
                    case 8: data[i] = new PointF(i, Convert.ToSingle((Convert.ToDouble(RussianDouble(texts[0].Text)) * Math.Cos(2 * Math.PI * Convert.ToDouble(RussianDouble(texts[1].Text)) * i) * Math.Cos(2 * Math.PI * Convert.ToDouble(RussianDouble(texts[2].Text)) * i + Convert.ToDouble(RussianDouble(texts[3].Text)))))); break;
                    case 9: data[i] = new PointF(i, Convert.ToSingle(Convert.ToDouble(RussianDouble(texts[0].Text)) * (1 + Convert.ToDouble(RussianDouble(texts[1].Text)) * Math.Cos(2 * Math.PI * Convert.ToDouble(RussianDouble(texts[2].Text)) * i)) * Math.Cos(2 * Math.PI * Convert.ToDouble(RussianDouble(texts[3].Text)) * i + Convert.ToDouble(RussianDouble(texts[4].Text))))); break;
                    case 10: data[i] = new PointF(i, Convert.ToSingle(Convert.ToDouble(RussianDouble(texts[0].Text)) + (Convert.ToDouble(RussianDouble(texts[1].Text)) - Convert.ToDouble(RussianDouble(texts[0].Text))) * ran.NextDouble())); break;
                    case 11: double nu = 0; for (int j = 1; j <= 12; j++) nu += ran.NextDouble(); nu = nu - 6;
                            data[i] = new PointF(i, Convert.ToSingle(Convert.ToDouble(RussianDouble(texts[0].Text)) + Math.Pow(Convert.ToDouble(RussianDouble(texts[1].Text)), 0.5) * nu)); break;
                    case 12:
                        List<TextBox> text = (List<TextBox>)disp.get("modell_t");
                        float d = tata[i].Y;
                        int a = Convert.ToInt32(RussianDouble(text[0].Text));
                        for (int j=1; j<= Convert.ToDouble(RussianDouble(text[1].Text)); j++)
                            if (i - j >= 0)
                                d += Convert.ToSingle(RussianDouble(texts[a-1+j].Text)) * tata[i - j].Y;
                        for (int j = 1; j <= Convert.ToDouble(RussianDouble(text[0].Text)); j++)
                            if (i - j >= 0)
                                d -= Convert.ToSingle(RussianDouble(texts[j-1].Text)) * tata[i - j].Y;
                        data[i] = new PointF(i, d);
                    break;
                    case 13:
                        CheckedListBox clb=(CheckedListBox) disp.get("super_ch");
                        int k = 0;
                        float beg = Convert.ToSingle(RussianDouble(texts[0].Text));
                        for (int j = 0; j < disp.getK(); j++)
                        {
                            if (clb.GetItemChecked(j) == true)
                            {
                                k += 1;
                                beg += Convert.ToSingle(RussianDouble(texts[k].Text)) * disp.getData()[j, i].Y;
                            }
                        }
                        data[i] = new PointF(i,beg);
                    break;
                    case 14:
                        CheckedListBox sub = (CheckedListBox)disp.get("super_ch");
                        float na = Convert.ToSingle(RussianDouble(texts[0].Text));
                        for (int j = 0; j < disp.getK(); j++)
                        {
                            if (sub.GetItemChecked(j) == true)
                            {
                                na *= disp.getData()[j, i].Y;
                            }
                        }
                        data[i] = new PointF(i, na);
                    break;
                }
                chart.Series[0].Points.AddXY(i, data[i].Y);
            }
            chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
            chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
            //изменение размеров окна
            //location();
            this.Width = this.W;
            this.Height = prob+ this.H * order.Count + 40;
            this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
            this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);
            chart.Tag = disp.get("model_k");
        }

        private void CreateChart(double min, double max, int n)
        {
            kol.Add(n);
            chart = new Chart();
            chart.Parent = this;
            chart.SetBounds(0, prob+H * order.Count, W, H);

            ChartArea area = new ChartArea();
            area.Name = "myGraph";

            area.AxisY.Minimum = min;
            area.AxisY.Maximum = max;
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = Convert.ToInt64(disp.getN());

            area.AxisY.LabelStyle.Format = "N1";
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
            area.AxisY.MajorGrid.Enabled = sharp;

            chart.ChartAreas.Add(area);

            Series series1 = new Series();
            series1.ChartArea = "myGraph";
            series1.ChartType = SeriesChartType.Line;

            if (dots) //добавление маркеров в зависимости от флага
                series1.MarkerStyle = MarkerStyle.None;
            else
                series1.MarkerStyle = MarkerStyle.Circle;

            series1.LegendText = disp.getTextmod().Trim();
            chart.Legends.Add(disp.getTextmod().Trim());
            chart.Legends[0].Docking = Docking.Bottom;

            chart.Series.Add(series1);

            area.AxisX.ScaleView.Zoom(0, Convert.ToInt64(disp.getN()));
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

        public void area(int x1, int x2) //изменяет интервал отображения//тут кад
        {
            foreach (Chart ch in order)
                ch.ChartAreas["myGraph"].AxisX.ScaleView.Zoom(x1, x2);
            location();
        }

        private void location()//изменяет местоположение графиков при удалении и задает нужный размер окна
        {
            for (int i = 0; i < order.Count; i++)
            {
                order[i].Bounds = new Rectangle(0, prob+ H * i, W, H);
                if (loc)
                {
                    order[i].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(data, (int)disp.getStart(), (int)disp.getFinish());
                    order[i].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(data, (int)disp.getStart(), (int)disp.getFinish());
                }
                else
                {
                    order[i].ChartAreas["myGraph"].AxisY.Minimum = disp.mini(data, 0, disp.getN());
                    order[i].ChartAreas["myGraph"].AxisY.Maximum = disp.maxi(data, 0, disp.getN());
                }
            }
            this.Width = this.W;
            this.Height = prob+ this.H * order.Count + 40;
        }

        private void scroller(object sender, System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs e)
        {
            disp.setStart(e.ChartArea.AxisX.ScaleView.Position);
            disp.setFinish(e.ChartArea.AxisX.ScaleView.Position + e.ChartArea.AxisX.ScaleView.Size);
        }
        private void viewchanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e)
        {
            disp.setStart(e.ChartArea.AxisX.ScaleView.Position);
            disp.setFinish(e.ChartArea.AxisX.ScaleView.Position + e.ChartArea.AxisX.ScaleView.Size);
            //location();
        }

        //удаление модели по клику правой клавишей
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
                //disp.getMf().UnCheckItem(kol[k]);
                kol.RemoveAt(k);
            }
        }

        //при закрытии удаляет осцилограмму в диспетчере
        public void close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            //disp.getMf().UnCheckItem();
            disp.setModel(null);
        }
        private void добавитьВНавигациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(disp.check("model_" + disp.get("model_k")))) {
                disp.set("model_" + disp.get("model_k"), 1);
                disp.addData(data, "Model_" + disp.get("model_k") +"_"+ "1");
            } else {
                disp.addData(data, "Model_" + disp.get("model_k") + "_" + Convert.ToString((int)disp.get("model_" + disp.get("model_k")) + 1));
                disp.set("model_" + disp.get("model_k"), (int)disp.get("model_" + disp.get("model_k")) + 1);
                
            }
        }

        private void глобальныйМасштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loc)
            {
                this.локальныйМасштабToolStripMenuItem.CheckState = CheckState.Unchecked;
                глобальныйМасштабToolStripMenuItem.CheckState = CheckState.Checked;
                toolStripLabel1.Text = "Локальный масштаб";
                loc = false;
                location();
            }
        }

        private void локальныйМасштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!loc)
            {
                this.локальныйМасштабToolStripMenuItem.CheckState = CheckState.Checked;
                глобальныйМасштабToolStripMenuItem.CheckState = CheckState.Unchecked;
                toolStripLabel1.Text = "Глобальный масштаб";
                loc = true;
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
                this.toolStripButton2.Checked = !dots;
                for (int i = 0; i < order.Count; i++)
                {
                    if (dots)
                        order[i].Series[0].MarkerStyle = MarkerStyle.None;
                    else
                        order[i].Series[0].MarkerStyle = MarkerStyle.Circle;
                }
            }
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            loc = !loc;
            this.локальныйМасштабToolStripMenuItem.Checked = loc;
            глобальныйМасштабToolStripMenuItem.Checked = !loc;
            if (loc)
                toolStripLabel1.Text = "Глобальный масштаб";
            else
                toolStripLabel1.Text = "Локальный масштаб";
            location();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            inter = new Interval();
            inter.Hide();
            inter.Show();
        }

        private void Model_Load(object sender, EventArgs e)
        {

        }
    }
}
