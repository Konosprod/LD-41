using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Card : MonoBehaviour
{

    public Vector3 targetDirection;
    public GameObject targetCharacter;
    public GameObject effect; // Both visual and trigger collider if needed + damage if applicable
    public GameObject effectInst; // Instance of the prefab above when needed
    public bool isTargeting = false;
    public Vector3 correspondingGhostPos;
    public Vector3 correspondingGhostRot;
    public Vector3 effectPosOffset;

    public string animationDo;

    // The card does what it has to do
    public abstract void Do();

    void OnDestroy()
    {
        if (effectInst != null)
            Destroy(effectInst);
    }
}
