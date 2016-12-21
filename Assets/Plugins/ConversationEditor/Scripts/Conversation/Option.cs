using UnityEngine;

namespace BrightBit
{

[System.Serializable]
public sealed class Option : ConversationNode
{
    public static Option Create(Conversation owner)
    {
        Option result = ScriptableObject.CreateInstance<Option>();

        result.Initialise(owner);

        return result;
    }

    protected override bool IsChildTypeValid(Node node)
    {
        bool result = node is Narration;

        if (!result) Debug.LogError("Child is supposed to be of type 'Narration'");

        return result;
    }
}

} // of namespace BrightBit
