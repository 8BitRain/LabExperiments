using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public int id;

    public string objective;
    public GameObject objectiveUIElement;
    public GameObject UIElementParent;


    public GameObject waypointA;

    public GameObject[] waypoints;

    private bool _questStarted = false;
    private bool _questCompleted = false;

    private GameObject _textInstance;

    void Start()
    {
        HideObjectiveUIElement();
        HideWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowWaypoint()
    {
        waypointA.SetActive(true);
    }

    void HideWaypoint()
    {
        waypointA.SetActive(false);
    }

    void ShowObjectiveUIElement()
    {
        //ObjectiveUIElement.SetActive(true);
        _textInstance = Instantiate(objectiveUIElement) as GameObject;
        _textInstance.transform.SetParent(UIElementParent.transform);
        print(_textInstance.name);
        _textInstance.GetComponentInChildren<Text>().text = objective;
        _textInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
    }

    void HideObjectiveUIElement()
    {
        //ObjectiveUIElement.SetActive(false);
        Destroy(_textInstance);
    }

    void ShowQuestCompleteText()
    {

    }

    void HideQuestCompleteText()
    {

    }

    public void SetQuestStarted()
    {
        _questStarted = true;
        ShowObjectiveUIElement();
        ShowWaypoint();
    }

    public bool GetQuestStarted()
    {
        return _questStarted;
    }

    public void SetQuestComplete()
    {
        _questCompleted = true;
        HideObjectiveUIElement();
        HideWaypoint();
    }

    public bool GetQuestComplete()
    {
        return _questCompleted;
    }

    public int GetQuestID()
    {
        return id;
    }
}
