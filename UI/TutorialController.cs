using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public Camera SceneCamera;
    public Canvas canvas;

    public ScreenManager ScreenManager;

    private Image currentModal;

    [Header("Modals")]
    public Image startGameModal;
    public Image[] tutorialModals;

    [Header("Waypoints")]
    public GameObject[] waypoints;

    [Header("Quests")]
    public GameObject[] quests;




    [Header("Enviroment Objects")]
    public GameObject deadPlanet;
    
    private bool _tutorialLoopActive = false;
    private bool _tutorialInitiated = false;

    [Header("Controller Inputs")]
    /// <summary>Vector2 action for pressing south face button to start game </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference StartGamePressed;

    /// <summary>Vector2 action for moving left thumbstick </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference LeftThumbStick;

    /// <summary>Vector2 action for moving right thumbstick </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference RightThumbStick;

    /// <summary>Vector2 action for pressing  </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference JumpPressed;

    public InputActionReference[] tutorialActions;


    
    // Start is called before the first frame update
    void Start()
    {
        //Assign all tutorial waypoints this tutorial Controller reference
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].GetComponent<Waypoint>().SetTutorialController(this.GetComponent<TutorialController>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(StartGamePressed.action.triggered && !_tutorialInitiated)
        {
            Debug.Log("Started");
            //StartGamePressed.action.
            StartGame();
        }

        if(_tutorialLoopActive)
        {
            TutorialLoop();
        }
        
    }

    public void StartGame()
    {
        deadPlanet.gameObject.transform.localScale = new Vector3(100000,100000,100000);
        SceneCamera.gameObject.SetActive(false);
        StartTutorial();
    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial Started");
        //Destroy(startGameModal.gameObject);

        if(ScreenManager != null)
        {
            ScreenManager.OpenPanel(tutorialModals[0].GetComponent<Animator>());
            _tutorialInitiated = true;

            StartCoroutine(InputWindowCoroutine(1));
        }
    
    }

    public void TutorialLoop()
    {
        if(this.ScreenManager.DoesCurrentModalExists())
        {
            switch (this.ScreenManager.GetCurrent())
            {
                case "Movement Modal":
                    if(LeftThumbStick.action.ReadValue<Vector2>().x > 0 || LeftThumbStick.action.ReadValue<Vector2>().y > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 0
                        quests[0].GetComponent<Quest>().SetQuestStarted();
                        //_tutorialLoopActive = false;
                    }
                    
                    break;
                case "Look Around Modal":
                    if(RightThumbStick.action.ReadValue<Vector2>().x > 0 || RightThumbStick.action.ReadValue<Vector2>().y > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 1
                        //waypoints[1].SetActive(true);
                        quests[1].GetComponent<Quest>().SetQuestStarted();
                        //_tutorialLoopActive = false;
                    }
                    break;
                case "Jump Modal":
                    if(JumpPressed.action.ReadValue<float>() > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 2
                        //waypoints[2].SetActive(true);
                        quests[2].GetComponent<Quest>().SetQuestStarted();
                        //_tutorialLoopActive = false;
                    }
                    break;
                case "Sprint Modal":
                    if(tutorialActions[0].action.ReadValue<float>() > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 3
                        quests[3].GetComponent<Quest>().SetQuestStarted();
                    }
                    break;
                case "Enviroment Interaction & Wall Run Modal":
                    if(tutorialActions[1].action.ReadValue<float>() > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 4
                        quests[4].GetComponent<Quest>().SetQuestStarted();
                    }
                    break;
                case "Dash Modal":
                    if(tutorialActions[2].action.ReadValue<float>() > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 5
                        quests[5].GetComponent<Quest>().SetQuestStarted();
                    }
                    break;
                
                case "Mantle Modal":
                    if(JumpPressed.action.ReadValue<float>() > 0)
                    {
                        this.ScreenManager.CloseCurrent();

                        //Start Quest 6
                        quests[6].GetComponent<Quest>().SetQuestStarted();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void ActivateTutorialLoop()
    {
        _tutorialLoopActive = true;
    }

    public void DeactivateTutorialLoop()
    {
        _tutorialLoopActive = false;
    }

    public void PlayerReachedWaypoint(int id)
    {
        switch (id)
        {
            case 0:
                //waypoints[0].SetActive(false);
                quests[0].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                ScreenManager.OpenPanel(tutorialModals[1].GetComponent<Animator>());
                break;
            case 1:
                //waypoints[id].SetActive(false);
                quests[1].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                ScreenManager.OpenPanel(tutorialModals[2].GetComponent<Animator>());
                break;
            case 2:
                quests[2].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                ScreenManager.OpenPanel(tutorialModals[3].GetComponent<Animator>());
                break;
            case 3:
                quests[3].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                ScreenManager.OpenPanel(tutorialModals[4].GetComponent<Animator>());
                break;
            case 4:
                quests[4].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                ScreenManager.OpenPanel(tutorialModals[5].GetComponent<Animator>());
                break;
            case 5:
                quests[5].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                quests[6].GetComponent<Quest>().SetQuestStarted();
                break;
            case 6:
                quests[6].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                quests[7].GetComponent<Quest>().SetQuestStarted();
                break;
            case 7:
                quests[7].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                quests[8].GetComponent<Quest>().SetQuestStarted();
                break;
            case 8:
                quests[8].GetComponent<Quest>().SetQuestComplete();
                DeactivateTutorialLoop();
                StartCoroutine(InputWindowCoroutine(1));
                break;
            default:
                break;
        }
    }

    //This delay prevents input from persisting multiple frames
    IEnumerator InputWindowCoroutine (float time) 
    {
        float elapsedTime = 0;

        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ActivateTutorialLoop();
    }

}
