using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameInputManager m_gameInputManager;


    [SerializeField] private PlayerActorControllerAbstract m_spectatorController;
    [SerializeField] private PlayerActorControllerAbstract m_carController;
    private GameInputManager.InputMap m_currentMode;

    public void Start()
    {
        //m_playerSpectatorMovementComponent.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Spectator));

        m_spectatorController.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Spectator));
        m_carController.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Car));

        m_currentMode = GameInputManager.InputMap.Car;

        SetCurrentMode(m_currentMode);
    }

    private void OnEnable()
    {
        GameEvents.OnChangeMode += GameEvents_OnChangeMode;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeMode -= GameEvents_OnChangeMode;
    }

    private void GameEvents_OnChangeMode()
    {
        var newMode = m_currentMode == GameInputManager.InputMap.Spectator ? GameInputManager.InputMap.Car : GameInputManager.InputMap.Spectator;
        SetCurrentMode(newMode);
    }

    private void SetCurrentMode(GameInputManager.InputMap currentMode)
    {
        if(currentMode == GameInputManager.InputMap.Spectator)
        {
            m_carController.Deactivate();
            m_spectatorController.Activate();


            m_gameInputManager.SetCurrentMap(GameInputManager.InputMap.Spectator);
        }
        else
        {
            m_spectatorController.Deactivate();
            m_carController.Activate();

            m_gameInputManager.SetCurrentMap(GameInputManager.InputMap.Car);
        }

        m_currentMode = currentMode;
    }
}
