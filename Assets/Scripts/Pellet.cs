using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Pellet : MonoBehaviour
{
    bool targetReached = false;
    Rigidbody body;
    Vector3 dir;
    Transform player;
    Animator animator;

    float seekSpeed = 10f;
    float maxSpeed = 20f;
    bool absorb = false;
    bool homing = false;
    bool enlarged = false;

    public Vector3 Dir
    {
        set
        {
            dir = value;
        }
    }

    public Transform Player
    {
        set
        {
            player = value;
        }
    }

    public bool Absorb
    {
        set
        {
            absorb = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.SetBool("targetReached", targetReached);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Movement();
    }
    void Movement()
    {
        if (targetReached)
        {
            Seek(player.position);
            if (absorb)
            {
                MoveToPlayer();
            }
        }
        else
        {
            body.velocity = dir.normalized * maxSpeed;
        }
        body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
    }

    void Seek(Vector3 target)
    {
        Vector3 desiredVelocity = target - transform.position;
        Vector3 forceDir = Vector3.zero;
        forceDir = desiredVelocity - body.velocity;

        body.AddForce(forceDir * seekSpeed);
    }

    void MoveToPlayer()
    {
        Vector3 desiredVelocity = player.position - transform.position;

        body.AddForce(desiredVelocity.normalized * seekSpeed * 20);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !targetReached)
        {
            gameObject.tag = "HeavyPellet";
            gameObject.layer = 13;
            targetReached = true;
            animator.SetTrigger("targetReached");
            body.velocity = -body.velocity/2;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().TakeHit(transform.position);
            }
        }
    }
}
