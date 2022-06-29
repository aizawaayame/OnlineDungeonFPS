using Utilities;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    #region Fields

    public float VerticalBobFrequency = 1f;
    public float BobbingAmount = 1f;
    public float RotatingSpeed = 360f;
    public AudioClip PickupSfx;
    public GameObject PickupVfxPrefab;
    
    Collider selfCollider;
    Vector3 startPosition;
    bool hasPlayedFeedback;
    #endregion

    #region Properties

    public Rigidbody PickupRigidbody { get; private set; }

    #endregion
    
    protected virtual void Start()
    {
        PickupRigidbody = GetComponent<Rigidbody>();
        selfCollider = GetComponent<Collider>();

        PickupRigidbody.isKinematic = true;
        selfCollider.isTrigger = true;

        startPosition = transform.position;
    }

    void Update()
    {

        float bobbingAnimationPhase = ((Mathf.Sin(Time.time * VerticalBobFrequency) * 0.5f) + 0.5f) * BobbingAmount;
        transform.position = startPosition + Vector3.up * bobbingAnimationPhase;

        transform.Rotate(Vector3.up, RotatingSpeed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController pickingPlayer = other.GetComponent<PlayerController>();
        if (pickingPlayer != null)
        {
            OnPicked(pickingPlayer);

            PickupEvent evt = Events.PickupEvent;
            evt.Pickup = gameObject;
            EventUtil.Broadcast(evt);
        }
    }

    protected virtual void OnPicked(PlayerController playerController)
    {
        PlayPickupFeedback();
    }

    public void PlayPickupFeedback()
    {
        if (hasPlayedFeedback)
            return;
        if (PickupVfxPrefab)
        {
            var pickupVfxInstance = Instantiate(PickupVfxPrefab, transform.position, Quaternion.identity);
        }
        hasPlayedFeedback = true;
    }
}

