using System;
using UnityEngine;
using Utilities;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameFlowManager : MonoSingleton<GameFlowManager>
    {

        #region Fields

        public float EndSceneLoadDelay = 3f;
        public CanvasGroup EndGameFadeCanvasGroup;
        public string WinSceneName = "WinScene";
        public float DelayBeforeFadeToBlack = 4f;
        public string WinGameMessage;
        public float DelayBeforeWinMessage = 2f;
        public AudioClip VictorySound;
        public string LoseSceneName = "LoseScene";

        float timeLoadEndGameScene;
        string sceneToLoad;
        
        #endregion

        #region Properties

        public bool GameIsEnding { get; private set; }

        #endregion

        protected override void OnStart()
        {
            EventUtil.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventUtil.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }

        void Update()
        {
            if (GameIsEnding)
            {
                float timeRatio = 1 - (timeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
                EndGameFadeCanvasGroup.alpha = timeRatio;
                if (Time.time >= timeLoadEndGameScene)
                {
                    SceneManager.Instance.LoadScene(sceneToLoad);
                    GameIsEnding = false;
                }
            }
        }
        void OnDestroy()
        {
            EventUtil.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventUtil.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
        
        void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt) => EndGame(true);
        void OnPlayerDeath(PlayerDeathEvent evt) => EndGame(false);

        void EndGame(bool win)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameIsEnding = true;
            EndGameFadeCanvasGroup.gameObject.SetActive(true);
            if (win)
            {
                sceneToLoad = WinSceneName;
                timeLoadEndGameScene = Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;
                
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = VictorySound;
                audioSource.playOnAwake = false;
                audioSource.PlayScheduled(AudioSettings.dspTime + DelayBeforeWinMessage);

                DisplayMessageEvent displayMessageEvent = Events.DisplayMessageEvent;
                displayMessageEvent.Message = WinGameMessage;
                displayMessageEvent.DelayBeforeDisplay = DelayBeforeWinMessage;
                EventUtil.Broadcast(displayMessageEvent);
            }
            else
            {
                sceneToLoad = LoseSceneName;
                timeLoadEndGameScene = Time.time + EndSceneLoadDelay;
            }
        }

    }
}
