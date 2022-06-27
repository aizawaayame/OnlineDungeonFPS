using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{
    public ScrollButton scrollButton;
    public GameObject[] shopPages;

    public Text pageText;

    private int page;

    public int Page
    {
        get { return page; }
        set
        {
            page = value;
            pageText.text = (page+1).ToString() + "/3";

        }
    }



    public int index = -1;
    // Start is called before the first frame update
    void Start()
    {
        scrollButton.OnChange += OnPageChange;
        index = 0;
        SelectShopView(index);
    }

    private void OnDestroy()
    {
        scrollButton.OnChange -= OnPageChange;
    }

    private void OnPageChange(bool Isadd)
    {
        if (Isadd)
        {
            if (index + 1 >= shopPages.Length)
            {
                return;
            }
            SelectShopView(index += 1);
        }
        else
        {
            if (index -1 < 0)
            {
                return;
            }
            SelectShopView(index -= 1);
        }
    }
    public void SelectShopView(int idx)
    {
        for (int i = 0; i < shopPages.Length; i++)
        {
            Page = idx;
            shopPages[i].SetActive(i==idx);
        }
    }
}
