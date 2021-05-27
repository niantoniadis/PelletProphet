using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoom : MonoBehaviour
{
    [Range(0,3)]
    public int enemyIndex;
    public GameObject entryArrows;
    public GameObject tempWall;

    bool active = false;
    float wave = 0;

    RoomManager manager;
    EnemySpawnPattern enemyAmts;
    GameObject pelletPickupPrefab;
    List<Enemy> enemies;
    List<Vector3> ballEnemySpawns;

    Vector3 worldPos;
    Vector2 extents = new Vector2(10, 10);

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();

        enemies = new List<Enemy>();
        ballEnemySpawns = new List<Vector3>();

        worldPos = transform.position;

        pelletPickupPrefab = manager.pelletPickupPrefab;

        switch (enemyIndex)
        {
            case 0:
                enemyAmts = new EnemySpawnPattern(1, 0, 0, 0);
                break;

            case 1:
                enemyAmts = new EnemySpawnPattern(0, 1, 0, 0);
                break;

            case 2:
                enemyAmts = new EnemySpawnPattern(0, 0, 1, 0);
                break;

            case 3:
                enemyAmts = new EnemySpawnPattern(0, 0, 0, 1);
                break;
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

    public Vector3 WorldPos
    {
        get
        {
            return worldPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (enemies.Count > 0)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].Health < 0)
                    {
                        EnemyDeath(enemies[i]);
                        i--;
                    }
                }
            }
            else
            {
                wave++;

                EnemySpawnPattern enemyWaveAmts = enemyAmts * wave;

                if(enemyWaveAmts[1] > 0)
                {
                    ballEnemySpawns.AddRange(manager.halfWaySquareSpawns(worldPos));
                }

                for (int i = 0; i < enemyWaveAmts[0]; i++)
                {
                    Vector3 spawnLocation = manager.randomSpawn(worldPos);
                    enemies.Add(Instantiate(manager.enemyPrefabs[0], spawnLocation, Quaternion.identity));
                }
                for (int i = 0; i < enemyWaveAmts[1]; i++)
                {
                    int index = Random.Range(0, ballEnemySpawns.Count);
                    if (ballEnemySpawns.Count > 0)
                    {
                        enemies.Add(Instantiate(manager.enemyPrefabs[1], ballEnemySpawns[index], Quaternion.identity));
                        ballEnemySpawns.RemoveAt(index);
                    }
                    else
                    {
                        Vector3 spawnLocation = manager.randomSpawn(worldPos);
                        enemies.Add(Instantiate(manager.enemyPrefabs[1], spawnLocation, Quaternion.identity));
                    }
                }
                for (int i = 0; i < enemyWaveAmts[2]; i++)
                {
                    Vector3 spawnLocation = manager.randomSpawn(worldPos);
                    enemies.Add(Instantiate(manager.enemyPrefabs[2], spawnLocation, Quaternion.identity));
                }
                for (int i = 0; i < enemyWaveAmts[3]; i++)
                {
                    Vector3 spawnLocation = manager.randomSpawn(worldPos);
                    enemies.Add(Instantiate(manager.enemyPrefabs[3], spawnLocation, Quaternion.identity));
                }
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

    public void ResetRoom()
    {
        active = false;
        wave = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[0].gameObject);
            enemies.RemoveAt(0);
        }
        enemies = new List<Enemy>();        
    }
}
