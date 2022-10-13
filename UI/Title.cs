using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public GameObject SFX;
    public GameData gameData;
    // Start is called before the first frame update
    void Start()
    {
        gameData.currentScene = "Title";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFirstLevel()
    {
        DontDestroyOnLoad(SFX);
        SceneManager.LoadScene("Level1");
    }
}
