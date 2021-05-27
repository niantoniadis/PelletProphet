using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    Renderer renderer2;
    UIManager uIManager;
    Animator bossAnimator;
    int shootChoice;

    Transform left;
    Transform right;

    bool active;
    bool enraged = false;
    bool rotated = false;

    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        health = 75;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        left = transform.GetChild(0);
        right = transform.GetChild(1);

        renderer = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        renderer2 = transform.GetChild(1).GetChild(0).GetComponent<Renderer>();
        originalMat = renderer.material;

        uIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        uIManager.UpdateBossHealth(health);

        bossAnimator = GetComponent<Animator>();

        shootChoice = bossAnimator.GetInteger("ShootChoice");
    }

    private void Update()
    {
        shootChoice = bossAnimator.GetInteger("ShootResult");
        Debug.Log(shootChoice);

        DisplayHit();
        if (!enraged)
        {
            if (health <= 50)
            {
                enraged = true;
                bossAnimator.SetBool("Phase2", true);
            }

            if (bossAnimator.GetBool("Fire") && shootChoice == 0)
            {
                Fire(transform.right);
                bossAnimator.SetBool("Fire", false);
            }
            else if (bossAnimator.GetBool("Fire"))
            {
                Fire(-transform.right);
                bossAnimator.SetBool("Fire", false);
            }
        }
        else
        {
            switch (shootChoice)
            {
                case 0:
                    if (bossAnimator.GetBool("Fire"))
                    {
                        Fire(transform.right);
                        Fire(-transform.right);
                        bossAnimator.SetBool("Fire", false);
                    }
                    break;
                case 1:
                    if (bossAnimator.GetBool("Fire"))
                    {
                        Fire(transform.right);
                        bossAnimator.SetBool("Fire", false);
                    }
                    if (bossAnimator.GetBool("SlideFire"))
                    {
                        StartCoroutine(SlideFire(bossAnimator.GetCurrentAnimatorStateInfo(0), false, true));
                        bossAnimator.SetBool("SlideFire", false);
                    }
                    break;
                case 2:
                    if (bossAnimator.GetBool("Fire"))
                    {
                        Fire(-transform.right);
                        bossAnimator.SetBool("Fire", false);
                    }
                    if (bossAnimator.GetBool("SlideFire"))
                    {
                        StartCoroutine(SlideFire(bossAnimator.GetCurrentAnimatorStateInfo(0), true, false));
                        bossAnimator.SetBool("SlideFire", false);
                    }
                    break;
                case 3:
                    if (bossAnimator.GetBool("SlideFire"))
                    {
                        StartCoroutine(SlideFire(bossAnimator.GetCurrentAnimatorStateInfo(0), true, true));
                        bossAnimator.SetBool("SlideFire", false);
                    }
                    break;
            }
            
            if(health <= 0)
            {
                Destroy(gameObject);
                //Death()
            }
        }
        
    }

    public void Fire(Vector3 dir)
    {
        Vector3 firePos;

        Rigidbody shotPellet;

        for (int i = -8; i <= 8; i++)
        {
                firePos = transform.position - dir * 9;
                firePos.z = firePos.z + i;
                shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
                shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
        }
    }

    //void SlideFire(int fireMode)
    //{
    //    Rigidbody shotPellet;
    //    Vector3 dir;
    //    Vector3 firePos;
    //
    //    switch (fireMode)
    //    {
    //        case 0:
    //            dir = player.position - left.position;
    //            dir.Normalize();
    //            firePos = left.position + dir;
    //            shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
    //            shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
    //            break;
    //        case 1:
    //            dir = player.position - right.position;
    //            dir.Normalize();
    //            firePos = right.position + dir;
    //            shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
    //            shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
    //            break;
    //        case 2:
    //            dir = player.position - left.position;
    //            dir.Normalize();
    //            firePos = left.position + dir;
    //            shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
    //            shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
    //
    //            dir = player.position - right.position;
    //            dir.Normalize();
    //            firePos = right.position + dir;
    //            shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
    //            shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
    //            break;
    //    }
    //    
    //}

    IEnumerator SlideFire(AnimatorStateInfo currStateInfo, bool leftFiring, bool rightFiring)
    {
        float duration = currStateInfo.length * (1 - currStateInfo.normalizedTime);
        float shotInterval = 0;
        Vector3 dir;
        Vector3 firePos;
        Rigidbody shotPellet;

        if (leftFiring && rightFiring)
        {
            while (duration >= 0)
            {
                duration -= Time.deltaTime;
                shotInterval -= Time.deltaTime;
                
                if (shotInterval <= 0)
                {
                    shotInterval = 0.1f;
                    dir = player.position - left.position;
                    dir.Normalize();
                    firePos = left.position + dir;
                    shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
                    shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);

                    dir = player.position - right.position;
                    dir.Normalize();
                    firePos = right.position + dir;
                    shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
                    shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
                }

                yield return null;
            }
        }
        else if (rightFiring)
        {
            while (duration >= 0)
            {
                duration -= Time.deltaTime;
                shotInterval -= Time.deltaTime;

                if (shotInterval <= 0)
                {
                    shotInterval = 0.1f;
                    dir = player.position - right.position;
                    dir.Normalize();
                    firePos = right.position + dir;
                    shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
                    shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
                }
                yield return null;
            }
        }
        else if (leftFiring)
        {
            while (duration >= 0)
            {
                duration -= Time.deltaTime;
                shotInterval -= Time.deltaTime;

                if (shotInterval <= 0)
                {
                    shotInterval = 0.1f;
                    dir = player.position - left.position;
                    dir.Normalize();
                    firePos = left.position + dir;
                    shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
                    shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);
                }
                yield return null;
            }

        }
    }

    protected override void UpdateRotation()
    {

    }

    protected override void DisplayHit()
    {
        if (hitBuffer >= 0)
        {
            hitBuffer -= Time.deltaTime;
            if (hitBuffer > 0.75f)
            {
                renderer.material = hitMat;
                renderer2.material = hitMat;
            }
            else if (hitBuffer > 0.05f)
            {
                renderer.material = hitMat;
                renderer2.material = hitMat;
            }
            else if (hitBuffer > 0.25f)
            {
                renderer.material = hitMat;
                renderer2.material = hitMat;
            }
            else
            {
                renderer.material = originalMat;
                renderer2.material = originalMat;
            }
        }
    }

    public override void TakeHit(Vector3 hitPoint)
    { 
        if (hitBuffer <= 0)
        {
            health--;
            hitBuffer = 0.1f;
            Vector3 particleDir = hitPoint - transform.position;
            Instantiate(hitParticles, hitPoint, Quaternion.LookRotation(-particleDir, Vector3.up));
            //StartCoroutine(Freeze(1f));

            if (health <= 0)
            {
                //Instantiate(deathParticles, new Vector3(transform.position.x, hitPoint.y, transform.position.z), Quaternion.Euler(-90, 0, 0));
            }
        }
        uIManager.UpdateBossHealth(health);
    }
}
