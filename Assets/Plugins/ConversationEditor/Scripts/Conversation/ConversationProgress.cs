using UnityEngine;
using System.Collections.Generic;

namespace BrightBit
{

public class ConversationProgress
{
    Narration currentNarration;
    List<Option> availableOptions;

    string processedNarrationText;
    List<string> processedOptionTexts;

    public ConversationProgress(Narration narration, List<Option> options, string narrationText, List<string> optionTexts)
    {
        this.currentNarration = narration;
        this.availableOptions = options;

        this.processedNarrationText = narrationText;
        this.processedOptionTexts   = optionTexts;
    }

    public bool HasOptions
    {
        get { return availableOptions.Count > 0; }
    }

    public Narration CurrentNarration      { get { return currentNarration;   } }
    public List<Option> CurrentOptions     { get { return availableOptions;   } }

    public string CurrentNarrationText     { get { return processedNarrationText; } }
    public List<string> CurrentOptionTexts { get { return processedOptionTexts;   } }
}

} // of namespace BrightBit
