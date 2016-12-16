using UnityEngine;
using System;
using System.Collections.Generic;

namespace BrightBit
{

[Serializable]
public class TreeViewState
{
    [SerializeField] Vector2 scrollPos;

    [SerializeField] int selectedNodeID;

    [SerializeField] List<int> expandedNodeIDs = new List<int>();

    public Vector2 ScrollPos
    {
        get { return scrollPos;  }
        set { scrollPos = value; }
    }

    public int SelectedNodeID
    {
        get { return selectedNodeID;  }
        set { selectedNodeID = value; }
    }

    public List<int> ExpandedNodeIDs
    {
        get { return expandedNodeIDs;  }
        set { expandedNodeIDs = value; }
    }
}

} // of namespace BrightBit
