using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidEnemy : Enemy
{
    float rotationSpeed = 30f;
    float maxSpeed = 2f;
    // Start is called before the first frame update
    new void Start()
    {
        renderer = transform.GetComponentInChildren<Renderer>();
        originalMat = renderer.material;
        body = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        seekSpeed = 2;
        pellets = Mathf.FloorToInt(Random.Range(0f, 2f));
        health = 10;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeight();
        UpdateRotation();
        Fire();
        DisplayHit();
        Seek(player.position);
        body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
    }

    protected override void UpdateHeight()
    {
        transform.position = new Vector3(transform.position.x, 0.66f, transform.position.z);
    }

    protected override void UpdateRotation()
    {
        transform.RotateAround(transform.position, transform.up, rotationSpeed * Time.deltaTime);
    }

    void Fire()
    {
        Vector3 vecToPlayer = (player.position - transform.position);
        vecToPlayer.y = 0;
        vecToPlayer.Normalize();
        float dotResult = Vector3.Dot(new Vector3(transform.forward.x, 0, transform.forward.z).normalized, vecToPlayer);

        if (shotCooldown <= 0)
        {
            if (dotResult >= 0.9999f)
            {
                SprayShot(vecToPlayer);
            }
            else if (dotResult <= 0.01 && dotResult >= -0.01f)
            {
                if (Vector3.Dot(transform.right, player.position - transform.position) > 0)
                {
                    SprayShot(vecToPlayer);
                }
                else
                {
                    SprayShot(vecToPlayer);
                }
            }
            else if (dotResult <= -0.9999f)
            {
                SprayShot(vecToPlayer);
            }
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }
    }

    void SprayShot(Vector3 dir)
    {
        dir.y = 0;
        dir.Normalize();

        float dirAngle = Mathf.Atan2(dir.z, dir.x);

        Vector3 firePos = transform.position + (dir * size);
        firePos.y = player.position.y;

        //middle pellet
        Rigidbody shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(dir * shotSpeed, ForceMode.Impulse);

        Vector3 fireDir = new Vector3(Mathf.Cos(dirAngle + Mathf.Deg2Rad * 7), 0, Mathf.Sin(dirAngle + Mathf.Deg2Rad * 7));
        fireDir.Normalize();
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(fireDir * shotSpeed, ForceMode.Impulse);

        fireDir = new Vector3(Mathf.Cos(dirAngle - Mathf.Deg2Rad * 7), 0, Mathf.Sin(dirAngle - Mathf.Deg2Rad * 7));
        fireDir.Normalize();
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(fireDir * shotSpeed, ForceMode.Impulse);

        fireDir = new Vector3(Mathf.Cos(dirAngle + Mathf.Deg2Rad * 15), 0, Mathf.Sin(dirAngle + Mathf.Deg2Rad * 15));
        fireDir.Normalize();
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(fireDir * shotSpeed, ForceMode.Impulse);

        fireDir = new Vector3(Mathf.Cos(dirAngle - Mathf.Deg2Rad * 15), 0, Mathf.Sin(dirAngle - Mathf.Deg2Rad * 15));
        fireDir.Normalize();
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(fireDir * shotSpeed, ForceMode.Impulse);

        shotCooldown = 0.5f;
    }
}
