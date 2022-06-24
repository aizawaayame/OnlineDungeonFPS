using System;
using System.Collections.Generic;
using Common;
using Protocol;
using UnityEngine;

namespace Managers
{
    class UIElement
    {
        public string Resouces { get; set; }
        public bool IsCached { get; set; }
        public GameObject Instance { get; set; }
    }


    public class UIManager : Singleton<UIManager>
    {
        #region Fields

        Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

        #endregion

        #region Constructor&Deconstrucor

        public UIManager()
        {
            
        }
        
        ~UIManager()
        {

        }
        #endregion

        #region Public Mothods

        public T Show<T>()
        {
            Type type = typeof(T);
            if (this.UIResources.ContainsKey(type))
            {
                UIElement element = this.UIResources[type];
                if (element.Instance != null)
                {
                    element.Instance.SetActive(true);
                }
                else
                {
                    UnityEngine.Object instance = Resources.Load(element.Resouces);
                    if (instance == null)
                    {
                        return default(T);
                    }
                    return element.Instance.GetComponent<T>();
                }
            }
            return default(T);
        }

        public void Close(Type type)
        {
            if (this.UIResources.ContainsKey(type))
            {
                UIElement element = this.UIResources[type];
                if (element.IsCached)
                {
                    element.Instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(element.Instance);
                    element.Instance = null;
                }
            }
        }
        #endregion

    }
}
