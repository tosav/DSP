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
//о программе подровнять
//Разобраться с NAVIGATION ПОЧЕМУ БЛЯТЬ ДВА РАЗА РИСУЕТСЯ
//удалить все лишние компоненты
//оптимизировать отрисовку на navigation в зависимости от количестква отсчетов
// убрать переменную date из MainForm (это я для себя). Аля
// окно осциллограмм меняет высоту при КАЖДОМ перемещении окна :с
namespace WindowsFormsApplication2 {

    public partial class MainForm : Form {
        About about;
        Navigation newForm;
        Dispatcher disp = Dispatcher.getInstance();
        Interval inter;
        DateTime date;// дата начала сигнала в формате ДД-ММ-ГГГГ
        TimeSpan time;// время начала сигнала в формате ЧЧ:MM:CC.CCC (секунды могут быть указаны с точностью до 3-го десятичного знака, на практике чаще всего задаются с точностью до единиц, т.е.применяется форма ЧЧ:MM:CC)
        PointF[,] data;  // массив для значений сигнала //мб удалить его после передачи? 
        ToolStripMenuItem[] items; //выпадающий список с названиями каналов
        ToolStripMenuItem[] calls; //выпадающий список с названиями каналов
        ToolStripMenuItem[] calls2; //выпадающий список с названиями каналов
        Modelling modellng;
        super sup;
        public MainForm() {
            InitializeComponent();
            disp.setMf(this); //добавляем главную форму в диспетчер
        }
        private void Form1_Load(object sender, EventArgs e) {
            this.IsMdiContainer = true; //флаг, указывающий, что данная форма является "контейнером" для дочерней формы
        }

        private string RussianDouble(string s) // заменяем точку на запятую для нормального считывания вещ.чисел
        {                                   
            return s.Replace(".", ",");
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e) {
            Help.ShowHelp(this, "help.chm");
        }

        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e) {
            about = new About();//справка, которая не дублируется
            about.Show();//пока пойдет
        }

        private void информацияОСигналеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disp.setSinf();
            disp.getSinf().Show();//пока пойдет
        }
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {
            disp.setDelNav(false);
            Stream myStream = null;
            OpenFileDialog theDialog = new OpenFileDialog(); // Открытие стандартного диалог. окна
            theDialog.Title = "Открыть файл";
            theDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK) {
                try {
                    if ((myStream = theDialog.OpenFile()) != null) {
                        disp.setPath(theDialog.FileName);
                        using (myStream)
                        {//тут будет инциализация данных
                            StreamReader sr = new StreamReader(theDialog.FileName, System.Text.Encoding.GetEncoding(1251));
                            string line;
                            int n = 0;
                            while (n!=6) {  //считывать первые значащие элементы 
                                line = sr.ReadLine().Trim();
                                if (line[0]!='#' && line.Length!=0) {
                                    switch (n) { //можно ещё создать какой-нибудь эррэйлист и в него добавлять эти необходимые данные (без переменных), но пока и так сойдёт
                                        case 0:
                                            disp.setK(Convert.ToInt32(line)); break;
                                        case 1:
                                            disp.setN(Convert.ToInt32(line)); break;
                                        case 2:
                                            disp.setFD(Convert.ToDouble(RussianDouble(line))); break;
                                        case 3:
                                            date = Convert.ToDateTime(line); break;
                                        case 4:
                                            time = TimeSpan.Parse(line);
                                            date += time;
                                            disp.setDateBegin(date) ; break; // теперь date содержит инфу и о дате, и о времени начала сигнала
                                        case 5:
                                            disp.setNames(line.Split(new Char[] { ';' }));
                                            break;
                                    }
                                    n += 1;
                                }
                            }
                            disp.setDateFin();
                            data = new PointF[disp.getK(),disp.getN()]; //создаём массив для данных по размеру кол-ва каналов
                            n = 0;
                            while(!sr.EndOfStream)//считывать массив значений
                            {
                                line = sr.ReadLine().Trim(); //считываем первые значения каналов (в строку)
                                String[] s= line.Split(new Char[] { ' ' }); // разбиваем строку на массив
                                int x = 0;
                                while (x!=s.Length)
                                {
                                    data[x,n] = new PointF(n, (Convert.ToSingle(RussianDouble(s[x]))));
                                    x += 1;
                                }
                                n += 1;
                            }
                            disp.setwasnavigation(true);
                            disp.setData(data);
                            sr.Close();
                            newForm = disp.getNav();
                            if (newForm != null)
                            {
                                newForm.Close();
                                SigInfo.Enabled = false;
                            }
                            newForm = new Navigation(this);
                            newForm.MdiParent = this;
                            newForm.Show();
                        }
                        disp.UstroyDestroy();
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        public void SigInf(bool lo)
        {
            SigInfo.Enabled = lo;
            save.Enabled = lo;
            арифметическаяСуперпозицияToolStripMenuItem.Enabled = lo;
            мультипликативнаяСуперпозицияToolStripMenuItem.Enabled = lo;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void CheckItem(int lev)
        {
            items[lev].CheckState = CheckState.Checked;
        }

        public void UnCheckItem(int lev)
        {
            items[lev].CheckState = CheckState.Unchecked;
        }

        public void UnCheckItem()
        {
            foreach (ToolStripMenuItem item in items)
                item.CheckState = CheckState.Unchecked;
        }
        public void UnCheckStatItem()
        {
            foreach (ToolStripMenuItem item in calls)
                item.CheckState = CheckState.Unchecked;
        }
        public void callItem()
        {
            this.осцилограммаToolStripMenuItem.DropDownItems.Clear();
            this.статистикаToolStripMenuItem.DropDownItems.Clear();
            this.дПToolStripMenuItem.DropDownItems.Clear();
            if (disp.getNav() != null) //если есть окно навигации //хотя,по-моему, оно всегда будет (Аля)
            {
                items = new ToolStripMenuItem[disp.getNames().Length]; //создаётся массив по кол-ву каналов
                calls = new ToolStripMenuItem[disp.getNames().Length]; //создаётся массив по кол-ву каналов
                calls2 = new ToolStripMenuItem[disp.getNames().Length]; //создаётся массив по кол-ву каналов
                for (int i = 0; i < disp.getNames().Length; i++)
                {
                        items[i] = new ToolStripMenuItem();
                        calls[i] = new ToolStripMenuItem();
                        calls2[i] = new ToolStripMenuItem();
                        items[i].Name = "dynamicItem" + disp.getNames()[i];
                        calls[i].Name = "dynamicItem" + disp.getNames()[i];
                        calls2[i].Name = "dynamicItem" + disp.getNames()[i];
                        items[i].Tag = i;
                        calls[i].Tag = i;
                        calls2[i].Tag = i;
                        items[i].Text = disp.getNames()[i];
                        calls[i].Text = disp.getNames()[i];
                        calls2[i].Text = disp.getNames()[i];
                        items[i].Click += new EventHandler(MenuItemClickHandler);
                        calls[i].Click += new EventHandler(StatMenuItemClickHandler);
                        calls2[i].Click += new EventHandler(ДПТItemClickHandler);
                }
                this.осцилограммаToolStripMenuItem.DropDownItems.AddRange(items);
                this.статистикаToolStripMenuItem.DropDownItems.AddRange(calls);
                this.дПToolStripMenuItem.DropDownItems.AddRange(calls2);
            }

        }
        //ОТКУДА ЭТО ЗДЕСЬ????
        //для передачи имён каналов в окно осциллограмм
        private void осцилограммаToolStripMenuItem_Click(object sender, EventArgs e)  { }

        private void ДПТToolStripMenuItem_Click(object sender, EventArgs e) { }
        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            //во первых надо узнать который из них
            if (clickedItem.CheckState.Equals(CheckState.Checked))
            {
                if (disp.getOsc().order.Count() > 1)
                    for (int j = 0; j < disp.getOsc().order.Count(); j++)
                    { //проверяем, какой пункт совпадает по названию с осциллограммой
                        if (disp.getOsc().order[j].Series[0].LegendText == clickedItem.Text)
                            disp.getOsc().remove(j);//передаём номер графика на удаление
                    }
                else
                    disp.getOsc().remove(0);
            }
            //тут галку впарить или передать добавить в осцилограммы
            else
                disp.CreateOsc((int)clickedItem.Tag);
        }
        private void StatMenuItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            //во первых надо узнать который из них
            Stat st =(Stat) disp.get("stat");
            if (clickedItem.CheckState.Equals(CheckState.Checked))
            {

                if (st != null)
                    if (st.order.Count() > 1)
                        for (int j = 0; j < st.order.Count(); j++)
                        { //проверяем, какой пункт совпадает по названию с осциллограммой
                            if (st.order[j].Series[0].LegendText == clickedItem.Text)
                                st.remove(j);//передаём номер графика на удаление
                        }
                    else
                        st.remove(0);
            }
            else
                disp.CreateStat((int)clickedItem.Tag);

            /*if (clickedItem.CheckState.Equals(CheckState.Checked))
            {
                if (  > 1)
                    for (int j = 0; j < disp.getOsc().order.Count(); j++)
                    { //проверяем, какой пункт совпадает по названию с осциллограммой
                        if (disp.getOsc().order[j].Series[0].LegendText == clickedItem.Text)
                            disp.getOsc().remove(j);//передаём номер графика на удаление
                    }
                else
                    disp.getOsc().remove(0);
            }
            //тут галку впарить или передать добавить в осцилограммы
            else
                disp.CreateOsc((int)clickedItem.Tag);*/
        }
        private void ДПТItemClickHandler(object sender, EventArgs e) {
        }
        public void osc(bool k)
        {
            this.задатьДиапазонToolStripMenuItem.Enabled = k;
        }

        public void sv(bool k)
        {
            this.save.Enabled = k;
        }

        private void задатьДиапазонToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inter = new Interval();
            inter.Hide();
            inter.Show();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disp.getNav() != null)
            {
                SaveOption sv = new SaveOption();
                sv.Show();
            }
        }

        private void modelling(object sender, EventArgs e)
        {
            delete_models();
            disp.setTextmod(sender.ToString());
            try
            {
                modellng = new Modelling(true);
                modellng.Show();
                disp.setModelling(modellng);
            }
            catch (Exception ex)
            {
                
            }
        }

        private void арифметическаяСуперпозицияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delete_models();
            disp.setTextmod(sender.ToString());
            if (disp.check("super"))
            {
                sup = (super)disp.get("super");
                sup.Close();
            }
            sup = new super();
            sup.Show();
            disp.set("super", sup);
        }

        private void мультипликативнаяСуперпозицияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delete_models();
            disp.setTextmod(sender.ToString());
            sup = new super();
            sup.Show();
            disp.set("super", sup);
        }
        private void delete_models()
        {

            if (disp.check("super"))
            {
                sup = (super)disp.get("super");
                sup.Close();
                disp.set("super", null);
            }
            if (disp.getModelling() != null)
            {
                disp.getModelling().Close();
                disp.setModelling(null);
            }
            if (disp.getModel() != null)
            {
                disp.getModel().Close();
                disp.setModel(null);
            }
        }
    }
}
