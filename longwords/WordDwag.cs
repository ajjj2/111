using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DawgLib.Dawg;

namespace longword
{
    public class WordDwag
    {
        public Dictionary<string, DawgNode> FollowingNodes = new Dictionary<string, DawgNode>(); // 后续节点
    }
}
