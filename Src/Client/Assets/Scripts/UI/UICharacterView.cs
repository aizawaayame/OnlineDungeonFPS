using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {

    public GameObject[] characters;

    int _currentCharacter = 0;

    public int CurrentCharacter
    {
        get {return _currentCharacter; }
        set
        {
            _currentCharacter = value;
            Debug.LogFormat("当前选择的对象为第{0}个",_currentCharacter);
            this.UpdateCharacter();
            
        }
    }

    private void UpdateCharacter()
    {
        for (int i = 0; i < 3; i++)
        {

            this.characters[i].SetActive(i==this.CurrentCharacter);
        }
    }
    
}
