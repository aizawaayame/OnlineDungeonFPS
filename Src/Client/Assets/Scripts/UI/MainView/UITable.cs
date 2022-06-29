
using UnityEngine;

public class UITable : MonoBehaviour
{

    #region Fields

    public float Offset;
    public bool Down;

    #endregion
    
    public void UpdateTable(GameObject newItem)
    {
        if (newItem != null)
            newItem.GetComponent<RectTransform>().localScale = Vector3.one;

        float height = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            Vector2 size = child.sizeDelta;
            height += Down ? -(1 - child.pivot.y) * size.y : (1 - child.pivot.y) * size.y;
            if (i != 0)
                height += Down ? -Offset : Offset;

            Vector2 newPos = Vector2.zero;

            newPos.y = height;
            newPos.x = 0;//-child.pivot.x * size.x * hi.localScale.x;
            child.anchoredPosition = newPos;
        }
    }
}

