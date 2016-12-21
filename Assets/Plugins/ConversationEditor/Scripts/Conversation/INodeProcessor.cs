using UnityEngine;
using System.Collections;

namespace BrightBit
{

public interface INodeProcessor
{
    string Process(ConversationNode node);
}

} // of namespace BrightBit
