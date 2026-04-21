namespace GitPreCommit
{
    /// <summary>
    /// 资源文件检测结果（通用，适用于 FBX 面数检测和贴图尺寸检测）
    /// </summary>
    public class CheckResult
    {
        /// <summary>检测类型（"FBX面数" 或 "贴图尺寸"）</summary>
        public string CheckType { get; set; }

        /// <summary>文件完整路径</summary>
        public string FilePath { get; set; }

        /// <summary>文件名</summary>
        public string FileName { get; set; }

        /// <summary>当前值描述（如 "50,000 面" 或 "8192 x 4096"）</summary>
        public string CurrentValue { get; set; }

        /// <summary>限制值描述（如 "40,000 面" 或 "4096 x 4096"）</summary>
        public string LimitValue { get; set; }

        /// <summary>错误信息（解析失败时使用）</summary>
        public string ErrorMessage { get; set; }

        /// <summary>状态描述文本</summary>
        public string StatusText
        {
            get
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                    return string.Format("解析错误: {0}", ErrorMessage);
                return string.Format("{0} 超过限制 {1}", CurrentValue, LimitValue);
            }
        }
    }
}
