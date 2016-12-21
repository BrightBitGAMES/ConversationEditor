using UnityEngine;
using System.Collections.Generic;

using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField, Range(1, 10)] int strength     = 5;
    [SerializeField, Range(1, 10)] int perception   = 5;
    [SerializeField, Range(1, 10)] int endurance    = 5;
    [SerializeField, Range(1, 10)] int charisma     = 5;
    [SerializeField, Range(1, 10)] int intelligence = 5;
    [SerializeField, Range(1, 10)] int agility      = 5;
    [SerializeField, Range(1, 10)] int luck         = 5;

    [SerializeField] List<string> items = new List<string>();

    public int Strength
    {
        get { return strength; }
    }

    public int Perception
    {
        get { return perception; }
    }

    public int Endurance
    {
        get { return endurance; }
    }

    public int Charisma
    {
        get { return charisma; }
    }

    public int Intelligence
    {
        get { return intelligence; }
    }

    public int Agility
    {
        get { return agility; }
    }

    public int Luck
    {
        get { return luck; }
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }
}
