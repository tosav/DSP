using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace WindowsFormsApplication2
{
    public partial class Stat : Form
    {
        public List<Chart> order = new List<Chart>();//порядок графиков
        int k;
        int[] g;
        Dispatcher disp = Dispatcher.getInstance();
        public List<int> kol = new List<int>();
        Chart chart;//текущий график
        private int X1;
        private int Y1;
        private int X2;
        private int Y2;
        List<Label>[] labels =new List < Label >[Dispatcher.getInstance().getK()];
        List<Label>[] texts=new List<Label>[Dispatcher.getInstance().getK()];
        int l = 22; //разрыв между лэйблами
        int W = 200; // стандартная длина графика
        //int Hi = 150; // стандартная высота графика + отступ с названием канала
        int H = 100; // стандартная высота графика + отступ с названием канала
        public Stat(MainForm ParrentForm)
        {
            InitializeComponent();
            this.BackColor = Color.White;
        }

        //не понимаю, что для чего
        public void SetData(int n, double mini, double maxi)
        {
            if (!kol.Contains(n)&&kol.Count<3)
            {
                Object z = disp.get("stat_k");
                if (z == null || !int.TryParse(z.ToString(), out k))
                {
                    /*Label lab = new Label();
                    lab.AutoSize = true;
                    lab.Name = "Колво интервалов";
                    lab.Text = "Колво интервалов";
                    lab.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
                    lab.Location = new System.Drawing.Point(14, 11);
                    lab.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    this.Controls.Add(lab);
                    Label tex = new Label();
                    tex.Name = "Колво интервалов";
                    tex.Text = "";
                    tex.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
                    tex.Location = new System.Drawing.Point(14+100, 11);
                    this.Controls.Add(tex);
                    */
                    k = 100;
                    disp.set("stat_k",k);// тут должно задаваться количество интервалов
                }
                g = new int[k];
                double di = disp.maxi(n, disp.getData(), 0, disp.getN()) - disp.mini(n, disp.getData(), 0, disp.getN());
                double p = di / (k-3);
                int min = (int)disp.mini(n, disp.getData(), 0, disp.getN());
                //double k = 1 / 60 / disp.getFD();
                double sr=0;
                for (int i = 0; i < k; i++) g[i] = 0;
                for (int i = 0; i < disp.getN(); i++)//инициализация массива
                {
                    sr+=disp.getData()[n, i].Y;
                    int l = (int)((disp.getData()[n, i].Y - min) / p);
                    g[l] += 1;
                }
                CreateChart(disp.mini(g, 0, k), disp.maxi(g, 0, k), n);
                order.Add(chart); //добавление чарта в общий список
                sr /=disp.getN();
                double σ2 = 0;
                double σ3 = 0;
                double σ4 = 0;
                for (int i = 0; i < disp.getN(); i++)//инициализация массива
                {
                    σ2 += Math.Pow(disp.getData()[n, i].Y- sr, 2);
                    σ3 += Math.Pow(disp.getData()[n, i].Y - sr, 3);
                    σ4 += Math.Pow(disp.getData()[n, i].Y - sr, 4);
                }
                σ2/= disp.getN();
                σ3/= disp.getN();
                σ4 /= disp.getN();
                double σ = Math.Pow(σ2, 0.5);
                double r = σ / sr;
                σ3 /= Math.Pow(σ, 3);
                σ4 /= Math.Pow(σ, 4);
                for (int i = 0; i < k; i++)//инициализация массива
                {
                    chart.Series[0].Points.AddXY(i, g[i]);
                }
                kol.Add(n);
                labels[kol.Count - 1] = new List<Label>();
                texts[kol.Count - 1] = new List<Label>();
                createtextlabel("Среднее =", String.Format("{0:0.00}", sr), kol.Count-1);
                createtextlabel("Дисперсия =", String.Format("{0:0.00}", σ2), kol.Count - 1);
                createtextlabel("Ср.кв.откл. =", String.Format("{0:0.00}", σ), kol.Count - 1);
                createtextlabel("Ассиметрия =", String.Format("{0:0.00}", σ3), kol.Count - 1);
                createtextlabel("Эксцесс =", String.Format("{0:0.00}", σ4-3), kol.Count - 1);
                createtextlabel("Максимум =", String.Format("{0:0.00}", disp.maxi(n, disp.getData(), 0, disp.getN())), kol.Count - 1);
                createtextlabel("Минимум =", String.Format("{0:0.00}", disp.mini(n, disp.getData(), 0, disp.getN())), kol.Count - 1);
                chart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                chart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
                //изменение размеров окна
                location();
                this.chart.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.scroller);
                this.chart.AxisViewChanged += new System.EventHandler<ViewEventArgs>(this.viewchanged);
            }
            else
            {
                MessageBox.Show("Слишком много графиков"); //слишком много дичи
            }
        }
        private void createtextlabel(string name, string text, int num)
        {
            labels[num].Add(new Label());
            labels[num][labels[num].Count - 1].AutoSize = true;
            labels[num][labels[num].Count - 1].Name = name;
            labels[num][labels[num].Count - 1].Text = name;
            labels[num][labels[num].Count - 1].Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            labels[num][labels[num].Count - 1].TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(labels[num][labels[num].Count - 1]);
            texts[num].Add(new Label());
            texts[num][texts[num].Count - 1].Name = name;
            texts[num][texts[num].Count - 1].Text = text;
            texts[num][texts[num].Count - 1].Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Controls.Add(texts[num][texts[num].Count - 1]);
        }

        //создание графиков
        private void CreateChart(double min, double max, int n)
        {
            // Создаём новый элемент управления Chart
            chart = new Chart();
            // Помещаем его на форму
            chart.Parent = this;
            // Задаём размеры элемента
            //chart.SetBounds(0, H*kol.Count(), W, H);

            // Создаём новую область для построения графика
            ChartArea area = new ChartArea();
            // Даём ей имя (чтобы потом добавлять графики)
            area.Name = "myGraph";
            area.BorderDashStyle = ChartDashStyle.Solid;
            area.BorderColor = Color.Black;
            area.BorderWidth = 1;
            area.AxisY.Minimum = min;
            area.AxisY.Maximum = max;
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = k;
            // убираем основные и вспомогаельные оси
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisX.Enabled = AxisEnabled.False;
            area.AxisY.Enabled = AxisEnabled.False;
            //area
            // Добавляем область в диаграмму
            chart.ChartAreas.Add(area);
            // Создаём объект для первого графика
            Series series1 = new Series();
            // Ссылаемся на область для построения графика
            series1.ChartArea = "myGraph";
            series1.ChartType = SeriesChartType.Column ;
            series1.LegendText = disp.getNames()[n];
            chart.Legends.Add(disp.getNames()[n]);
            chart.Legends[disp.getNames()[n]].Docking = Docking.Bottom;
            // Добавляем в список графиков диаграммы
            chart.Series.Add(series1);

        }
        // для определения графика для удаления
        private void position1(object sender, MouseEventArgs e)
        {
            X1 = e.X;
            Y1 = e.Y;
            if (e.Button == MouseButtons.Right)
            {
                int zq = e.Y/ ((labels[0].Count + 1) * l + H);
                chart = order[zq];
            }
        }
        private void position2(object sender, MouseEventArgs e)
        {
            X2 = e.X;
            Y2 = e.Y;
            //disp.setFinish(X2);
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
                disp.getMf().UnCheckItemSt(kol[k]);
                kol.RemoveAt(k);
            }
        }
        private void location()//изменяет местоположение графиков при удалении и задает нужный размер окна
        {
            for (int i = 0; i < order.Count; i++)
            {
                for (int j = 0; j < labels[i].Count; j++)
                {
                    labels[i][j].Location = new System.Drawing.Point(14, (labels[0].Count + 1) * l * i + H * i + 11 + j * l);
                    texts[i][j].Location = new System.Drawing.Point(14+100, (texts[0].Count + 1) * l * i + H * i + 11 + j * l);
                }
                order[i].Bounds = new Rectangle(0, (labels[i].Count+1)*l* (i+1)+H*i, W, H);
            }
            this.Width = W+16;
            this.Height = ((labels[0].Count+1) * l + H)*order.Count+40;
        }
        //при закрытии удаляет осцилограмму в диспетчере
        public void close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            disp.getMf().UnCheckItemSt();
            disp.set("stat", null);
        }
        private void resize(object sender, EventArgs e)
        {
            //Console.WriteLine();
            if (order.Count > 0)
            {
                W = this.Width - 16;
                if (this.Height > ((labels[0].Count + 1) * l) * order.Count + 50 * order.Count + 40)
                    H = (this.Height - 40) / order.Count - (labels[0].Count + 1) * l;
                else
                    H = 50;
                location();
            }
        }
    }
}
