using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameInputManager m_gameInputManager;

    [SerializeField] private MovementComponent m_playerSpectatorMovementComponent;

    public void Start()
    {
        m_playerSpectatorMovementComponent.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Spectator));
    }
}
