using System;
using System.Collections.Generic;

namespace DawgLib
{
    public class Dawg
    {
        // 上一个插入的词语
        private string _previousWord = "";

        // 已最小化的节点
        private readonly Dictionary<DawgNode, DawgNode> _minimizedNodes = new Dictionary<DawgNode, DawgNode>();

        // 尚未检查的节点
        private readonly Stack<Tuple<DawgNode, char, DawgNode>> _uncheckedNodes = new Stack<Tuple<DawgNode, char, DawgNode>>();

        // 根节点
        public DawgNode Root = new DawgNode();

        // 插入一个词语到 DAWG 中
        public void Insert(string word)
        {
            // 确保词语按字典顺序插入
            if (word.CompareTo(_previousWord) < 0)
                throw new ArgumentException("Words must be inserted in alphabetical order.");

            // 找到与上一个词语的公共前缀
            int commonPrefix = CommonPrefix(word);
            // 最小化节点
            Minimize(commonPrefix);

            // 从公共前缀的末尾开始插入新节点
            DawgNode node = _uncheckedNodes.Count == 0 ? Root : _uncheckedNodes.Peek().Item3;
            for (int i = commonPrefix; i < word.Length; i++)
            {
                var nextNode = new DawgNode();
                node.FollowingNodes[word[i]] = nextNode;
                _uncheckedNodes.Push(new Tuple<DawgNode, char, DawgNode>(node, word[i], nextNode));
                node = nextNode;
            }
            node.CanStop = true; // 标记词语结束
            _previousWord = word; // 更新上一个插入的词语
        }

        // 完成 DAWG 构建
        public void Finish()
        {
            Minimize(0);
        }

        // 最小化节点
        private void Minimize(int downTo)
        {
            while (_uncheckedNodes.Count > downTo)
            {
                var (parent, letter, child) = _uncheckedNodes.Pop();
                if (_minimizedNodes.TryGetValue(child, out var existing))
                {
                    parent.FollowingNodes[letter] = existing;
                }
                else
                {
                    _minimizedNodes[child] = child;
                }
            }
        }

        // 找到与上一个词语的公共前缀长度
        private int CommonPrefix(string word)
        {
            int maxLength = Math.Min(word.Length, _previousWord.Length);
            for (int i = 0; i < maxLength; i++)
            {
                if (word[i] != _previousWord[i])
                    return i;
            }
            return maxLength;
        }

        // DAWG 节点类
        public class DawgNode
        {
            public bool CanStop; // 是否可以作为一个词语的结束
            public Dictionary<char, DawgNode> FollowingNodes = new Dictionary<char, DawgNode>(); // 后续节点

            public DawgNode() { }

            // 重写 Equals 方法以便进行节点比较
            public override bool Equals(object obj)
            {
                if (obj is DawgNode other)
                    return CanStop == other.CanStop && FollowingNodes.SequenceEqual(other.FollowingNodes);
                return false;
            }

            // 重写 GetHashCode 方法以生成节点的哈希值
            public override int GetHashCode()
            {
                int hash = CanStop.GetHashCode();
                foreach (var node in FollowingNodes)
                    hash = hash * 31 + node.GetHashCode();
                return hash;
            }
        }
    }
}
