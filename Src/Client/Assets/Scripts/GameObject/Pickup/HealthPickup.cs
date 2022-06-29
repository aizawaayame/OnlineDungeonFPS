
public class HealthPickup : Pickup
{
    public float HealAmount = 200;
    
    protected override void OnPicked(PlayerController player)
    {
        HealthController playerHealth = player.GetComponent<HealthController>();
        if (playerHealth && playerHealth.CanPickup)
        {
            playerHealth.Heal(HealAmount);
            PlayPickupFeedback();
            Destroy(gameObject);
        }
    }
}

