using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour {

    public Button returnButton;

	// Use this for initialization
	void Start () {
        returnButton.onClick.AddListener(Return);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Return()
    {
        SettingsManager._instance.Save();
        gameObject.SetActive(false);
    }
}
