using System.Security.AccessControl;
using System.Windows.Forms;

namespace Interpreter
{
    partial class IntDevEnv
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntDevEnv));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.interpretButton = new System.Windows.Forms.Button();
            this.debugButton = new System.Windows.Forms.Button();
            this.programVariablesCollection = new System.Windows.Forms.DataGridView();
            this.VariableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VariableValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.programMethodsCollection = new System.Windows.Forms.DataGridView();
            this.MethodName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nExpressions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.debugStepButton = new System.Windows.Forms.Button();
            this.exitDebugButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.programVariablesCollection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.programMethodsCollection)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.BackColor = System.Drawing.Color.Black;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.Lime;
            this.richTextBox1.Location = new System.Drawing.Point(201, 0);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(30, 3, 30, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(584, 427);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            this.richTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyDown);
            this.richTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyUp);
            // 
            // interpretButton
            // 
            this.interpretButton.Location = new System.Drawing.Point(6, 19);
            this.interpretButton.Name = "interpretButton";
            this.interpretButton.Size = new System.Drawing.Size(75, 23);
            this.interpretButton.TabIndex = 1;
            this.interpretButton.Text = "Run";
            this.interpretButton.UseVisualStyleBackColor = true;
            this.interpretButton.Click += new System.EventHandler(this.interpretButton_Click);
            // 
            // debugButton
            // 
            this.debugButton.Location = new System.Drawing.Point(6, 49);
            this.debugButton.Name = "debugButton";
            this.debugButton.Size = new System.Drawing.Size(75, 23);
            this.debugButton.TabIndex = 2;
            this.debugButton.Text = "Debug";
            this.debugButton.UseVisualStyleBackColor = true;
            this.debugButton.Click += new System.EventHandler(this.debugButton_Click);
            // 
            // programVariablesCollection
            // 
            this.programVariablesCollection.AllowUserToAddRows = false;
            this.programVariablesCollection.AllowUserToDeleteRows = false;
            this.programVariablesCollection.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.programVariablesCollection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.programVariablesCollection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VariableName,
            this.VariableValue});
            this.programVariablesCollection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.programVariablesCollection.Location = new System.Drawing.Point(0, 0);
            this.programVariablesCollection.MultiSelect = false;
            this.programVariablesCollection.Name = "programVariablesCollection";
            this.programVariablesCollection.RowHeadersVisible = false;
            this.programVariablesCollection.Size = new System.Drawing.Size(201, 212);
            this.programVariablesCollection.TabIndex = 3;
            // 
            // VariableName
            // 
            this.VariableName.HeaderText = "Variable Name";
            this.VariableName.Name = "VariableName";
            // 
            // VariableValue
            // 
            this.VariableValue.HeaderText = "Variable Value";
            this.VariableValue.Name = "VariableValue";
            // 
            // programMethodsCollection
            // 
            this.programMethodsCollection.AllowUserToAddRows = false;
            this.programMethodsCollection.AllowUserToDeleteRows = false;
            this.programMethodsCollection.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.programMethodsCollection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.programMethodsCollection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MethodName,
            this.nExpressions});
            this.programMethodsCollection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.programMethodsCollection.Location = new System.Drawing.Point(0, 0);
            this.programMethodsCollection.MultiSelect = false;
            this.programMethodsCollection.Name = "programMethodsCollection";
            this.programMethodsCollection.ReadOnly = true;
            this.programMethodsCollection.RowHeadersVisible = false;
            this.programMethodsCollection.Size = new System.Drawing.Size(201, 211);
            this.programMethodsCollection.TabIndex = 4;
            this.programMethodsCollection.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.methodCollection_DoubleClick);
            // 
            // MethodName
            // 
            this.MethodName.HeaderText = "Method Name";
            this.MethodName.Name = "MethodName";
            this.MethodName.ReadOnly = true;
            // 
            // nExpressions
            // 
            this.nExpressions.HeaderText = "Expressions";
            this.nExpressions.Name = "nExpressions";
            this.nExpressions.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.debugStepButton);
            this.groupBox1.Controls.Add(this.exitDebugButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.interpretButton);
            this.groupBox1.Controls.Add(this.debugButton);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(785, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(111, 427);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // debugStepButton
            // 
            this.debugStepButton.Location = new System.Drawing.Point(6, 195);
            this.debugStepButton.Name = "debugStepButton";
            this.debugStepButton.Size = new System.Drawing.Size(75, 23);
            this.debugStepButton.TabIndex = 5;
            this.debugStepButton.Text = "Step Over";
            this.debugStepButton.UseVisualStyleBackColor = true;
            this.debugStepButton.Click += new System.EventHandler(this.debugStepButton_Click);
            // 
            // exitDebugButton
            // 
            this.exitDebugButton.Location = new System.Drawing.Point(6, 224);
            this.exitDebugButton.Name = "exitDebugButton";
            this.exitDebugButton.Size = new System.Drawing.Size(75, 23);
            this.exitDebugButton.TabIndex = 4;
            this.exitDebugButton.Text = "Exit Debug";
            this.exitDebugButton.UseVisualStyleBackColor = true;
            this.exitDebugButton.Click += new System.EventHandler(this.exitDebugButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Debugging";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(200, 200);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.programVariablesCollection);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.programMethodsCollection);
            this.splitContainer1.Size = new System.Drawing.Size(201, 427);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 6;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 427);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(896, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // IntDevEnv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 449);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(700, 300);
            this.Name = "IntDevEnv";
            this.Text = "IDE for programming language";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.programVariablesCollection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.programMethodsCollection)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button interpretButton;
        private System.Windows.Forms.Button debugButton;
        private System.Windows.Forms.DataGridView programVariablesCollection;
        private System.Windows.Forms.DataGridViewTextBoxColumn VariableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn VariableValue;
        private System.Windows.Forms.DataGridView programMethodsCollection;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button debugStepButton;
        private System.Windows.Forms.Button exitDebugButton;
        private System.Windows.Forms.Label label1;
        private DataGridViewTextBoxColumn MethodName;
        private DataGridViewTextBoxColumn nExpressions;
        private SplitContainer splitContainer1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
    }
}

