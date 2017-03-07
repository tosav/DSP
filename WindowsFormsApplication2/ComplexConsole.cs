using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexConsole
{
    public class Complex
    {
        public static readonly Complex I = new Complex(0d, 1d);
        public double Re { get; set; }
        public double Im { get; set; }
        public double Amplitude { get; set; }//Амплитуда для АЧХ
        public double Faza { get; set; }//Фаза для ФЧХ
        public double Frecuensy { get; set; }//Частота гармоники
        public double Arg { get; set; }//Значение в точке
        public Complex(double r, double i)
        {
            Re = r;
            Im = i;
        }
        public Complex()
        {
            Re = 0d;
            Im = 1d;
        }
    }
}
