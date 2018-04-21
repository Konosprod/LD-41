using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public int maxHp = 100;
    private int hp;

    public Image healthBar;

    public bool dead = false;

	// Use this for initialization
	void Start () {
        healthBar = GameObject.Find("HealthBar").GetComponentsInChildren<Image>()[1];
        hp = maxHp;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateHealthBar();
	}

    private void UpdateHealthBar()
    {
        //Debug.Log("hp : " + hp + ", maxHp : " + maxHp + ", value : " + hp / maxHp);
        healthBar.fillAmount = hp / maxHp;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if(hp<=0)
        {
            hp = 0;
            dead = true;
        }

        UpdateHealthBar();
    }
}
