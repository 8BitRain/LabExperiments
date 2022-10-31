﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndTutorialLogic()
    {
        GameObject BGM = GameObject.FindGameObjectWithTag("BGM");
        if(BGM != null)
        {
            Destroy(BGM);
        }
        SceneManager.LoadScene("Level1");
    }
}
