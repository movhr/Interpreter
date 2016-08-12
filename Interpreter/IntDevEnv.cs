using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class IntDevEnv : Form
    {
        public IntDevEnv()
        {
            InitializeComponent();
        }

        private void interpretButton_Click(object sender, EventArgs e)
        {


            //Disable user interaction to ensure correct results
            debugButton.Enabled = false;
            debugStepButton.Enabled = false;
            exitDebugButton.Enabled = false;
            interpretButton.Enabled = false;

            //Do the action and add program variables + methods
            Interpreter.Interpret(richTextBox1.Text);
            Interpreter.ProgramVariables.ToList().ForEach(pair => dataGridView1.Rows.Add(new DataGridViewRow
            {
                Cells = {
                    new DataGridViewTextBoxCell {Value = pair.Key},
                    new DataGridViewTextBoxCell {Value = pair.Value.Value}
                }
            }));
            Interpreter.ProgramMethods.ToList().ForEach(pair => dataGridView2.Rows.Add(new DataGridViewRow
            {
                Cells =
                {
                    new DataGridViewTextBoxCell {Value = pair.Key},
                    new DataGridViewTextBoxCell {Value = pair.Value.Body.Count()}
                }
            }));

            //Re-enable user interaction
            debugButton.Enabled = true;
            debugStepButton.Enabled = true;
            exitDebugButton.Enabled = true;
            interpretButton.Enabled = true;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void debugButton_Click(object sender, EventArgs e)
        {
            Interpreter.StartDebug(richTextBox1.Text);
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
    }
}