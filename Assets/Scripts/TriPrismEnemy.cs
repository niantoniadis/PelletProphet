using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class TriPrismEnemy : Enemy
{
    float maxSpeed = 5f;
    Transform turret;
    Rigidbody playerBody;
    Vector3 turretDir;
    // Start is called before the first frame update
    new void Start()
    {
        health = 3;
        renderer = transform.GetChild(1).GetComponent<Renderer>();
        originalMat = renderer.material;
        body = GetComponent<Rigidbody>();
        turret = transform.GetChild(0);
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        seekSpeed = 3;
        shotCooldown = 0.5f;
        shotSpeed = 5f;
        pellets = Mathf.FloorToInt(Random.Range(0f, 2f));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
        
        if ((playerBody.position - transform.position).sqrMagnitude < 49f && shotCooldown <= 0)
        {
            Fire();
            shotCooldown = 0.75f;
        }
        shotCooldown -= Time.deltaTime;
        DisplayHit();
        Seek(playerBody.position);
        body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
    }
    private void LateUpdate()
    {
        UpdateTurretRotation();
    }
    void Fire()
    {
        Vector3 dir = turretDir;
        dir.y = 0;
        dir.Normalize();

        if (shotCooldown <= 0)
        {
            Vector3 firePos = turret.position;
            firePos.y = playerBody.position.y;

            //middle pellet
            Rigidbody shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
            float velocityMag = body.velocity.magnitude;
            if(velocityMag < 2)
            {
                velocityMag = 2;
            }
            shotPellet.AddForce(dir * shotSpeed * velocityMag/2, ForceMode.Impulse);
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }
    }

    protected void UpdateTurretRotation()
    {
        turretDir = (playerBody.position + playerBody.velocity) - transform.position;

        float turretRotation = Mathf.Atan2(-turretDir.z, turretDir.x);
        turret.transform.eulerAngles = new Vector3(0, turretRotation * Mathf.Rad2Deg + 90,0);
    }

    protected override void UpdateRotation()
    {
        Vector3 dir = playerBody.position - transform.position;
        dir.y = 0;
        float rotationAngle = Mathf.Atan2(-dir.z, dir.x);
        transform.eulerAngles = new Vector3(0, rotationAngle * Mathf.Rad2Deg, 0);
    }
}
