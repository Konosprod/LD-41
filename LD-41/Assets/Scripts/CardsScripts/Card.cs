using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public abstract class Card : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{

    public Vector3 targetDirection;
    public GameObject targetCharacter;
    public bool isTargeting = false;

    private Image sprite;
    private Color targetColor;
    private Color originalColor;

    public void Awake()
    {
        sprite = GetComponent<Image>();
        targetColor = originalColor = sprite.color;
    }

    public void Update()
    {
        if (sprite != null)
            sprite.color = Vector4.MoveTowards(sprite.color, targetColor, Time.deltaTime * 10f);
    }

    // The card does what it has to do
    public abstract void Do();

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("I was clicked : " + gameObject.name);
        targetColor = Color.blue;
        GameManager._instance.SelectCardInHand(gameObject);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Enter mouse-over : " + gameObject.name);
        if (GameManager._instance.selectedCard != gameObject)
            targetColor = Color.green;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Exit mouse-over : " + gameObject.name);
        if(GameManager._instance.selectedCard != gameObject)
            targetColor = originalColor;
    }

    public void DeselectCard()
    {
        targetColor = originalColor;
    }
}
