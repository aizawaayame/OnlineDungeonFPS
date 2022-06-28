using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using Protocol.Message;
using ProtoBuf;
using Services;
using Managers;

public class LoadingManager : MonoBehaviour {

    #region Fields
    [SerializeField]
    UnityEngine.GameObject UILoading;
    [SerializeField]
    UnityEngine.GameObject UILogin;

    [SerializeField]
    Slider progressBar;
    [SerializeField]
    Text progressText;

    #endregion
    
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");
        Application.targetFrameRate = 60;
        //初始化面板
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        UILoading.SetActive(true);
        //yield return new WaitForSeconds(1f);
        yield return DataManager.Instance.LoadData();
        MapService.Instance.Init();
        UserService.Instance.Init();
        StatusService.Instance.Init();
        
        //进度条加载
        for (float i = 50; i <100;)
        {
            i += Random.Range(0.1f,1.5f);
            progressBar.value = i;
            progressText.text =((int)i)+"%";
            yield return new WaitForEndOfFrame();
        }
        UILoading.SetActive(false);
        UILogin.SetActive(true);

        yield return null;    
    }

}
