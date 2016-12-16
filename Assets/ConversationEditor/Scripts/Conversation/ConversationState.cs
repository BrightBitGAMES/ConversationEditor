using UnityEngine;

namespace BrightBit
{

[System.Serializable]
public class ConversationState
{
    [SerializeField] SerializableDictionary<ConversationNode, int> visitedNodes = new SerializableDictionary<ConversationNode, int>();

    public SerializableDictionary<ConversationNode, int> VisitedNodes
    {
        get { return visitedNodes;  }
        set { visitedNodes = value; }
    }
}

} // of namespace BrightBit
