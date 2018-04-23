using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour {

    // Amount of degrees rotated per frame
    public float rotateSpeed = 2.0f;

    // The card that the player gets on pickup
    public GameObject cardPrefab;

    public GameObject pointer;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, rotateSpeed);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 8)
            Debug.LogError("Pickups shouldn't trigger when anything other the player");

        GameManager._instance.PickupCard(this);

        if (pointer != null)
        {
            Destroy(pointer);
        }

        Destroy(gameObject);
    }
}
