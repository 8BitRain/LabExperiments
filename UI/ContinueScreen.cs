using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueScreen : MonoBehaviour
{
    public GameData gameData;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToTitleScreen()
    {
        SceneManager.LoadScene("Title");
    }


    public void ReturnToCurrentLevel()
    {
        SceneManager.LoadScene(gameData.currentScene);
    }
}
