using UnityEngine;

/// <summary>
/// Event object example
/// </summary>
public class Fireplace : MonoBehaviour
{
    /// <summary>
    /// Start animation
    /// </summary>
    public void StartFire()
    {
        Animation anim = GetComponent<Animation>();
        if (anim != null)
        {
            anim.wrapMode = WrapMode.Loop;
            anim.Play();
        }
    }

    /// <summary>
    /// Stop animation
    /// </summary>
    public void StopFire()
    {
        Animation anim = GetComponent<Animation>();
        if (anim != null)
        {
            anim.wrapMode = WrapMode.Once;
        }
    }

    /// <summary>
    /// Timeline event start handler
    /// </summary>
    private void TimelineEventStart()
    {
        StartFire();
    }

    /// <summary>
    /// Timeline event ended handler
    /// </summary>
    private void TimelineEventEnd()
    {
        StopFire();
    }
}
