using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;

public class DebugWindowController : MonoBehaviour
{

    public bool displayDebugWindow = true;
    public GameObject Player;
    private CombatController combatControllerRef;
    private AnimationController animationController;
    public bool canPauseEditor;
    public InputActionReference pauseEditorInput;

    [Header("Debug Text Fields")]
    /*public Text targetDistanceText;
    public Text playerTargetText;*/
    public Text inputFrameText;
    public Text comboStateText;
    public Text currentAttackStateText;
    public Text currentAttackSAnimation;

    void Awake()
    {
        pauseEditorInput.action.started += ctx => PauseEditor();
    }

    void OnEnable()
    {
        pauseEditorInput.action.Enable();
    }

    void OnDisable()
    {
        pauseEditorInput.action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        combatControllerRef = Player.GetComponent<CombatController>();
        animationController = Player.GetComponent<AnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Player != null && playerScriptReference != null && playerScriptReference.GetCurrentTarget() != null)
            targetDistanceText.text = "Target Distance: " + Vector3.Distance(Player.transform.position, playerScriptReference.GetCurrentTarget().transform.position);
            playerTargetText.text = "Player Target: " + playerScriptReference.GetCurrentTarget();*/
        
        if(Player != null && combatControllerRef != null)
            inputFrameText.text = "Input Window Open: " + combatControllerRef.GetInputWindowOpen();
            comboStateText.text = "Current Combo State: " + combatControllerRef.GetCurrentComboState();
            currentAttackStateText.text = "Current Attack State: " + combatControllerRef.GetCurrentAttackState();
            currentAttackSAnimation.text = "Current Attack Animation: " + animationController.GetCurrentState();


        if(displayDebugWindow)
            ShowDebugWindow();
        
        if(!displayDebugWindow)
            HideDebugWindow();
    }
    
    void HideDebugWindow()
    {
        this.gameObject.SetActive(false);
    }

    void ShowDebugWindow()
    {
        this.gameObject.SetActive(true);
    }

    void PauseEditor()
    {
        if(canPauseEditor)
        {
            Debug.Break();
        }
    }
}
