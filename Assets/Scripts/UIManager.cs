using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Player player;
    Text healthNum;
    Text bossHealthNum;
    GameObject pauseMenu;

    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        healthNum = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        bossHealthNum = transform.GetChild(0).GetChild(3).GetComponent<Text>();
        pauseMenu = transform.GetChild(0).GetChild(4).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.Health <= 0)
        {
            SceneManager.LoadScene(4);
            return;
        }

        if (healthNum.text != player.Health.ToString())
        {
            healthNum.text = player.Health.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                paused = false;
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
            }
            else
            {
                paused = true;
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
            }
        }

        
    }

    public void UpdateBossHealth(int newHealth)
    {
        if (!bossHealthNum.gameObject.activeSelf)
        {
            bossHealthNum.gameObject.SetActive(true);
            bossHealthNum.text = newHealth.ToString();
        }
        else if(bossHealthNum.text != newHealth.ToString() && newHealth >= 0)
        {
            bossHealthNum.text = newHealth.ToString();
        }
    }
}
