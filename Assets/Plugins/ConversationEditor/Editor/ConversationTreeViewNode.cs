using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BrightBit
{

public class ConversationTreeViewNode : TreeViewNode
{
    Texture2D icon = null;
    Node node      = null;

    public override Texture2D Icon { get { return icon;  } }

    public override Color LabelColor
    {
        get
        {
            if (IsLink)                 return ((ConversationTreeView) owner).LinkColor;
            else if (node is Narration) return ((ConversationTreeView) owner).NarrationColor;
            else if (node is Option)    return ((ConversationTreeView) owner).OptionColor;

            return EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }
    }

    public bool IsLink
    {
        get
        {
            ConversationTreeViewNode target = node.EditorData as ConversationTreeViewNode;

            if (target == null) throw new System.InvalidOperationException("The ConversationTreeViewNode instance isn't initialized properly yet!");

            return target != this;
        }
    }

    public ConversationTreeViewNode LinkTarget
    {
        get
        {
            return (ConversationTreeViewNode) node.EditorData;
        }
    }

    public override string DisplayName
    {
        get
        {
            ConversationTreeView treeView = (ConversationTreeView) owner;

            string prefix = treeView.HideNodeIDs ? string.Empty : ID.ToString() + ":";
            string result = GetText(treeView.DisplayLanguage);

            if (node is Option && string.IsNullOrEmpty(result))       result = "[" + prefix + "CONTINUE]";
            else if (node is Option && !string.IsNullOrEmpty(prefix)) result = "[" + prefix.Remove(prefix.Length - 1) + "] - " + result;
            else if (node is RootNode)                                result = "[" + prefix + "ROOT]";
            else if (node is Narration)
            {
                string tag = ((Narration) node).Tag;

                result = string.IsNullOrEmpty(tag) ? "[" + prefix + "NARRATION] - " + result : "[" + prefix + tag + "] - " + result;
            }

            return result;
        }
    }

    public Node Node
    {
        get
        {
            return node;
        }
    }

    public bool IsRoot      { get { return (node is RootNode);  } }
    public bool IsOption    { get { return (node is Option);    } }
    public bool IsNarration { get { return (node is Narration); } }

    public string Tag
    {
        get
        {
            Narration n = node as Narration;

            if (n != null) return n.Tag;
            else           return string.Empty;
        }

        set
        {
            Narration n = node as Narration;

            if (n != null) n.Tag = value;
        }
    }

    public string Comments
    {
        get
        {
            ConversationNode eNode = node as ConversationNode;

            if (eNode != null) return eNode.Comments;
            else               return string.Empty;
        }

        set
        {
            ConversationNode eNode = node as ConversationNode;

            if (eNode != null) eNode.Comments = value;
        }
    }

    public string GetText(Conversation.Language language)
    {
        ConversationNode en = node as ConversationNode;

        return en != null ? en.GetText(language) : string.Empty;
    }

    public void SetText(Conversation.Language language, string text)
    {
        ConversationNode en = node as ConversationNode;

        if (en != null) en.SetText(language, text);
    }

    public ConversationTreeViewNode(ConversationTreeView owner, Node node, int id, bool isLink) : base(owner, id)
    {
        this.node = node;

        if (node is Narration)   icon = AssetDatabase.LoadAssetAtPath("Assets/Plugins/ConversationEditor/Images/Narration.png", typeof(Texture2D)) as Texture2D;
        else if (node is Option) icon = AssetDatabase.LoadAssetAtPath("Assets/Plugins/ConversationEditor/Images/Option.png", typeof(Texture2D)) as Texture2D;

        if (!isLink) node.EditorData = this;
    }

    public void RemoveLink(ConversationTreeViewNode child)
    {
        for (int i = 0; i < NumChildren; ++i)
        {
            ConversationTreeViewNode current = (ConversationTreeViewNode) Children[i];

            if (current.IsLink && current.Node == child.Node)
            {
                RemoveChild(current);
                return;
            }
        }
    }

    public bool CanMoveUp()
    {
        ConversationTreeViewNode parent = (ConversationTreeViewNode) Parent;

        if (parent == null) return false;

        List<TreeViewNode> children = parent.Children;

        return children.IndexOf(this) > 0;
    }

    public bool CanMoveDown()
    {
        ConversationTreeViewNode parent = (ConversationTreeViewNode) Parent;

        if (parent == null) return false;

        List<TreeViewNode> children = parent.Children;

        return children.IndexOf(this) < (children.Count - 1);
    }

    public void MoveUp()
    {
        if (CanMoveUp()) Swap(-1);
        else throw new System.InvalidOperationException("The node can't be moved up!");
    }

    public void MoveDown()
    {
        if (CanMoveDown()) Swap(+1);
        else throw new System.InvalidOperationException("The node can't be moved down!");
    }

    void Swap(int offset)
    {
        ConversationTreeViewNode parent = (ConversationTreeViewNode) Parent;
        List<TreeViewNode> children     = parent.Children;

        int index = children.IndexOf(this);

        children.Swap(index, index + offset);

        parent.Node.SwapChildren(index, index + offset);
    }
}

} // of namespace BrightBit
