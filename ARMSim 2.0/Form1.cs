using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace ARMSim_2._0
{
    public partial class Form1 : Form
    {
        Computer computer;
        Options arguments;
        

        public Form1(Options args)
        {
            InitializeComponent();
            arguments = args;
            computer = new Computer(arguments);
            if (arguments.fileName == "")
            {
                runbtn.Enabled = false;
                resetbtn.Enabled = false;
                stepbtn.Enabled = false;
            }
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
            var FD = new OpenFileDialog();
            FD.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*";
            FD.FilterIndex = 1;
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                arguments.fileName = FD.FileName;
                computer = new Computer(arguments);
                updateGUI();
                runbtn.Enabled = true;
                openbtn.Enabled = true;
                stepbtn.Enabled = true;
                breakbtn.Enabled = false;
                resetbtn.Enabled = true;
            }
        }

        private void updateGUI()
        {
            md5label.Text = "MD5: " + computer.getMD5();
            var j = registertree.TopNode.FirstNode;
            while (j.NextNode != null)
            {
                j.FirstNode.Text = "Value: + NUM";
                j.LastNode.Text = "Decoded Value: + NUM";
                j = j.NextNode;
            }
        }

        private void KeyEvent(object sender, KeyEventArgs e) //Keyup Event 
        {
            // Function Keys
            switch (e.KeyCode)
            {
                case Keys.F5:
                    if (runbtn.Enabled)
                        runbtn.PerformClick();
                    break;
                case Keys.F10:
                    if (stepbtn.Enabled)
                        stepbtn.PerformClick();
                    break;
            }

            // Ctrl + Keys
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.O:
                        if (openbtn.Enabled)
                            openbtn.PerformClick();
                        break;
                    case Keys.T:
                        // TRACE
                        break;
                    case Keys.R:
                        if (resetbtn.Enabled)
                            resetbtn.PerformClick();
                        break;
                    case Keys.B:
                        if (breakbtn.Enabled)
                            breakbtn.PerformClick();
                        break;
                }
            }
        }

        private void resetbtn_Click(object sender, EventArgs e)
        {
            computer = new Computer(arguments);
            updateGUI();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
