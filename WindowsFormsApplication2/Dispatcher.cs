using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//вроде всё считывает

namespace WindowsFormsApplication2
{
    public class Dispatcher
    {
        bool wasnavigation = false;//указывает наличие или отсутсвие навигации в прошлом
        private static Dispatcher transmitter;
        MainForm mf;
        SignalInfo sinf;
        int K = 0; //число каналов в многоканальном сигнале
        int N = 0;// количество отсчетов в сигнале  - N
        double FD;// частота дискретизации в Герцах: fd = 1/T, где T – шаг между отсчетами в секундах
        DateTime date_beg, date_fin;// дата начала сигнала в формате ДД-ММ-ГГГГ 
        String[] names;  //имена всех каналов, разделенные знаком “;” (в конце последнего имени может быть или не быть “;”, программа в любом должна правильно прочитать это имя, при записи TXT рекомендуем писать этот разделительный знак в конце строки)
        PointF[,] data;  // массив для значений сигнала 
        String path;
        oscillogram Osc;
        double start = 0, finish = 0;
        Navigation nav;
        Modelling mod;
        public Model mo;
        //int number_model;
        String textmod;
        Hashtable hash = new Hashtable(); //для чего хэш?
        bool nav_was_del = true;
        DFT dpf;
        public Dispatcher() { }

        public static Dispatcher getInstance()
        {
            if (transmitter != null)
                return transmitter;
            else
            {
                transmitter = new Dispatcher();
                return transmitter;
            }
        }

        public void setK(int k) { K = k; }

        public int getK() { return K; }

        public void setMf(MainForm m) { mf = m; }

        public MainForm getMf() { return mf; }

        public void setSinf()
        {
            if (nav != null)
                sinf = new SignalInfo();
            else
                sinf = null;
        }

        public SignalInfo getSinf() { return sinf; }

        public void setModelling(Modelling mo)
        {
            mod = mo;
            if (mod != null)
                mod.MdiParent = mf;
        }

        public Modelling getModelling() { return mod; }

        public void setModel(Model m)
        {
            mo = m;
            if (mo != null)
                mo.MdiParent = mf;
        }

        public Model getModel() { return mo; }

        public void setNav(Navigation m)
        {
            nav = m;
            if (m == null)
            {
                mf.SigInf(false);
            }
            else
            {
                mf.SigInf(true);
            }
        }

        public Navigation getNav() { return nav; }

        public void setN(int n) { N = n; }

        public int getN() { return N; }

        public void setOsc(oscillogram osc)
        {
            if (osc == null)
                mf.osc(false);
            else
                mf.osc(true);
            Osc = osc;
        }

        public oscillogram getOsc() { return Osc; }

        public void setFD(double fd) { FD = fd; }

        public double getFD() { return FD; }

        public void setDateBegin(DateTime date) { date_beg = date; }

        public DateTime getDateFin() { return date_fin; }

        public void setDateFin() { date_fin = date_beg.Add(TimeSpan.FromSeconds((1 / FD) * N)); }

        public DateTime getDateBegin() { return date_beg; }

        public void setNames(String[] nam)
        {
            names = nam;
        }

        public String[] getNames()
        {
            return names;
        }

        public void setData(PointF[,] d)
        {
            data = d;
        }

        public PointF[,] getData()
        {
            return data;
        }

        public void addData(PointF[] d, String name)//надо ему дать имя
        {
            if (data == null)//если ничего в навигации нет
            {
                if (d != null)
                {
                    PointF[,] xex = new PointF[1, N]; //создаём новый массив из 1 канала и пихаем значения для нового канала
                    for (int i = 0; i < N; i++)
                        xex[0, i] = d[i];
                    data = xex; //и присваиваем его
                }
            }
            else
            {
                PointF[,] xex = new PointF[K + 1, N]; //создаём нов массивс с кол-вом каналов на 1 больше
                for (int i = 0; i < K; i++)
                    for (int j = 0; j < N; j++)
                        xex[i, j] = data[i, j]; //перекидываем все старые значения
                for (int i = 0; i < N; i++)
                    xex[K, i] = d[i]; //и добавляем новый канал
                data = xex;
            }
            Array.Resize(ref names, K + 1);
            names[K] = name;
            K += 1;
            if (nav == null)
            {
                path = "Модели сигналов";
                nav = new Navigation(mf);
                nav.MdiParent = mf;
                nav.Show();
            }
            else
            {
                nav.blabla();
                setNav(nav); //добавление формы навигации (this) в диспетчер
                mf.callItem();
            }
        }

        public void setPath(String s)
        {
            path = s;
        }

        public String getPath()
        {
            return path;
        }
        public void setwasnavigation(bool was)
        {
            wasnavigation = was;
        }
        public bool getwasnavigation()
        {
            return wasnavigation;
        }
        public void setTextmod(String s)
        {
            textmod = s;
        }

        public String getTextmod()
        {
            return textmod;
        }
        //вычисление мин и макс значений для каждого канала
        public double mini(int n, PointF[,] data, int s, int f)
        {
            double mini = data[n, s].Y;
            for (int i = s; i < f; i++)
            {
                mini = mini > data[n, i].Y ? data[n, i].Y : mini;
            }
            return mini;
        }
        public double mini(PointF[] data, int s, int f)
        {
            double mini = data[s].Y;
            for (int i = s; i < f; i++)
            {
                mini = mini > data[i].Y ? data[i].Y : mini;
            }
            return mini;
        }
        public double mini(int[] data, int s, int f)
        {
            double mini = data[s];
            for (int i = s; i < f; i++)
            {
                mini = mini > data[i] ? data[i] : mini;
            }
            return mini;
        }
        public double maxi(int n, PointF[,] data, int s, int f)
        {
            double maxi = data[n, s].Y;
            for (int i = s; i < f; i++)
            {
                maxi = maxi < data[n, i].Y ? data[n, i].Y : maxi;
            }
            return maxi;
        }
        public double maxi(PointF[] data, int s, int f)
        {
            double maxi = data[s].Y;
            for (int i = s; i < f; i++)
            {
                maxi = maxi < data[i].Y ? data[i].Y : maxi;
            }
            return maxi;
        }
        public double maxi(int[] data, int s, int f)
        {
            double maxi = data[s];
            for (int i = s; i < f; i++)
            {
                maxi = maxi < data[i] ? data[i] : maxi;
            }
            return maxi;
        }
        public void setStart(double st)
        {
            start = st;
            if (nav != null)
                if (data != null)
                    nav.VAsf(0);
        }
        public bool getDelNav()
        {
            return nav_was_del;
        }

        public void setDelNav(bool model_st)
        {
            nav_was_del = model_st;
        }
        public double getStart()
        {
            return start;
        }
        //при задании нового диапазона будет отрисовываться в каждом окне в зависимости от наличия
        public void setFinish(double fn)
        {
            finish = fn;
            if (nav != null)
                if (data != null)
                    nav.VAsf(1);
            if (start > finish)
            {
                finish = start;
                start = fn;
            }
            if ((finish - start) < 1)
            {
                finish = start + 1;
            }
            if (Osc != null)
                Osc.area(Convert.ToInt32(start), Convert.ToInt32(finish));
            if (mo != null)
                mo.area(Convert.ToInt32(start), Convert.ToInt32(finish));
        }
        public double getFinish()
        {
            if (finish == 0)
                finish = N;
            return finish;
        }
        public void UstroyDestroy()
        {
            if (Osc != null) //закрытие окна осциллограмм при открытии нового файла (если осц.были открыты)
                Osc.Close();
            start = 0; //обнуление переменных начала и конца фрагмента
            finish = N;
            mf.osc(false);
        }
        public void CreateOsc(int level)
        {
            if (getOsc() == null) //если не создана осциллограмма
            {
                setOsc(new oscillogram(mf)); //то создаётся новая дочерняя форма с осциллограммами
                getOsc().MdiParent = mf;

                try
                {
                    getOsc().Owner = mf;
                }
                catch (ArgumentException argEx)
                {
                    //MessageBox.Show("Error: Could not do this. Original error: " + argEx.Message);
                }
            }
            getOsc().SetData(level, mini(level, data, 0, N), maxi(level, data, 0, N));
            getOsc().Show();
        }
        public Object get(String objectName)
        {
            return hash[objectName];
        }
        public void remove(String objectName)
        {
            hash.Remove(objectName);
        }
        public void set(String objectName, Object obj)
        {
            if (check(objectName))
                hash[objectName] = obj;
            else
                hash.Add(objectName, obj);
        }
        public bool check(String objectName)
        {
            return hash.ContainsKey(objectName);
        }

        public T DeepClone<T>(T obj) //уууууу что это?
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                // уууууууууу
                return (T)formatter.Deserialize(ms);
            }
        }

        public void CreateStat(int level)
        {
            Stat st = (Stat)get("stat");
            if (st == null) //если не создана осциллограмма //статистика мб??
            {
                st = new Stat(mf);
                st.MdiParent = mf;
                try { st.Owner = mf; }
                catch (ArgumentException argEx) { }
            }
            st.SetData(level, mini(level, data, 0, N), maxi(level, data, 0, N));
            st.Show();
            set("stat", st); //то создаётся новая дочерняя форма с осциллограммами
        }

        public void CreateDPF(int level)
        {
            if (getDPF() == null) //если не создана осциллограмма
            {
                setDPF(new DFT(mf)); //то создаётся новая дочерняя форма с осциллограммами
                getDPF().MdiParent = mf;

                try
                {
                    getDPF().Owner = mf;
                }
                catch (ArgumentException argEx)
                {
                    //MessageBox.Show("Error: Could not do this. Original error: " + argEx.Message);
                }
            }
            getDPF().SetData(level, mini(level, data, 0, N), maxi(level, data, 0, N));
            getDPF().Show();
        }
        public DFT getDPF()
        {
            return dpf;
        }
        public void setDPF(DFT d)
        {
            if (d == null)
                mf.dpf(false);
            else
                mf.dpf(true);
            dpf = d;
        }
    }
}
