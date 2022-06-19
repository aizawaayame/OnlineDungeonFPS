using System.Linq;
using Common;
using Common.Network;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using Protocol;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        #region Public Methods

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }


        public void Init()
        {

        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: characterID {0}:{1}", character.Id, character.Info.Name);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
        #endregion

        #region Events
        
        /// <summary>
        /// UserLoginRequest callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);
            
            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.FirstOrDefault(u => u.Username == request.User);
            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo{
                        Id = c.ID,
                        Name = c.Name,
                        Class = (CharacterClass)c.Class,
                        Type = CharacterType.Player,
                    };
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }

            }
            sender.SendResponse();
        }

        /// <summary>
        /// UserRegisterRequest callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);
            sender.Session.Response.userRegister = new UserRegisterResponse();
            
            TUser user = DBService.Instance.Entities.Users.FirstOrDefault(u => u.Username == request.User);
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }
            sender.SendResponse();
        }

        /// <summary>
        /// UserCreateCharacterRequest callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        // ReSharper disable once MemberCanBeMadeStatic.Local
        void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 6471,
                MapPosY = 2225,
                MapPosZ = 4386,
            };
            
            DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";

            sender.Session.Response.createChar = new UserCreateCharacterResponse()
            {
                Result = Result.Success,
                Errormsg = "None"
            };
            
            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo{
                    Id = c.ID,
                    Name = c.Name,
                    Class = (CharacterClass)c.Class,
                    Type = (CharacterType.Player),
                };
                message.Response.createChar.Characters.Add(info);
            }
            sender.SendResponse();
        }

        /// <summary>
        /// UserGameEnterRequest callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);
            SessionManager.Instance.AddSession(character.Id, sender);
            sender.Session.Response.gameEnter = new UserGameEnterResponse(){
                Result = Result.Success,
                Errormsg = "None",
                Character = character.Info
            };
            sender.SendResponse();
            
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
        }
        
        /// <summary>
        /// UserGameLeaveRequest callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeavaRequest: characterID {0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);
            CharacterLeave(character);

            SessionManager.Instance.RemoveSession(character.Id);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse(){
                Errormsg = "None",
                Result = Result.Success
            };
            
            sender.SendResponse();
        }

        #endregion
        
  
    }
}
