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
        
        #endregion

        #region Private Methods

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave: characterID {0}:{1}", character.CharacterId, character.CharacterName);
            CharacterManager.Instance.RemoveCharacter(character.CharacterId);
            character.Clear();
            MapManager.Instance[character.MapId].MapCharacterLeave(character);
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
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.userName, request.Passward);
            
            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.FirstOrDefault(u => u.Username == request.userName);
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
                sender.Session.Response.userLogin.nUser = new NUser(){
                    Id = (int)user.ID,
                    Player = new NPlayer()
                    {
                        Id = user.Player.ID
                    }
                };
                foreach (var c in user.Player.Characters)
                {
                    NCharacter info = new NCharacter(){
                        Id = c.ID,
                        Class = (CharacterClass)c.Class,
                        ConfigId = c.TID,
                        Exp = c.Exp,
                        Gold = c.Gold,
                        Level = c.Level,
                        Name = c.Name,
                        mapId = 1,
                    };
                    sender.Session.Response.userLogin.nUser.Player.Characters.Add(info);
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
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.userName, request.Passward);
            sender.Session.Response.userRegister = new UserRegisterResponse();
            
            TUser user = DBService.Instance.Entities.Users.FirstOrDefault(u => u.Username == request.userName);
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.userName, Password = request.Passward, Player = player });
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
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.characterName, request.characterClass);

            TCharacter character = new TCharacter()
            {
                Name = request.characterName,
                Class = (int)request.characterClass,
                TID = (int)request.characterClass,
                Exp = 0,
                Gold = 5000,
                Level = 1,
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
                NCharacter info = new NCharacter(){
                    Id = c.ID,
                    ConfigId = c.TID,
                    Name = c.Name,
                    Class = (CharacterClass)c.Class,
                    Type = (CharacterType.Player),
                };
                message.Response.createChar.nCharacters.Add(info);
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
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1}", dbchar.ID, dbchar.Name);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);
            SessionManager.Instance.AddSession(character.CharacterId, sender);
            sender.Session.Response.gameEnter = new UserGameEnterResponse(){
                Result = Result.Success,
                Errormsg = "None",
                nCharacter = character.NCharacter,
            };
            sender.SendResponse();
       
            sender.Session.Character = character;
            sender.Session.PostResponser = character;
            MapManager.Instance[1].MapCharacterEnter(sender, character);
        }
        
        /// <summary>
        /// UserGameLeaveRequest callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeavaRequest: characterID {0}:{1} Map:{2}", character.CharacterId, character.CharacterName, character.MapId);
            CharacterLeave(character);

            SessionManager.Instance.RemoveSession(character.CharacterId);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse(){
                Errormsg = "None",
                Result = Result.Success
            };
            
            sender.SendResponse();
        }

        #endregion
    }
}
