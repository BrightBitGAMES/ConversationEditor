using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace BrightBit
{

[System.Serializable]
//[CreateAssetMenu(menuName = "BrightBit/Conversation", order = 94)]
[CreateAssetMenu(menuName = "Conversation", order = 94)]
public sealed class Conversation : ScriptableObject
{
    static Conversation instance                 = null;
    static ConversationOwner instanceOwner       = null;
    static ConversationProgress instanceProgress = null;

    static DefaultNodeProcessor defaultNodeProcessor = new DefaultNodeProcessor();
    static INodeProcessor nodeProcessor              = defaultNodeProcessor;
    static Conversation.Language language            = Conversation.Language.English;

    public delegate void OnOptionSelectedHandler();
    public static event OnOptionSelectedHandler OnOptionSelected;

    public delegate void OnConversationStartedHandler();
    public static event OnConversationStartedHandler OnConversationStarted;

    public delegate void OnConversationEndedHandler();
    public static event OnConversationEndedHandler OnConversationEnded;

    public enum Language
    {
        English,
        ChineseSimplified,
        ChineseTraditional,
        Spanish,
        German,
        French,
        Italian,
        Japanese,
        Russian,
        Polish,
        Korean,
        UserDefinedLanguage1,
        UserDefinedLanguage2,
        UserDefinedLanguage3
    }

    [SerializeField, HideInInspector] RootNode root;
    [SerializeField, TextArea(5,5)] string comment;

    public static Conversation CurrentConversation      { get { return instance;         } }
    public static ConversationOwner CurrentOwner        { get { return instanceOwner;    } }
    public static ConversationProgress CurrentProgress  { get { return instanceProgress; } }

    public static Conversation.Language CurrentLanguage
    {
        get { return language;  }
        set
        {
            language = value;

            if (instanceProgress != null)
            {
                // ToDo update progress to reflect language change
            }
        }
    }

    public static INodeProcessor NodeProcessor
    {
        get { return nodeProcessor;  }

        set
        {
            nodeProcessor = value;

            if (nodeProcessor == null) nodeProcessor = defaultNodeProcessor;
        }
    }

    public ConversationState State { get; set; }

    public int GetTimesShown(Narration narration)
    {
        return State.VisitedNodes[narration];
    }

    public int GetTimesChosen(Option option)
    {
        return State.VisitedNodes[option];
    }

    public RootNode Root
    {
        get
        {
#if UNITY_EDITOR
                if (root == null && AssetDatabase.Contains(this)) root = RootNode.Create(this);
#endif
                return root;
        }
    }

    public static bool IsTalking()
    {
        return instance != null;
    }

    public static void Begin(Conversation conversation, ConversationOwner owner)
    {
        if (IsTalking()) throw new System.InvalidOperationException("A conversation is already \"running\"! Call \"Stop()\" first!");

        instanceProgress = Advance(conversation.Root);

        if (instanceProgress == null)
        {
            Debug.LogWarning("The conversation \"" + conversation.name + "\" is empty!");
            return;
        }

        instance         = conversation;
        instanceOwner    = owner;

        OnConversationStarted();
        OnOptionSelected();
    }

    public static void End()
    {
        if (!IsTalking()) throw new System.InvalidOperationException("There is no conversation \"running\"! You can start a conversation with a call for \"Start()\"!");

        OnConversationEnded();

        instance         = null;
        instanceOwner    = null;
        instanceProgress = null;
    }

    public static void Select(int optionIndex)
    {
        Option option = instanceProgress.CurrentOptions[optionIndex];

        option.InvokeAction();

        instanceProgress = Advance(option);

        if (instanceProgress != null) OnOptionSelected();
        else                          End();
    }

    static ConversationProgress Advance(Node node)
    {
        Narration currentNarration = null;
        List<Option> availableOptions = new List<Option>();

        string processedNarrationText = string.Empty;
        List<string> processedOptionTexts = new List<string>();

        for (int i = 0; i < node.NumChildren(); ++i)
        {
            Narration narration = node.GetChild(i) as Narration;

            if (narration.InvokeCondition())
            {
                currentNarration       = narration;
                processedNarrationText = nodeProcessor.Process(currentNarration);

                currentNarration.InvokeAction();

                break;
            }
        }

        if (currentNarration == null) return null;

        for (int i = 0; i < currentNarration.NumChildren(); ++i)
        {
            Option child = currentNarration.GetChild(i) as Option;

            if (child.InvokeCondition())
            {
                availableOptions.Add(child);
                processedOptionTexts.Add(nodeProcessor.Process(child));
            }
        }

        return new ConversationProgress(currentNarration, availableOptions, processedNarrationText, processedOptionTexts);
    }
}

} // of namespace BrightBit
