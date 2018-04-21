using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour {

    public float maxHp = 100;
    public float hp;

    public Image healthBar;

    public bool dead = false;

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        hp = maxHp;
	}
	
	// Update is called once per frame
	void Update () {
        //UpdateHealthBar();
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
            if(gameObject.layer == 9)
            {
                animator.SetTrigger("isDead");
                //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);
                StartCoroutine(OnDeath(animator.GetCurrentAnimatorStateInfo(0).length));
            }
        }
        else
        {
            animator.SetBool("isHit", true);
        }

        UpdateHealthBar();
    }

    private IEnumerator OnDeath(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager._instance.RemoveMonster(gameObject);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name);
        if(GameManager._instance.IsPlayTurn() && gameObject.layer == 9) // 9 = monster
        {
            TakeDamage(other.gameObject.GetComponent<Damage>().damage);
        }
    }
}
