using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour {

    public float maxHp = 100;
    public float hp;

    public float maxShield = 100;
    public float shield = 0; 

    public Image healthBar;
    public Image shieldBar;

    public bool dead = false;

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        hp = maxHp;
        shield = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(transform.position.y < -100f)
        {
            GameManager._instance.RemoveMonster(gameObject);
            Destroy(gameObject);
        }
        //UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        //Debug.Log("hp : " + hp + ", maxHp : " + maxHp + ", value : " + hp / maxHp);
        healthBar.fillAmount = hp / maxHp;
    }

    private void UpdateShieldBar()
    {
        if (shieldBar)
        {
            if (shield > 0)
            {
                shieldBar.transform.parent.gameObject.SetActive(true);
                shieldBar.fillAmount = shield / maxShield;
            }
            else
            {
                shieldBar.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void Heal(int heal)
    {
        if (hp + heal <= maxHp)
            hp += heal;
        else
            hp = maxHp;

        UpdateHealthBar();
    }

    public void AddShield(int shield)
    {
        if (this.shield + shield <= maxShield)
        {
            if (shield == 0)
                shieldBar.transform.parent.gameObject.SetActive(false);

            this.shield += shield;
        }
        else
            this.shield = maxShield;

        UpdateShieldBar();
    }

    public void TakeDamage(int damage)
    {
        if (GameManager._instance.IsPlayTurn())
        {
            if (shield - damage < 0)
            {
                float left = damage - shield;
                shield = 0;
                hp -= left;
            }
            else
            {
                shield -= damage;
            }

            if (hp <= 0)
            {
                hp = 0;
                dead = true;
                if (gameObject.layer == 9) // Monster
                {
                    animator.SetBool("isDead", true);
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    gameObject.GetComponent<Collider>().enabled = false;
                    GameManager._instance.GainScore(gameObject.GetComponent<IAMonster>().scoreGiven);
                    GameManager._instance.RemoveMonster(gameObject);
                    StartCoroutine(WaitForFrameToFinish());
                }

                if (gameObject.layer == 8)
                {
                    GameManager._instance.Lose();
                }
            }
            else
            {
                if (gameObject.layer == 9)
                    animator.Play("ZombieReactionHit");
                else
                    animator.Play("Reaction");

            }
        }

        UpdateShieldBar();
        UpdateHealthBar();
    }

    private IEnumerator WaitForFrameToFinish()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(OnDeath(animator.GetCurrentAnimatorStateInfo(0).length));
    }

    private IEnumerator OnDeath(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(gameObject.layer == 9) // 9 = monster
        {
            TakeDamage(other.gameObject.GetComponent<Damage>().damage);
        }
    }
}
