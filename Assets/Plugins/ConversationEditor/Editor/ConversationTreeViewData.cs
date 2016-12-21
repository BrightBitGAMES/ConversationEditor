using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BrightBit
{

public class ConversationTreeViewData : TreeViewData
{
    Conversation content;

    int counterID = 0;

    public void CreateCopyAndAdd(ConversationTreeViewNode node, ConversationTreeViewNode parent)
    {
        if (node.Node is RootNode) throw new System.Exception("RootNodes must not be copied!");

        if (node.IsLink)
        {
            CreateLink(parent, node);
            ForceUpdate();
            return;
        }

        Stack<ConversationTreeViewNode> treeViewNodes = new Stack<ConversationTreeViewNode>();
        Stack<ConversationNode> dialogueNodes   = new Stack<ConversationNode>();

        ConversationNode copy = ConversationNode.CreateDeepCopy(node.Node as ConversationNode);

        ConversationTreeView conversationTreeView = (ConversationTreeView) observer;

        ConversationTreeViewNode result = new ConversationTreeViewNode(conversationTreeView, copy, CreateID(), false);

        parent.AddChild(result);
        parent.Node.AddChild(copy);

        treeViewNodes.Push(result);
        dialogueNodes.Push(copy);

        while (treeViewNodes.Count > 0)
        {
            ConversationTreeViewNode currentTreeViewNode   = treeViewNodes.Pop();
            ConversationNode currentConversationNode = dialogueNodes.Pop();

            for (int i = 0; i < currentConversationNode.NumChildren(); ++i)
            {
                bool isLink = currentConversationNode.IsLink(i);

                ConversationNode childConversationNode = currentConversationNode.GetChild(i) as ConversationNode;
                ConversationTreeViewNode childTreeViewNode   = new ConversationTreeViewNode(conversationTreeView, childConversationNode, CreateID(), isLink);

                currentTreeViewNode.AddChild(childTreeViewNode);

                if (!isLink)
                {
                    dialogueNodes.Push(childConversationNode);
                    treeViewNodes.Push(childTreeViewNode);
                }
            }

        }
    }

    public ConversationTreeViewNode OnCutNode(ConversationTreeViewNode current)
    {
        ConversationTreeViewNode result = null;

        List<ConversationNode> nodesUsedAsLink = FindNodesUsedAsLink(current);

        bool hasNodesUsedAsLink = nodesUsedAsLink.Count > 0;

        if (!hasNodesUsedAsLink || EditorUtility.DisplayDialog("Confirmation", "Removing this element will destroy all links associated with it. Continue?", "Yes", "No"))
        {
            ConversationTreeViewNode parent = current.Parent as ConversationTreeViewNode;
            Node nodeParent                 = parent.Node;

            foreach (ConversationNode node in nodesUsedAsLink)
            {
                List<ConversationNode> parents = new List<ConversationNode>(node.LinkParents);

                foreach (ConversationNode linkParent in parents)
                {
                    ConversationTreeViewNode treeNode       = (ConversationTreeViewNode) node.EditorData;
                    ConversationTreeViewNode treeNodeParent = (ConversationTreeViewNode) linkParent.EditorData;

                    linkParent.RemoveLink(node);
                    treeNodeParent.RemoveLink(treeNode);
                }
            }

            parent.RemoveChild(current);

            if (current.IsLink)
            {
                ((ConversationNode) nodeParent).RemoveLink((ConversationNode) current.Node);
                result = current;
            }
            else
            {
                nodeParent.RemoveChild(current.Node);
                result = current;
            }

            ForceUpdate();
        }

        return result;
    }

    /// <summary>
    /// Finds all nodes that are parents for links to one of the nodes within the argument's hierarchy 
    /// </summary>
    List<ConversationNode> FindNodesUsedAsLink(ConversationTreeViewNode node)
    {
        List<ConversationNode> nodesUsedAsLink = new List<ConversationNode>();

        if (node.IsLink) return nodesUsedAsLink;

        Stack<ConversationTreeViewNode> nodesToVisit = new Stack<ConversationTreeViewNode>();

        nodesToVisit.Push(node);

        while (nodesToVisit.Count > 0)
        {
            ConversationTreeViewNode current = nodesToVisit.Pop();

            if (!current.IsLink)
            {
                if (current.HasChildren)
                {
                    foreach (TreeViewNode c in current.Children)
                        nodesToVisit.Push((ConversationTreeViewNode) c);
                }

                ConversationNode conversationNode = (ConversationNode) current.Node;

                if (conversationNode.IsUsedAsLink)
                {
                    nodesUsedAsLink.Add(conversationNode);
                }
            }
        }

        return nodesUsedAsLink;
    }

    public ConversationTreeViewNode CreateLink(ConversationTreeViewNode parent, ConversationTreeViewNode target)
    {
        ConversationTreeViewNode treeViewNode = new ConversationTreeViewNode((ConversationTreeView) observer, target.Node, CreateID(), true);

        ConversationNode parentNode = parent.Node as ConversationNode;
        ConversationNode targetNode = target.Node as ConversationNode;

        parent.AddChild(treeViewNode);
        parentNode.AddLink(targetNode);

        ForceUpdate();

        return treeViewNode;
    }

    public ConversationTreeViewData(ConversationTreeView treeView, Conversation content) : base(treeView)
    {
        this.content = content;

        Load();
    }

    public int CreateID()
    {
        return counterID++;
    }

    public override void Load()
    {
        if (content == null || content.Root == null) return;

        Stack<Node> dialogueNodes = new Stack<Node>();
        Stack<ConversationTreeViewNode> treeViewNodes = new Stack<ConversationTreeViewNode>();

        Node currentConversationNode = null;
        ConversationTreeViewNode currentTreeViewNode = null;

        dialogueNodes.Push(content.Root);

        ConversationTreeView conversationTreeView = (ConversationTreeView) observer;

        Root = new ConversationTreeViewNode(conversationTreeView, content.Root, CreateID(), false);

        treeViewNodes.Push(Root as ConversationTreeViewNode);

        while (dialogueNodes.Count > 0)
        {
            currentConversationNode = dialogueNodes.Pop();
            currentTreeViewNode     = treeViewNodes.Pop();

            for (int i = 0; i < currentConversationNode.NumChildren(); ++i)
            {
                ConversationNode currentExtended = currentConversationNode as ConversationNode;

                bool isLink = currentExtended != null ? currentExtended.IsLink(i) : false;

                ConversationNode childConversationNode = currentConversationNode.GetChild(i) as ConversationNode;
                ConversationTreeViewNode childTreeViewNode   = new ConversationTreeViewNode(conversationTreeView, childConversationNode, CreateID(), isLink);

                currentTreeViewNode.AddChild(childTreeViewNode);

                if (!isLink)
                {
                    dialogueNodes.Push(childConversationNode);
                    treeViewNodes.Push(childTreeViewNode);
                }
            }
        }

        ForceUpdate();
    }
}

} // of namespace BrightBit
