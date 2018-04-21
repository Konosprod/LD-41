﻿using System.Collections;
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
        // The IA only plays during play turn (how fair)
        /*if (GameManager._instance.IsPlayTurn())
        {*/
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
            if (distToPlayer < attackRange)
            {
                // Attack the player
                animator.SetBool("isMoving", false);
                animator.Play("Kicking");

            }
            else
            {
                // Move towards the player
                //Debug.Log("Distance : " + distToPlayer + ", hp : " + health.hp);
                animator.SetBool("isMoving", true);
                transform.position += (transform.forward * moveSpeed * Time.deltaTime);
            }
        }
        //}
    }
}
