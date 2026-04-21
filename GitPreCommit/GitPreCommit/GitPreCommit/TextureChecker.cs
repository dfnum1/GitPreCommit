using System;
using System.Drawing;
using System.IO;

namespace GitPreCommit
{
    /// <summary>
    /// 贴图尺寸检测工具
    /// 支持 PNG, JPG, JPEG, BMP, TIF, TIFF（通过 System.Drawing）
    /// 以及 TGA, PSD, EXR（通过自定义头部解析）
    /// </summary>
    public static class TextureChecker
    {
        /// <summary>
        /// 获取贴图的宽度和高度
        /// </summary>
        /// <param name="filePath">贴图文件路径</param>
        /// <param name="width">输出：宽度</param>
        /// <param name="height">输出：高度</param>
        public static void GetImageDimensions(string filePath, out int width, out int height)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("贴图文件不存在", filePath);

            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            switch (ext)
            {
                case ".tga":
                    ReadTgaDimensions(filePath, out width, out height);
                    break;
                case ".psd":
                    ReadPsdDimensions(filePath, out width, out height);
                    break;
                case ".exr":
                    ReadExrDimensions(filePath, out width, out height);
                    break;
                default:
                    // PNG, JPG, JPEG, BMP, TIF, TIFF, GIF 等 - 使用 System.Drawing
                    ReadImageDimensions(filePath, out width, out height);
                    break;
            }
        }

        /// <summary>
        /// 使用 System.Drawing 读取图片尺寸
        /// 支持 PNG, JPG, BMP, TIFF, GIF 等常见格式
        /// </summary>
        private static void ReadImageDimensions(string filePath, out int width, out int height)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var img = Image.FromStream(fs, false, false))
            {
                width = img.Width;
                height = img.Height;
            }
        }

        /// <summary>
        /// 读取 TGA 文件头获取尺寸
        /// TGA Header:
        ///   Byte 0:     ID Length
        ///   Byte 1:     Color Map Type
        ///   Byte 2:     Image Type
        ///   Bytes 3-7:  Color Map Spec
        ///   Bytes 8-9:  X Origin (uint16 LE)
        ///   Bytes 10-11: Y Origin (uint16 LE)
        ///   Bytes 12-13: Width (uint16 LE)
        ///   Bytes 14-15: Height (uint16 LE)
        /// </summary>
        private static void ReadTgaDimensions(string filePath, out int width, out int height)
        {
            byte[] header = new byte[18];
            using (var fs = File.OpenRead(filePath))
            {
                if (fs.Length < 18)
                    throw new Exception("文件太小，不是有效的 TGA 文件");
                fs.Read(header, 0, 18);
            }

            width = BitConverter.ToUInt16(header, 12);
            height = BitConverter.ToUInt16(header, 14);

            if (width == 0 || height == 0)
                throw new Exception("无效的 TGA 尺寸");
        }

        /// <summary>
        /// 读取 EXR 文件头获取尺寸
        /// OpenEXR Header:
        ///   Bytes 0-3:   Magic number (0x762f3101)
        ///   Bytes 4-7:   Version + flags
        ///   然后是 name\0 type\0 size(int32LE) value 的属性序列
        ///   查找 "dataWindow" 属性 (box2i)，包含 xMin, yMin, xMax, yMax (int32 LE)
        ///   Width  = xMax - xMin + 1
        ///   Height = yMax - yMin + 1
        /// </summary>
        private static void ReadExrDimensions(string filePath, out int width, out int height)
        {
            width = 0;
            height = 0;

            using (var fs = File.OpenRead(filePath))
            using (var br = new BinaryReader(fs))
            {
                if (fs.Length < 8)
                    throw new Exception("文件太小，不是有效的 EXR 文件");

                // 验证 Magic number: 0x762f3101
                uint magic = br.ReadUInt32();
                if (magic != 0x01312f76)
                    throw new Exception("不是有效的 EXR 文件（Magic number 不匹配）");

                // 跳过 version + flags
                br.ReadUInt32();

                // 遍历 header 属性
                while (fs.Position < fs.Length)
                {
                    // 读取属性名（以 \0 结尾）
                    string attrName = ReadNullTerminatedString(br);
                    if (string.IsNullOrEmpty(attrName))
                        break; // 空名表示 header 结束

                    // 读取类型名（以 \0 结尾）
                    string attrType = ReadNullTerminatedString(br);

                    // 读取属性值大小
                    int attrSize = br.ReadInt32();

                    if (attrName == "dataWindow" && attrType == "box2i" && attrSize == 16)
                    {
                        int xMin = br.ReadInt32();
                        int yMin = br.ReadInt32();
                        int xMax = br.ReadInt32();
                        int yMax = br.ReadInt32();
                        width = xMax - xMin + 1;
                        height = yMax - yMin + 1;
                        return;
                    }
                    else
                    {
                        // 跳过不关心的属性值
                        if (attrSize > 0)
                            fs.Seek(attrSize, SeekOrigin.Current);
                    }
                }
            }

            if (width <= 0 || height <= 0)
                throw new Exception("无法从 EXR 文件中读取有效尺寸");
        }

        /// <summary>
        /// 从 BinaryReader 读取以 \0 结尾的字符串
        /// </summary>
        private static string ReadNullTerminatedString(BinaryReader br)
        {
            var bytes = new System.Collections.Generic.List<byte>();
            while (true)
            {
                byte b = br.ReadByte();
                if (b == 0) break;
                bytes.Add(b);
            }
            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }

        /// <summary>
        /// 读取 PSD 文件头获取尺寸
        /// PSD Header:
        ///   Bytes 0-3:   Signature "8BPS"
        ///   Bytes 4-5:   Version (1 or 2)
        ///   Bytes 6-11:  Reserved
        ///   Bytes 12-13: Channels
        ///   Bytes 14-17: Height (uint32 BE)
        ///   Bytes 18-21: Width (uint32 BE)
        /// </summary>
        private static void ReadPsdDimensions(string filePath, out int width, out int height)
        {
            byte[] header = new byte[22];
            using (var fs = File.OpenRead(filePath))
            {
                if (fs.Length < 22)
                    throw new Exception("文件太小，不是有效的 PSD 文件");
                fs.Read(header, 0, 22);
            }

            // 验证 PSD 签名
            string sig = System.Text.Encoding.ASCII.GetString(header, 0, 4);
            if (sig != "8BPS")
                throw new Exception("不是有效的 PSD 文件（签名不匹配）");

            // PSD 使用大端序
            height = (header[14] << 24) | (header[15] << 16) | (header[16] << 8) | header[17];
            width = (header[18] << 24) | (header[19] << 16) | (header[20] << 8) | header[21];

            if (width <= 0 || height <= 0)
                throw new Exception("无效的 PSD 尺寸");
        }
    }
}
