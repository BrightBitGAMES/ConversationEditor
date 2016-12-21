using UnityEngine;

namespace BrightBit
{

[System.Serializable]
public class RootNode : Node
{
    public static RootNode Create(Conversation owner)
    {
        RootNode result = ScriptableObject.CreateInstance<RootNode>();

        result.Initialise(owner);

        return result;
    }

    protected override bool IsChildTypeValid(Node node)
    {
        bool result = node is Narration;

        if (!result) Debug.LogError("Child is supposed to be of type 'Narration'");

        return node is Narration;
    }
}

} // of namespace BrightBit
