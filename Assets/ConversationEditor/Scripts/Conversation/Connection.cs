using UnityEngine;
using System.Collections.Generic;

namespace BrightBit
{

[System.Serializable]
public struct Connection
{
    [SerializeField, HideInInspector] Node target;
    [SerializeField, HideInInspector] bool isLink;

    public Node Target { get { return target; } }
    public bool IsLink { get { return isLink; } }

    public Connection(Node target, bool isLink)
    {
        this.target = target;
        this.isLink = isLink;
    }
}

} // of namespace BrightBit
