using System;
using System.Collections.Generic;
using System.Globalization;

namespace DawgLib
{
    public class Dawg
    {
        // 保存上一个插入的词语
        private string _previousWord = "";

        // 存储已最小化的节点
        private readonly Dictionary<DawgNode, DawgNode> _minimizedNodes = new Dictionary<DawgNode, DawgNode>();

        // 尚未检查的节点
        private readonly Stack<Tuple<DawgNode, string, DawgNode>> _uncheckedNodes = new Stack<Tuple<DawgNode, string, DawgNode>>();

        // 根节点
        public DawgNode Root = new DawgNode();

        public void Insert(string[] words)
        {
            if (words.Length == 0) return;

            DawgNode node = Root;
            string previousWord = "";
            foreach (string word in words)
            {

                if (!node.FollowingWords.ContainsKey(word))
                {
                    var nextNode = new DawgNode();
                    node.FollowingWords[word] = nextNode;
                    node = nextNode;
                }
                else
                {
                    node = node.FollowingWords[word];
                }

                previousWord = word;
            }
            node.CanStop = true; // 标记词组结束
        }

        // 最小化节点
        private void Minimize(int downTo)
        {
            while (_uncheckedNodes.Count > downTo)
            {
                var (parent, word, child) = _uncheckedNodes.Pop();
                if (_minimizedNodes.TryGetValue(child, out var existing))
                {
                    parent.FollowingWords[word] = existing;
                }
                else
                {
                    _minimizedNodes[child] = child;
                }
            }
        }

        // 公共词语
        private int CommonPrefix(string[] words, string[] previousWords)
        {
            int maxLength = Math.Min(words.Length, previousWords.Length);
            for (int i = 0; i < maxLength; i++)
            {
                if (words[i] != previousWords[i])
                    return i;
            }
            return maxLength;
        }

        //搜索单词是否在DAWG中
        public bool Search(string word)
        {
            DawgNode node = Root;
            if (!node.FollowingWords.ContainsKey(word))
                return false;
            return node.FollowingWords[word].CanStop;
        }


        // 完成 DAWG 构建
        public void Finish()
        {
            Minimize(0);
        }


        // DAWG 节点类
        public class DawgNode
        {
            public bool CanStop; // 标记这是否可以是词组的结束
            public Dictionary<string, DawgNode> FollowingWords = new Dictionary<string, DawgNode>(); // 存储后续词语的节点

            public DawgNode() { }

            public override bool Equals(object obj)
            {
                if (obj is DawgNode other)
                {
                    return CanStop == other.CanStop && FollowingWords.SequenceEqual(other.FollowingWords);
                }
                return false;
            }

            public override int GetHashCode()
            {
                int hash = CanStop.GetHashCode();
                foreach (var node in FollowingWords)
                {
                    hash = HashCode.Combine(hash, node.Key.GetHashCode(), node.Value.GetHashCode());
                }
                return hash;
            }
        }

    }
}
