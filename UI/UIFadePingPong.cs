using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class UIFadePingPong : MonoBehaviour
{
    private Color textColor;
    public bool fadeIn = false;
    public bool fadeOut = false;
    void Start()
    {
        FadeOut();
    }

    void FadeOut()
    {
        this.GetComponent<Text>().DOFade(0, 1).OnComplete(() => {
            Debug.Log("Fade in");
            FadeIn();
        });
    }

    void FadeIn()
    {
        this.GetComponent<Text>().DOFade(1, 1).OnComplete(() => {
            FadeOut();
        });
    }
}
