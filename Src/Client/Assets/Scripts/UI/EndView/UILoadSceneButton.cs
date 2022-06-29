

using UnityEngine;
using UnityEngine.EventSystems;

public class UILoadSceneButton : MonoBehaviour
{
    public string SceneName = "";

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject
            && Input.GetButtonDown("Submit"))
        {
            LoadTargetScene();
        }
    }

    public void LoadTargetScene()
    {
        SceneManager.Instance.LoadScene(SceneName);
    }
}

