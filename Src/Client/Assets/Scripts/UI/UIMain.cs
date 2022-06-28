using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{

    #region Fields
    
    [SerializeField] TMP_Text avatarText;
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text healthText;
    
    HealthController health;
    #endregion

    void Start()
    {
        health = User.Instance.CurrentCharacterObject.GetComponent<HealthController>();
        
        this.UpdateAvatar();

        health.onDamaged += OnTakeDamage;
        health.onHealed += OnHealed;
    }

    void OnDestroy()
    {
        health.onDamaged -= OnTakeDamage;
        health.onHealed -= OnHealed;
    }
    private void UpdateAvatar()
    {
        this.avatarText.text = string.Format("{0}，{1}", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Level);
        
    }
    
 
    #region Events

    void OnHealed(float amount)
    {
        
    }

    void OnTakeDamage(float dmg, GameObject damageSource)
    {
        
    }

    #endregion

    public void OnClicBackSelectCharacter()
    {
        
        SceneManager.Instance.LoadScene("CharSelect");
        UserService.Instance.SendGameLeave();
    }

    public void OnClickTest()
    {
        //UIManager.Instance.Show<UITest>();
    }
    
}
