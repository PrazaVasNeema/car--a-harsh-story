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

    [SerializeField] private CameraManager m_cameraManager;

    [SerializeField] private Material m_skyboxDay;
    [SerializeField] private Material m_skyboxNight;

    [SerializeField] private SwapperManager m_swapperManager;

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
        GameEvents.OnChangeDaytime += GameEvents_OnChangeDaytime;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeMode -= GameEvents_OnChangeMode;
        GameEvents.OnChangeDaytime -= GameEvents_OnChangeDaytime;
    }

    private void GameEvents_OnChangeMode()
    {
        var newMode = m_currentMode == GameInputManager.InputMap.Spectator ? GameInputManager.InputMap.Car : GameInputManager.InputMap.Spectator;
        SetCurrentMode(newMode);
    }
    
    private void GameEvents_OnChangeDaytime()
    {   
        RenderSettings.skybox = m_swapperManager.swapStatus ? m_skyboxDay : m_skyboxNight;
        m_swapperManager.SwapIt();
    }

    private void SetCurrentMode(GameInputManager.InputMap currentMode)
    {
        if(currentMode == GameInputManager.InputMap.Spectator)
        {
            m_carController.Deactivate();
            m_spectatorController.Activate();


            m_gameInputManager.SetCurrentMap(GameInputManager.InputMap.Spectator);
            m_cameraManager.ChangeActiveVirtualCamera(1);
        }
        else
        {
            m_spectatorController.Deactivate();
            m_carController.Activate();

            m_gameInputManager.SetCurrentMap(GameInputManager.InputMap.Car);
            m_cameraManager.ChangeActiveVirtualCamera(0);

        }

    }
    
    private void SetCurrentDaytime(GameInputManager.InputMap currentMode)
    {

    }
}
