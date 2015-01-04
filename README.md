ARM processor simulator project for Cps 310 at Bob Jones University

Project Journal: https://docs.google.com/spreadsheets/d/1z99c5LJql3Flhe80zSGj2ysrfxUq2hxqghhLrzynDYY

ARMSim-2.0
==========


<h3>Introduction</h3>
The ARMSim 2.0 Simulator gives a GUI disassembler/debugger for ARM v4 architecture executables. The simulator allows the user to take an ELF file, load it into a simulated RAM and registers, and step through or run the program. The GUI allows the user to interact with the program and observe things like memory, registers, the stack, console input/output, and a disassembly of the currently executing instructions. This report contains information concerning the software development process and usage of the simulator.

<h3>Features</h3>
Basic features of the simulator

<h4>Instructions Implemented</h4>

<h5>Data Processing Instructions:</h5>
 |  |  |  |  | |
 ----|----|----|----|----|----
AND | EOR | SUB | RSB | ADD | ORR
MOV | BIC | MVN | CMP | MOVS | 



<h5>Load/Store Instructions:</h5>
* Pre-Indexed with and without writeback
* Post-Indexed with writeback

<h5>Load/Store Multiple Instructions:</h5>
 |  |  |  |  
 ----|----|----|----
LDMFD | LDMFA | LDMED | LDMEA
STMFD | STMFA | STMED | STMEA
*With and without writeback*

<h5>Multiply Instructions:</h5>
* MUL

<h5>SWI Instructions:</h5>
|  |  |  |
----|----|----
SWI #0 | SWI #1 | SWI #2

<h5>MSR/MRS Instructions:</h5>
|  |  |
-----|-----
MSR | MRS


<h4>Addressing Modes Implemented</h4>
* Immediate
* Register
* Logical Shift Left by immediate
* Logical Shift Left by register
* Logical Shift Right by immediate
* Logical Shift Right by register
* Arithmetic Shift Right by immediate
* Arithmetic Shift Right by register
* Rotate Right by immediate
* Rotate Right by register


<h4>Processor Modes:</h4>
* System
* Supervisor
* IRQ

<h4>GUI Features</h4>
* Flags
* Registers
* Stack
* Memory
* Disassembly
* Console I/O
* Trace Enable/Disable
* Run, Step, Break, Open, and Reset buttons
* Status Information
* Hotkeys

<h5>Input/Output</h5>
The console in the GUI supports all the keys on a regular keyboard that would be used to input characters except tab. Backspace is not supported, but the enter key is.

<h5>Extra Credit</h5>
All of the shift by register instructions are extra credit and can be tested on sieve.exe. The extra Load/Store addressing modes are tested, but no programs were made to use them. The extra Load/Store Multiple instructions are untested.


<h3>Software Prerequisites</h3>
* Windows 7/8
* Visual Studio 2012
* .NET 4.5


<h3>Build and Test</h3>
To build and test the application, open the project file in Visual Studio 2012. Right click the project name and select “Properties.” In the debug section’s command line arguments section, type “--test”(without the quotes). Close the Properties window and either press f5 or click the green start button at the top of the screen to run the program in test mode.
Configuration
To turn logging on, open up the “app.config” file and find the line which says,

      <!--<add name="MyLog" type="System.Diagnostics.TextWriterTraceListener" initializeData="mylogfile.txt" />-->

and uncomment it.  To redirect the logging to a file other than mylogfile.txt, replace it with your preferred file. To turn the logging off, reinsert the comment syntax around the line.


<h3>User Guide</h3>
There are two options for running the simulator. You can either open it from the command line, providing any of the options listed below, or double click the executable. 

<h5>Command Line Options:</h5>
Instruction | Description
----|------
*--load  fileName* 	| Tell the loader what the name of your file is
*--mem  memory-size* | Choose how much RAM to simulate (in bytes)
*--test*	| Run unit tests and exit
*--exec*	| Combined with --load, runs requested program and quits
*Note: Maximum RAM size is 100mb*

Once an ELF program has been successfully loaded, you can use the following buttons to perform the specified action(s) in the GUI.
<h5>Button Operations:</h5>
Button | Action
----|----
Run | Begin Running the loaded ELF program
Step | Run a single line of disassembled code
Break | Stop the execution of a running program
Open | Opens up a file browser for selecting an ELF program to load
Reset | Reset system to having just opened the Loaded ELF program
Trace | Turn use of the trace log on and off

The following explains the meaning and use of each GUI component.

<h5>GUI Elements:</h5>
Element | Usage
----|----
Disassembly | Disassembly of instructions surrounding currently executing instruction
Flags | Current status of flags
Registers | Values in each register
Processor Mode | Current processor mode
Running Status | Running or Not Running
MD5 Hash | The MD5 checksum of the simulated RAM
Console | Used for input and output to and from the simulated program
Memory | Peek into memory at a specified address
Stack | View values above and below the current stack pointer


<h3>Software Architecture</h3>

The GUI class, or view, at the very top owns and controls an instance of the computer class which encapsulates the model.The Computer has an instance of the CPU, and the CPU has instances of Memory and Registers representing the RAM and registers of the computer. The Computer uses model-view separation when interacting with the GUI only to trigger events and never to directly modify any GUI elements.

Pressing Run in the GUI will cause the GUI to spawn a thread in Computer executing the instructions of the loaded file. The thread will end if some type of end-of-program instruction occurs or the user presses the Break button in the GUI. Upon completion, the thread will trigger the GUI to update its elements. The Step button will perform one fetch-decode-execute cycle without generating a thread. Any console I/O that occurs during the running of the Run thread or the Step operation will trigger a GUI update that immediately puts new characters into the console.

The Instruction classes all inherit directly or indirectly from the Instruction abstract class which contains some useful universal methods and variables. Some classes also inherit from other subclasses. Each Instruction subclass contains the implementation of a category of instruction, and contains the ability to execute and disassemble instructions of its type. 

The registers class contains support for flags and mode bits which can be manipulated by certain instructions to switch between processor modes. Switching between processor modes will swap out certain banked registers. The GUI will also keep the user informed of the current processor mode.

The general design of the simulator was focused on class inheritance and model-view separation.


<h3><s>Bug Report</s></h3>
~~X level simulator does not allow for running any of the old executables. However, the following list of OS_X executables appear to be working:~~
* ~~mersenne.exe~~
* ~~mersenne_no_input.exe~~
* ~~quicksort.exe~~
* ~~sieve.exe~~
