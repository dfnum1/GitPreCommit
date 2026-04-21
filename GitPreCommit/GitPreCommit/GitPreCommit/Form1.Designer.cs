namespace GitPreCommit
{
    partial class PreCommitForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.colCheckType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLocate = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colCurrentValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLimitValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblIcon = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIcon
            // 
            this.lblIcon.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIcon.ForeColor = System.Drawing.Color.Red;
            this.lblIcon.Location = new System.Drawing.Point(12, 12);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(40, 40);
            this.lblIcon.TabIndex = 3;
            this.lblIcon.Text = "⚠";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(55, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(933, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "以下资源文件不符合规范，提交已被阻止：";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvErrors
            // 
            this.dgvErrors.AllowUserToAddRows = false;
            this.dgvErrors.AllowUserToDeleteRows = false;
            this.dgvErrors.AllowUserToResizeRows = false;
            this.dgvErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvErrors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvErrors.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvErrors.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheckType,
            this.colFileName,
            this.colFilePath,
            this.colLocate,
            this.colCurrentValue,
            this.colLimitValue,
            this.colStatus});
            this.dgvErrors.Location = new System.Drawing.Point(12, 60);
            this.dgvErrors.Name = "dgvErrors";
            this.dgvErrors.ReadOnly = true;
            this.dgvErrors.RowHeadersVisible = false;
            this.dgvErrors.RowTemplate.Height = 28;
            this.dgvErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvErrors.Size = new System.Drawing.Size(976, 330);
            this.dgvErrors.TabIndex = 1;
            this.dgvErrors.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvErrors_CellContentClick);
            // 
            // colCheckType
            // 
            this.colCheckType.FillWeight = 50F;
            this.colCheckType.HeaderText = "检测类型";
            this.colCheckType.Name = "colCheckType";
            this.colCheckType.ReadOnly = true;
            // 
            // colFileName
            // 
            this.colFileName.FillWeight = 70F;
            this.colFileName.HeaderText = "文件名";
            this.colFileName.Name = "colFileName";
            this.colFileName.ReadOnly = true;
            // 
            // colFilePath
            // 
            this.colFilePath.FillWeight = 120F;
            this.colFilePath.HeaderText = "文件路径";
            this.colFilePath.Name = "colFilePath";
            this.colFilePath.ReadOnly = true;
            // 
            // colLocate
            // 
            this.colLocate.FillWeight = 35F;
            this.colLocate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.colLocate.HeaderText = "操作";
            this.colLocate.Name = "colLocate";
            this.colLocate.ReadOnly = true;
            this.colLocate.Text = "定位";
            this.colLocate.ToolTipText = "在资源管理器中定位该文件";
            this.colLocate.UseColumnTextForButtonValue = true;
            // 
            // colCurrentValue
            // 
            this.colCurrentValue.FillWeight = 55F;
            this.colCurrentValue.HeaderText = "当前值";
            this.colCurrentValue.Name = "colCurrentValue";
            this.colCurrentValue.ReadOnly = true;
            // 
            // colLimitValue
            // 
            this.colLimitValue.FillWeight = 55F;
            this.colLimitValue.HeaderText = "限制值";
            this.colLimitValue.Name = "colLimitValue";
            this.colLimitValue.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.FillWeight = 90F;
            this.colStatus.HeaderText = "状态说明";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(888, 400);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 35);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // PreCommitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 450);
            this.Controls.Add(this.lblIcon);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dgvErrors);
            this.Controls.Add(this.btnOK);
            this.MinimumSize = new System.Drawing.Size(750, 350);
            this.Name = "PreCommitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "资源检测结果 - 提交被阻止";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgvErrors;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCheckType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFilePath;
        private System.Windows.Forms.DataGridViewButtonColumn colLocate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrentValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLimitValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Button btnOK;
    }
}
