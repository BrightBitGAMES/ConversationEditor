using UnityEngine;
using System;
using System.Reflection;

namespace BrightBit
{

// supported method signatures:

// bool ()
// bool (int)
// bool (float)
// bool (bool)
// bool (string)
// bool (string, int)

// bool (ConversationNode)
// bool (ConversationNode, int)
// bool (ConversationNode, float)
// bool (ConversationNode, bool)
// bool (ConversationNode, string)
// bool (ConversationNode, string, int)

[Serializable]
public class NodeCondition : NodeEvent
{
    public bool Check(ConversationNode node)
    {
        if (script == null) return true;
        if (method == null) return true;

        return (bool) method.Invoke(null, CreateParameterArray(node));
    }
}

} // of namespace BrightBit
