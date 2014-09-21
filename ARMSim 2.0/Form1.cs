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
                memorypanel.Rows.Add(newRow);
            }
            //Initialize stack panel
            for (int i = 0; i < 12; ++i)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(stackpanel);
                stackpanel.Rows.Add(newRow);
            }
            //Initialize trace file
            traceFile = new FileStream("trace.log", FileMode.Create, FileAccess.Write);
            //Initialize computer
            computer = new Computer(arguments, ref traceFile);
            //Did they run from command line with file?
            if (arguments.fileName == "")
            {
                runbtn.Enabled = false;
                resetbtn.Enabled = false;
                stepbtn.Enabled = false;
            }
        }

        // Create thread to run computer
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

        // Perform step and update GUI
        private void stepbtn_Click(object sender, EventArgs e)
        {
            computer.step();
            UpdateGUI();
        }

        // Stop computer's running and update GUI
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

        // If file is selected, reset computer with new file
        private void openbtn_Click(object sender, EventArgs e)
        {
            var FD = new OpenFileDialog();
            FD.Filter = "exe files (*.exe)|*.exe|All files (*.*)|*.*";
            FD.FilterIndex = 1;
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                arguments.fileName = FD.FileName;
                computer.Reset(arguments);
                UpdateGUI();
                runbtn.Enabled = true;
                openbtn.Enabled = true;
                stepbtn.Enabled = true;
                resetbtn.Enabled = true;
                tracecheckbox.Enabled = true;
                breakbtn.Enabled = false;
            }
        }

        // Updates all GUI components
        private void UpdateGUI()
        {
            md5label.Text = "MD5: " + computer.GetMD5();
            //Update Registers
            UpdateRegisters();
            //Update Memory Panel
            UpdateMemoryPanel();
            //Update Stack Panel
            UpdateStackPanel();
            //Update Flags
            UpdateFlags();
        }
        
        //Update Register panel values
        private void UpdateRegisters()
        {
            List<uint> registers = computer.GetRegisters();
            for (int i = 0; i < 16; ++i)
            {
                registergrid.Rows[i].Cells[1].Value = "0x" + String.Format("{0:X8}", registers[i]);
            }
        }

        // Update memory panel depending on contents of address box
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
                        row.Cells[0].Value = UintToHexExtended(address + (i * 16));
                        // Set column values for basic memory
                        row.Cells[1].Value = UintToHexExtended(computer.GetWord(address + (i * 16)));
                        row.Cells[2].Value = UintToHexExtended(computer.GetWord(address + (i * 16) + 4));
                        row.Cells[3].Value = UintToHexExtended(computer.GetWord(address + (i * 16) + 8));
                        row.Cells[4].Value = UintToHexExtended(computer.GetWord(address + (i * 16) + 12));
                        // Compute ascii version values of bytes
                        byte[] asciiCharacters = new byte[16];
                        for (uint increment = 0; increment < 4; increment++)
                        {
                            BitConverter.GetBytes(computer.GetWord(address + (i * 16) + (increment * 4))).CopyTo(asciiCharacters, increment * 4);
                        }
                        string asciiString = "";
                        char[] characters = new char[16];
                        new ASCIIEncoding().GetChars(asciiCharacters, 0, 16, characters, 0);
                        foreach (char c in characters)
                        {
                            asciiString += ((c == '\0' || c == '?') ? '.' : c);
                        }
                        row.Cells[5].Value = asciiString;
                    }
                }
            }
            catch { }
        }

        // Update stack panel values
        private void UpdateStackPanel()
        {
            uint ebp = computer.GetStackPointer();
            for (int i = 11; i >= 0; --i)
            {
                DataGridViewRow row = stackpanel.Rows[11 - i];
                row.Cells[0].Value = UintToHexExtended(ebp + (uint)(i * 4)) + (i == 0 ? " (sp) " : "");
                row.Cells[1].Value = UintToHexExtended(computer.GetWord(ebp + ((uint)i * 4)));
            }
        }

        // Update flag check boxes
        private void UpdateFlags()
        {
            List<bool> flags = computer.GetFlags();
            nflagcheckbox.Checked = flags[0];
            zflagcheckbox.Checked = flags[1];
            cflagcheckbox.Checked = flags[2];
            fflagcheckbox.Checked = flags[3];
        }

        // Hot key catcher
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

        // Reload computer and update GUI
        private void resetbtn_Click(object sender, EventArgs e)
        {
            computer.Reset(arguments);
            UpdateGUI();
        }

        private void memoryaddressentry_TextChanged(object sender, EventArgs e)
        {
            UpdateMemoryPanel();
        }

        // Open or close trace file
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

        // Convert <num> to Hexadecimal in little endian form
        private string UintToHex(uint num)
        {
            StringBuilder sBuilder = new StringBuilder();
            byte[] numBytes = BitConverter.GetBytes(num);
            for (int i = 0; i < numBytes.Length; i++)
            {
                sBuilder.Append(numBytes[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        // Convert <num> to Hexadecimal in little endian form with "0x" on the front
        private string UintToHexExtended(uint num)
        {
            return "0x" + UintToHex(num);
        }

    }
}
