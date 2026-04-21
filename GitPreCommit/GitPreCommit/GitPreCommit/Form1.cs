using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GitPreCommit
{
    public partial class PreCommitForm : Form
    {
        private readonly List<CheckResult> _errorList;

        /// <summary>
        /// 带错误列表的构造函数（由 Program.Main 调用）
        /// </summary>
        public PreCommitForm(List<CheckResult> errorList)
        {
            _errorList = errorList ?? new List<CheckResult>();
            InitializeComponent();
            LoadErrors();
        }

        /// <summary>
        /// 无参构造函数（设计器兼容）
        /// </summary>
        public PreCommitForm()
        {
            _errorList = new List<CheckResult>();
            InitializeComponent();
        }

        /// <summary>
        /// 将错误列表加载到 DataGridView 中
        /// </summary>
        private void LoadErrors()
        {
            dgvErrors.Rows.Clear();

            foreach (var error in _errorList)
            {
                int rowIndex = dgvErrors.Rows.Add();
                var row = dgvErrors.Rows[rowIndex];

                row.Cells["colCheckType"].Value = error.CheckType;
                row.Cells["colFileName"].Value = error.FileName;
                row.Cells["colFilePath"].Value = error.FilePath;
                row.Cells["colCurrentValue"].Value = error.CurrentValue;
                row.Cells["colLimitValue"].Value = error.LimitValue;
                row.Cells["colStatus"].Value = error.StatusText;

                // 超标的行用红色标记
                row.DefaultCellStyle.ForeColor = Color.Red;

                // 将文件路径存入 Tag，方便按钮点击时使用
                row.Tag = error.FilePath;
            }

            // 更新标题文字
            lblTitle.Text = string.Format(
                "以下 {0} 个资源文件不符合规范，提交已被阻止：",
                _errorList.Count);
        }

        /// <summary>
        /// DataGridView 单元格点击事件 - 处理「定位」按钮
        /// </summary>
        private void dgvErrors_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 确保点击的是「定位」按钮列，且不是表头
            if (e.RowIndex < 0) return;
            if (dgvErrors.Columns[e.ColumnIndex].Name != "colLocate") return;

            var row = dgvErrors.Rows[e.RowIndex];
            string filePath = row.Tag as string;

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        // 在资源管理器中选中并定位该文件
                        Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
                    }
                    else
                    {
                        // 文件不存在，尝试打开所在目录
                        string dir = Path.GetDirectoryName(filePath);
                        if (Directory.Exists(dir))
                        {
                            Process.Start("explorer.exe", string.Format("\"{0}\"", dir));
                        }
                        else
                        {
                            MessageBox.Show(
                                string.Format("路径不存在: {0}", filePath),
                                "提示",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format("打开目录失败: {0}", ex.Message),
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
