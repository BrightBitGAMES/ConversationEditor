using UnityEngine;

namespace BrightBit
{

public interface IConversationalPartner {}

public struct ConversationInfo
{
    Conversation conversation;
    ConversationNode currentNode;

    IConversationalPartner owner;
    IConversationalPartner initiator;

    public ConversationInfo(Conversation conversation, IConversationalPartner owner, IConversationalPartner initiator, ConversationNode currentNode)
    {
        this.conversation = conversation;
        this.initiator    = initiator;
        this.owner        = owner;
        this.currentNode  = currentNode;
    }

    public IConversationalPartner Initiator { get { return initiator;    } }
    public IConversationalPartner Owner     { get { return owner;        } }
    public Conversation Conversation        { get { return conversation; } }
    public ConversationNode CurrentNode     { get { return currentNode;  } }
}

} // of namespace BrightBit
