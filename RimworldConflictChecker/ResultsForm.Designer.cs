namespace RimworldConflictChecker
{
    partial class ResultsForm
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.modBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.xmlFilesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.modRankDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fullDirNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dirNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modXmlDetailsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.coreCheckedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.coreOverrightsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dllCheckedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.modEnabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xmlFilesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(996, 731);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(988, 705);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.modRankDataGridViewTextBoxColumn,
            this.fullDirNameDataGridViewTextBoxColumn,
            this.dirNameDataGridViewTextBoxColumn,
            this.modXmlDetailsDataGridViewTextBoxColumn,
            this.checkedDataGridViewCheckBoxColumn,
            this.coreCheckedDataGridViewCheckBoxColumn,
            this.coreOverrightsDataGridViewTextBoxColumn,
            this.dllCheckedDataGridViewCheckBoxColumn,
            this.modEnabledDataGridViewCheckBoxColumn});
            this.dataGridView1.DataSource = this.modBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(982, 699);
            this.dataGridView1.TabIndex = 0;
            // 
            // modBindingSource
            // 
            this.modBindingSource.DataSource = typeof(RimworldConflictChecker.Mod);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(988, 705);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // xmlFilesBindingSource
            // 
            this.xmlFilesBindingSource.DataMember = "XmlFiles";
            this.xmlFilesBindingSource.DataSource = this.modBindingSource;
            // 
            // modRankDataGridViewTextBoxColumn
            // 
            this.modRankDataGridViewTextBoxColumn.DataPropertyName = "ModRank";
            this.modRankDataGridViewTextBoxColumn.HeaderText = "ModRank";
            this.modRankDataGridViewTextBoxColumn.Name = "modRankDataGridViewTextBoxColumn";
            this.modRankDataGridViewTextBoxColumn.ReadOnly = true;
            this.modRankDataGridViewTextBoxColumn.Width = 79;
            // 
            // fullDirNameDataGridViewTextBoxColumn
            // 
            this.fullDirNameDataGridViewTextBoxColumn.DataPropertyName = "FullDirName";
            this.fullDirNameDataGridViewTextBoxColumn.HeaderText = "FullDirName";
            this.fullDirNameDataGridViewTextBoxColumn.Name = "fullDirNameDataGridViewTextBoxColumn";
            this.fullDirNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.fullDirNameDataGridViewTextBoxColumn.Width = 89;
            // 
            // dirNameDataGridViewTextBoxColumn
            // 
            this.dirNameDataGridViewTextBoxColumn.DataPropertyName = "DirName";
            this.dirNameDataGridViewTextBoxColumn.HeaderText = "DirName";
            this.dirNameDataGridViewTextBoxColumn.Name = "dirNameDataGridViewTextBoxColumn";
            this.dirNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.dirNameDataGridViewTextBoxColumn.Width = 73;
            // 
            // modXmlDetailsDataGridViewTextBoxColumn
            // 
            this.modXmlDetailsDataGridViewTextBoxColumn.DataPropertyName = "ModXmlDetails";
            this.modXmlDetailsDataGridViewTextBoxColumn.HeaderText = "ModXmlDetails";
            this.modXmlDetailsDataGridViewTextBoxColumn.Name = "modXmlDetailsDataGridViewTextBoxColumn";
            this.modXmlDetailsDataGridViewTextBoxColumn.ReadOnly = true;
            this.modXmlDetailsDataGridViewTextBoxColumn.Width = 102;
            // 
            // checkedDataGridViewCheckBoxColumn
            // 
            this.checkedDataGridViewCheckBoxColumn.DataPropertyName = "Checked";
            this.checkedDataGridViewCheckBoxColumn.HeaderText = "Checked";
            this.checkedDataGridViewCheckBoxColumn.Name = "checkedDataGridViewCheckBoxColumn";
            this.checkedDataGridViewCheckBoxColumn.ReadOnly = true;
            this.checkedDataGridViewCheckBoxColumn.Width = 56;
            // 
            // coreCheckedDataGridViewCheckBoxColumn
            // 
            this.coreCheckedDataGridViewCheckBoxColumn.DataPropertyName = "CoreChecked";
            this.coreCheckedDataGridViewCheckBoxColumn.HeaderText = "CoreChecked";
            this.coreCheckedDataGridViewCheckBoxColumn.Name = "coreCheckedDataGridViewCheckBoxColumn";
            this.coreCheckedDataGridViewCheckBoxColumn.ReadOnly = true;
            this.coreCheckedDataGridViewCheckBoxColumn.Width = 78;
            // 
            // coreOverrightsDataGridViewTextBoxColumn
            // 
            this.coreOverrightsDataGridViewTextBoxColumn.DataPropertyName = "CoreOverrights";
            this.coreOverrightsDataGridViewTextBoxColumn.HeaderText = "CoreOverrights";
            this.coreOverrightsDataGridViewTextBoxColumn.Name = "coreOverrightsDataGridViewTextBoxColumn";
            this.coreOverrightsDataGridViewTextBoxColumn.ReadOnly = true;
            this.coreOverrightsDataGridViewTextBoxColumn.Width = 102;
            // 
            // dllCheckedDataGridViewCheckBoxColumn
            // 
            this.dllCheckedDataGridViewCheckBoxColumn.DataPropertyName = "DllChecked";
            this.dllCheckedDataGridViewCheckBoxColumn.HeaderText = "DllChecked";
            this.dllCheckedDataGridViewCheckBoxColumn.Name = "dllCheckedDataGridViewCheckBoxColumn";
            this.dllCheckedDataGridViewCheckBoxColumn.ReadOnly = true;
            this.dllCheckedDataGridViewCheckBoxColumn.Width = 68;
            // 
            // modEnabledDataGridViewCheckBoxColumn
            // 
            this.modEnabledDataGridViewCheckBoxColumn.DataPropertyName = "ModEnabled";
            this.modEnabledDataGridViewCheckBoxColumn.HeaderText = "ModEnabled";
            this.modEnabledDataGridViewCheckBoxColumn.Name = "modEnabledDataGridViewCheckBoxColumn";
            this.modEnabledDataGridViewCheckBoxColumn.ReadOnly = true;
            this.modEnabledDataGridViewCheckBoxColumn.Width = 73;
            // 
            // ResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.tabControl1);
            this.Name = "ResultsForm";
            this.Text = "Results";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xmlFilesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource modBindingSource;
        private System.Windows.Forms.BindingSource xmlFilesBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn modRankDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fullDirNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dirNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modXmlDetailsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn coreCheckedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn coreOverrightsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dllCheckedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn modEnabledDataGridViewCheckBoxColumn;
    }
}