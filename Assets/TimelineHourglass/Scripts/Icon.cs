using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Icon example
/// </summary>
public class Icon : MonoBehaviour
{
    public Button button;                   // Interactive element
    public Text text;                       // Displayed data

    public string activeText = "Click me";  // Data on event active state
    public string goaledText = "Goal";      // If icon was clicked during active state
    public string missedText = "Too late";  // If icon was not clicked during active state

    /// <summary>
    /// Timeline event start handler
    /// </summary>
    private void TimelineEventStart()
    {
        if (button != null)
        {
            button.interactable = true;     // Make button clicable
        }
        if (text != null)
        {
            text.text = activeText;         // Display text
        }
    }

    /// <summary>
    /// Timeline event end handler
    /// </summary>
    private void TimelineEventEnd()
    {
        if (button != null)
        {
            button.interactable = false;    // Make button inactive
        }
        if (text != null)
        {
            if (text.text != goaledText)    // If button was not clicked
            {
                text.text = missedText;     // Display text
            }
        }
    }

    /// <summary>
    /// Icon click handler
    /// </summary>
    public void OnIconClick()
    {
        if (button != null)
        {
            button.interactable = false;    // Make button inactive
        }
        if (text != null)
        {
            text.text = goaledText;         // Display text
        }
    }
}
