#region Description
// --------------------------
// Came from
// https://github.com/roboryantron/Unite2017
// 
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date: 10/04/17
// ---------------------------
#endregion

using UnityEngine;
using UnityEngine.Events;

public class UnityGameEventListener : MonoBehaviour, IGameEventListener
{
    //[InfoBox("Game Event to Listen to.")]
    [Tooltip("Event to register with.")]
    [SerializeField]
    private GameEvent @event;

    [Tooltip("Response to invoke when Event is raised.")]
    //[InfoBox("Unity Events to perform when the Game Event is raised")]
    [SerializeField]
    private UnityEvent response;
    
    public void OnEnable()
    {
        if(@event != null) @event.RegisterListener(this);
    }

    public void OnDisable()
    {
        @event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        //?. is a null conditional operator. It'll perform the operation only if the operand isn't null.
        //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators
        response?.Invoke();
    }


}