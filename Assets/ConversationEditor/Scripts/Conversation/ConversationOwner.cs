using UnityEngine;
using System.Collections.Generic;

namespace BrightBit
{

public class ConversationOwner : MonoBehaviour, IConversationalPartner
{
    [SerializeField] Conversation target;

    //[SerializeField, HideInInspector] SerializableDictionary<ConversationNode, int> visitedNodes = new SerializableDictionary<ConversationNode, int>();
    [SerializeField, HideInInspector] ConversationNode currentNode;

    IConversationalPartner initiator;

    Narration currentNarration = null;
    List<Option> currentOptions = new List<Option>();

    public Narration CurrentNarration  { get { return currentNarration; } }
    public List<Option> CurrentOptions { get { return currentOptions;   } }

    public bool CanAdvance { get { return currentOptions.Count > 0; } }

    public void StartTalking(IConversationalPartner initiator)
    {
        this.initiator = initiator;

        AdvanceNarration(target.Root);
        AdvanceOptions();
    }

    void AdvanceNarration(Node node)
    {
        for (int i = 0; i < node.NumChildren(); ++i)
        {
            Narration narration = node.GetChild(i) as Narration;

            ConversationInfo info = CreateConversationInfo(narration);

            if (narration.RunShowCondition(info))
            {
                currentNarration = narration;

                currentNarration.RunSomeAction(info);

                return;
            }
        }

        currentNarration = null;
    }

    void AdvanceOptions()
    {
        currentOptions.Clear();

        if (currentNarration == null) return;

        for (int i = 0; i < currentNarration.NumChildren(); ++i)
        {
            Option child = currentNarration.GetChild(i) as Option;

            if (child.RunShowCondition(CreateConversationInfo(child))) currentOptions.Add(child);
        }
    }

    ConversationInfo CreateConversationInfo(ConversationNode node)
    {
        return new ConversationInfo(target, this, initiator, node);
    }

    public void StopTalking()
    {
        currentNarration = null;
    }

    public bool IsTalking()
    {
        return currentNarration != null;
    }

    public void Advance(Option option)
    {
        if (option == null) throw new System.ArgumentException("The argument is not supposed to be null!", "option");
        if (currentNarration == null) throw new System.InvalidOperationException("You need to call Begin() first or the corresponding conversation is empty!");
        if (!option.SameOwner(currentNarration)) throw new System.ArgumentException("The argument is not an element of the conversation!", "option");
        if (!currentOptions.Contains(option)) throw new System.ArgumentException("The argument is not an element of the currently available options!", "option");

        option.RunSomeAction(CreateConversationInfo(option));

        AdvanceNarration(option);
        AdvanceOptions();
    }
}

} // of namespace BrightBit
