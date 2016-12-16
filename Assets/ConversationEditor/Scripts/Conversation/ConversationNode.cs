using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BrightBit
{

[System.Serializable]
public abstract class ConversationNode : Node
{
    [SerializeField, HideInInspector] protected List<ConversationNode> linkParents = new List<ConversationNode>();

    [SerializeField, HideInInspector] protected NodeCondition showCondition = new NodeCondition();
    [SerializeField, HideInInspector] protected NodeAction    showAction    = new NodeAction();

    [SerializeField, HideInInspector] protected AudioClip audioClip;

    [SerializeField, HideInInspector] protected AudioClip[] audioClips = new AudioClip[Enum.GetNames(typeof(Conversation.Language)).Length];
    [SerializeField, HideInInspector] protected string[] texts         = new string[Enum.GetNames(typeof(Conversation.Language)).Length];

    [SerializeField, HideInInspector] protected string comments;

    public string Comments
    {
        get { return comments;  }
        set { comments = value; }
    }

    // internal use only
    public bool RunShowCondition(ConversationInfo info)
    {
        return showCondition.Check(info);
    }

    // internal use only
    public void RunSomeAction(ConversationInfo info)
    {
        showAction.Call(info);
    }

    public List<ConversationNode> LinkParents { get { return linkParents; } }

    static ConversationNode CreateCopy(ConversationNode source)
    {
        ConversationNode result = null;

        if (source is Narration)   result = Narration.Create(source.owner);
        else if (source is Option) result = Option.Create(source.owner);
        else                       throw new System.Exception("RootNodes must not be copied!");

        result.CopyFrom(source);

        return result;
    }

    public static ConversationNode CreateDeepCopy(ConversationNode source)
    {
        ConversationNode result = CreateCopy(source);

        Stack<ConversationNode> originals = new Stack<ConversationNode>();
        Stack<ConversationNode> copies    = new Stack<ConversationNode>();

        originals.Push(source);
        copies.Push(result);

        while (copies.Count > 0)
        {
            ConversationNode currentOriginal = originals.Pop();
            ConversationNode currentCopy     = copies.Pop();

            for (int i = 0; i < currentOriginal.NumChildren(); ++i)
            {
                bool isLink = currentOriginal.IsLink(i);

                ConversationNode childOriginal = currentOriginal.GetChild(i) as ConversationNode;

                if (!isLink)
                {
                    ConversationNode childCopy = CreateCopy(childOriginal);

                    currentCopy.AddChild(childCopy);

                    originals.Push(childOriginal);
                    copies.Push(childCopy);
                }
                else currentCopy.AddLink(childOriginal);
            }

        }

        return result;
    }

    protected void CopyFrom(ConversationNode source)
    {
        this.texts = (string[]) source.texts.Clone();

        this.showCondition = source.showCondition;
        this.showAction    = source.showAction;
        this.comments      = source.comments;
    }

    public bool IsUsedAsLink
    {
        get
        {
            return linkParents.Count > 0;
        }
    }

    public void AddLink(ConversationNode child)
    {
        if (!IsChildTypeValid(child)) return;

        child.linkParents.Add(this);

        connections.Add(new Connection(child, true));
    }

    public void RemoveLink(ConversationNode child)
    {
        if (!IsChildTypeValid(child)) return;

        int index = connections.FindIndex(connection => connection.Target == child && connection.IsLink);

        connections.RemoveAt(index);

        child.LinkParents.Remove(this);
    }

    public bool IsLink(int i)
    {
        return connections[i].IsLink;
    }

    public void SetText(Conversation.Language language, string text)
    {
        texts[(int) language] = text;
    }

    public string GetText(Conversation.Language language)
    {
        return texts[(int) language];
    }
}

} // of namespace BrightBit
