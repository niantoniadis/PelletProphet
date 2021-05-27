using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState { Moving, Attacking };
public abstract class Enemy : MonoBehaviour
{
    public GameObject enemyPelletPrefab;
    public Material hitMat;
    public ParticleSystem hitParticles;
    public ParticleSystem deathParticles;
    protected Material originalMat;
    protected Renderer renderer;
    protected Rigidbody body;
    protected int health = 5;
    protected float hitBuffer = 0f;
    protected int pellets;
    protected float size;
    protected float shotSpeed = 5f;
    protected float moveSpeed = 10f;
    protected float seekSpeed;
    protected float shotCooldown;
    protected Transform player;

    public int Pellets
    {
        get
        {
            return pellets;
        }
    }

    public int Health
    {
        get
        {
            return health;
        }
    }

    protected void Start()
    {
        renderer = GetComponent<Renderer>();
        originalMat = renderer.material;
        body = GetComponent<Rigidbody>();
    }

    protected virtual void ShootTowards(Vector3 dir)
    {
        Vector3 firePos = transform.position + dir.normalized * size;

        Rigidbody shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(dir.normalized * shotSpeed, ForceMode.Impulse);
    }

    protected virtual void ShootOutwards(int bulletAmt, float startAngle)
    {
        float angularStep = 360f / bulletAmt;
        float currAngle = startAngle;

        for(int i = 0; i < bulletAmt; i++)
        {
            Vector3 dir = new Vector3(Mathf.Cos(currAngle * Mathf.Deg2Rad), 0, Mathf.Sin(currAngle * Mathf.Deg2Rad));

            Vector3 firePos = transform.position + dir.normalized * size;
            firePos.y = player.position.y;

            Rigidbody shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
            shotPellet.AddForce(dir.normalized * shotSpeed, ForceMode.Impulse);
            currAngle += angularStep;
        }
    }

    protected virtual void UpdateHeight()
    {
        transform.position = new Vector3(transform.position.x, size/2, transform.position.z);
    }

    protected void Seek(Vector3 target)
    {
        Vector3 desiredVelocity = target - transform.position;
        Vector3 forceDir = Vector3.zero;

        forceDir = desiredVelocity - body.velocity;

        body.AddForce(forceDir * seekSpeed);
    }

    protected virtual void DisplayHit()
    {
        if (hitBuffer >= 0)
        {
            hitBuffer -= Time.deltaTime;
            if (hitBuffer > 0.75f)
            {
                renderer.material = hitMat;
            }
            else if (hitBuffer > 0.05f)
            {
                renderer.material = hitMat;
            }
            else if (hitBuffer > 0.25f)
            {
                renderer.material = hitMat;
            }
            else
            {
                renderer.material = originalMat;
            }
        }
    }

    public virtual void TakeHit(Vector3 hitPoint)
    {
        if (hitBuffer <= 0.5)
        {
            health--;
            hitBuffer = 0.1f;
            Vector3 particleDir = hitPoint - transform.position;
            Instantiate(hitParticles, transform.position, Quaternion.LookRotation(-particleDir, Vector3.up));
            //StartCoroutine(Freeze(1f));

            if(health <= 0)
            {
                Instantiate(deathParticles, new Vector3(transform.position.x, hitPoint.y, transform.position.z), Quaternion.Euler(-90, 0, 0));
            }
        }
    }

    IEnumerator Freeze(float duration)
    {
        Time.timeScale = 0.1f;
        Debug.Log("frozen");
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    protected abstract void UpdateRotation();
}
