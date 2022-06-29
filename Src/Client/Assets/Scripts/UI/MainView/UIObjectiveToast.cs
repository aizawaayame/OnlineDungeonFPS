using UnityEngine;
using UnityEngine.UI;

public class UIObjectiveToast : MonoBehaviour
{

    #region Fields

    public TMPro.TextMeshProUGUI TitleTextContent;
    public TMPro.TextMeshProUGUI DescriptionTextContent;
    public TMPro.TextMeshProUGUI CounterTextContent;
    public RectTransform SubTitleRect;
    public CanvasGroup CanvasGroup;
    public HorizontalOrVerticalLayoutGroup LayoutGroup;
    public float CompletionDelay;
    public float FadeInDuration = 0.5f;
    public float FadeOutDuration = 2f;
    public AudioClip InitSound;
    public AudioClip CompletedSound;
    public float MoveInDuration = 0.5f;
    public AnimationCurve MoveInCurve;
    public float MoveOutDuration = 2f;
    public AnimationCurve MoveOutCurve;

    float startFadeTime;
    bool isFadingIn;
    bool isFadingOut;
    bool isMovingIn;
    bool isMovingOut;
    AudioSource audioSource;
    RectTransform rectTransform;

    #endregion
    
    public void Initialize(string titleText, string descText, string counterText, bool isOptionnal, float delay)
    {
        // set the description for the objective, and forces the content size fitter to be recalculated
        Canvas.ForceUpdateCanvases();

        TitleTextContent.text = titleText;
        DescriptionTextContent.text = descText;
        CounterTextContent.text = counterText;

        if (GetComponent<RectTransform>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        startFadeTime = Time.time + delay;
        // start the fade in
        isFadingIn = true;
        isMovingIn = true;
    }

    public void Complete()
    {
        startFadeTime = Time.time + CompletionDelay;
        isFadingIn = false;
        isMovingIn = false;

        // if a sound was set, play it
        PlaySound(CompletedSound);

        // start the fade out
        isFadingOut = true;
        isMovingOut = true;
    }

    void Update()
    {
        float timeSinceFadeStarted = Time.time - startFadeTime;

        SubTitleRect.gameObject.SetActive(!string.IsNullOrEmpty(DescriptionTextContent.text));

        if (isFadingIn && !isFadingOut)
        {
            // fade in
            if (timeSinceFadeStarted < FadeInDuration)
            {
                // calculate alpha ratio
                CanvasGroup.alpha = timeSinceFadeStarted / FadeInDuration;
            }
            else
            {
                CanvasGroup.alpha = 1f;
                // end the fade in
                isFadingIn = false;

                PlaySound(InitSound);
            }
        }

        if (isMovingIn && !isMovingOut)
        {
            // move in
            if (timeSinceFadeStarted < MoveInDuration)
            {
                LayoutGroup.padding.left = (int) MoveInCurve.Evaluate(timeSinceFadeStarted / MoveInDuration);

                if (GetComponent<RectTransform>())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }
            }
            else
            {
                // making sure the position is exact
                LayoutGroup.padding.left = 0;

                if (GetComponent<RectTransform>())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }

                isMovingIn = false;
            }

        }

        if (isFadingOut)
        {
            // fade out
            if (timeSinceFadeStarted < FadeOutDuration)
            {
                // calculate alpha ratio
                CanvasGroup.alpha = 1 - (timeSinceFadeStarted) / FadeOutDuration;
            }
            else
            {
                CanvasGroup.alpha = 0f;

                // end the fade out, then destroy the object
                isFadingOut = false;
                Destroy(gameObject);
            }
        }

        if (isMovingOut)
        {
            // move out
            if (timeSinceFadeStarted < MoveOutDuration)
            {
                LayoutGroup.padding.left = (int) MoveOutCurve.Evaluate(timeSinceFadeStarted / MoveOutDuration);

                if (GetComponent<RectTransform>())
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }
            }
            else
            {
                isMovingOut = false;
            }
        }
    }

    void PlaySound(AudioClip sound)
    {
        if (!sound)
            return;

        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.PlayOneShot(sound);
    }
}

