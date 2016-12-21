using UnityEngine;
using System.Collections.Generic;

namespace BrightBit
{

[System.Serializable]
public sealed class Narration : ConversationNode
{
    [SerializeField, HideInInspector] string tag;

    public string Tag
    {
        get { return tag;  }
        set { tag = value; }
    }

    public static Narration Create(Conversation owner)
    {
        Narration result = ScriptableObject.CreateInstance<Narration>();

        result.Initialise(owner);

        return result;
    }

    protected override bool IsChildTypeValid(Node node)
    {
        bool result = node is Option;

        if (!result) Debug.LogError("Child is supposed to be of type 'Option'");

        return result;
    }
}

} // of namespace BrightBit
