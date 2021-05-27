using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEntryPoint : MonoBehaviour
{
    RoomManager manager;
    public TestRoom testRoom;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manager.EnterTestRoom(testRoom);
            gameObject.SetActive(false);
        }
    }
}
