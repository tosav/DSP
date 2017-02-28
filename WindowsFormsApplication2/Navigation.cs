using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

// написать преобразование координат выделения в границы отсчётов

namespace WindowsFormsApplication2
{
    public partial class Navigation : Form
    {
        Dispatcher disp = Dispatcher.getInstance();
        List<Chart> order = new List<Chart>();//порядок графиков
        Chart chart;
        bool draw = false; // для чего это?
        private int X1;
        private int Y1;
        private int X2;
        private int Y2;
        Graphics formGraphics;
        int W = 200; // стандартная длина графика
        int H = 100; // стандартная высота графика + отступ с названием канала
        MainForm Parrent;
        bool mouse = false;//флаг, отвечающий за наличие верт линий на графике

        public Navigation(MainForm ParrentForm)
        {
            InitializeComponent(); //тут нажимай F12 чтоб увидеть вызов графика
            blabla();
            Parrent = ParrentForm;
            disp.setNav(this); //добавление формы навигации (this) в диспетчер
            disp.getMf().callItem();
        }

        //отрисовка графиков
        public void blabla()
        {
            this.Text = disp.getPath().Split('\\')[disp.getPath().Split('\\').Length - 1]; // имя окна с каналами по названию файла
            for (int k = 0; k < disp.getK(); k++)
            {
                CreateChart(disp.mini(k, disp.getData(), 0, disp.getN()), disp.maxi(k, disp.getData(), 0, disp.getN()), k); //создаётся график
                order.Add(chart); // добавляется в список графиков
                for (int i = 0; i < disp.getN(); i++)//инициализация массива
                {
                    chart.Series[0].Points.AddY(disp.getData()[k, i].Y);
                }
                order[k].MouseDown += new System.Windows.Forms.MouseEventHandler(this.position1);
                order[k].MouseMove += new System.Windows.Forms.MouseEventHandler(this.position);
                order[k].MouseUp += new System.Windows.Forms.MouseEventHandler(this.position2);
            }
            draw = true;

            this.Height = disp.getK() * H + 40; // утсановить высоту формы "Каналы"
            this.Width = W + 16;
        }

        //создание графиков
        private void CreateChart(double min, double max, int n)
        {
            // Создаём новый элемент управления Chart
            chart = new Chart();
            // Помещаем его на форму
            chart.Parent = this;

            // Задаём размеры элемента
            chart.SetBounds(0, H * n, W, H);

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
            area.AxisX.Maximum = disp.getN();
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
            series1.ChartType = SeriesChartType.Line;
            series1.LegendText = disp.getNames()[n];
            chart.Legends.Add(disp.getNames()[n]);
            chart.Legends[disp.getNames()[n]].Docking = Docking.Bottom;
            // Добавляем в список графиков диаграммы
            chart.Series.Add(series1);

        }
        
        //добавление графиков в оскно осциллограм при нажатии в контекстном меню
        private void осцилограммаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int level;
            for (level = 0; !chart.Equals(order[level]); level++) { }//определеяем, на каком графике будт рисоваться линия (из какого графика событие)
            disp.CreateOsc(level);
        }

        //задает аннотации в начале и конце, для того чтобы вызывать это при изменении диапазона в любом месте
        public void VAsf(int j)
        {
            switch (j)
            {
                case 0://стартовая вертикальная линия
                    for (int i = 0; i < order.Count; i++) //цикл для всех графиков
                    {
                        if (disp.getOsc() != null)
                        {
                            if (disp.getOsc().kol.Contains(i) || chart == order[i]/*|| i == level*/)//если на графике уже есть вертикальная линия
                            {
                                order[i].Annotations.Clear();//то она обнуляется и рисуется заново в новом месте
                                order[i].Annotations.Add(VAlines(order[i], "line1", disp.getStart()));
                            }
                            else
                            {
                                order[i].Annotations.Clear();
                            }
                        }
                        else
                        {
                            order[i].Annotations.Clear();
                        }
                    }
                    break;
                case 1://финишная вертикальная линия
                    if (chart.Annotations.Count > 1) //эта штука проверяет наличие промежуточной линии при ее наличии удаляет
                        chart.Annotations.RemoveAt(chart.Annotations.Count - 1);
                    for (int i = 0; i < order.Count; i++)//тут добавляется финиш во все не пустые графики
                        if (order[i].Annotations.Count > 0)
                        {
                            try
                            {
                                order[i].Annotations.Add(VAlines(order[i], "line2", disp.getFinish()));
                            }
                            catch (Exception ex) { }
                        }
                    break;

            }
        }

        //добавление верт линии
        private VerticalLineAnnotation VAlines(Chart ch, string name, double x)
        {
            VerticalLineAnnotation VA = new VerticalLineAnnotation();
            VA.AxisX = ch.ChartAreas[0].AxisX;
            VA.AllowMoving = true;
            VA.IsInfinitive = true;
            VA.ClipToChartArea = ch.ChartAreas[0].Name;
            VA.Name = name;
            VA.LineColor = Color.Red;
            VA.LineWidth = 1;         // use your numbers!
            try
            {
                VA.X = x;//тут должна быть позиция в графике, а не в пикселях
            }
            catch (Exception ex) { }
            return VA;
        }

        //позиция для первой выделительной линии
        private void position1(object sender, MouseEventArgs e)
        {
            if (draw) //если можно рисовать лиииинии :3
            {
                X1 = e.X;
                Y1 = e.Y;
                chart = (Chart)sender;
                if (e.Button == MouseButtons.Left)
                {
                    mouse = true;//началась отрисовка вертикальных линий 
                    //добавление нач позиции в диспетчер
                    disp.setStart(chart.ChartAreas["myGraph"].AxisX.PixelPositionToValue(e.X));//перевод из пикселя в точку на графике
                }
            }
        }

        // ииии для второй
        private void position2(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                X2 = e.X;
                Y2 = e.Y;
                if (e.Button == MouseButtons.Left && mouse)
                {
                    mouse = false;//закончилась отрисовка верт. линий
                    try
                    {
                        disp.setFinish(chart.ChartAreas["myGraph"].AxisX.PixelPositionToValue(e.X));
                    }
                    catch (Exception ex) { }
                }
            }
        }

        //при отрисовке в этом окне в выделенном канале будет рисоваться зажатая верт. линия
        private void position(object sender, MouseEventArgs e)
        {
            if (draw && mouse)
            {
                int X = e.X;
                int Y = e.Y;
                if (e.Button == MouseButtons.Left)
                {
                    if (chart.Annotations.Count > 1)//если в выделенном канале есть такие же промежуточные верт. лин
                        chart.Annotations.RemoveAt(chart.Annotations.Count - 1);//удал последнюю линию
                    try
                    {
                        chart.Annotations.Add(VAlines(chart, "line", chart.ChartAreas["myGraph"].AxisX.PixelPositionToValue(e.X)));
                    }
                    catch (Exception ex) { }
                }
            }
        }
    private void Exercise_Paint(object sender, PaintEventArgs e)
        {
            formGraphics = e.Graphics;
        }
        //при закрытии удаляет в диспетчере
        public void close(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            disp.setNav(null);
            disp.setDelNav(true);
            disp.getMf().sv(false);
            disp.getMf().callItem();
            if (disp.getOsc()!=null)
                disp.getOsc().Close();
        }

        private void dPFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int level;
            for (level = 0; !chart.Equals(order[level]); level++) { }//определеяем, на каком графике будт рисоваться линия (из какого графика событие)
            disp.CreateDPF(level);
        }
    }
}
 