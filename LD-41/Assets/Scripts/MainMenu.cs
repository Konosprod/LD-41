using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    public GameObject panelOptions;

    // Use this for initialization
	void Start () {

        startButton.onClick.AddListener(StartGame);
        optionsButton.onClick.AddListener(Options);
        quitButton.onClick.AddListener(QuitGame);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test");
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
