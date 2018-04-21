using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Image))]
public class CardInHand : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    private Image sprite;
    private Color targetColor;
    private Color originalColor;

    public Card card;

    public void Awake()
    {
        sprite = GetComponent<Image>();
        targetColor = originalColor = sprite.color;
    }
        
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (sprite != null)
            sprite.color = Vector4.MoveTowards(sprite.color, targetColor, Time.deltaTime * 10f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("I was clicked : " + gameObject.name);
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
        if (GameManager._instance.selectedCard != gameObject)
            targetColor = originalColor;
    }


    public void SelectCard()
    {
        targetColor = Color.blue;
        card.isTargeting = true;
    }
    public void DeselectCard()
    {
        targetColor = originalColor;
        card.isTargeting = false;
        if (card.effectInst != null)
            Destroy(card.effectInst);
    }
}
