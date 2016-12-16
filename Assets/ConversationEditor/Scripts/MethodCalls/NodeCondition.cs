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

// bool (ConversationInfo)
// bool (ConversationInfo, int)
// bool (ConversationInfo, float)
// bool (ConversationInfo, bool)
// bool (ConversationInfo, string)
// bool (ConversationInfo, string, int)

[Serializable]
public class NodeCondition : NodeEvent
{
    public bool Check(ConversationInfo info)
    {
        if (script == null) return true;
        if (method == null) return true;

        return (bool) method.Invoke(null, CreateParameterArray(info));
    }
}

} // of namespace BrightBit
