using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    RoomManager manager;
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manager.EnterNextRoom();
            Destroy(gameObject);
        }
    }
}
