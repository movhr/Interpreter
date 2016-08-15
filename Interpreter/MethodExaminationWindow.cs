using System;
using System.Linq;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class MethodExaminationWindow : Form
    {
        private readonly string _methodName;
        
        public MethodExaminationWindow(string methodName)
        {
            InitializeComponent();
            this._methodName = methodName;
        }

        private void MethodExaminationWindow_Load(object sender, EventArgs e)
        {
            Text = ($"Examining \'{_methodName}\'");
            var functionBody = Interpreter.GetMethodBody(_methodName).ToArray();
            for (var i = 0; i < functionBody.Count(); i++)
                dataGridView1.Rows.Add(new DataGridViewRow { Cells =
                {
                    new DataGridViewTextBoxCell { Value = i+1 },
                    new DataGridViewTextBoxCell { Value = functionBody[i].ToString() }
                } });
        }
    }
}