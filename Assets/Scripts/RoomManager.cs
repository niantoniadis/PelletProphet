using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public bool practiceRooms;
    public List<Enemy> enemyPrefabs;
    public GameObject pelletPickupPrefab;
    public GameObject[] roomPrefab;
    List<Vector3[]> enemySpawnLocations;
    int[,] enemySpawnNum;
    GameObject player;
    Room currRoom;
    Room nextRoom = null;
    CameraController camControl;
    EnemySpawnPatterns potentialSpawnPatterns = new EnemySpawnPatterns();
    public int startTier = 0;
    int currTier = 0;
    float roomSize = 20;
    float transitionSpeed = 2.5f;
    float nextTierChance = 0f;

    public int CurrTier
    {
        get
        {
            return currTier;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currTier = startTier;
        if (!practiceRooms)
        {
            currRoom = GameObject.FindGameObjectWithTag("Room").GetComponent<Room>();
            currRoom.Active = true;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        camControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        camControl.practiceRooms = practiceRooms;
        player.GetComponent<Player>().InPractice = practiceRooms;

        enemySpawnLocations = new List<Vector3[]>();
        enemySpawnLocations.Add(new Vector3[4]);
        enemySpawnLocations.Add(new Vector3[4]);

        for (int i = 0, j = 0, x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if ((x == 0 || z == 0) && (z != 0 || x != 0))
                {
                    enemySpawnLocations[0][i] = new Vector3(x * roomSize / 4, 0, z * roomSize / 4);
                    i++;
                }
                if (z != 0 && x != 0)
                {
                    enemySpawnLocations[1][j] = new Vector3(x * roomSize / 4, 0, z * roomSize / 4);
                    j++;
                }
            }
        }
    }

    public void ActivateTestRoom(TestRoom testRoom)
    {
        testRoom.tempWall.SetActive(true);
        testRoom.Active = true;
    }

    public EnemySpawnPattern GetRandomSpawnPatternOfTier(int tier)
    {
        List<EnemySpawnPattern> spawnPatternsInTier = potentialSpawnPatterns.GetPatternsOfTier(tier);
        int index = Random.Range(0, spawnPatternsInTier.Count);
        return spawnPatternsInTier[index];
    }

    public Vector3[] halfWayPlusSpawns(Vector3 worldPos)
    {
        Vector3[] result = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = enemySpawnLocations[0][i] + worldPos;
        }

        return result;
    }

    public Vector3[] halfWaySquareSpawns(Vector3 worldPos)
    {
        Vector3[] result = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = enemySpawnLocations[1][i] + worldPos;
        }

        return result;
    }

    public Vector3 randomSpawn(Vector3 worldPos)
    {
        return worldPos + new Vector3(Random.Range(-7.5f, 7.5f), 0, Random.Range(-7.5f, 7.5f));
    }

    public void SpawnNextRoom()
    {
        Vector3 currPosition;
        if (currTier < 3)
        {
            nextTierChance += Random.Range(0.35f, 0.75f);
        }
        if(nextTierChance >= 1)
        {
            currTier++;
            nextTierChance = 0;
        }

        if (currTier < 3)
        {
            int index = Random.Range(0, 4);
            switch (index)
            {
                case 0:
                    currPosition = currRoom.WorldPos;
                    nextRoom = Instantiate(roomPrefab[index], new Vector3(currPosition.x - roomSize, currPosition.y, currPosition.z), Quaternion.identity).GetComponent<Room>();
                    currRoom.leftWall.SetActive(false);
                    nextRoom.Tier = currTier;
                    break;

                case 1:
                    currPosition = currRoom.WorldPos;
                    nextRoom = Instantiate(roomPrefab[index], new Vector3(currPosition.x, currPosition.y, currPosition.z + roomSize), Quaternion.identity).GetComponent<Room>();
                    currRoom.topWall.SetActive(false);
                    nextRoom.Tier = currTier;
                    break;

                case 2:
                    currPosition = currRoom.WorldPos;
                    nextRoom = Instantiate(roomPrefab[index], new Vector3(currPosition.x + roomSize, currPosition.y, currPosition.z), Quaternion.identity).GetComponent<Room>();
                    currRoom.rightWall.SetActive(false);
                    nextRoom.Tier = currTier;
                    break;

                case 3:
                    currPosition = currRoom.WorldPos;
                    nextRoom = Instantiate(roomPrefab[index], new Vector3(currPosition.x, currPosition.y, currPosition.z - roomSize), Quaternion.identity).GetComponent<Room>();
                    currRoom.bottomWall.SetActive(false);
                    nextRoom.Tier = currTier;
                    break;
            }
        }
        else
        {
            currPosition = currRoom.WorldPos;
            nextRoom = Instantiate(roomPrefab[4], new Vector3(currPosition.x, currPosition.y, currPosition.z + roomSize), Quaternion.identity).GetComponent<Room>();
            currRoom.topWall.SetActive(false);
            nextRoom.Tier = currTier;
        }
    }

    public void EnterNextRoom()
    {
        camControl.Offset = nextRoom.WorldPos;
        StartCoroutine(TransitionToRoom(nextRoom));
    }

    public void EnterTestRoom(TestRoom testRoom)
    {
        camControl.Offset = testRoom.WorldPos;
        camControl.CurrTestRoom = testRoom;
        StartCoroutine(TransitionToTestRoom(testRoom));
    }

    public void LeaveTestRoom()
    {
        TestRoom currTestRoom = camControl.CurrTestRoom;
        currTestRoom.entryArrows.SetActive(true);
        currTestRoom.tempWall.SetActive(false);
        currTestRoom.ResetRoom();
        camControl.CurrTestRoom = null;
        camControl.Offset = Vector3.zero;
        player.transform.position = new Vector3(currTestRoom.WorldPos.x, player.transform.position.y, 16);
    }

    IEnumerator TransitionToRoom(Room room)
    {
        Vector3 destination = nextRoom.WorldPos - player.transform.position;
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.isKinematic = true;
        while (!((player.transform.position - nextRoom.WorldPos).sqrMagnitude <= 72.25f))
        {
            player.transform.position += destination * Time.deltaTime * transitionSpeed;
            yield return new WaitForEndOfFrame();
        }

        playerBody.isKinematic = false;

        SwapRooms();
    }

    IEnumerator TransitionToTestRoom(TestRoom testRoom)
    {
        Vector3 destination = testRoom.WorldPos - player.transform.position;
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.isKinematic = true;
        while (!((player.transform.position - testRoom.WorldPos).sqrMagnitude <= 72.25f))
        {
            player.transform.position += destination * Time.deltaTime * transitionSpeed;
            yield return new WaitForEndOfFrame();
        }

        playerBody.isKinematic = false;
        ActivateTestRoom(testRoom);
    }

    void SwapRooms()
    {
        Destroy(currRoom);

        currRoom = nextRoom;
        nextRoom = null;

        if (!currRoom.leftWall.activeSelf)
        {
            currRoom.leftWall.SetActive(true);
        }
        else if (!currRoom.topWall.activeSelf)
        {
            currRoom.topWall.SetActive(true);
        }
        else if (!currRoom.rightWall.activeSelf)
        {
            currRoom.rightWall.SetActive(true);
        }
        else
        {
            currRoom.bottomWall.SetActive(true);
        }
        currRoom.Active = true;
    }
}
