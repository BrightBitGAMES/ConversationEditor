using UnityEngine;
using System.Collections;

namespace BrightBit
{

public class DefaultNodeProcessor : INodeProcessor
{
    public string Process(ConversationNode node) { return node.Text; }
}

} // of namespace BrightBit
