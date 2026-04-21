using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GitPreCommit
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// 接收一个参数：包含资源文件路径列表的临时文件路径。
        /// 每行格式为: [FBX]路径 或 [TEX]路径
        /// 返回 0 表示所有文件合格，返回 1 表示有不合格文件。
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            // 加载配置文件（从 exe 同目录下的 GitPreCommit.json）
            AppConfig.Load();

            if (args.Length < 1)
            {
                MessageBox.Show(
                    "用法: GitPreCommit.exe <资源文件列表路径>\n\n" +
                    "该程序由 git pre-commit hook 自动调用，\n" +
                    "请勿手动运行。",
                    "参数错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }

            string fileListPath = args[0];
            fileListPath = NormalizePath(fileListPath);

            if (!File.Exists(fileListPath))
            {
                MessageBox.Show(
                    string.Format("文件列表不存在: {0}", fileListPath),
                    "文件错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 1;
            }

            // 读取文件列表
            string[] lines = File.ReadAllLines(fileListPath);

            // 检测结果
            var errorList = new List<CheckResult>();

            foreach (string line in lines)
            {
                string trimmedLine = (line ?? "").Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;

                // 解析行格式: [FBX]路径 或 [TEX]路径
                if (trimmedLine.StartsWith("[FBX]"))
                {
                    string filePath = NormalizePath(trimmedLine.Substring(5));
                    CheckFbxFile(filePath, errorList);
                }
                else if (trimmedLine.StartsWith("[TEX]"))
                {
                    string filePath = NormalizePath(trimmedLine.Substring(5));
                    CheckTextureFile(filePath, errorList);
                }
            }

            if (errorList.Count > 0)
            {
                // 有不合格文件，弹出检测结果窗口
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new PreCommitForm(errorList));
                return 1; // 返回 1 表示检测不合格，阻止提交
            }

            return 0; // 返回 0 表示所有文件检测合格
        }

        /// <summary>
        /// 检测 FBX 文件面数
        /// </summary>
        private static void CheckFbxFile(string filePath, List<CheckResult> errorList)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                int polygonCount = FbxChecker.GetPolygonCount(filePath);
                if (polygonCount > AppConfig.MaxPolygonCount)
                {
                    errorList.Add(new CheckResult
                    {
                        CheckType = "FBX面数",
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        CurrentValue = string.Format("{0:N0} 面", polygonCount),
                        LimitValue = string.Format("{0:N0} 面", AppConfig.MaxPolygonCount)
                    });
                }
            }
            catch (Exception ex)
            {
                //errorList.Add(new CheckResult
                //{
                //    CheckType = "FBX面数",
                //    FilePath = filePath,
                //    FileName = Path.GetFileName(filePath),
                //    CurrentValue = "解析失败",
                //    LimitValue = string.Format("{0:N0} 面", MAX_POLYGON_COUNT),
                //    ErrorMessage = ex.Message
                //});
            }
        }

        /// <summary>
        /// 检测贴图文件尺寸
        /// </summary>
        private static void CheckTextureFile(string filePath, List<CheckResult> errorList)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                int width, height;
                TextureChecker.GetImageDimensions(filePath, out width, out height);

                // 宽或高任一超过阈值即为不合格
                if (width > AppConfig.MaxTextureSize || height > AppConfig.MaxTextureSize)
                {
                    errorList.Add(new CheckResult
                    {
                        CheckType = "贴图尺寸",
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        CurrentValue = string.Format("{0} x {1}", width, height),
                        LimitValue = string.Format("{0} x {0}", AppConfig.MaxTextureSize)
                    });
                }
            }
            catch (Exception ex)
            {
                //errorList.Add(new CheckResult
                //{
                //    CheckType = "贴图尺寸",
                //    FilePath = filePath,
                //    FileName = Path.GetFileName(filePath),
                //    CurrentValue = "解析失败",
                //    LimitValue = string.Format("{0} x {0}", MAX_TEXTURE_SIZE),
                //    ErrorMessage = ex.Message
                //});
            }
        }

        /// <summary>
        /// 将 MSYS/Unix 风格路径转为 Windows 路径
        /// 例如: /e/work/project -> E:\work\project
        /// </summary>
        private static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            path = path.Trim();
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // 处理 MSYS 路径格式: /e/work/... -> E:\work\...
            if (path.Length >= 3 && path[0] == '/' && char.IsLetter(path[1]) && path[2] == '/')
            {
                path = string.Format("{0}:{1}", char.ToUpper(path[1]), path.Substring(2));
            }

            return path.Replace('/', '\\');
        }
    }
}
