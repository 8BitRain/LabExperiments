using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingAttack : MonoBehaviour
{
    [Header("3D UI GameObject")]
    public GameObject floatingAttackPromptUI;
    public ParticleSystem promptStateAttackHintIndicator;
    public ParticleSystem promptStateAttackActivated;

    private GameManager gameManager;
    private int _attackTriggered = 0;
    private GameObject _affectedTarget;
    private GameObject _floatingEntity;
    
    private bool _triggeredOnce = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if Player 1 exists and player 2 does not
        if(gameManager.GetSpawnedPlayers().Length == 2 && gameManager.GetSpawnedPlayers()[1] == null)
        {
            //Face the UI Prompt towards the character.
            floatingAttackPromptUI.transform.LookAt(gameManager.GetSpawnedPlayers()[0].GetComponentInChildren<ThirdPersonMovement>().cam);

            
        }

        if(_attackTriggered == 1)
        {
            //Disable special effect
            promptStateAttackHintIndicator.Stop();
            
            //Instantiate Particle System
            promptStateAttackActivated.gameObject.SetActive(true);
            promptStateAttackActivated.Play();
            
            _attackTriggered++;
            
        }
        //TODO Implement logic to spawn second floatingAttackPromptUI if there are additional players
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 position = transform.position;
        Gizmos.DrawWireSphere(position, 15f);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "P1" || other.tag == "P2" && _triggeredOnce == false)
        {
            Debug.Log("Floating Attack: Player is in range of floating attack");
            PlayerCombatController combatController = other.GetComponent<PlayerCombatController>();
            combatController.SetFloatingAttackState(true);
            combatController.SetFloatingAttackInstance(this.gameObject);
            _triggeredOnce = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "P1" || other.tag == "P2")
        {
            // Destroy everything that leaves the trigger
            PlayerCombatController combatController = other.GetComponent<PlayerCombatController>();
            combatController.SetFloatingAttackState(false);
            //Destroy(this.gameObject);
        }
    }

    public void SetAffectedTarget(GameObject target)
    {
        _affectedTarget = target;
    }

    public void SetFloatingEntity(GameObject floatingEntity)
    {
        this._floatingEntity = floatingEntity;
    }

    public GameObject GetFloatingEntity()
    {
        return _floatingEntity;
    }

    public void SetAttackTriggered()
    {
        _attackTriggered = 1;
    }
}
