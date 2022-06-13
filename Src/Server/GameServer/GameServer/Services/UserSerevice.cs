using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {
        #region Constructors
        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnUserCreateCharacter);
        }
        #endregion

        #region Public Methods
        public void Init()
        {

        }
        #endregion

        #region Events
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "注册成功";
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User: {0}", request.User);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;
                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "登录成功";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = (int)user.ID;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach(var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnUserCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserLoginRequest: Name: {0},Class: {1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 0,
                MapPosY = 0,
                MapPosZ = 0
            };

            character = DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "角色创建成功";

            sender.SendResponse();
        }
        #endregion
    }


}

