
using Utilities;

namespace Modules
{
    public class User : Singleton<User>
    {
        public SkillBridge.Message.NUserInfo Info { get; set; }
        public SkillBridge.Message.NCharacterInfo CurrentCharacterInfo { get; set; }
    }
}
