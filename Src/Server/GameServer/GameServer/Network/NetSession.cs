using Common.Network;
using GameServer.Entities;
using GameServer.Services;
using GameServer;
using Protocol;

namespace Network
{
    class NetSession : INetSession
    {

        #region Fields

        NetMessage response;

        #endregion
        
        #region Public Properties

        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }
                return response.Response;
            }
        }
        
        #endregion

        #region Public Methods

        public void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(Response);
                }

                byte[] data = PackageHandler.PackMessage(response);
                response = null;
                return data;
            }
            return null;
        }
        #endregion
    }
}
