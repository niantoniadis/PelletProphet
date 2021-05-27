using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletUI : MonoBehaviour
{
    Animator[] pelletAmmo;
    int loaded;

    // Start is called before the first frame update
    void Start()
    {
        pelletAmmo = new Animator[10];

        for(int i = 0; i < pelletAmmo.Length; i++)
        {
            pelletAmmo[i] = transform.GetChild(i).GetComponent<Animator>();
        }

        loaded = pelletAmmo.Length - 1;
    }

    public void PelletShot()
    {
        if(loaded >= 0)
        {
            pelletAmmo[loaded].SetTrigger("Shot");
            loaded--;
        }

    }

    public void PelletReceived()
    {
        if (loaded + 1 < pelletAmmo.Length)
        {
            loaded++;
            pelletAmmo[loaded].SetTrigger("Received");
        }
    }
}
