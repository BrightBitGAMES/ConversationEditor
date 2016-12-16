using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightBit
{

public abstract class TreeViewData
{
    public Action OnExpandedStateChanged { get; set;           }
    public TreeViewNode Root             { get; protected set; }

    protected bool someNodeChanged                       = true;
    protected List<TreeViewNode> expandedRowsAndChildren = null;

    protected TreeView observer;

    public TreeViewData(TreeView treeView)
    {
        observer = treeView;
    }

    public abstract void Load();

    public void ForceUpdate()
    {
        someNodeChanged = true;

        InitialiseExpandedRowsAndTheirChildren();
    }

    public virtual TreeViewNode FindNode(int nodeID)
    {
        if (Root == null) return null;

        Stack<TreeViewNode> stack = new Stack<TreeViewNode>();

        stack.Push(Root);

        while (stack.Count > 0)
        {
            TreeViewNode node = stack.Pop();

            if (node.ID == nodeID) return node;

            if (node.HasChildren)
            {
                foreach (TreeViewNode child in node.Children)
                {
                    stack.Push(child);
                }
            }
        }

        return null;
    }

    public virtual bool IsHighlighted(int nodeID)
    {
        List<TreeViewNode> rows = ExpandedRowsAndTheirChildren;

        return rows.Find(node => node.ID == nodeID) != null;
    }

    public virtual void Highlight(int nodeID)
    {
        if (IsHighlighted(nodeID)) return;

        TreeViewNode treeViewNode = FindNode(nodeID);

        if (treeViewNode != null)
        {
            TreeViewNode node = treeViewNode.Parent;

            while (node != null)
            {
                SetExpanded(node, true);

                node = node.Parent;
            }
        }
    }

    protected List<TreeViewNode> CollectExpandedNodesAndChildren()
    {
        List<TreeViewNode> result = new List<TreeViewNode>();
        Stack<TreeViewNode> stack = new Stack<TreeViewNode>();

        stack.Push(Root);

        while (stack.Count > 0)
        {
            TreeViewNode node = stack.Pop();

            if (node.HasChildren && IsExpanded(node))
            {
                foreach (TreeViewNode child in Enumerable.Reverse(node.Children))
                {
                    stack.Push(child);
                }
            }

            result.Add(node);
        }

        return result;
    }

    public virtual int GetIndexOfExpandedRowOrItsChild(int nodeID)
    {
        List<TreeViewNode> rows = ExpandedRowsAndTheirChildren;

        for (int index = 0; index < rows.Count; ++index)
        {
            if (rows[index].ID == nodeID) return index;
        }

        return -1;
    }

    public virtual TreeViewNode GetRow(int row)
    {
        return ExpandedRowsAndTheirChildren[row];
    }

    public virtual List<TreeViewNode> ExpandedRowsAndTheirChildren
    {
        get
        {
            InitialiseExpandedRowsAndTheirChildren();

            return expandedRowsAndChildren;
        }
    }

    void InitialiseExpandedRowsAndTheirChildren()
    {
        if (expandedRowsAndChildren == null || someNodeChanged)
        {
            expandedRowsAndChildren = Root != null ? CollectExpandedNodesAndChildren() : new List<TreeViewNode>();

            someNodeChanged = false;
        }
    }

    public virtual void SetExpandedIDs(List<int> ids)
    {
        observer.State.ExpandedNodeIDs = ids;
        observer.State.ExpandedNodeIDs.Sort();

        ExpandedStateChanged();
    }

    public virtual bool IsExpanded(TreeViewNode node)
    {
        return IsExpanded(node.ID);
    }

    public virtual bool IsExpanded(int id)
    {
        return observer.State.ExpandedNodeIDs.BinarySearch(id) >= 0;
    }

    public virtual void SetExpanded(TreeViewNode node, bool expand)
    {
        SetExpanded(node.ID, expand);
    }

    public virtual void SetExpanded(int id, bool expand)
    {
        if (expand != IsExpanded(id))
        {
            if (expand)
            {
                observer.State.ExpandedNodeIDs.Add(id);
                observer.State.ExpandedNodeIDs.Sort();
            }
            else
            {
                observer.State.ExpandedNodeIDs.Remove(id);
            }

            ExpandedStateChanged();
        }
    }

    public virtual void SetExpandedWithChildren(TreeViewNode parent, bool expand)
    {
        Stack<TreeViewNode> stack = new Stack<TreeViewNode>();

        stack.Push(parent);

        HashSet<int> nodesToChange = new HashSet<int>();

        while (stack.Count > 0)
        {
            TreeViewNode node = stack.Pop();

            if (node.HasChildren)
            {
                nodesToChange.Add(node.ID);

                foreach (TreeViewNode child in node.Children)
                {
                    stack.Push(child);
                }
            }
        }

        HashSet<int> currentlyExpanded = new HashSet<int>(observer.State.ExpandedNodeIDs);

        if (expand) currentlyExpanded.UnionWith(nodesToChange);
        else        currentlyExpanded.ExceptWith(nodesToChange);

        SetExpandedIDs(currentlyExpanded.ToList());
    }

    protected virtual void ExpandedStateChanged()
    {
        someNodeChanged = true;

        if (OnExpandedStateChanged != null) OnExpandedStateChanged();
    }

    public virtual bool IsExpandable(TreeViewNode node) { return node.HasChildren; }
    public virtual bool CanBeParent(TreeViewNode node)  { return true;             }
}

} // of namespace BrightBit
