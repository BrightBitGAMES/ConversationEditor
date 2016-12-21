using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BrightBit
{

public class NPC : ConversationOwner
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Conversation.IsTalking())
        {
            StartTalking();
        }
    }
}

} // of namespace BrightBit
