using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ARMSim_2._0
{
    public partial class Form1 : Form
    {
        Computer computer;
        Options arguments;

        public Form1(Options arguments)
        {
            InitializeComponent();
            computer = new Computer(arguments);
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void registersgrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void runbtn_Click(object sender, EventArgs e)
        {
            new Thread(computer.run).Start();
            runbtn.Enabled = false;
            openbtn.Enabled = false;
            stepbtn.Enabled = false;
            breakbtn.Enabled = true;
        }

        private void stepbtn_Click(object sender, EventArgs e)
        {
            computer.step();
            updateGUI();
        }

        private void breakbtn_Click(object sender, EventArgs e)
        {
            computer.stopRun();
            runbtn.Enabled = true;
            openbtn.Enabled = true;
            stepbtn.Enabled = true;
            breakbtn.Enabled = false;
            updateGUI();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Options arguments = new Options(new[] {"--load", FD.FileName});
                computer = new Computer(arguments);
                updateGUI();
            }
        }

        private void updateGUI()
        {
            md5label.Text = "MD5: " + computer.getMD5();
        }

    }
}
