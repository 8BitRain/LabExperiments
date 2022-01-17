using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public RectTransform UI;
    public GameManager gameManager;
    
    public Image attackPromptImg;

    private Image _attackPromptImgInstance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Listen for an update 
    }

    public void CreateFloatingStateZoneUI(GameObject target)
    {
        _attackPromptImgInstance = Instantiate(attackPromptImg);
        _attackPromptImgInstance.transform.SetParent(UI);

        //Specifically for P1 (P1 should always be the first spawned player);
        Transform player1 = gameManager.GetComponent<GameManager>().GetSpawnedPlayers()[0];
        Camera cam = player1.GetComponentInChildren<ThirdPersonMovement>().cam.GetComponent<Camera>();

        
        //_attackPromptImgInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(cam.WorldToScreenPoint(target.transform.position).x, cam.WorldToScreenPoint(target.transform.position).y);
        _attackPromptImgInstance.GetComponent<RectTransform>().position = cam.WorldToScreenPoint(target.transform.position);
    }
}
