namespace ConfigAdmin
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnUndo = new System.Windows.Forms.Button();
            this.IsEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FolderPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.KeepFilesInDays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnAddItem = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Decide which folders should be cleaned up automatically";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsEnabled,
            this.FolderPath,
            this.KeepFilesInDays,
            this.ActionDelete});
            this.dataGridView1.Location = new System.Drawing.Point(15, 37);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(754, 312);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(694, 355);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(576, 355);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(82, 23);
            this.btnUndo.TabIndex = 3;
            this.btnUndo.Text = "Undo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // IsEnabled
            // 
            this.IsEnabled.DataPropertyName = "IsEnabled";
            this.IsEnabled.Frozen = true;
            this.IsEnabled.HeaderText = "Enabled";
            this.IsEnabled.MinimumWidth = 60;
            this.IsEnabled.Name = "IsEnabled";
            this.IsEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsEnabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsEnabled.Width = 60;
            // 
            // FolderPath
            // 
            this.FolderPath.DataPropertyName = "FolderPath";
            this.FolderPath.Frozen = true;
            this.FolderPath.HeaderText = "Folder";
            this.FolderPath.MinimumWidth = 450;
            this.FolderPath.Name = "FolderPath";
            this.FolderPath.Width = 450;
            // 
            // KeepFilesInDays
            // 
            this.KeepFilesInDays.DataPropertyName = "KeepFilesInDays";
            this.KeepFilesInDays.Frozen = true;
            this.KeepFilesInDays.HeaderText = "Keep files for (in days)";
            this.KeepFilesInDays.Name = "KeepFilesInDays";
            // 
            // ActionDelete
            // 
            this.ActionDelete.DataPropertyName = "ActionDelete";
            this.ActionDelete.Frozen = true;
            this.ActionDelete.HeaderText = "Delete entry";
            this.ActionDelete.Name = "ActionDelete";
            this.ActionDelete.ReadOnly = true;
            this.ActionDelete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ActionDelete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // btnAddItem
            // 
            this.btnAddItem.Location = new System.Drawing.Point(237, 109);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(126, 23);
            this.btnAddItem.TabIndex = 4;
            this.btnAddItem.Text = "Add new entry";
            this.btnAddItem.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 390);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Configure folders to clean up";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn FolderPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeepFilesInDays;
        private System.Windows.Forms.DataGridViewButtonColumn ActionDelete;
        private System.Windows.Forms.Button btnAddItem;
    }
}

