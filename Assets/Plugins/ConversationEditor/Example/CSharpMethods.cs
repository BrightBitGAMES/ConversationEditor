using UnityEngine;
using BrightBit;

public class CSharpMethods
{
    // some supported methods:

    public static bool AnInfoOnly(ConversationNode node)
    {
        Player p = (Player) Object.FindObjectOfType(typeof(Player));

        return p.Intelligence > 5;
    }

    public static bool IsPlayerIntelligenceHigherThan(ConversationNode node, int intelligence)
    {
        Player p = (Player) Object.FindObjectOfType(typeof(Player));

        return p.Intelligence > intelligence;
    }

    public static bool IsNodeVisible(bool visibility)
    {
        return visibility;
    }

    public static bool OneFloat(float floatValue)
    {
        return true;
    }

    public static bool OneStringAndAnInteger(string itemName, int numElements)
    {
        return true;
    }

    public static bool NoParameter()
    {
        return true;
    }

    public static void LogMessage(string message)
    {
        Debug.Log(message);
    }

    public static void LogMessage(ConversationNode node, string message)
    {
        Player player = (Player) Object.FindObjectOfType(typeof(Player));

        Debug.Log(message + " : " + player + " : " + Conversation.CurrentOwner);
    }

    // some unsupported methods:

    bool SomeInstanceMethod(int i)
    {
        return i > 0;
    }

    public static bool OneIntegerAndAString(int numElements, string itemName)
    {
        return true;
    }

    public static bool SomeGenericMethod<T>(string stringValue, int intValue)
    {
        T a = default(T);

        return a is T;
    }

    public static bool TwoFloats(float first, float second)
    {
        return true;
    }

    public static bool ThreeFloats(float first, float second, float third)
    {
        return true;
    }

    public static bool FourFloats(float first, float second, float third, float fourth, float fifth)
    {
        return true;
    }
}
