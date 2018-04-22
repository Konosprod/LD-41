using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class IAMonster : MonoBehaviour
{

    public enum Type { Aggressive, Ranged };

    public enum State { Idle, MoveToPlayer, Attack, Flee };

    public Type type = Type.Aggressive;

    public State state = State.Idle;

    public float moveSpeed = 1f;
    public float attackRange = 1.5f;
    // Number of seconds between each attack
    public float attackDelay = 2.0f;
    private float currentAttackDelay = 0f;

    public int scoreGiven = 50;

    private Health health;
    private Animator animator;

    private Vector3 playerPos;

    // Use this for initialization
    void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager._instance.isGameOver())
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            // The IA only plays during play turn (how fair)
            if (GameManager._instance.IsPlayTurn())
            {
                // Cooldown for the attack
                currentAttackDelay -= Time.deltaTime;

                playerPos = GameManager._instance.GetPlayerPosition();
                Vector3 diff = playerPos - transform.position;

                if (diff.x > 0)
                {
                    transform.localEulerAngles = new Vector3(0, 90f, 0);
                }
                else if (diff.x < 0)
                {
                    transform.localEulerAngles = new Vector3(0, -90f, 0);
                }

                float distToPlayer = Vector3.Distance(transform.position, playerPos);
                //Debug.Log("Distance : " + distToPlayer + ", hp : " + hp);

                if (!health.dead)
                {
                    // Conditon that makes monsters incapable of hitting you if they are above / below you. You can multiply Mathf.Abs(diff.x) to make the window of no-attack narrower
                    // if (Mathf.Abs(diff.z) > Mathf.Abs(diff.x))
                    if (distToPlayer < attackRange)
                    {
                        // Attack the player
                        animator.SetBool("isMoving", false);
                        if (currentAttackDelay <= 0f)
                        {
                            animator.Play("Kicking");
                            currentAttackDelay = attackDelay;
                        }
                    }
                    else
                    {
                        // Move towards the player
                        //Debug.Log("Distance : " + distToPlayer + ", hp : " + health.hp);
                        animator.SetBool("isMoving", true);

                        Vector3 forwardMove = (transform.forward * moveSpeed * Time.deltaTime);
                        Vector3 sideMove = Vector3.zero;

                        if (transform.eulerAngles.y == 90)
                        {
                            sideMove = (-transform.right * moveSpeed * Time.deltaTime) * Mathf.Sign(diff.z);
                        }
                        else
                        {
                            sideMove = (transform.right * moveSpeed * Time.deltaTime) * Mathf.Sign(diff.z);
                        }

                        /*Debug.Log("diff : x= " + diff.x + ", y= " + diff.y + ", z= " + diff.z);
                        Debug.Log("forwardMove : x= " + forwardMove.x + ", y= " + forwardMove.y + ", z= " + forwardMove.z);
                        Debug.Log("sideMove : x= " + sideMove.x + ", y= " + sideMove.y + ", z= " + sideMove.z);*/

                        if (Mathf.Abs(diff.z) < 0.2f)
                        {
                            transform.position += forwardMove;
                        }
                        else
                        {
                            Vector3 combinedMovement = (forwardMove * Mathf.Abs(diff.x)) / (Mathf.Abs(diff.x) + Mathf.Abs(diff.z)) + (sideMove * Mathf.Abs(diff.z)) / (Mathf.Abs(diff.z) + Mathf.Abs(diff.x));
                            //Debug.Log("combinedMovement : x= " + combinedMovement.x + ", y= " + combinedMovement.y + ", z= " + combinedMovement.z);
                            transform.position += combinedMovement;
                        }
                    }
                }
            }
        }
    }
}
