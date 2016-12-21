using UnityEngine;
using System.Collections.Generic;

namespace BrightBit
{

public class ConversationOwner : MonoBehaviour
{
    [SerializeField] protected Conversation conversation;

    public void StartTalking() { Conversation.Begin(conversation, this); }
    public void StopTalking()  { Conversation.End();                     }
}

} // of namespace BrightBit
