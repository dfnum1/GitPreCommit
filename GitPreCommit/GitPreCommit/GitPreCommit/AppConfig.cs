using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GitPreCommit
{
    /// <summary>
    /// 应用程序配置管理
    /// 从 exe 同目录下的 GitPreCommit.json 文件读取配置
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// FBX 面数上限阈值（默认4万面）
        /// </summary>
        public static int MaxPolygonCount { get; private set; } = 40000;

        /// <summary>
        /// 贴图尺寸上限（宽或高不得超过此值，默认4096）
        /// </summary>
        public static int MaxTextureSize { get; private set; } = 4096;

        /// <summary>
        /// 配置文件名
        /// </summary>
        private const string ConfigFileName = "GitPreCommit.json";

        /// <summary>
        /// 加载配置文件
        /// 如果配置文件不存在或解析失败，将使用默认值
        /// </summary>
        public static void Load()
        {
            string configPath = GetConfigPath();
            if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                return;

            try
            {
                string json = File.ReadAllText(configPath);
                ParseConfig(json);
            }
            catch
            {
                // 配置文件读取或解析失败，使用默认值
            }
        }

        /// <summary>
        /// 获取配置文件路径（与 exe 同目录）
        /// </summary>
        private static string GetConfigPath()
        {
            try
            {
                string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!string.IsNullOrEmpty(exeDir))
                    return Path.Combine(exeDir, ConfigFileName);
            }
            catch
            {
                // ignore
            }
            return null;
        }

        /// <summary>
        /// 简易 JSON 解析（不依赖第三方库）
        /// 仅支持扁平的 key-value 结构，值为整数
        /// </summary>
        private static void ParseConfig(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            int? polygonCount = TryParseIntValue(json, "MaxPolygonCount");
            if (polygonCount.HasValue && polygonCount.Value > 0)
                MaxPolygonCount = polygonCount.Value;

            int? textureSize = TryParseIntValue(json, "MaxTextureSize");
            if (textureSize.HasValue && textureSize.Value > 0)
                MaxTextureSize = textureSize.Value;
        }

        /// <summary>
        /// 从 JSON 字符串中提取指定 key 的整数值
        /// 匹配格式: "key": 12345 或 "key" : 12345
        /// </summary>
        private static int? TryParseIntValue(string json, string key)
        {
            // 正则匹配: "key" : 数字
            string pattern = string.Format("\"{0}\"\\s*:\\s*(\\d+)", Regex.Escape(key));
            Match match = Regex.Match(json, pattern);
            if (match.Success)
            {
                int value;
                if (int.TryParse(match.Groups[1].Value, out value))
                    return value;
            }
            return null;
        }
    }
}
