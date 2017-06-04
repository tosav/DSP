﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Length_of_the_segment : Form
    {
        private Spectral sp;
        public Length_of_the_segment()
        {
            InitializeComponent();
            if (Dispatcher.getInstance().L != 0)
                textBox1.Text = Dispatcher.getInstance().L.ToString();
            else
                textBox1.Text = Math.Round(Math.Sqrt((double)Dispatcher.getInstance().getN())).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sp.setL( Convert.ToInt32(textBox1.Text));
            Dispatcher.getInstance().L = Convert.ToInt32(textBox1.Text);
            this.Close();
        }

        public void Sp(Spectral SP)
        {
            sp = SP;
        }
    }
}
