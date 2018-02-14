using UnityEngine;
using System;
using System.Reflection;

namespace BrightBit
{

// supported method signatures:

// void ()
// void (int)
// void (float)
// void (bool)
// void (string)
// void (string, int)

// void (ConversationNode)
// void (ConversationNode, int)
// void (ConversationNode, float)
// void (ConversationNode, bool)
// void (ConversationNode, string)
// void (ConversationNode, string, int)

[Serializable]
public class NodeAction : NodeEvent
{
    public void Call(ConversationNode node)
    {
#if UNITY_EDITOR
            if (script == null) return;
#endif
            if (method == null) return;

        method.Invoke(null, CreateParameterArray(node));
    }
}

} // of namespace BrightBit
