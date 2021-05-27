using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

enum GameAudioState { Menu, Normal, Boss };

public class AudioManager : MonoBehaviour
{
    public AudioSource intro;
    public AudioSource loop;

    GameAudioState currState = GameAudioState.Menu;

    bool looping;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case GameAudioState.Menu:
                if(!intro.isPlaying)
                {
                    intro.Play();

                    if (SceneManager.GetActiveScene().buildIndex > 0)
                    {
                        currState = GameAudioState.Normal;
                        loop.PlayDelayed(intro.clip.length - intro.time - Time.deltaTime);
                    }

                }
                break;

            case GameAudioState.Normal:
                if(!intro.isPlaying && !loop.isPlaying)
                {
                    loop.Play();
                }
                break;

            case GameAudioState.Boss:
                break;
        }
    }
}
