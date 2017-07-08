using UnityEngine;

/// <summary>
/// Timeline control unit example
/// </summary>
public class DummyControlUnit : MonoBehaviour
{
    public Timeline timeline;                       // Link to Timeline object
    public float timeout = 15f;                     // Set max time for timer (in seconds)
    public Fireplace fireplace;                     // link to event object example
	
    /// <summary>
    /// Check for initial data valid
    /// </summary>
	void Start ()
    {
	    if (    timeline == null
            ||  fireplace == null)
        {
            Debug.Log("Wrong default settings");
        }
        // Display timer on start
        timeline.PrepareTimer(timeout, GetComponentsInChildren<TimelineEvent>());
    }

    /// <summary>
    /// Start timer
    /// </summary>
    public void StartHourglass()
    {
        fireplace.StopFire();
        timeline.StartTimer(timeout, GetComponentsInChildren<TimelineEvent>());
    }

    /// <summary>
    /// Pause/Resume timer
    /// </summary>
    public void PauseHourglass()
    {
        timeline.TogglePause();
    }

    /// <summary>
    /// Stop timer
    /// </summary>
    public void StopHourglass()
    {
        fireplace.StopFire();
        timeline.StopTimer();
    }

    /// <summary>
    /// Timeline event start handler
    /// </summary>
    /// <param name="eventObj"> Link to active event object </param>
    public void TimelineEventStart(GameObject eventObj)
    {
        
    }

    /// <summary>
    /// Timeline event end handler
    /// </summary>
    /// <param name="eventObj"> Link to active event object </param>
    public void TimelineEventEnd(GameObject eventObj)
    {
        
    }

    /// <summary>
    /// Timeline timeout handler
    /// </summary>
    private void TimelineTimeoutOccur()
    {
        
    }
}
