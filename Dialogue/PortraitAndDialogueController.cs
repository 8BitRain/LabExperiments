using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class PortraitAndDialogueController : MonoBehaviour
{
    public Image portraitOji;
    public Image portraitAngel;
    public Image portraitLuck;
    public Image portraitCurse;

    public Image textBoxBackground;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [YarnCommand("set_portrait")]
    public void SetPortrait(string portraitOwner)
    {
        switch (portraitOwner)
        {
            case "angel":
                portraitAngel.gameObject.SetActive(true);
                portraitOji.gameObject.SetActive(false);
                portraitCurse.gameObject.SetActive(false);
                portraitLuck.gameObject.SetActive(false);
                textBoxBackground.GetComponent<Image>().color = new Color32(108, 146, 147, 255);
                return;
            case "oji":
                portraitAngel.gameObject.SetActive(false);
                portraitOji.gameObject.SetActive(true);
                portraitCurse.gameObject.SetActive(false);
                portraitLuck.gameObject.SetActive(false);
                textBoxBackground.GetComponent<Image>().color = new Color32(255, 200, 67, 255);
                return;
            case "luck":
                portraitAngel.gameObject.SetActive(false);
                portraitOji.gameObject.SetActive(false);
                portraitCurse.gameObject.SetActive(false);
                portraitLuck.gameObject.SetActive(true);
                textBoxBackground.GetComponent<Image>().color = new Color32(223, 124, 67, 213);
                return;
            case "curse":
                portraitAngel.gameObject.SetActive(false);
                portraitOji.gameObject.SetActive(false);
                portraitCurse.gameObject.SetActive(true);
                portraitLuck.gameObject.SetActive(false);
                textBoxBackground.GetComponent<Image>().color = new Color32(79, 57, 158, 255);
                return;
            default:
                return;
        }
    }
}
