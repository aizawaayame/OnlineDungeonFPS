﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    class UIElement
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        /*UIResources.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest",Cache = true });
        UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
        UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
        UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
        UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });
        UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
        UIResources.Add(typeof(UIFriend), new UIElement() { Resources = "UI/UIFriends", Cache = false });*/
    }

    ~UIManager()
    {

    }

    public T Show<T>()
    {
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Instance!=null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab==null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }


}
