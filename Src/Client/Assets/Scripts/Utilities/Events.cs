using SkillBridge.Message;

namespace Utilities
{
    public static class GameEvents
    {
        public static UserRegisterEvent userRegisterEvent = new UserRegisterEvent();
        public static UserLoginEvent userLoginEvent = new UserLoginEvent();
        public static UserCreateCharacterEvent userCreateCharacterEvent = new UserCreateCharacterEvent();
    }

    # region GameEvents definition
    public class UserRegisterEvent : GameEvent
    {
        public SkillBridge.Message.Result result;
        public string msg;
    }
    
    public class UserLoginEvent : GameEvent
    {
        public SkillBridge.Message.Result result;
        public string msg;
    }

    public class UserCreateCharacterEvent : GameEvent
    {
        public SkillBridge.Message.Result result;
        public string msg;
    }
    # endregion

}
