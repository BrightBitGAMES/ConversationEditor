using UnityEngine;
using System.Collections;
using System.Linq;
using BrightBit;

public class SimpleNodeProcessor : INodeProcessor
{
    public string Process(ConversationNode node)
    {
        string text = node.GetText(Conversation.CurrentLanguage);

        if (node is Option) return string.IsNullOrEmpty(text) ? "[CONTINUE]" : text;
        else                return text;
    }
}
