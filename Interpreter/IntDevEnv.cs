using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class IntDevEnv : Form
    {
        private KeyEventArgs lastKeyEventArgs;
        private char[] ignoreList = {' ', '\n'};

        public IntDevEnv()
        {
            InitializeComponent();
        }

        private void interpretButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = @"Initializing...";
            programMethodsCollection.Rows.Clear();
            programVariablesCollection.Rows.Clear();

            //Disable user interaction to ensure correct results
            debugButton.Enabled = false;
            debugStepButton.Enabled = false;
            exitDebugButton.Enabled = false;
            interpretButton.Enabled = false;

            //Do the action and add program variables + methods
            var initResult = Interpreter.Start(richTextBox1.Text, "interpret");
            if (initResult == null)
            {   
                Interpreter.GetProgramVariables().ToList().ForEach(pair => programVariablesCollection.Rows.Add(new DataGridViewRow
                {
                    Cells = {
                    new DataGridViewTextBoxCell {Value = pair.Key},
                    new DataGridViewTextBoxCell {Value = pair.Value}
                }
                }));
                Interpreter.GetProgramMethods().ToList().ForEach(pair => programMethodsCollection.Rows.Add(new DataGridViewRow
                {
                    Cells =
                {
                    new DataGridViewTextBoxCell {Value = pair.Key},
                    new DataGridViewTextBoxCell {Value = pair.Value}
                }
                }));

                toolStripStatusLabel1.Text = @"Success!";
            }
            else
                {
                    toolStripStatusLabel1.Text = ($"Error at line {initResult.CodeLine}: {initResult.ErrorInfo}.");
                    MessageBox.Show($"Error at: {initResult.CodeLine}.\n{initResult.ErrorInfo}.");
                }


            //Re-enable user interaction
            debugButton.Enabled = true;
            debugStepButton.Enabled = true;
            exitDebugButton.Enabled = true;
            interpretButton.Enabled = true;
        }

        private void methodCollection_DoubleClick(object sender, EventArgs e) =>
            new MethodExaminationWindow(programMethodsCollection[0, ((DataGridView)sender).SelectedCells[0].RowIndex].Value.ToString()).Show();
        

        private void debugButton_Click(object sender, EventArgs e)
        {
            var initResult = Interpreter.Start(richTextBox1.Text, "debug");
            if(initResult != null)
                MessageBox.Show($"Error at: {initResult.CodeLine}.\n{initResult.ErrorInfo}.");
            debugStepButton.Enabled = true;
        }

        private void debugStepButton_Click(object sender, EventArgs e)
        {
            if (!Interpreter.StepDebug())
                debugStepButton.Enabled = false;
        }

        private void exitDebugButton_Click(object sender, EventArgs e)
        {
            Interpreter.ExitDebug();
            debugStepButton.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            debugButton.Enabled = true;
            interpretButton.Enabled = true;
            debugStepButton.Enabled = false;
            exitDebugButton.Enabled = false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Gets the closest character to a given position in a given string.
        /// </summary>
        /// <param name="str">the string to search through</param>
        /// <param name="pos">the position to start searching from</param>
        /// <param name="ignoreList">a list containing characters to ignore</param>
        /// <param name="nRead">number of characters searched</param>
        /// <param name="backwards">whether to search backwards or forward</param>
        /// <returns>Returns ' ' if no character is found, else return the found character</returns>
        private static char GetClosestCharToPosition(string str, int pos, IEnumerable<char> ignoreList, out int nRead, bool backwards = false)
        {
            var i = pos;
            if (!backwards)
            {
                for (; i < str.Length; i++)
                    if (!ignoreList.Contains(str[i]))
                    {
                        nRead = pos - i;
                        return str[i];
                    }
                nRead = pos - i;
                return Convert.ToChar(' ');
            }

            for (; i >= 0; i--)
                if (!ignoreList.Contains(str[i]))
                {
                    nRead = i - pos;
                    return str[i];
                }
            nRead = i - pos;
            return ' ';
        }

        private void InsertString(string str, int shiftPositions = 1)
        {
            var cursorPosition = richTextBox1.SelectionStart;
            richTextBox1.SelectedText = str;
            richTextBox1.SelectionStart = cursorPosition + shiftPositions;
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (lastKeyEventArgs.KeyCode == Keys.OemOpenBrackets && lastKeyEventArgs.Shift)
            {
                InsertString("\n\t\n}\n", 2);
                lastKeyEventArgs = new KeyEventArgs(Keys.None);
                return;
            }
            if (lastKeyEventArgs.KeyCode == Keys.D9 && lastKeyEventArgs.Shift)
            {
                InsertString("  )");
                lastKeyEventArgs = new KeyEventArgs(Keys.None);
                return;
            }
            if(lastKeyEventArgs.KeyCode == Keys.OemSemicolon)
            {
                InsertString("\n");
                lastKeyEventArgs = new KeyEventArgs(Keys.None);
                return;
            }
            
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            lastKeyEventArgs = e;
        }
    }
}