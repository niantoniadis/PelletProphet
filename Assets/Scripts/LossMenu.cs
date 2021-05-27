using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LossMenu : MonoBehaviour
{
    GameObject canvas;
    Button playButton;
    Button trainingButton;
    Button quitButton;
    Player player = null;

    // Start is called before the first frame update
    void Start()
    {
        canvas = transform.GetChild(0).gameObject;
        playButton = canvas.transform.GetChild(0).GetComponent<Button>();
        playButton.onClick.AddListener(LoadGameScene);
        //trainingButton = canvas.transform.GetChild(1).GetComponent<Button>();
        //trainingButton.onClick.AddListener(LoadBossScene);
        quitButton = canvas.transform.GetChild(2).GetComponent<Button>();
        quitButton.onClick.AddListener(QuitGame);
    }

    void LoadGameScene()
    {
        canvas.SetActive(false);
        SceneManager.LoadScene(1);
    }

    void LoadTrainingScene()
    {
        canvas.SetActive(false);
        SceneManager.LoadScene(2);
    }

    void LoadBossScene()
    {
        canvas.SetActive(false);
        SceneManager.LoadScene(3);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
