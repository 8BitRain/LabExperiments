using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class AdversaryDebugWindowController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Character;
    private Camera mainCamera;
    public Text distanceText;
    public Text stateText;
    public Text blockTimer;
    public Text actionWindowText;

    private void OnEnable()
    {
        Adversary.onUpdateDistanceUI += UpdateDistanceUI;
        Adversary.onUpdateCurrentStateUI += UpdateCurrentStateUI;
        Adversary.onUpdateBlockTimerUI += UpdateBlockTimerUI;
        Adversary.onUpdateActionWindowUI += UpdateActionWindowUI;
    }

    private void OnDisable()
    {
        Adversary.onUpdateDistanceUI -= UpdateDistanceUI;
        Adversary.onUpdateCurrentStateUI -= UpdateCurrentStateUI;
        Adversary.onUpdateBlockTimerUI -= UpdateBlockTimerUI;
        Adversary.onUpdateBlockTimerUI += UpdateActionWindowUI;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateDistanceUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
        distanceText.text = text;
    }

    void UpdateCurrentStateUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
        stateText.text = text;
    }

    void UpdateBlockTimerUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
        blockTimer.text = text;
    }

    void UpdateActionWindowUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
        actionWindowText.text = text;
    }
}
