using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Card
{

    // Use this for initialization
    void Start()
    {
        animationDo = "CrossPunch";
    }

    // Update is called once per frame
    void Update()
    {
        if (isTargeting)
        {
            if (effectInst == null)
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
                effectPosOffset = new Vector3(4f, 1f, 0f);
                effectPos += effectPosOffset;
                correspondingGhostRot = new Vector3(0, 90f, 0);
            }
            else
            {
                //Debug.Log("To the left");
                effectPosOffset = new Vector3(-4f, 1f, 0f);
                effectPos += effectPosOffset;
                correspondingGhostRot = new Vector3(0, -90f, 0);
            }

            effectInst.transform.position = effectPos;
        }
    }

    public override void Do()
    {
        effectInst = Instantiate(effect, GameManager._instance.GetPlayerPosition() + effectPosOffset, Quaternion.identity);
        effectInst.GetComponent<MeshRenderer>().enabled = false;

        effectInst.GetComponent<ParticleSystem>().Play();

        if (SoundManager._instance != null)
        {
            SoundManager._instance.PlaySFX(SFXType.Explosion);
        }

        StartCoroutine(SelfDestroy());
    }

    IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(effectInst);
    }
}
