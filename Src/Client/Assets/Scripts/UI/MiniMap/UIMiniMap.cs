using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap :MonoBehaviour
{
    public Collider miniMapbox;
    public Text mapName;
    public Image miniMap;
    public Image arrow;

    private Transform playerTransform;


    // Start is called before the first frame update
    void Start()
    {
        MinimapManager.Instance.miniMap = this;
        UpdateMap();
    }

    public void UpdateMap()
    {
        
        this.mapName.text = User.Instance.CurrentMapData.Name;
        this.miniMap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
        this.miniMapbox = MinimapManager.Instance.MinimapBoundBox;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (playerTransform==null)
        {
            playerTransform = MinimapManager.Instance.PlayerTransform;
        }
        
        if (miniMapbox==null||playerTransform==null)
        {
            return;
        }
 
        float realWidth = this.miniMapbox.bounds.size.x;
        float realHeight = this.miniMapbox.bounds.size.z;
        
        float relativeX = playerTransform.position.x - this.miniMapbox.bounds.min.x;
        float relativeY = playerTransform.position.z - this.miniMapbox.bounds.min.z;

        float pivotX = relativeX / realWidth;
        float pivotY = relativeY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX,pivotY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles=new Vector3(0,0,-playerTransform.eulerAngles.y);
    }
}
