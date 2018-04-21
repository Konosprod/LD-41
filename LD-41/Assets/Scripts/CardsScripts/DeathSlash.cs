using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSlash : Card {

	// Use this for initialization
	void Start () {
		
	}    

    // Update is called once per frame
    void Update()
    {        
        if (isTargeting)
        {
            if(effectInst == null)
            {
                effectInst = Instantiate(effect);
            }

            Vector3 effectPos = GameManager._instance.GetCurrentGhostPosition();

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 12;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            if (mousePos.x > GameManager._instance.GetCurrentGhostPosition().x)
            {
                //Debug.Log("To the right");
                effectPos.x += 2f;
                effectPosOffset = new Vector3(2f, 0f, 0f);
            }
            else
            {
                //Debug.Log("To the left");
                effectPos.x -= 2f;
                effectPosOffset = new Vector3(-2f, 0f, 0f);
            }

            effectInst.transform.position = effectPos;
        }
    }

    public override void Do()
    {
        effectInst = Instantiate(effect, GameManager._instance.GetPlayerPosition() + effectPosOffset, Quaternion.identity);
        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(effectInst);
    }
}
