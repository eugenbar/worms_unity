using UnityEngine;

/// <summary>
/// All timeline events objects must have this component
/// </summary>
public class TimelineEvent : MonoBehaviour
{
    public float startTime;             // When event will be started (in seconds)
    public float endTime;               // When event will be ended (in seconds)
    public GameObject eventObject;      // Link to object that need to be notified on events occur
    public Transform eventBarFolder;    // Parent folder for rect bar (determines the position)
    public GameObject eventIcon;        // Link to event icon
    public Transform iconFolder;        // Parent folder for icon (determines the position)
    public bool started = false;        // Is event started
    public bool ended = false;          // Is event ended
}
