using UnityEngine;
using UnityEngine.UI;

public class UICompassMarker : MonoBehaviour
{
    public Image MainImage;
    public CanvasGroup CanvasGroup;
    public Color DefaultColor;
    public Color AltColor;
    public bool IsDirection;
    public TMPro.TextMeshProUGUI TextContent;

    MonsterController monsterController;

    public void Initialize(UICompassElement compassElement, string textDirection)
    {
        if (IsDirection && TextContent)
        {
            TextContent.text = textDirection;
        }
        else
        {
            monsterController = compassElement.transform.GetComponent<MonsterController>();

            if (monsterController)
            {
                monsterController.onDetectedTarget += DetectTarget;
                monsterController.onLostTarget += LostTarget;

                LostTarget();
            }
        }
    }

    public void DetectTarget()
    {
        MainImage.color = AltColor;
    }

    public void LostTarget()
    {
        MainImage.color = DefaultColor;
    }
}

