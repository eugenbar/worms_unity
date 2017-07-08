using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Slider based timer with events triggers
/// </summary>
public class Timeline : MonoBehaviour
{
    public GameObject controlUnit;                                      // Will be notified about all events occur
    public Slider slider;                                               // Convert time to linebar
    public Transform eventsTemporaryFolder;                             // Parent for events clones
    public Text counter;                                                // Displayed remaining time
    public float barRectMinSize = 0.05f;                                // Minimum bar size (in part of slider rect)

    private float timeout = 0f;                                         // Timer max time
    private bool needToUpdate = false;                                  // Initial data changed and timer need to be updated on next FixedUpdate step
    private bool prepared = false;                                      // Initial data was setted an timer can be started
    private TimelineEvent[] preparedLink;                               // Link for events data
    private bool started = false;                                       // Timer was initialised and started
    private bool inProgress = false;                                    // Count in progress
    private float expiredTime = 0f;                                     // Time expired after start
    private List<TimelineEvent> events = new List<TimelineEvent>();     // Events clones list
    private List<GameObject> barsList = new List<GameObject>();         // List of bar rectangles, displayed on timeline
    private List<GameObject> iconsList = new List<GameObject>();        // List of icons clones, displayed on timeline

    /// <summary>
    /// Validation initial data
    /// </summary>
    void Start()
    {
	    if (    controlUnit == null
            ||  slider == null
            ||  eventsTemporaryFolder == null)
        {
            Debug.Log("Wrong default settings");
        }
	}

    /// <summary>
    /// Operate timer and check for events occur
    /// </summary>
    void FixedUpdate()
    {
        // If initial condition were changed
        if (needToUpdate == true)
        {
            needToUpdate = false;
            PrepareNow();                                                           // Update data and display new conditions
        }
        // If count in progress - update time and check events
        if ((started == true) && (inProgress == true))
        {
            expiredTime += Time.deltaTime;                                          // Update time
            if (events != null)
            {
                // Check every event in list
                foreach (TimelineEvent ev in events)
                {
                    // Check if event started since last update
                    if (ev.started == false)
                    {
                        if (expiredTime >= ev.startTime)
                        {
                            ev.started = true;                                      // Marked event as started
                            // Notify control unit with link to event obect
                            if ((controlUnit != null) && (ev.eventObject != null))
                            {
                                controlUnit.SendMessage("TimelineEventStart", ev.eventObject, SendMessageOptions.DontRequireReceiver);
                            }
                            // Notify event object
                            if (ev.eventObject != null)
                            {
                                ev.eventObject.SendMessage("TimelineEventStart", SendMessageOptions.DontRequireReceiver);
                            }
                            // Notify event icon
                            if (ev.eventIcon != null)
                            {
                                ev.eventIcon.SendMessage("TimelineEventStart", SendMessageOptions.DontRequireReceiver);
                            }
                        }
                    }
                    // Check if event ended since last update
                    if (ev.ended == false)
                    {
                        if (expiredTime >= ev.endTime)
                        {
                            ev.ended = true;                                        // Marked as ended
                            // Notify control unit with link to event obect
                            if ((controlUnit != null) && (ev.eventObject != null))
                            {
                                controlUnit.SendMessage("TimelineEventEnd", ev.eventObject, SendMessageOptions.DontRequireReceiver);
                            }
                            // Notify event object
                            if (ev.eventObject != null)
                            {
                                ev.eventObject.SendMessage("TimelineEventEnd", SendMessageOptions.DontRequireReceiver);
                            }
                            // Notify event icon
                            if (ev.eventIcon != null)
                            {
                                ev.eventIcon.SendMessage("TimelineEventEnd", SendMessageOptions.DontRequireReceiver);
                            }
                        }
                    }
                }
            }
            if (expiredTime < timeout)
            {
                slider.value = 1f - ((timeout - expiredTime) / timeout);            // Update slider value equal remainind time
            }
            // Time is over
            else
            {
                expiredTime = timeout;
                ResetTimer();                                                       // Stop counting
                // Notify control unit
                controlUnit.SendMessage("TimelineTimeoutOccur", SendMessageOptions.DontRequireReceiver);
            }
            UpdateCounter();                                                        // Display remaining time
        }
    }

    /// <summary>
    /// Set initial condition and display it
    /// </summary>
    /// <param name="timeoutInSeconds"> Max time </param>
    /// <param name="eventsList"> Links to objects with TimelineEvent data </param>
    /// <returns> true - successful, false - wrong params </returns>
    public bool PrepareTimer(float timeoutInSeconds, TimelineEvent[] eventsList)
    {
        // Beqause method may be called after event occur (inside FixedUpdate handler),
        // to prevent initial data corruption timer data update will be happened only on next FixedUpdate
        bool res = false;
        if (timeoutInSeconds > 0f)
        {
            ResetTimer();                                                           // Stop counting
            timeout = timeoutInSeconds;                                             // Update max time
            preparedLink = eventsList;                                              // Update link to events objects
            needToUpdate = true;                                                    // Set flag for data updating on next FixedUpdate
            prepared = true;                                                        // Timer was prepared for starting
            res = true;
        }
        else
        {
            Debug.Log("Timeout must be above zero");
        }
        return res;
    }

    /// <summary>
    /// Start counting
    /// </summary>
    public void StartTimer()
    {
        if (prepared == true)
        {
            started = true;
            inProgress = true;
        }
    }

    /// <summary>
    /// Set initial condition, display it and start timer
    /// </summary>
    /// <param name="timeoutInSeconds"> Max time </param>
    /// <param name="eventsList"> Links to objects with TimelineEvent data </param>
    /// <returns> true - successful, false - wrong params </returns>
    public bool StartTimer(float timeoutInSeconds, TimelineEvent[] eventsList)
    {
        bool res = false;
        res = PrepareTimer(timeoutInSeconds, eventsList);
        if (res == true)
        {
            StartTimer();
        }
        return res;
    }

    /// <summary>
    /// Pause timer without data modification
    /// </summary>
    public void PauseTimer()
    {
        inProgress = false;
    }

    /// <summary>
    /// Resume timer after pause
    /// </summary>
    public void ResumeTimer()
    {
        if (started == true)
        {
            if (expiredTime < timeout)
            {
                inProgress = true;
            }
        }
    }

    /// <summary>
    /// Toggle between Pause and Resume
    /// </summary>
    public void TogglePause()
    {
        if (started == true)
        {
            if (inProgress == true)
            {
                PauseTimer();
            }
            else
            {
                ResumeTimer();
            }
        }
    }

    /// <summary>
    /// Stop timer and reset
    /// </summary>
    public void StopTimer()
    {
        if (prepared == true)
        {
            PrepareTimer(timeout, preparedLink);                                        // Display existing conditions
            prepared = false;
        }
    }

    /// <summary>
    /// Check if timer counting now
    /// </summary>
    /// <returns> true - count in progress, false - no active counting (stoped or paused) </returns>
    public bool IsTimerInProgress()
    {
        bool res = false;
        if ((started == true) && (inProgress == true))
        {
            res = true;
        }
        return res;
    }

    /// <summary>
    /// Increase or decrease expired time
    /// </summary>
    /// <param name="deltaInSeconds"> Delta time in seconds </param>
    public void ModifyExpiredTime(float deltaInSeconds)
    {
        if (started == true)
        {
            expiredTime += deltaInSeconds;
            if (expiredTime < 0f)
            {
                expiredTime = 0f;
            }
            else if (expiredTime > timeout)
            {
                expiredTime = timeout;
            }
        }
    }

    /// <summary>
    /// Check if specified event started
    /// </summary>
    /// <param name="eventData"> Link to obect with TimelineEvent component </param>
    /// <returns> true - event started, false - event not started </returns>
    public bool IsEventStarted(GameObject eventData)
    {
        bool res = false;
        foreach (TimelineEvent ev in events)
        {
            if (ev.eventObject == eventData)
            {
                if (ev.started == true)
                {
                    res = true;
                }
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// Check if specified event ended
    /// </summary>
    /// <param name="eventData"> Link to obect with TimelineEvent component </param>
    /// <returns> true - event ended, false - event not ended </returns>
    public bool IsEventEnded(GameObject eventData)
    {
        bool res = false;
        foreach (TimelineEvent ev in events)
        {
            if (ev.eventObject == eventData)
            {
                if (ev.ended == true)
                {
                    res = true;
                }
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// Check if specified event in progress
    /// </summary>
    /// <param name="eventData"> Link to obect with TimelineEvent component </param>
    /// <returns> true - event in progress, false - event not in progress </returns>
    public bool IsEventInProgress(GameObject eventData)
    {
        bool res = false;
        foreach (TimelineEvent ev in events)
        {
            if (ev.eventObject == eventData)
            {
                if ((ev.started == true) && (ev.ended == false))
                {
                    res = true;
                }
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// Get list af events in progress
    /// </summary>
    /// <returns> List of active events </returns>
    public List<GameObject> GetEventsInProgress()
    {
        List<GameObject> activeEvents = new List<GameObject>();
        foreach (TimelineEvent ev in events)
        {
            if (IsEventInProgress(ev.eventObject) == true)
            {
                activeEvents.Add(ev.eventObject);
            }
        }
        return activeEvents;
    }

    /// <summary>
    /// Update timer initial data
    /// </summary>
    private void PrepareNow()
    {
        expiredTime = 0f;                                                                   // Reset expired time
        slider.value = 0f;                                                                  // Reset slider
        UpdateCounter();                                                                    // Reset displayed remaining time
        ClearEventsData();                                                                  // Destroy all events and icons clones
        if (preparedLink != null)
        {
            RectTransform mainRect = GetComponent<RectTransform>();                         // Get dimension of timer image
            if (mainRect != null)
            {
                foreach (TimelineEvent timelineEvent in preparedLink)
                {
                    GameObject eventClone = Instantiate(timelineEvent.gameObject);          // Clone event
                    eventClone.SetActive(false);                                            // Disable it vision
                    foreach (Transform child in eventClone.transform)
                    {
                        Destroy(child.gameObject);                                          // Destroy all children objects of event clone
                    }
                    foreach (Component comp in eventClone.GetComponents<Component>())
                    {
                        if (!(comp is Transform) && !(comp is TimelineEvent))
                        {
                            Destroy(comp);                                                  // Destroy all excess components of event clone
                        }
                    }
                    eventClone.transform.SetParent(eventsTemporaryFolder, true);            // Parent clone to local events folder
                    TimelineEvent ev = eventClone.GetComponent<TimelineEvent>();
                    ev.eventObject = timelineEvent.eventObject;                             // Update link to origin object
                    events.Add(ev);                                                         // Add evemt to local list
                    if ((ev.eventBarFolder != null) && (ev.eventIcon != null) && (ev.iconFolder != null))
                    {
                        Image barImage = ev.eventBarFolder.GetComponentInChildren<Image>(); // Get bar rect template from specified folder
                        if (barImage != null)
                        {
                            GameObject newBar = Instantiate(barImage.gameObject);           // Clone bar rect
                            barsList.Add(newBar);                                           // Add bar to local list
                            newBar.transform.SetParent(ev.eventBarFolder, false);           // Parent to specified folder
                            newBar.GetComponent<Image>().enabled = true;                    // Make visible
                            RectTransform barRect = newBar.GetComponent<RectTransform>();
                            if (barRect != null)
                            {
                                // Check for data correct
                                if ((ev.startTime < 0f) || (ev.startTime > timeout))
                                {
                                    Debug.Log("Incorrect event's start time");
                                }
                                if ((ev.endTime < ev.startTime) || (ev.endTime > timeout))
                                {
                                    Debug.Log("Incorrect event's end time");
                                }
                                float delta = (ev.endTime - ev.startTime) / timeout;        // Calculate bar rect size in part of max time
                                // If bar size less than specified
                                if (delta < barRectMinSize)
                                {
                                    delta = barRectMinSize;                                 // Set it to min possible size
                                }
                                // Set rect size depending on timer rect dimension
                                barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, delta * mainRect.sizeDelta.y);
                            }
                            else // if (barRect != null)
                            { Debug.Log("Necessary component is absent"); }
                            GameObject newIcon = Instantiate(timelineEvent.eventIcon);      // Clone event icon
                            iconsList.Add(newIcon);                                         // Add to local list
                            ev.eventIcon = newIcon;                                         // Update event link to icon clone
                            newIcon.transform.SetParent(ev.iconFolder, false);            // Parent to specified folder
                            newIcon.SetActive(true);                                        // Make visible
                            float start = ev.startTime / timeout;                           // Calculate position
                            float pos = -start * mainRect.sizeDelta.y;                      // Consider to timer rect
                            // Set bar clone position
                            newBar.transform.localPosition = new Vector3(newBar.transform.localPosition.x,
                                                                            pos,
                                                                            newBar.transform.localPosition.z);
                            // Set icon clone position
                            newIcon.transform.localPosition = new Vector3(newIcon.transform.localPosition.x,
                                                                            pos,
                                                                            newIcon.transform.localPosition.z);

                        } // if (barImage != null)
                        else { Debug.Log("Necessary component is absent"); }
                    } // if ((ev.eventBarFolder != null) && (ev.eventIcon != null) && (ev.iconFolder != null))
                    else { Debug.Log("Incorrect event initial data"); }
                }
            } // if (mainRect != null)
            else { Debug.Log("Necessary component is absent"); }
        }
    }

    /// <summary>
    /// Stop counting
    /// </summary>
    private void ResetTimer()
    {
        started = false;
        inProgress = false;
    }

    /// <summary>
    /// Destroy events, bars and icons clones
    /// </summary>
    private void ClearEventsData()
    {
        foreach (TimelineEvent ev in events)
        {
            Destroy(ev.gameObject);
        }
        events.Clear();
        foreach (GameObject icon in iconsList)
        {
            Destroy(icon);
        }
        iconsList.Clear();
        foreach (GameObject bar in barsList)
        {
            Destroy(bar);
        }
        barsList.Clear();
    }

    /// <summary>
    /// Display remaining time
    /// </summary>
    private void UpdateCounter()
    {
        if (counter != null)
        {
            counter.text = Mathf.CeilToInt(timeout - expiredTime).ToString();
        }
    }
}
