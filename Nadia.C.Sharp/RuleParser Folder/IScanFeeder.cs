using System;
using Nadia.C.Sharp.NodeFolder;

namespace Nadia.C.Sharp.RuleParserFolder
{
    public interface IScanFeeder
    {
         void HandleParent(string parentText, int lineNumber);
         void HandleChild(string parentText, string childText, string firstKeywordsGroup, int lineNumber);
        //   void HandleNeedWant(string parentText, string childText, int lineNumber);
         void HandleListItem(string parentText, string itemText, MetaType? metaTyp);
        //   void HandleIterateCheck(string iterateParent, string parentText, string checkText, int lineNumber);
         string HandleWarning(string parentText);
         NodeSet GetNodeSet();
         void SetNodeSet(NodeSet ns);
         int[,] CreateDependencyMatrix();
    }
}
