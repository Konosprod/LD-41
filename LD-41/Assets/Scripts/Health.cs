using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour {

    public float maxHp = 100;
    public float hp;

    public Slider healthBar;

    public bool dead = false;

	// Use this for initialization
	void Start () {
        hp = maxHp;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateHealthBar();
	}

    private void UpdateHealthBar()
    {
        //Debug.Log("hp : " + hp + ", maxHp : " + maxHp + ", value : " + hp / maxHp);
        healthBar.value = hp / maxHp;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if(hp<=0)
        {
            hp = 0;
            dead = true;
            if(gameObject.layer == 9)
            {
                Destroy(gameObject);
            }
        }

        UpdateHealthBar();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name);
        if(GameManager._instance.isPlayTurn() && gameObject.layer == 9) // 9 = monster
        {
            TakeDamage(other.gameObject.GetComponent<Damage>().damage);
        }
    }
}
