using System;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [SerializeField] public Material f_mat;
    [SerializeField] public Material g_mat;

    [Space, SerializeField] public PlayerBaseMovement _pbm;
    [SerializeField] public FlightMovement _fm;

    [Space, SerializeField] public CameraController _cc;

    private void Start()
    {
        PlayerState[] states = FindObjectsByType<PlayerState>(FindObjectsSortMode.None);

        foreach (var state in states)
        {
            state.Initialize(this);
            Debug.Log("Found state on: " + state.gameObject.name);
        }
    }
}

public class PlayerState : MonoBehaviour
{
    protected PlayerStateController controller;
    public virtual void Initialize(PlayerStateController _psc) 
    {
        controller = _psc;
    }
    public virtual void SwitchPlayerState() { }
}

public static class CurrentState
{
    public enum States
    {
        Grounded,
        Flying,
    }
    public static States state = States.Grounded;
}