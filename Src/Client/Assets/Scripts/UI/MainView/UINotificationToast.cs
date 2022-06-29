
using UnityEngine;

public class UINotificationToast : MonoBehaviour
{

    #region Fields

    [SerializeField] TMPro.TextMeshProUGUI TextContent;
    [SerializeField] CanvasGroup CanvasGroup;
    [SerializeField] float VisibleDuration;
    [SerializeField] float FadeInDuration = 0.5f;
    [SerializeField] float FadeOutDuration = 2f;


    float initTime;
    #endregion

    #region Properties

    public bool Initialized { get; private set; }
    public float TotalRunTime => VisibleDuration + FadeInDuration + FadeOutDuration;
    
    #endregion

    public void Initialize(string text)
    {
        TextContent.text = text;
        initTime = Time.time;

        // start the fade out
        Initialized = true;
    }

    void Update()
    {
        if (Initialized)
        {
            float timeSinceInit = Time.time - initTime;
            if (timeSinceInit < FadeInDuration)
            {
                // fade in
                CanvasGroup.alpha = timeSinceInit / FadeInDuration;
            }
            else if (timeSinceInit < FadeInDuration + VisibleDuration)
            {
                // stay visible
                CanvasGroup.alpha = 1f;
            }
            else if (timeSinceInit < FadeInDuration + VisibleDuration + FadeOutDuration)
            {
                // fade out
                CanvasGroup.alpha = 1 - (timeSinceInit - FadeInDuration - VisibleDuration) / FadeOutDuration;
            }
            else
            {
                CanvasGroup.alpha = 0f;

                // fade out over, destroy the object
                Destroy(gameObject);
            }
        }
    }
}

