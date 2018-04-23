using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [Header("Main Menu")]
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Difficulty")]
    public GameObject panelDiff;
    public Button buttonEasy;
    public Button buttonNormal;
    public Button buttonHard;

    [Header("Options")]
    public GameObject panelOptions;

    // Use this for initialization
	void Start () {

        startButton.onClick.AddListener(SelectDiff);
        optionsButton.onClick.AddListener(Options);
        quitButton.onClick.AddListener(QuitGame);

        buttonEasy.onClick.AddListener(SetEasy);
        buttonNormal.onClick.AddListener(SetNormal);
        buttonHard.onClick.AddListener(SetHard);


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetEasy()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Easy");
    }

    private void SetNormal()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Normal");
    }

    private void SetHard()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hard");
    }

    private void SelectDiff()
    {
        panelDiff.SetActive(true);
    }

    private void Options()
    {
        panelOptions.SetActive(true);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
