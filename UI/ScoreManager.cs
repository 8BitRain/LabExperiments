using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public NumberCounter numberCounter;
    public static ScoreManager instance;

    public Score score;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Debug.Log("Destroying ScoreManager instance");
            Destroy(this);
        }
    }

    public void UpdateScore(int value)
    {
        score.value = score.value + value;
        numberCounter.Value = score.value;
    }
}
