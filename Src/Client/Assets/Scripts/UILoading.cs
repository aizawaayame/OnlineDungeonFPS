using System.Collections;
using Managers;
using Services;
using UnityEngine;
using UnityEngine.UI;


public class UILoading : MonoBehaviour
{
    public GameObject UILoad;
    public GameObject UILogin;

    public Slider progressBar;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        UILogin.SetActive(false);
        UILoad.SetActive(true);
        yield return new WaitForSeconds(1f);


        yield return DataManager.Instance.LoadData();
        //Init basic services
        MapService.Instance.Init();
        UserService.Instance.Init();

        for (float i = 0; i < 100;)
        {
            i += Random.Range(0.1f, 1.5f);
            progressBar.value = i;
            yield return new WaitForEndOfFrame();
        }

        UILoad.SetActive(false);
        UILogin.SetActive(true);
        yield return null;

    }


}


