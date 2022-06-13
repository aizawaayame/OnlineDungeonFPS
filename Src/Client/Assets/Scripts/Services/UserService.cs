using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Common;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using UI;
using UnityEditor;

namespace Services
{
    public class UserService : Utilities.Singleton<UserService>, IDisposable
    {
        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

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

        void OnGameServerConnect(int result, string reason)
        {
            //Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
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
        
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
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

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);
            UserRegisterEvent userRegisterEvent = GameEvents.userRegisterEvent;
            userRegisterEvent.result = response.Result;
            userRegisterEvent.msg = response.Errormsg;
            EventManager.Broadcast(userRegisterEvent);
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user:{0}  psw:{1}",user,psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
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
        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                Models.User.Instance.Info = response.Userinfo;
            }

            UserLoginEvent userLoginEvent = GameEvents.userLoginEvent;
            userLoginEvent.result = response.Result;
            userLoginEvent.msg = response.Errormsg;
            EventManager.Broadcast(userLoginEvent);

        }

        public void SendUserCreateCharacter(string name, CharacterClass cls)
        {
            Debug.LogFormat("SenderCharacterCreate:{0} {1}",name,cls);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = name;
            message.Request.createChar.Class = cls;

            if (this.connected && NetClient.Instance.Connected)
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

        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0},{1}",response.Result,response.Errormsg);
            if (response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear(); 
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }
            UserCreateCharacterEvent userCreateCharacterEvent = GameEvents.userCreateCharacterEvent;
            userCreateCharacterEvent.result = response.Result;
            userCreateCharacterEvent.msg = response.Errormsg;
            EventManager.Broadcast(userCreateCharacterEvent);
        }
    }
}
