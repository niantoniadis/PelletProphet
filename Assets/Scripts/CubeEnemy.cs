using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEnemy : Enemy
{
    float rotationSpeed = 20f;
    Vector3 velocity;
    int layerMask;
    float horizRayBuffer = 0;
    float vertRayBuffer = 0;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        layerMask = 1 << 8;
        layerMask = layerMask | (1 << 12);
        layerMask = ~layerMask;

        health = 5;

        size = transform.localScale.x;
        shotCooldown = 0.25f;
        moveSpeed = 5f;

        Vector3 startDir;
        startDir = new Vector3(1,0,1);

        velocity = startDir.normalized * moveSpeed;

        pellets = Mathf.FloorToInt(Random.Range(0f, 2f));
    }

    // Update is called once per frame
    void Update()
    {
        DisplayHit();

        UpdateHeight();
        UpdateRotation();

        shotCooldown -= Time.deltaTime;
        if ((Mathf.Abs(transform.eulerAngles.x) % 90 <= 3 || Mathf.Abs(transform.eulerAngles.y) % 90 <= 3 || Mathf.Abs(transform.eulerAngles.z) % 90 <= 3) && shotCooldown <= 0)
        {
            shotCooldown = 0.5f;
            Fire();
        }
        if ((player.position - transform.position).sqrMagnitude <= Mathf.Pow(size + 0.5f, 2) && shotCooldown <= 0)
        {
            shotCooldown = 0.5f;
            Fire();
        }
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    void Fire()
    {
        Vector3 firePos = transform.position + (Vector3.right * size);
        firePos.y = player.position.y;
        Rigidbody shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(Vector3.right * shotSpeed, ForceMode.Impulse);

        firePos = transform.position - Vector3.right * size;
        firePos.y = player.position.y;
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(Vector3.left * shotSpeed, ForceMode.Impulse);
        
        firePos = transform.position + Vector3.forward * size;
        firePos.y = player.position.y;
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(Vector3.forward * shotSpeed, ForceMode.Impulse);

        firePos = transform.position - Vector3.forward * size;
        firePos.y = player.position.y;
        shotPellet = Instantiate(enemyPelletPrefab, firePos, Quaternion.identity).GetComponent<Rigidbody>();
        shotPellet.AddForce(Vector3.back * shotSpeed, ForceMode.Impulse);
    }

    void UpdateMovement()
    {
        horizRayBuffer -= Time.fixedDeltaTime;
        vertRayBuffer -= Time.fixedDeltaTime;
            

        if(horizRayBuffer <= 0 && (Physics.Raycast(transform.position + Vector3.right * size, Vector3.right, 0.5f, layerMask) || Physics.Raycast(transform.position - Vector3.right * size, -Vector3.right, 0.5f, layerMask)))
        {
            velocity = new Vector3(-velocity.x, 0, velocity.z);
            horizRayBuffer = 0.25f;
        }
        if (vertRayBuffer <= 0 && (Physics.Raycast(transform.position + Vector3.forward * size, Vector3.forward, 0.5f, layerMask) || Physics.Raycast(transform.position - Vector3.forward * size, -Vector3.forward, 0.5f, layerMask)))
        {
            velocity = new Vector3(velocity.x, 0, -velocity.z);
            vertRayBuffer = 0.25f;
        }

        transform.position += velocity * Time.fixedDeltaTime;
    }

    protected override void UpdateRotation()
    {
        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.right, rotationSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
