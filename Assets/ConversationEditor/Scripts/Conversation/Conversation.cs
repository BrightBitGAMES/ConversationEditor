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
public class Conversation : ScriptableObject
{
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
            if (root == null && AssetDatabase.Contains(this))
            {
                root = RootNode.Create(this);

                //asdasfasfsdfa();
            }

            return root;
        }
    }

    // public void asdasfasfsdfa()
    // {
        // Narration child;
        // Option subChild;
        // Narration subSubChild;
        // Narration subSubSubChild;
        // Narration subSubSubSubChild;

        // Narration link = null;

        // child = Narration.Create(this, "You are standing in the center of the bank's entrance hall. You've noticed that there are at least three new security guards. What do you want to do?");
        // root.AddChild(child);

        // subChild = Option.Create(this, "Start a robbery");
        // child.AddChild(subChild);

            // subSubChild = Narration.Create(this, "Everybody be cool, this is a robbery!");
            // subChild.AddChild(subSubChild);

            // link = subSubChild;

            // Option inbetween = Option.Create(this, "");
            // subSubChild.AddChild(inbetween);

                // subSubSubChild = Narration.Create(this, "The cashier suddenly gets frightened and tosses all available money into plastic bags. You say thank you and clear off as fast as you came. You get away with 5620$.");
                // inbetween.AddChild(subSubSubChild);

                // subSubSubChild = Narration.Create(this, "The cashier is not impressed and triggers the security alarm! Immediately several guards come running.");
                // inbetween.AddChild(subSubSubChild);

                // inbetween = Option.Create(this, "");
                // subSubSubChild.AddChild(inbetween);

                    // subSubSubSubChild = Narration.Create(this, "After you have bumped the guards off, the cashier also feels forced to hand over the demanded money. You get away with 4210$.");
                    // inbetween.AddChild(subSubSubSubChild);

                    // subSubSubSubChild = Narration.Create(this, "The guards have finished you! They have already called the police which come to arrest you. What do you want to do?");
                    // inbetween.AddChild(subSubSubSubChild);

        // subChild = Option.Create(this, "Commit a burglary at night"); child.AddChild(subChild);
        // subChild.AddLink(link);

        // subChild = Option.Create(this, "Blackmail the manager");      child.AddChild(subChild);
        // subChild.AddLink(link);

        // subChild = Option.Create(this, "Leave");                      child.AddChild(subChild);
        // subChild.AddLink(link);
    // }
}

} // of namespace BrightBit
