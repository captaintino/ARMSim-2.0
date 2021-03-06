﻿using System;
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

        // Event Handler variables
        public delegate void UpdateEverything();
        public delegate void WriteCharacter();
        public UpdateEverything myDelegate;
        public WriteCharacter myDelegate2;

        public Form1(Options args)
        {
            InitializeComponent();
            myDelegate = new UpdateEverything(UpdateEverythingMethod);
            myDelegate2 = new WriteCharacter(writeCharacter);
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
            computer.ProvideParentForm(this);
            //Did they run from command line with file?
            if (!computer.Initialized())
            {
                runbtn.Enabled = false;
                resetbtn.Enabled = false;
                stepbtn.Enabled = false;
            }
            else
            {
                UpdateGUI();
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

        // Update GUI - to be triggered when program ends
        public void UpdateEverythingMethod()
        {
            breakbtn.PerformClick();
        }

        // Update Console - to be triggered when computer performs a writechar
        public void writeCharacter()
        {
            if (computer.outputBuffer.Count > 0)
            {
                char c = computer.outputBuffer.Dequeue();
                console.Text += (c == '\n' || c == '\r' ? "\r\n" : c.ToString());
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
            //Update Disassembly
            ResetDisassembly();
            //Update Processor Mode
            UpdateProcessorMode();
        }
        
        //Update Register panel values
        private void UpdateRegisters()
        {
            List<uint> registers = computer.GetRegisters();
            for (int i = 0; i < 16; ++i)
            {
                registergrid.Rows[i].Cells[1].Value = "0x" + String.Format("{0:X8}", registers[i] - (i == 15 ? 8 : 0));
            }
        }

        // Update memory panel depending on contents of address box
        private void UpdateMemoryPanel()
        {
            try
            {
                uint address = Convert.ToUInt32(memoryaddressentry.Text, 16);
                if (address % 4 == 0 && address < arguments.memorySize)
                {
                    for (uint i = 0; i < 16; ++i)
                    {
                        DataGridViewRow row = memorypanel.Rows[(int)i];
                        row.Cells[0].Value = "0x" + String.Format("{0:X8}", address + (i * 16));
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
            for (int i = -5; i <= 6; ++i)
            {
                DataGridViewRow row = stackpanel.Rows[6 - i];
                row.Cells[0].Value = (i == 0 ? " (SP) " : "") + "0x" + String.Format("{0:X8}", ebp + (i * 4));
                row.Cells[1].Value = UintToHexExtended(computer.GetWord((uint)(ebp + ((i) * 4))));
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
            irqflagcheckbox.Checked = computer.IRQ;
        }

        // Set processor mode label to correct value
        private void UpdateProcessorMode()
        {
            switch (computer.GetProcMode())
            {
                case 16:
                    procMode.Text = "Processor Mode: User";
                    return;
                case 17:
                    procMode.Text = "Processor Mode: FIQ";
                    return;
                case 18:
                    procMode.Text = "Processor Mode: IRQ";
                    return;
                case 19:
                    procMode.Text = "Processor Mode: Supervisor";
                    return;
                case 23:
                    procMode.Text = "Processor Mode: Abort";
                    return;
                case 27:
                    procMode.Text = "Processor Mode: Undefined";
                    return;
                case 31:
                    procMode.Text = "Processor Mode: System";
                    return;
            }
        }

        // Update disassembly panel
        private void ResetDisassembly()
        {
            string disassembly = "";
            int progc = (int)computer.getProgramCounter() - 8;
            int begin = progc - (7*4);
            int end = progc + (8*4);
            while ((begin += 4) <= end)
            {
                if (begin >= 0)
                {
                    Instruction nop = computer.getInstruction((uint)begin);
                    if (nop != null)
                    {
                        disassembly += (begin == progc ? ">>>\t" : "") + String.Format("{0:X8}\t{1:X8}\t", begin, nop.data) + (nop.data == 0 ? "nop" : nop.ToString()) + "\r\n";
                    }
                }
            }
            disassembledfile.Text = disassembly;
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
                    return;
                case Keys.F10:
                    if (stepbtn.Enabled)
                        stepbtn.PerformClick();
                    return;
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
            else if(e.KeyCode == Keys.Back){
                if (computer.inputBuffer.Count > 0)
                {
                    //computer.inputBuffer.Dequeue();
                    //console.Text = console.Text.Remove(console.Text.Length - 1);
                    //if (computer.inputBuffer.Count == 0)
                    //{
                    //    computer.IRQ = false;
                    //}
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                computer.inputBuffer.Enqueue(' ');
                computer.IRQ = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                computer.inputBuffer.Enqueue('\r');
                computer.inputBuffer.Enqueue('\n');
                computer.IRQ = true;
            }
            else if (e.KeyValue < 128)
            {
                char newChar = (char)e.KeyValue;
                if (Char.IsLetterOrDigit(newChar))
                {
                    if (Char.IsDigit(newChar))
                    {
                        computer.inputBuffer.Enqueue(e.Shift ? Global.convertNumbers[Convert.ToInt32(newChar.ToString())] : newChar);
                        computer.IRQ = true;
                    }
                    else
                    {
                        computer.inputBuffer.Enqueue((char)((Char.IsLetter(newChar) ? (Convert.ToInt32(newChar) | (e.Shift ? 0 : 32)) : 0)));
                        computer.IRQ = true;
                    }
                }
            }
            else
            {
                if (Global.convertPunctuation.ContainsKey(e.KeyValue))
                {
                    computer.inputBuffer.Enqueue(e.Shift ? Global.convertPunctuation[e.KeyValue].Item2 : Global.convertPunctuation[e.KeyValue].Item1);
                    computer.IRQ = true;
                }
            }

        }

        // Reload computer and update GUI
        private void resetbtn_Click(object sender, EventArgs e)
        {
            computer.Reset(arguments);
            UpdateGUI();
            console.Text += "\r\nRESET\r\n";
        }

        // Update memory panel with new memory address input by user
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
                computer.activateTrace(traceFile);
            }
            else
            {
                traceFile.Close();
                traceFile = null;
                computer.deactivateTrace();
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

        // Run loaded program and return
        public void runOnce()
        {
            computer.ProvideParentForm(null);
            computer.run();
        }
    }
}
