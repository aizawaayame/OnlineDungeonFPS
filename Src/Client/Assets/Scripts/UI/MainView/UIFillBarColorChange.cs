using UnityEngine;
using UnityEngine.UI;

public class UIFillBarColorChange : MonoBehaviour
{

    #region Fields

    [SerializeField] Image ForegroundImage;
    [SerializeField] Color DefaultForegroundColor;
    [SerializeField] Color FlashForegroundColorFull;
    [SerializeField] Image BackgroundImage;
    [SerializeField] Color DefaultBackgroundColor;
    [SerializeField] Color FlashBackgroundColorEmpty;
    [SerializeField] float FullValue = 1f;
    [SerializeField] float EmptyValue = 0f;
    [SerializeField]float ColorChangeSharpness = 5f;

    float previousValue;

    #endregion
    
    public void Initialize(float fullValueRatio, float emptyValueRatio)
    {
        FullValue = fullValueRatio;
        EmptyValue = emptyValueRatio;

        previousValue = fullValueRatio;
    }

    public void UpdateVisual(float currentRatio)
    {
        if (currentRatio == FullValue && currentRatio != previousValue)
        {
            ForegroundImage.color = FlashForegroundColorFull;
        }
        else if (currentRatio < EmptyValue)
        {
            BackgroundImage.color = FlashBackgroundColorEmpty;
        }
        else
        {
            ForegroundImage.color = Color.Lerp(ForegroundImage.color, DefaultForegroundColor,
                Time.deltaTime * ColorChangeSharpness);
            BackgroundImage.color = Color.Lerp(BackgroundImage.color, DefaultBackgroundColor,
                Time.deltaTime * ColorChangeSharpness);
        }

        previousValue = currentRatio;
    }
}

