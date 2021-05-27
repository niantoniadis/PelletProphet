using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    bool active = false;
    bool completed = false;
    bool spawned = false;
    int tier = 0;

    RoomManager manager;
    EnemySpawnPattern enemyAmts;
    GameObject pelletPickupPrefab;
    List<Enemy> enemies;
    List<Vector3> ballEnemySpawns;

    Dictionary<string, GameObject> walls;

    Vector3 worldPos;
    Vector2 extents = new Vector2(10, 10);

    public GameObject leftWall
    {
        get
        {
            return walls["left"];
        }
    }

    public GameObject topWall
    {
        get
        {
            return walls["top"];
        }
    }

    public GameObject rightWall
    {
        get
        {
            return walls["right"];
        }
    }

    public GameObject bottomWall
    {
        get
        {
            return walls["bottom"];
        }
    }

    public bool Active
    {
        set
        {
            active = value;
        }
        get
        {
            return active;
        }
    }

    public int Tier
    {
        set
        {
            tier = value;
        }
    }

    public Vector3 WorldPos
    {
        get
        {
            return worldPos;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        tier = manager.CurrTier;
        worldPos = transform.position;
        enemies = new List<Enemy>();

        if (tier < 3)
        {
            ballEnemySpawns = new List<Vector3>();

            pelletPickupPrefab = manager.pelletPickupPrefab;

            ballEnemySpawns.AddRange(manager.halfWaySquareSpawns(worldPos));
        }
        walls = new Dictionary<string, GameObject>();
        walls.Add("left", transform.GetChild(0).gameObject);
        walls.Add("top", transform.GetChild(1).gameObject);
        walls.Add("right", transform.GetChild(2).gameObject);
        walls.Add("bottom", transform.GetChild(3).gameObject);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (active && tier < 3)
        {
            if (enemies.Count > 0)
            {
                for(int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].Health <= 0)
                    {
                        EnemyDeath(enemies[i]);
                        i--;
                    }
                }
            }
            else if(!spawned)
            {
                spawned = true;

                enemyAmts = manager.GetRandomSpawnPatternOfTier(tier);

                for (int i = 0; i < enemyAmts[0]; i++)
                {
                    Vector3 spawnLocation = manager.randomSpawn(worldPos);
                    enemies.Add(Instantiate(manager.enemyPrefabs[0], spawnLocation, Quaternion.identity));
                }
                for (int i = 0; i < enemyAmts[1]; i++)
                {
                    int index = Random.Range(0, ballEnemySpawns.Count);
                    enemies.Add(Instantiate(manager.enemyPrefabs[1], ballEnemySpawns[index], Quaternion.identity));
                    ballEnemySpawns.RemoveAt(index);
                }
                for (int i = 0; i < enemyAmts[2]; i++)
                {
                    Vector3 spawnLocation = manager.randomSpawn(worldPos);
                    enemies.Add(Instantiate(manager.enemyPrefabs[2], spawnLocation, Quaternion.identity));
                }
                for (int i = 0; i < enemyAmts[3]; i++)
                {
                    Vector3 spawnLocation = manager.randomSpawn(worldPos);
                    enemies.Add(Instantiate(manager.enemyPrefabs[3], spawnLocation, Quaternion.identity));
                }
            }
            else if(!completed)
            {
                completed = true;
                manager.SpawnNextRoom();
                active = false;
            }
        }
        else if (active)
        {
            if (!spawned)
            {
                enemies.Add(Instantiate(manager.enemyPrefabs[4].gameObject, worldPos, Quaternion.identity).GetComponent<Enemy>());
                spawned = true;
                active = false;
            }
            else if (enemies[0].Health <= 0)
            {
                completed = true;
                active = false;
            }
        }
    }

    void EnemyDeath(Enemy deadEnemy)
    {
        for (int i = 0; i < deadEnemy.Pellets; i++)
        {
            Instantiate(pelletPickupPrefab, deadEnemy.transform);
            Debug.Log("pellet drop");
        }
        enemies.Remove(deadEnemy);
        Destroy(deadEnemy.gameObject);
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
