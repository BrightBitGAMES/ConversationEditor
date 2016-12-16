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

// void (ConversationInfo)
// void (ConversationInfo, int)
// void (ConversationInfo, float)
// void (ConversationInfo, bool)
// void (ConversationInfo, string)
// void (ConversationInfo, string, int)

[Serializable]
public class NodeAction : NodeEvent
{
    public void Call(ConversationInfo info)
    {
        if (script == null) return;
        if (method == null) return;

        method.Invoke(null, CreateParameterArray(info));
    }
}

} // of namespace BrightBit
