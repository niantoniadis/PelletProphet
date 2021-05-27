using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Power { FireRate = 50, Dash = 100, Homing = 75, Protection = 125, None = 0}

public class Player : MonoBehaviour
{
    public GameObject pelletPrefab;
    public Material hitMat;

    RoomManager roomManager;
    Transform firePos;
    Rigidbody body;
    Renderer renderer;
    Material originalMat;
    Animator playerAnimator;
    CameraController camControl;
    Camera cam;
    [SerializeField]
    PelletUI pelletUI;
    

    List<GameObject> shotPellets;
    Vector3 dir;
    int maxAmmo = 30;
    int currAmmo = 10;
    int health = 10;
    float size = 0.5f;
    float acceleration = 500f;
    float maxSpeed = 9f;
    float shotCooldown;
    float shotCooldownReset = 0.25f;
    float hitBuffer = 1.5f;
    float powerTimer;
    bool inPractice = false;
    bool coroutineRunning = false;
    bool pelletRecieved = false;
    bool faceMouse = false;
    Power currPower;

    public int Health
    {
        get
        {
            return health;
        }
    }

    public bool InPractice
    {
        set
        {
            inPractice = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        firePos = transform.GetChild(0);
        shotPellets = new List<GameObject>();
        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        renderer = GetComponent<Renderer>();
        originalMat = renderer.material;
        playerAnimator = GetComponent<Animator>();
        camControl = Camera.main.GetComponent<CameraController>();
        cam = Camera.main;
        currPower = Power.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            faceMouse = false;
            if (health <= 0 && inPractice)
            {
                roomManager.LeaveTestRoom();
                health = 10;
            }

            if (body.isKinematic)
            {
                for (int i = 0; i < shotPellets.Count; i++)
                {
                    ReceivePellet(shotPellets[i].gameObject);
                }
                shotPellets = new List<GameObject>();
                health = 10;
            }

            shotCooldown -= Time.deltaTime;
            size = transform.localScale.x;

            if (Input.GetMouseButton(0))
            {
                faceMouse = true;
                if (currAmmo > 0 && shotCooldown <= 0)
                {
                    Vector3 shootDir = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) - transform.position;
                    shotPellets.Add(ShootTowards(shootDir.normalized));
                    pelletUI.PelletShot();
                }
            }

            if (powerTimer <= 0 && currPower != Power.None)
            {
                currPower = Power.None;
                ResetPellets();
                powerTimer = 0;
            }

            if (currPower == Power.None)
            {
                powerTimer -= Time.deltaTime;
            }

            bool absorbing = Input.GetKey(KeyCode.Space);
            for (int i = 0; i < shotPellets.Count; i++)
            {
                shotPellets[i].GetComponent<Pellet>().Absorb = absorbing;
            }

            if (!coroutineRunning)
                UpdateSize();

            DisplayHit();

            hitBuffer -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        HandleMovement();
        UpdateRotation();
        UpdateHeight();
    }

    void HandleMovement()
    {
        if (hitBuffer <= 1.25f)
        {
            float zDir = Input.GetAxisRaw("Vertical");
            float xDir = Input.GetAxisRaw("Horizontal");

            Vector3 moveStep = new Vector3(xDir, 0, zDir);
            moveStep.Normalize();
            if (moveStep.sqrMagnitude != 0)
            {
                dir = moveStep;
            }
            body.AddForce(moveStep * acceleration, ForceMode.Force);
            body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
        }
    }

    void UpdateSize()
    {
        float ammoScale = Mathf.InverseLerp(0, maxAmmo, currAmmo);
        size = Mathf.Pow(2,(ammoScale * 2.33f - 1));
        transform.localScale = new Vector3(size, size, size);
    }

    protected void UpdateHeight()
    {
        transform.position = new Vector3(transform.position.x, size / 2, transform.position.z);
    }

    void UpdateRotation()
    {
        if (!faceMouse)
            transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * Mathf.Atan2(-dir.z, dir.x), 0);
        else
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 mouseDir = mousePos - transform.position;
            transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * Mathf.Atan2(-mouseDir.z, mouseDir.x), 0);
        }
    }

    GameObject ShootTowards(Vector3 dir)
    {
        currAmmo--;
        shotCooldown = shotCooldownReset;
        firePos.position = transform.position + dir.normalized * size;

        Pellet shotPellet = Instantiate(pelletPrefab, firePos.position, Quaternion.identity).GetComponent<Pellet>();
        shotPellet.Player = transform;
        shotPellet.Dir = dir;
        return shotPellet.gameObject;
    }

    void ReceivePellet(GameObject pellet)
    {
        shotPellets.Remove(pellet);
        Destroy(pellet);
        currAmmo++;
        pelletUI.PelletReceived();
        if (coroutineRunning)
        {
            StopAllCoroutines();
            UpdateSize();
            StartCoroutine(Grow());
        }
        else
        {
            StartCoroutine(Grow());
            coroutineRunning = true;
        }
    }

    protected void DisplayHit()
    {
        if (hitBuffer <= .05f && hitBuffer > 0.005)
        {
            playerAnimator.SetTrigger("DoneTakingDamage");
        }
    }

    private void ResetPellets()
    {
        for (int i = 0; i < shotPellets.Count; i++)
        {
            ReceivePellet(shotPellets[i].gameObject);
        }
        shotPellets = new List<GameObject>();
        currAmmo = 10;
        shotCooldownReset = 0.25f;
    }

    IEnumerator Grow()
    {
        float shrinkSpeed = 0.1f;
        float scaleOrig = transform.localScale.x;
        float scale = scaleOrig + 0.15f;
        transform.localScale = new Vector3(scale, scale, scale);

        yield return new WaitForEndOfFrame();

        while (Mathf.Abs(scale - scaleOrig) > 0.01f)
        {
            scale = Mathf.Lerp(scale, scaleOrig, shrinkSpeed);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForEndOfFrame();
        }

        coroutineRunning = false;
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("PelletPickup"))
        //{
        //    Destroy(other.gameObject);
        //    maxAmmo++;
        //    currAmmo++;
        //}

        switch (other.gameObject.tag)
        {
            case "EnemyPellet":
                Destroy(other.gameObject);
                if (hitBuffer <= 0)
                {
                    hitBuffer = 1.5f;
                    health--;
                    playerAnimator.SetTrigger("DamageTaken");

                    IEnumerator cameraShake = camControl.Shake(0.5f, 0.5f);
                    StartCoroutine(cameraShake);
                }
                break;

            case "FireRate":
                if (currPower != Power.FireRate)
                {
                    currPower = Power.FireRate;
                    powerTimer = ((float)Power.FireRate) / 10f;
                    currAmmo += 10;
                    shotCooldownReset = 0.1f;
                }
                else
                {
                    powerTimer = ((float)Power.FireRate) / 10f;
                }
                break;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("HeavyPellet"))
        {
            ReceivePellet(other.gameObject);
        }
    }
}
