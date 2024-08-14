using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JiebaNet.Segmenter;

// 1890ms
namespace TextAnalysis
{
    class Program
    {
        static void Main()
        {
            // 注册编码提供程序以支持 GB2312 编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 定义输入和输出文件的路径
            string textPath = @"D:\新词与语义挖掘\中文语料处理\内科.txt";
            string outputPath = @"C:\Users\86139\Desktop\词语共现关联.csv";

            // 开始计时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 读取文本内容
            string text = File.ReadAllText(textPath, Encoding.GetEncoding("GB2312"));

            // 分词处理
            var segmenter = new JiebaSegmenter();
            var words = segmenter.Cut(text).ToList();

            // 过滤掉非汉字的词语
            words = FilterWords(words);

            // 计算词语共现频率（假设最多考虑5个词以内的组合）
            var frequencies = CalculateFrequencies(words, 5);

            // 将结果写入 CSV 文件
            WriteFrequenciesToCsv(outputPath, frequencies);

            // 停止计时并输出执行时间
            stopwatch.Stop();
            Console.WriteLine($"分析完成。输出已写入 CSV 文件。");
            Console.WriteLine($"执行时间: {stopwatch.ElapsedMilliseconds} 毫秒");
        }

        // 过滤非汉字的词语
        static List<string> FilterWords(List<string> words)
        {
            var filteredWords = new List<string>();

            foreach (var word in words)
            {
                bool isValid = true;
                foreach (char ch in word)
                {
                    if (!(ch >= 0x4e00 && ch <= 0x9fff)) // 检查是否是汉字
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid && !string.IsNullOrWhiteSpace(word)) // 如果全是汉字，且非空白
                {
                    filteredWords.Add(word);
                }
            }

            return filteredWords;
        }

        // 计算词语共现频率
        static Dictionary<string, int> CalculateFrequencies(List<string> words, int maxLength)
        {
            var frequencies = new Dictionary<string, int>();

            for (int i = 0; i < words.Count; i++)
            {
                for (int j = i + 1; j < i + maxLength && j < words.Count; j++)
                {
                    string pair = $"{words[i]} {words[j]}";

                    if (frequencies.ContainsKey(pair))
                    {
                        frequencies[pair]++;
                    }
                    else
                    {
                        frequencies[pair] = 1;
                    }
                }
            }

            return frequencies;
        }

        // 将频率写入 CSV 文件
        static void WriteFrequenciesToCsv(string filePath, Dictionary<string, int> frequencies)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("词语组合,频率");

                foreach (var pair in frequencies.OrderByDescending(p => p.Value))
                {
                    writer.WriteLine($"{pair.Key},{pair.Value}");
                }
            }
        }
    }
}
