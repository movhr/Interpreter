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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.interpretButton = new System.Windows.Forms.Button();
            this.debugButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.VariableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VariableValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.MethodName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nExpressions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.debugStepButton = new System.Windows.Forms.Button();
            this.exitDebugButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(13, 13);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(219, 234);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "var aval = 5;\nsub somefun(avalue) { avalue = 10; }\nsomefun(aval);";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // interpretButton
            // 
            this.interpretButton.Location = new System.Drawing.Point(6, 19);
            this.interpretButton.Name = "interpretButton";
            this.interpretButton.Size = new System.Drawing.Size(75, 23);
            this.interpretButton.TabIndex = 1;
            this.interpretButton.Text = "Interpret";
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
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VariableName,
            this.VariableValue});
            this.dataGridView1.Location = new System.Drawing.Point(239, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(245, 114);
            this.dataGridView1.TabIndex = 3;
            // 
            // VariableName
            // 
            this.VariableName.HeaderText = "Variable Name";
            this.VariableName.Name = "VariableName";
            this.VariableName.ReadOnly = true;
            // 
            // VariableValue
            // 
            this.VariableValue.HeaderText = "Variable Value";
            this.VariableValue.Name = "VariableValue";
            this.VariableValue.ReadOnly = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MethodName,
            this.nExpressions});
            this.dataGridView2.Location = new System.Drawing.Point(239, 133);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(245, 114);
            this.dataGridView2.TabIndex = 4;
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
            this.groupBox1.Location = new System.Drawing.Point(495, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 255);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
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
            // IntDevEnv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 255);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "IntDevEnv";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button interpretButton;
        private System.Windows.Forms.Button debugButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn VariableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn VariableValue;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn MethodName;
        private System.Windows.Forms.DataGridViewTextBoxColumn nExpressions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button debugStepButton;
        private System.Windows.Forms.Button exitDebugButton;
        private System.Windows.Forms.Label label1;
    }
}

