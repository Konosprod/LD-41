using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {

    public float fadeTime;
    public Image toFade;

    public GameObject[] slides;

    private bool fadeOut = false;
    private bool fadeIn = false;

    private int currentSlide = 0;
    private float elaspedTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (currentSlide < slides.Length)
        {
            if (elaspedTime >= fadeTime)
            {
                if(!fadeOut && !fadeIn)
                {
                    fadeOut = true;
                }
                else if(fadeOut && !fadeIn)
                {
                    fadeIn = true;
                }
                elaspedTime = 0;
            }
            else
            {
                elaspedTime += Time.deltaTime;
            }

            if(!fadeOut)
            {
                float percentage = elaspedTime / fadeTime;
                //Debug.Log(Mathf.Lerp(1, 0, percentage));
                toFade.color = new Color(toFade.color.r, toFade.color.g, toFade.color.b, Mathf.Lerp(1, 0, percentage));
            }
            else if(!fadeIn)
            {
                float percentage = elaspedTime / fadeTime;
                toFade.color = new Color(toFade.color.r, toFade.color.g, toFade.color.b, Mathf.Lerp(0, 01, percentage));
            }

            if(fadeIn && fadeOut)
            {
                fadeIn = false;
                fadeOut = false;
                elaspedTime = 0;
                slides[currentSlide].gameObject.SetActive(false);
                currentSlide++;
            }
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
	}
}
