
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIStanceHUD : MonoBehaviour
{

    #region Fields

    [SerializeField] Image StanceImage;
    [SerializeField]  Sprite StandingSprite;
    [SerializeField]  Sprite CrouchingSprite;

    #endregion


    void Start()
    {
        PlayerController player = User.Instance.CurrentCharacterObject.GetComponent<PlayerController>();
        player.onStanceChanged += OnStanceChanged;
        OnStanceChanged(player.IsCrouching);
    }

    void OnStanceChanged(bool crouched)
    {
        StanceImage.sprite = crouched ? CrouchingSprite : StandingSprite;
    }
}

