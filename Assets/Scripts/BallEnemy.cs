using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BallEnemy : Enemy
{
    EnemyState currState;
    Vector3[] points;
    int nextIndex;
    float attackTime = 1f;
    float shootRotation;
    Vector3 dir;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        size = transform.localScale.x;
        moveSpeed = 200f;
        currState = EnemyState.Moving;
        points = new Vector3[2];

        health = 4;

        if (Random.Range(0, 5000) >= 2500)
        {
            dir = new Vector3(0,0,1);
        }
        else
        {
            dir = new Vector3(1, 0, 0);
        }

        RaycastHit posHit;
        RaycastHit negHit;
        int layerMask = 1 << 10;

        Physics.Raycast(transform.position, dir, out posHit, Mathf.Infinity, layerMask);
        Physics.Raycast(transform.position, -dir, out negHit, Mathf.Infinity, layerMask);

        points[0] = posHit.point - dir * size * 2;
        points[1] = negHit.point + dir * size * 2;
        pellets = Mathf.FloorToInt(Random.Range(0f, 2f));
        shootRotation = Random.Range(0,180);
        transform.eulerAngles = new Vector3(0, shootRotation, 0);
        shotCooldown = 0.5f;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if(currState == EnemyState.Moving)
        {
            bool pointReached = MoveTowards(points[nextIndex]);
            if (pointReached)
            {
                nextIndex++;
                if (nextIndex == points.Length)
                {
                    nextIndex = 0;
                }

                currState = EnemyState.Attacking;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DisplayHit();

        switch (currState)
        {
            case EnemyState.Attacking:
                attackTime -= Time.deltaTime;
                if(shotCooldown == 0.5f)
                {
                    ShootOutwards(6, transform.eulerAngles.y);
                }
                else if(shotCooldown <= 0)
                {
                    ShootOutwards(6, transform.eulerAngles.y);
                    shotCooldown = 8f;
                }
                shotCooldown -= Time.deltaTime;
                if(attackTime <= 0)
                {
                    attackTime = 1f;
                    shotCooldown = 0.5f;
                    currState = EnemyState.Moving;
                }
                break;
        }
        UpdateHeight();
    }

    bool MoveTowards(Vector3 pos)
    {
        if((pos - transform.position).sqrMagnitude <= 1)
        {
            return true;
        }
        Vector3 velocity = pos - transform.position;
        body.AddForce(velocity.normalized * moveSpeed, ForceMode.Force);
        return false;
    }

    

    protected override void UpdateRotation()
    {
        throw new System.NotImplementedException();
    }
}
