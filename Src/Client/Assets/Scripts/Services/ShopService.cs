
using System;
using Network;
using Protocol.Message;

public class ShopService : Singleton<ShopService>, IDisposable  
{
    public ShopService()
    {
    }
    public void Dispose()
    {
    }
    public void Init()
    {
        
    }
    public void SendBuyWeapon(int cost)
    {
        NetMessage message = new NetMessage();
        message.Request = new NetMessageRequest();
        message.Request.buyWeapon = new BuyWeaponRequest();
        message.Request.buyWeapon.goldCost = cost;
        NetClient.Instance.SendMessage(message);
    }
    public void SendAddGold(int gold)
    {
        NetMessage message = new NetMessage();
        message.Request = new NetMessageRequest();
        message.Request.buyWeapon = new BuyWeaponRequest();
        message.Request.buyWeapon.goldCost = gold;
        NetClient.Instance.SendMessage(message);
    }
}

