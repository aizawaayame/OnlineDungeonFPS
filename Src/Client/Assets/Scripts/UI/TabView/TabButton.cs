using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    private Sprite normalSprite;

    public TabView tabView;
    public bool selected=false;

    private Image tableImage;
    public int tabIndex=-1;

    // Start is called before the first frame update
    void Start()
    {
        this.tableImage = this.gameObject.GetComponent<Image>();
        normalSprite = tableImage.sprite;
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Select(bool select)
    {
        tableImage.overrideSprite = select ? activeImage : normalSprite;
    }

    void  OnClick()
    {
        tabView.SelectTab(this.tabIndex);
    }
}
