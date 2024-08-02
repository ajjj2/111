using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DawgLib;

namespace TextAnalysis
{
    class Program
    {
        static void Main()
        {
            // 注册编码提供程序以支持 GB2312 编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 定义输入和输出文件的路径
            string termsPath = @"C:\Users\86139\Desktop\词语总汇.csv";
            string textPath = @"C:\Users\86139\Desktop\内科.txt";
            string outputPath = @"C:\Users\86139\Desktop\词频统计.csv";

            // 开始计时以测量执行时间
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 从词汇文件中提取词语
            var terms = ExtractTerms(termsPath);

            // 创建并构建 DAWG（Directed Acyclic Word Graph）
            var dawg = new Dawg();
            foreach (var term in terms.OrderBy(x => x))
            {
                dawg.Insert(term);
            }
            dawg.Finish();

            // 计算文本文件中的词语频率
            var frequencies = CalculateFrequencies(textPath, dawg);

            // 将频率写入 CSV 文件
            WriteFrequenciesToCsv(outputPath, frequencies);

            // 停止计时并输出执行时间
            stopwatch.Stop();
            Console.WriteLine($"分析完成。输出已写入 CSV 文件。");
            Console.WriteLine($"执行时间: {stopwatch.ElapsedMilliseconds} 毫秒");
        }

        // 从文件中提取术语
        static HashSet<string> ExtractTerms(string filePath)
        {
            var terms = new HashSet<string>();
            using (var reader = new StreamReader(filePath, Encoding.GetEncoding("GB2312")))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        terms.Add(line.Trim());
                    }
                }
            }
            Console.WriteLine($"从文件中提取了 {terms.Count} 个词语。");
            return terms;
        }

        // 计算文本文件中的词语频率
        static Dictionary<string, int> CalculateFrequencies(string filePath, Dawg dawg)
        {
            var frequencies = new Dictionary<string, int>();
            string text = File.ReadAllText(filePath, Encoding.GetEncoding("GB2312"));

            Console.WriteLine("开始使用 DAWG 计算频率。");

            for (int i = 0; i < text.Length; i++)
            {
                var node = dawg.Root;
                for (int j = i; j < text.Length; j++)
                {
                    if (!node.FollowingNodes.ContainsKey(text[j]))
                        break;
                    node = node.FollowingNodes[text[j]];
                    if (node.CanStop)
                    {
                        string term = text.Substring(i, j - i + 1);
                        if (frequencies.ContainsKey(term))
                            frequencies[term]++;
                        else
                            frequencies[term] = 1;
                    }
                }
            }

            Console.WriteLine("完成频率计算。");
            return frequencies;
        }

        // 将频率写入 CSV 文件
        static void WriteFrequenciesToCsv(string filePath, Dictionary<string, int> frequencies)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("词语,频率");
                foreach (var pair in frequencies.OrderByDescending(p => p.Value))
                {
                    writer.WriteLine($"{pair.Key},{pair.Value}");
                }
            }
        }
    }
}
