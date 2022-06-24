using System;
using Network;
using Common.Network;
using Entities;
using Models;
using Protocol;
using UnityEngine;
using UI;

namespace Services
{
    public class UserService : Utilities.Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;
        
        #region Fields
        NetMessage pendingMessage = null;
        bool isConnected = false;
        #endregion
        
        #region Constructors&Deconstructor

        public UserService()
        {
            Debug.Log("用户服务加载成功");
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnUserGameLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnUserGameLeave);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        #endregion

        #region Private Methods

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userLogin!=null)
                {
                    this.OnLogin?.Invoke(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                }
                else if(this.pendingMessage.Request.userRegister!=null)
                {

                    this.OnRegister?.Invoke(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
   
                }
                else
                {
                    this.OnCharacterCreate?.Invoke(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                }
                return true;
            }
            return false;
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
            message.Request.userRegister = new UserRegisterRequest(){
                userName = user,
                Passward = psw,
            };

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
            message.Request.userLogin = new UserLoginRequest(){
                userName = user,
                Passward = psw,
            };
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
            message.Request.createChar = new UserCreateCharacterRequest(){
                characterClass = cls,
                characterName = name,
            };
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

        public void SendGameLeave()
        {
            Debug.LogFormat("UserGameLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }
        #endregion
        
        #region Events

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
            OnRegister?.Invoke(response.Result, response.Errormsg);
        }
        
        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                User.Instance.NUser = response.nUser;
            }

            OnLogin?.Invoke(response.Result, response.Errormsg);
        }
        
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0},{1}",response.Result,response.Errormsg);
            if (response.Result == Result.Success)
            {
                User.Instance.NUser.Player.Characters.Clear(); 
                Debug.LogFormat("response返回了{0}个角色",response.nCharacters.Count);
                User.Instance.NUser.Player.Characters.AddRange(response.nCharacters);
            }
            OnCharacterCreate?.Invoke(response.Result, response.Errormsg);
        }

        void OnUserGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                if (response.nCharacter != null)
                {
                    User.Instance.NCharacter = response.nCharacter;
                }
            }
        }

        void OnUserGameLeave(object sender, UserGameLeaveResponse message)
        {
            Debug.LogFormat("UserGameLeaveResponse");
            MapService.Instance.CurrentMapId = 0;
            User.Instance.NCharacter = null;
        }
        #endregion
    }
}
