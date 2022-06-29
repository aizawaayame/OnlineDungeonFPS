using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class UIObjectiveHUD : MonoBehaviour
{

    #region Fields

    public RectTransform ObjectivePanel;
    public GameObject PrimaryObjectivePrefab;
    public GameObject SecondaryObjectivePrefab;

    Dictionary<Objective, UIObjectiveToast> objectivesDictionnary;


    #endregion

    void Awake()
    {
        objectivesDictionnary = new Dictionary<Objective, UIObjectiveToast>();

        EventUtil.AddListener<ObjectiveUpdateEvent>(OnUpdateObjective);

        Objective.OnObjectiveCreated += RegisterObjective;
        Objective.OnObjectiveCompleted += UnregisterObjective;
    }

    public void RegisterObjective(Objective objective)
    {

        GameObject objectiveUIInstance =
            Instantiate(objective.IsOptional ? SecondaryObjectivePrefab : PrimaryObjectivePrefab, ObjectivePanel);

        if (!objective.IsOptional)
            objectiveUIInstance.transform.SetSiblingIndex(0);

        UIObjectiveToast toast = objectiveUIInstance.GetComponent<UIObjectiveToast>();

        toast.Initialize(objective.Title, objective.Description, "", objective.IsOptional, objective.DelayVisible);

        objectivesDictionnary.Add(objective, toast);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ObjectivePanel);
    }

    public void UnregisterObjective(Objective objective)
    {
        // if the objective if in the list, make it fade out, and remove it from the list
        if (objectivesDictionnary.TryGetValue(objective, out UIObjectiveToast toast) && toast != null)
        {
            toast.Complete();
        }

        objectivesDictionnary.Remove(objective);
    }

    void OnUpdateObjective(ObjectiveUpdateEvent evt)
    {
        if (objectivesDictionnary.TryGetValue(evt.Objective, out UIObjectiveToast toast) && toast != null)
        {
            // set the new updated description for the objective, and forces the content size fitter to be recalculated
            Canvas.ForceUpdateCanvases();
            if (!string.IsNullOrEmpty(evt.DescriptionText))
                toast.DescriptionTextContent.text = evt.DescriptionText;

            if (!string.IsNullOrEmpty(evt.CounterText))
                toast.CounterTextContent.text = evt.CounterText;

            if (toast.GetComponent<RectTransform>())
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(toast.GetComponent<RectTransform>());
            }
        }
    }

    void OnDestroy()
    {
        EventUtil.AddListener<ObjectiveUpdateEvent>(OnUpdateObjective);

        Objective.OnObjectiveCreated -= RegisterObjective;
        Objective.OnObjectiveCompleted -= UnregisterObjective;
    }
}

