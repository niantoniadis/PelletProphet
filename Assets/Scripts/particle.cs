using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle : MonoBehaviour
{
    float timeAlive = 0f;

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if(timeAlive >= 1)
        {
            Destroy(gameObject);
        }
    }
}
