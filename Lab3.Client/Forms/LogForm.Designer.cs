namespace Lab3.Client.Forms
{
    partial class LogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.CommandText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommandResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommandDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CommandText,
            this.CommandResult,
            this.CommandDate});
            this.dataGridView1.Location = new System.Drawing.Point(-2, -2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 29;
            this.dataGridView1.Size = new System.Drawing.Size(883, 516);
            this.dataGridView1.TabIndex = 0;
            // 
            // CommandText
            // 
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CommandText.DefaultCellStyle = dataGridViewCellStyle1;
            this.CommandText.FillWeight = 106.9519F;
            this.CommandText.HeaderText = "Command Text";
            this.CommandText.MinimumWidth = 6;
            this.CommandText.Name = "CommandText";
            this.CommandText.ReadOnly = true;
            // 
            // CommandResult
            // 
            this.CommandResult.HeaderText = "Command Result";
            this.CommandResult.MinimumWidth = 6;
            this.CommandResult.Name = "CommandResult";
            this.CommandResult.ReadOnly = true;
            // 
            // CommandDate
            // 
            this.CommandDate.FillWeight = 93.04813F;
            this.CommandDate.HeaderText = "Command Date";
            this.CommandDate.MinimumWidth = 6;
            this.CommandDate.Name = "CommandDate";
            this.CommandDate.ReadOnly = true;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(879, 509);
            this.Controls.Add(this.dataGridView1);
            this.Name = "LogForm";
            this.Text = "LogForm";
            this.Load += new System.EventHandler(this.LogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandText;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommandDate;
    }
}