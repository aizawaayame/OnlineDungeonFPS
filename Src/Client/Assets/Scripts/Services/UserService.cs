using System;
using Network;
using Common.Network;
using Modules;
using Protocol;
using UnityEngine;
using Utilities;
using UI;
using Unity.FPS.UI;
using Object = System.Object;

namespace Services
{
    public class UserService : Utilities.Singleton<UserService>, IDisposable
    {
        NetMessage pendingMessage = null;
        bool isConnected = false;

        #region Constructors&Deconstructor

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnUserGameEnter);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            
        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }
        
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.isConnected && NetClient.Instance.IsConnected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        
        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user:{0}  psw:{1}",user,psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.isConnected && NetClient.Instance.IsConnected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        
        public void SendUserCreateCharacter(string name, CharacterClass cls)
        {
            Debug.LogFormat("SenderCharacterCreate:{0} {1}",name,cls);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = name;
            message.Request.createChar.Class = cls;

            if (this.isConnected && NetClient.Instance.IsConnected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        
        public void SendGameEnter(int characterIdx)
        {
            Debug.LogFormat("UserGameEnterRequest::characterId :{0}", characterIdx);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = characterIdx;

            if (this.isConnected && NetClient.Instance.IsConnected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }
        #endregion

        #region Private Methods

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userRegister!=null)
                {
                    UserRegisterEvent userRegisterEvent = GameEvents.userRegisterEvent;
                    userRegisterEvent.result = Result.Failed;
                    userRegisterEvent.msg = string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason);
                    EventManager.Broadcast(userRegisterEvent);
                }
                return true;
            }
            return false;
        }

        #endregion
        
        #region Subscriber

        void OnGameServerConnect(int result, string reason)
        {
            //Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.IsConnected)
            {
                this.isConnected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }
        
        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }
        
        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);
            UserRegisterEvent userRegisterEvent = GameEvents.userRegisterEvent;
            userRegisterEvent.result = response.Result;
            userRegisterEvent.msg = response.Errormsg;
            EventManager.Broadcast(userRegisterEvent);
        }
        
        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                User.Instance.Info = response.Userinfo;
            }

            UserLoginEvent userLoginEvent = GameEvents.userLoginEvent;
            userLoginEvent.result = response.Result;
            userLoginEvent.msg = response.Errormsg;
            EventManager.Broadcast(userLoginEvent);

        }
        
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0},{1}",response.Result,response.Errormsg);
            if (response.Result == Result.Success)
            {
                User.Instance.Info.Player.Characters.Clear(); 
                Debug.LogFormat("response返回了{0}个角色",response.Characters.Count);
                User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }
            UserCreateCharacterEvent userCreateCharacterEvent = GameEvents.userCreateCharacterEvent;
            userCreateCharacterEvent.result = response.Result;
            userCreateCharacterEvent.msg = response.Errormsg;
            EventManager.Broadcast(userCreateCharacterEvent);
        }

        void OnUserGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                if (response.Character != null)
                {
                    User.Instance.CurrentCharacterInfo = response.Character;
                    MapService.Instance.SendMapCharacterEnter(1);
                }
            }
        }
        #endregion
    }
}
