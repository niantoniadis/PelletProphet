using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPellet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("HeavyPellet"))
        {
            Destroy(gameObject);
        }
    }
}
