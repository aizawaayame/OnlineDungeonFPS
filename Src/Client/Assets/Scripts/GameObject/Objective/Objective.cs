using Utilities;
using System;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public static event Action<Objective> OnObjectiveCreated;
    public static event Action<Objective> OnObjectiveCompleted;

    #region Fields

    public string Title;
    public string Description;
    public bool IsOptional;
    public float DelayVisible;

    #endregion

    #region Properties

    public bool IsCompleted { get; private set; }
    public bool IsBlocking() => !(IsOptional || IsCompleted);

    #endregion
    
    protected virtual void Start()
    {
        OnObjectiveCreated?.Invoke(this);

        DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
        displayMessage.Message = Title;
        displayMessage.DelayBeforeDisplay = 0.0f;
        EventUtil.Broadcast(displayMessage);
    }

    public void UpdateObjective(string descriptionText, string counterText, string notificationText)
    {
        ObjectiveUpdateEvent evt = Events.ObjectiveUpdateEvent;
        evt.Objective = this;
        evt.DescriptionText = descriptionText;
        evt.CounterText = counterText;
        evt.NotificationText = notificationText;
        evt.IsComplete = IsCompleted;
        EventUtil.Broadcast(evt);
    }

    public void CompleteObjective(string descriptionText, string counterText, string notificationText)
    {
        IsCompleted = true;

        ObjectiveUpdateEvent evt = Events.ObjectiveUpdateEvent;
        evt.Objective = this;
        evt.DescriptionText = descriptionText;
        evt.CounterText = counterText;
        evt.NotificationText = notificationText;
        evt.IsComplete = IsCompleted;
        EventUtil.Broadcast(evt);

        OnObjectiveCompleted?.Invoke(this);
    }
}

