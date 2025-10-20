using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WordTrain
{
    public partial class FmWait : Form
    {
        public FmWait()
        {
            InitializeComponent();
        }
    }

    public static class FWait
    {
        private static FmWait fm = new FmWait();

        public static void SetText(string s, double per)
        {
            fm.label1.Visible = true;
            fm.label3.Visible = false;
            fm.label4.Visible = false;

            fm.Visible = (per >= 0);
            fm.label1.Text = s;
            fm.progressBar1.Value = Math.Abs(Convert.ToInt32(Math.Round(per)));
            fm.Refresh();
            Application.DoEvents();
        }

        public static void SetText(string s1, string s2, double per)
        {
            fm.label1.Visible = false;
            fm.label3.Visible = true;
            fm.label4.Visible = true;

            fm.Visible = (per >= 0);
            fm.label3.Text = s1;
            fm.label4.Text = s2;
            fm.progressBar1.Value = Math.Abs(Convert.ToInt32(Math.Round(per)));
            fm.Refresh();
            Application.DoEvents();
        }

    
    }

    
}