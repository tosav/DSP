using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Modelling : Form
    {
        Dispatcher disp = Dispatcher.getInstance();
        static string str = "-,.";
        static char[] symb = str.ToCharArray(0, 3); //массив символов, которые можно вводить (я чёт больше не нашла другого способа, си шарп такой си шарп)

        private System.Windows.Forms.Label label = new System.Windows.Forms.Label(); //верхняя запись - с названием выбранного пункта моделирования
        List<Label> labels = new List<Label>(); // лист подписей
        List<TextBox> texts = new List<TextBox>(); //лист текст-боксов

        private System.Windows.Forms.Button but = new System.Windows.Forms.Button();

        int l = 25; //разрыв между лэйблами
        int t; //для подсчёта кол-ва лейблов и текстбоксов, чтобы потом расположить кнопку ОК// НО ЭТО МОЖНО УБРАТЬ
        Model mo;

        MyDelegate[] check = new MyDelegate[9]; //массив делегатов
        delegate void MyDelegate(object sender, EventArgs e); //"шаблон" процедуры-делегата

        Navigation newForm;
        String textmod;
        Modelling modellng;

        public Modelling(bool k) { 
            fisha();
        }

        private void fisha() {
            InitializeComponent();
            check[0] = check1;
            check[1] = check2;
            check[2] = check3;
            check[3] = check4;
            check[4] = check5;
            check[5] = check6;
            check[6] = check7;
            check[7] = check8;
            check[8] = check9;
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(14, 11);
            this.label.Name = "label";
            textmod = disp.getTextmod();
            this.label.Text = textmod;
            this.label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label.TabIndex = 10;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.label);
            if (disp.getN() == 0 || disp.getFD() == 0) //если нет навигации и не сохранены отсчёты и частота, то
            {
                createtextlabel("n ( целое число )", 7);
                createtextlabel("fd ( >0 )", 1);
            }
            findtype();
        }

        private static string RussianDouble(string s) // заменяем точку на запятую для нормального считывания вещ.чисел
        {
            return s.Replace(".", ",").Replace(",", ",");  
        }

        private void createmodel(object sender, EventArgs e/*, int type*/)
        {
            Boolean flag = true;
            int i = 0;
            while ((flag == true) & (i < texts.Count)) //Проверка на пустоту текстбокса
            {
                if (texts[i].Text == String.Empty)
                    flag = false;
                i++;
            }
            if (flag == true) //отрисовка модели, если все текстбоксы заполнены
            {
                if (texts[0].Name == "n ( целое число )")
                {
                    disp.setN(Convert.ToInt32(RussianDouble(texts[0].Text)));
                    texts.RemoveAt(0);
                }
                if (texts[0].Name == "fd ( >0 )")
                {
                    disp.setFD(Convert.ToDouble(RussianDouble(texts[0].Text)));
                    texts.RemoveAt(0);
                }
                if (!disp.check("modell_t") && textmod.Trim() == "Случайный сигнал АРСС (p,q)")
                {
                    if (disp.check("modell"))
                    {
                        modellng = (Modelling)disp.get("modell");
                        modellng.Close();
                    }
                    disp.set("modell_t", texts);
                    modellng = new Modelling(false);
                    modellng.MdiParent = disp.getMf();
                    modellng.Show();
                    disp.set("modell", modellng);
                }
                else
                {
                    if (disp.getModel() != null)
                        disp.getModel().Close();
                    mo = new Model(disp.getMf());
                    disp.set("model", texts);
                    disp.setModel(mo);
                    disp.getModel().SetData(0, 0, Convert.ToDouble(disp.getN()));
                }
            }
            else { MessageBox.Show("Пожалуйста, введите значения во все поля"); }
        }

        private void createtextlabel(string name, int ch) // название + номер обработчика ввода в поле
        {
            labels.Add(new Label());
            labels[labels.Count - 1].AutoSize = true;
            labels[labels.Count - 1].Location = new System.Drawing.Point(14, 11 + labels.Count * l);
            labels[labels.Count - 1].Name = name;
            labels[labels.Count - 1].Text = name;
            labels[labels.Count - 1].Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            labels[labels.Count - 1].TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(labels[labels.Count - 1]);
            texts.Add(new TextBox());
            texts[texts.Count - 1].Location = new System.Drawing.Point(14 + 125, 11 + texts.Count * l);
            texts[texts.Count - 1].Name = name;
            texts[texts.Count - 1].Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Controls.Add(texts[texts.Count - 1]);
            texts[texts.Count - 1].KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Start_KeyPress);
            if (ch != -1)
                texts[texts.Count - 1].LostFocus += new EventHandler(check[ch]);
            t = texts.Count + 1;
        }
        
        private void findtype() //выдать нужные поля в зависимости от выбранного моделирования
        {
            //int t;
            //тут будет куча кейсов которая определит тип и будет выводить то, что нужно
            switch (textmod.Trim())
            {
                case "Задержанный единичный импульс":
                    disp.set("model_k", 1);
                    createtextlabel("n0 ( значение от 0 до 1 )", 1);
                    break;
                case "Задержанный единичный скачок":
                    disp.set("model_k", 2);
                    createtextlabel("n0 ( значение от 0 до 1 )", 1);
                    break;
                case "Дискретизированная убывающая экспонента":
                    disp.set("model_k", 3);
                    createtextlabel("a ( значение от 0 до 1 )", 0);
                    break;
                case "Дискретизированная синусоида с заданными амплитудой a, круговой частотой \u03C9 и начальной фазой \u03C6.":
                    disp.set("model_k", 4);
                    createtextlabel("a ( значение от 0 до 1 )", 4);
                    createtextlabel("\u03C9 ( в градусах )", 2);
                    createtextlabel("\u03C6 ( в градусах )", 3);
                    break;
                case "\"Меандр\" с периодом L":
                    disp.set("model_k", 5);
                    createtextlabel("L ( >0 )", 5);
                    break;
                case "\"Пила\" с периодом L":
                    disp.set("model_k", 6);
                    createtextlabel("L ( >0 )", 5);
                    break;
                case "Cигнал с экспоненциальной огибающей - амплитудная модуляция":
                    disp.set("model_k", 7);
                    createtextlabel("a", 4);
                    createtextlabel("\u03C4", 8);
                    createtextlabel("\u0192", 8); //нужно поменять
                    createtextlabel("\u03C6 ( в градусах )", 3);
                    break;
                case "Cигнал с балансной огибающей - амплитудная модуляция":
                    disp.set("model_k", 8);
                    createtextlabel("a", 4);
                    createtextlabel("\u0192", 4);
                    createtextlabel("\u0192", 4);
                    createtextlabel("\u03C6 ( в градусах )", 3);
                    break;
                case "Cигнал с тональной огибающей. - амплитудная модуляция":
                    disp.set("model_k", 9);
                    createtextlabel("a", 4);
                    createtextlabel("m", 0);
                    createtextlabel("\u0192", 4);
                    createtextlabel("\u0192", 4);
                    createtextlabel("\u03C6 ( в градусах )", 3);
                    break;
                case "Белый шум равномерный в [a,b]":
                    disp.set("model_k", 10);
                    createtextlabel("a", 8);
                    createtextlabel("b", 8);
                    break;
                case "Белый шум распределенный по нормальному закону с заданным средним a, и дисперсией σ ²":
                    disp.set("model_k", 11);
                    createtextlabel("a", 8);
                    createtextlabel("σ ²", 4);
                    break;
                case "Случайный сигнал АРСС (p,q)":
                    if (disp.check("modell_t"))
                    {
                        List<TextBox> texts = (List<TextBox>)disp.get("modell_t");
                        for (int i = 1; i <= Convert.ToInt32(RussianDouble(texts[0].Text)); i++)
                        {
                            string a = "a" + i.ToString();
                            createtextlabel(a, 8);
                        }
                        for (int i = 1; i <= Convert.ToInt32(RussianDouble(texts[1].Text)); i++)
                        {
                            string b = "b" + i.ToString();
                            createtextlabel(b, 8);
                        }
                    }
                    else
                    {
                        disp.set("model_k", 12);
                        createtextlabel("p", 6);
                        createtextlabel("q", 6);
                    }
                    break;
                case "Арифметическая суперпозиция":
                    disp.set("model_k", 13);
                    for (int i = 0; i <= (int)disp.get("super_c"); i++)
                    {
                        string a = "a" + i.ToString();
                        createtextlabel(a, 8);
                    }
                    break;
                case "Мультипликативная суперпозиция":
                    disp.set("model_k", 14);
                    createtextlabel("a", 8);
                    break;
            }
            but.Text = "Ok";
            but.Location = new Point(14, 11 + l * t);
            but.Click += new System.EventHandler(createmodel);
            this.Controls.Add(this.but);
            this.Height = 11 + l * (t + 3);
            Width = 270;
        }

        private void Start_KeyPress(object sender, KeyPressEventArgs e)
        {   //если цифра, есть в массиве допустимых символов или клавиша бэкспэйс
            if (!Char.IsDigit(e.KeyChar) && !(Array.IndexOf(symb, e.KeyChar) != -1) && e.KeyChar != Convert.ToChar(8))
            {
                e.Handled = true;
            }
        }

        /*private void TextChange(object sender, EventArgs e) // хз, зачем это
        {
            Console.WriteLine(sender.ToString());
        }*/

        // функции для обработки введённых значений в текстбоксы
        private static void check1(object sender, EventArgs e) //0 //значение а от 0 до 1
        {
            Double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if ((Convert.ToDouble(RussianDouble(((Control)sender).Text)) >= 1) || (Convert.ToDouble(RussianDouble(((Control)sender).Text)) <= 0))
                    {
                        MessageBox.Show("Значение должно быть в интервале от 0 до 1!");
                        ((Control)sender).Text = "0,1";
                    }
                }
            }
        }

        private static void check2(object sender, EventArgs e) //1 //дробное больше нуля
        {
            Double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if (Convert.ToDouble(RussianDouble(((Control)sender).Text)) <= 0)
                    {
                        MessageBox.Show("Значениие должно быть больше нуля !");
                        ((Control)sender).Text = "1";
                    }
                }
            }
        }

        private static void check3(object sender, EventArgs e) //2 //значение круговой частоты от 0 до 180
        {
            Double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if ((Convert.ToDouble(RussianDouble(((Control)sender).Text)) > 180) || (Convert.ToDouble(RussianDouble(((Control)sender).Text)) < 0))
                    {
                        MessageBox.Show("Значениие должно быть в конечном диапазоне от 0 до 180 (град.)");
                        ((Control)sender).Text = "0";
                    }
                }
            }
        }

        private static void check4(object sender, EventArgs e) //3 //значение начальной фазы от 0 до 360
        {
            Double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if ((Convert.ToDouble(RussianDouble(((Control)sender).Text)) > 360) || (Convert.ToDouble(RussianDouble(((Control)sender).Text)) < 0))
                    {
                        MessageBox.Show("Значениие должно быть в конечном диапазоне от 0 до 360 (град.)");
                        ((Control)sender).Text = "0";
                    }
                }
            }
        }

        private static void check5(object sender, EventArgs e) //4 //дробное, больше либо равно нулю
        {
            Double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if (Convert.ToDouble(RussianDouble(((Control)sender).Text)) < 0)
                    {
                        MessageBox.Show("Значениие амплитуды должно быть неотрицательным");
                        ((Control)sender).Text = "1";
                    }
                }
            }
        }

        private static void check6(object sender, EventArgs e) //5 //нет значения в нуле в нуле // для деления по модулю
        {
            Double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if (Convert.ToDouble(RussianDouble(((Control)sender).Text)) == 0)
                    {
                        MessageBox.Show("Значениие не может быть равно нулю");
                        ((Control)sender).Text = "1";
                    }
                }
            }
        }

        private static void check7(object sender, EventArgs e) //6 //целое, больше либо равно нулю
        {
            int number;
            if (((Control)sender).Text != "")
            {
                if (!Int32.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if (Convert.ToInt32(RussianDouble(((Control)sender).Text)) < 0)
                    {
                        MessageBox.Show("Значениие должно быть неотрицательным");
                        ((Control)sender).Text = "1";
                    }
                }
            }
        }

        private static void check8(object sender, EventArgs e) //7 //целое, больше нуля       
        {
            int number;
            if (((Control)sender).Text != "")
            {
                if (!Int32.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
                else
                {
                    if (Convert.ToInt32(RussianDouble(((Control)sender).Text)) <= 0)
                    {
                        MessageBox.Show("Значениие должно быть больше нуля");
                        ((Control)sender).Text = "1";
                    }
                }
            }
        }

        private static void check9(object sender, EventArgs e) //8 //есть цифры       
        {
            double number;
            if (((Control)sender).Text != "")
            {
                if (!Double.TryParse(RussianDouble(((Control)sender).Text), out number))
                {
                    MessageBox.Show("Введено неверное значение");
                    ((Control)sender).Text = "";
                }
            }
        }

        private void Modelling_Load(object sender, EventArgs e)
        {

        }
    }
}
