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
using System.IO;

namespace ARMSim_2._0
{
    public partial class Form1 : Form
    {
        Computer computer;
        Options arguments;
        FileStream traceFile;
        

        public Form1(Options args)
        {
            InitializeComponent();
            arguments = args;
            //Initialize registers
            for (int i = 0; i < 16; ++i)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(registergrid);
                newRow.Cells[0].Value = "r" + i.ToString();
                newRow.Cells[1].Value = "0x00000000";
                registergrid.Rows.Add(newRow);
            }
            //Initialize memory panel
            for (int i = 0; i < 16; ++i)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(memorypanel);
                newRow.Cells[0].Value = newRow.Cells[1].Value = newRow.Cells[2].Value = newRow.Cells[3].Value = newRow.Cells[4].Value = "";
                memorypanel.Rows.Add(newRow);
            }
            //Initialize stack panel
            for (int i = 0; i < 12; ++i)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(stackpanel);
                newRow.Cells[0].Value = newRow.Cells[1].Value = "";
                stackpanel.Rows.Add(newRow);
            }
            //Initialize computer
            computer = new Computer(arguments, ref traceFile);
            //Initialize trace file
            traceFile = new FileStream("trace.log", FileMode.Create, FileAccess.Write);
            //Did they run from command line with file?
            if (arguments.fileName == "")
            {
                runbtn.Enabled = false;
                resetbtn.Enabled = false;
                stepbtn.Enabled = false;
                tracecheckbox.Enabled = false;
            }
        }

        private void runbtn_Click(object sender, EventArgs e)
        {
            new Thread(computer.run).Start();
            activitylabel.Text = "Running";
            runbtn.Enabled = false;
            openbtn.Enabled = false;
            stepbtn.Enabled = false;
            resetbtn.Enabled = false;
            tracecheckbox.Enabled = false;
            breakbtn.Enabled = true;
        }

        private void stepbtn_Click(object sender, EventArgs e)
        {
            computer.step();
            UpdateGUI();
        }

        private void breakbtn_Click(object sender, EventArgs e)
        {
            computer.stopRun();
            runbtn.Enabled = true;
            openbtn.Enabled = true;
            stepbtn.Enabled = true;
            resetbtn.Enabled = true;
            tracecheckbox.Enabled = true;
            breakbtn.Enabled = false;
            activitylabel.Text = "Not Running";
            UpdateGUI();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            var FD = new OpenFileDialog();
            FD.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*";
            FD.FilterIndex = 1;
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                arguments.fileName = FD.FileName;
                computer = new Computer(arguments, ref traceFile);
                UpdateGUI();
                runbtn.Enabled = true;
                openbtn.Enabled = true;
                stepbtn.Enabled = true;
                resetbtn.Enabled = true;
                tracecheckbox.Enabled = true;
                breakbtn.Enabled = false;
            }
        }

        private void UpdateGUI()
        {
            md5label.Text = "MD5: " + computer.GetMD5();
            //Update Registers
            List<uint> registers = computer.GetRegisters();
            for (int i = 0; i < 16; ++i)
            {
                registergrid.Rows[i].Cells[1].Value = "0x" + String.Format("{0:X8}", registers[i]);
            }
            //Update Memory Panel
            UpdateMemoryPanel();
            //Update Stack Panel
            uint ebp = computer.GetStackPointer();
            for (int i = 11; i >= 0; --i)
            {
                DataGridViewRow row = stackpanel.Rows[11 - i];
                row.Cells[0].Value = "0x" + String.Format("{0:X8}", ebp + (i * 4)) + (i == 0 ? " (ebp) " : "");
                row.Cells[1].Value = "0x" + String.Format("{0:X8}", computer.GetWord(ebp + ((uint)i * 4)));
            }
        }

        private void UpdateMemoryPanel()
        {
            try
            {
                uint address = Convert.ToUInt32(memoryaddressentry.Text);
                if (address % 4 == 0 && address < arguments.memorySize)
                {
                    for (uint i = 0; i < 16; ++i)
                    {
                        DataGridViewRow row = memorypanel.Rows[(int)i];
                        row.Cells[0].Value = "0x" + String.Format("{0:X8}", address + (i * 16));
                        row.Cells[1].Value = "0x" + String.Format("{0:X8}", computer.GetWord(address + (i * 16)));
                        row.Cells[2].Value = "0x" + String.Format("{0:X8}", computer.GetWord(address + (i * 16) + 4));
                        row.Cells[3].Value = "0x" + String.Format("{0:X8}", computer.GetWord(address + (i * 16) + 8));
                        row.Cells[4].Value = "0x" + String.Format("{0:X8}", computer.GetWord(address + (i * 16) + 12));
                    }
                }
            }
            catch { }
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
            computer.Reset(arguments);
            UpdateGUI();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void memoryaddressentry_TextChanged(object sender, EventArgs e)
        {
            UpdateMemoryPanel();
        }

        private void tracecheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (tracecheckbox.Checked)
            {
                traceFile = new FileStream("trace.log", FileMode.Create, FileAccess.Write);
            }
            else
            {
                traceFile.Close();
                traceFile = null;
            }
        }

    }
}
