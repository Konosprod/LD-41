using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

    public GameObject target;

    private Vector3 offset;
    

    public void SetTarget(GameObject tar)
    {
        target = tar;
        offset = transform.position - tar.transform.position;
    }

	// Update is called once per frame
	void Update () {
	    if(target != null)
        {
            transform.position = target.transform.position + offset;
        }
	}
}
