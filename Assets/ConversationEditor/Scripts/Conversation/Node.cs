using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace BrightBit
{

[System.Serializable]
public abstract class Node : ScriptableObject
{
    [SerializeField, HideInInspector] protected Conversation owner;
    [SerializeField, HideInInspector] protected List<Connection> connections = new List<Connection>();
    [SerializeField, HideInInspector] protected Node parent;

    public Node Parent { get { return parent; } }

    public object EditorData { get; set; } // internal use only

    protected abstract bool IsChildTypeValid(Node node);

    protected Node() {}

    protected void Initialise(Conversation owner)
    {
        hideFlags = HideFlags.HideInHierarchy;

        this.owner = owner;

#if UNITY_EDITOR
        AssetDatabase.AddObjectToAsset(this, owner);
#endif
    }

    public bool SameOwner(Node other)
    {
        return owner == other.owner;
    }

    public void AddChild(Node child)
    {
        if (!IsChildTypeValid(child)) return;

        child.parent = this;

        connections.Add(new Connection(child, false));
    }

    protected void AddChildBefore(Node child, int before)
    {
        if (!IsChildTypeValid(child)) return;

        child.parent = this;

        connections.Insert(before, new Connection(child, false));
    }

    protected void AddChildAfter(Node child, int after)
    {
        if (!IsChildTypeValid(child)) return;

        child.parent = this;

        connections.Insert(after + 1, new Connection(child, false));
    }

    public int NumChildren()
    {
        return connections.Count;
    }

    public Node GetChild(int i)
    {
        return connections[i].Target;
    }

    public virtual void RemoveChild(Node child)
    {
        if (!IsChildTypeValid(child)) return;

        int index = connections.FindIndex(connection => connection.Target == child && !connection.IsLink);

        connections.RemoveAt(index);

        child.parent = null;
    }

    public void SwapChildren(int a, int b)
    {
        connections.Swap(a, b);
    }
}

} // of namespace BrightBit
