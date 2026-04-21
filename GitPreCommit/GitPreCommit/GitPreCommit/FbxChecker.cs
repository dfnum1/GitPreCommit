using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using FbxSharp;

namespace GitPreCommit
{
    /// <summary>
    /// FBX 文件面数检测工具
    /// 支持二进制 FBX 和 ASCII FBX 两种格式
    /// </summary>
    public static class FbxChecker
    {
        /// <summary>
        /// 获取 FBX 文件的总面数（多边形数量）
        /// 自动检测文件格式（二进制/ASCII），选择对应的解析方式
        /// </summary>
        /// <param name="filePath">FBX 文件完整路径</param>
        /// <returns>总面数（所有 Mesh 的多边形数量之和）</returns>
        public static int GetPolygonCount(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("FBX 文件不存在", filePath);

            // 读取文件头判断格式
            byte[] header = new byte[27];
            using (var fs = File.OpenRead(filePath))
            {
                if (fs.Length < 27)
                    throw new Exception("文件太小，不是有效的 FBX 文件");
                fs.Read(header, 0, 27);
            }

            string magic = Encoding.ASCII.GetString(header, 0, 20);
            if (magic.StartsWith("Kaydara FBX Binary"))
            {
                // 二进制 FBX 格式 - 使用自定义解析器
                return CountPolygonsBinary(filePath, header);
            }
            else
            {
                // ASCII FBX 格式 - 使用 FbxSharp 库
                return CountPolygonsAscii(filePath);
            }
        }

        #region 二进制 FBX 解析

        /// <summary>
        /// 解析二进制 FBX 文件，统计总面数
        /// 通过查找所有 "PolygonVertexIndex" 节点中的负数索引来计算多边形数量
        /// （FBX 中每个多边形的最后一个顶点索引以按位取反的方式存储为负数）
        /// </summary>
        private static int CountPolygonsBinary(string filePath, byte[] header)
        {
            byte[] fileData = File.ReadAllBytes(filePath);

            uint version = BitConverter.ToUInt32(header, 23);
            bool is64 = version >= 7500;

            int totalPolygons = 0;
            int offset = 27; // 跳过文件头

            ReadNodesRecursive(fileData, ref offset, fileData.Length, is64, ref totalPolygons);

            return totalPolygons;
        }

        /// <summary>
        /// 递归读取二进制 FBX 节点树
        /// </summary>
        private static void ReadNodesRecursive(byte[] data, ref int offset, long parentEnd, bool is64, ref int totalPolygons)
        {
            while (offset < parentEnd && offset < data.Length)
            {
                int nullSize = is64 ? 25 : 13;

                // 检查是否为空节点（哨兵，标识子节点列表结束）
                if (offset + nullSize > data.Length) return;

                bool isNull = true;
                for (int i = 0; i < nullSize; i++)
                {
                    if (data[offset + i] != 0) { isNull = false; break; }
                }
                if (isNull)
                {
                    offset += nullSize;
                    return;
                }

                // 读取节点头
                long endOffset, numProperties, propertyListLen;
                if (is64)
                {
                    if (offset + 25 > data.Length) return;
                    endOffset = (long)BitConverter.ToUInt64(data, offset); offset += 8;
                    numProperties = (long)BitConverter.ToUInt64(data, offset); offset += 8;
                    propertyListLen = (long)BitConverter.ToUInt64(data, offset); offset += 8;
                }
                else
                {
                    if (offset + 13 > data.Length) return;
                    endOffset = BitConverter.ToUInt32(data, offset); offset += 4;
                    numProperties = BitConverter.ToUInt32(data, offset); offset += 4;
                    propertyListLen = BitConverter.ToUInt32(data, offset); offset += 4;
                }

                if (endOffset == 0 || endOffset > data.Length) return;

                byte nameLen = data[offset]; offset += 1;
                string name = "";
                if (nameLen > 0)
                {
                    name = Encoding.ASCII.GetString(data, offset, nameLen);
                    offset += nameLen;
                }

                long propertiesEnd = offset + propertyListLen;

                // 如果是 PolygonVertexIndex 节点，解析其 int32 数组属性
                if (name == "PolygonVertexIndex" && numProperties > 0)
                {
                    try
                    {
                        int[] indices = ReadInt32ArrayProperty(data, offset);
                        if (indices != null)
                        {
                            // 统计负数索引的数量 = 多边形（面）的数量
                            // FBX 中每个多边形最后一个顶点索引为 -(index+1)
                            foreach (int idx in indices)
                            {
                                if (idx < 0) totalPolygons++;
                            }
                        }
                    }
                    catch
                    {
                        // 解析单个属性失败，继续处理其他节点
                    }
                }

                // 跳过属性数据区域
                offset = (int)propertiesEnd;

                // 递归读取子节点（如果有的话）
                if (offset < (int)endOffset)
                {
                    ReadNodesRecursive(data, ref offset, endOffset, is64, ref totalPolygons);
                }

                // 跳到当前节点末尾
                offset = (int)endOffset;
            }
        }

        /// <summary>
        /// 读取一个 int32 数组类型的 FBX 属性
        /// </summary>
        private static int[] ReadInt32ArrayProperty(byte[] data, int offset)
        {
            if (offset >= data.Length) return null;

            byte typeCode = data[offset]; offset += 1;

            if (typeCode == (byte)'i')
            {
                // int32 数组
                if (offset + 12 > data.Length) return null;

                uint arrayLen = BitConverter.ToUInt32(data, offset); offset += 4;
                uint encoding = BitConverter.ToUInt32(data, offset); offset += 4;
                uint compressedLen = BitConverter.ToUInt32(data, offset); offset += 4;

                if (arrayLen > 100000000) return null; // 安全检查：数组不超过1亿

                byte[] rawData;
                if (encoding == 0)
                {
                    // 未压缩的原始数据
                    int dataSize = (int)(arrayLen * 4);
                    if (offset + dataSize > data.Length) return null;
                    rawData = new byte[dataSize];
                    Array.Copy(data, offset, rawData, 0, dataSize);
                }
                else if (encoding == 1)
                {
                    // Zlib 压缩数据（跳过2字节 zlib 头）
                    if (offset + compressedLen > data.Length) return null;
                    rawData = DecompressZlib(data, offset, (int)compressedLen, (int)(arrayLen * 4));
                }
                else
                {
                    return null; // 未知编码
                }

                if (rawData == null) return null;

                int[] result = new int[arrayLen];
                for (uint i = 0; i < arrayLen; i++)
                {
                    result[i] = BitConverter.ToInt32(rawData, (int)(i * 4));
                }
                return result;
            }

            return null; // 非 int32 数组类型
        }

        /// <summary>
        /// 解压 Zlib 压缩的数据
        /// Zlib 格式 = 2字节头 + Deflate 数据 + 4字节校验
        /// </summary>
        private static byte[] DecompressZlib(byte[] data, int offset, int compressedLen, int decompressedLen)
        {
            try
            {
                if (compressedLen < 3) return null;

                byte[] result = new byte[decompressedLen];
                // 跳过2字节 zlib 头（CMF + FLG）
                using (var ms = new MemoryStream(data, offset + 2, compressedLen - 2))
                using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    int totalRead = 0;
                    while (totalRead < decompressedLen)
                    {
                        int read = ds.Read(result, totalRead, decompressedLen - totalRead);
                        if (read == 0) break;
                        totalRead += read;
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region ASCII FBX 解析（使用 FbxSharp 库）

        /// <summary>
        /// 使用 FbxSharp 库解析 ASCII 格式的 FBX 文件
        /// </summary>
        private static int CountPolygonsAscii(string filePath)
        {
            var importer = new Importer();
            Scene scene = importer.Import(filePath);

            if (scene == null)
            {
                throw new Exception(string.Format("无法导入 FBX 文件: {0}", filePath));
            }

            int totalPolygons = 0;
            Node rootNode = scene.GetRootNode();
            if (rootNode != null)
            {
                CountPolygonsFromNodeTree(rootNode, ref totalPolygons);
            }

            return totalPolygons;
        }

        /// <summary>
        /// 递归遍历 FbxSharp 的节点树，统计所有 Mesh 的多边形数量
        /// </summary>
        private static void CountPolygonsFromNodeTree(Node node, ref int totalPolygons)
        {
            if (node == null) return;

            for (int i = 0; i < node.GetNodeAttributeCount(); i++)
            {
                NodeAttribute attr = node.GetNodeAttributeByIndex(i);
                Mesh mesh = attr as Mesh;
                if (mesh != null)
                {
                    totalPolygons += mesh.PolygonIndexes.Count;
                }
            }

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                CountPolygonsFromNodeTree(node.GetChild(i), ref totalPolygons);
            }
        }

        #endregion
    }
}
