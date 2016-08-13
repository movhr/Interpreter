using System;
using System.Linq;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class MethodExaminationWindow : Form
    {
        private readonly string _methodName;
        private Interpreter.IExpression[] _functionBody;

        public MethodExaminationWindow(string methodName)
        {
            InitializeComponent();
            this._methodName = methodName;
        }

        private void MethodExaminationWindow_Load(object sender, EventArgs e)
        {
            Text = ($"Examining \'{_methodName}\'");
            _functionBody = Interpreter.ProgramMethods[_methodName].Body.ToArray();
            for (var i = 0; i < _functionBody.Count(); i++)
                dataGridView1.Rows.Add(new DataGridViewRow { Cells =
                {
                    new DataGridViewTextBoxCell { Value = i+1 },
                    new DataGridViewTextBoxCell { Value = _functionBody[i].ToString() }
                } });
        }
    }
}