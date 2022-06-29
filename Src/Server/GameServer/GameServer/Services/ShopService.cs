using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using Protocol.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class ShopService : Singleton<ShopService>
    {
        public ShopService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BuyWeaponRequest>(this.OnBuyWeapon);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GoldAddRequest>(this.OnAddGold);

        }

        public void Init()
        {

        }

        private void OnAddGold(NetConnection<NetSession> sender, GoldAddRequest message)
        {
           
            long gold = sender.Session.Character.Gold;
            Log.InfoFormat($"AddGold:{gold}");
            gold += message.goldAdd;
            sender.Session.Character.Gold = gold;
            int dbid = sender.Session.Character.Id;
            TCharacter dbchar = DBService.Instance.Entities.Characters.ElementAt(dbid);
            dbchar.Gold = gold;
            DBService.Instance.Entities.SaveChanges();

        }

        private void OnBuyWeapon(NetConnection<NetSession> sender, BuyWeaponRequest message)
        {
            long gold = sender.Session.Character.Gold;
            Log.InfoFormat($"BuyWeapon:{gold}");
            gold -= message.goldCost;
            sender.Session.Character.Gold = gold;
            int dbid = sender.Session.Character.Id;
            TCharacter dbchar = DBService.Instance.Entities.Characters.ElementAt(dbid);
            dbchar.Gold = gold;
            DBService.Instance.Entities.SaveChanges();
        }
    }
}
