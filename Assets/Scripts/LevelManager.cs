using System;
using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;
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

    [SerializeField] private bool m_nowIsDay = true;
    
    [SerializeField] private GameInputManager.InputMap m_initialGameMode = GameInputManager.InputMap.Spectator;
    
    public void Start()
    {
        //m_playerSpectatorMovementComponent.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Spectator));

        m_spectatorController.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Spectator));
        m_carController.Init(m_gameInputManager.GetActorInputMapManager(GameInputManager.InputMap.Car));

        // m_currentMode = GameInputManager.InputMap.Car;
        m_currentMode = m_initialGameMode;

        SetCurrentMode(m_currentMode,false);
        
        // SetCurrentDaytime(true);
        // Invoke("SetCurrentDaytime", 2f);
        StartCoroutine(WaitAndCall());

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
        m_nowIsDay = !m_nowIsDay;
        SetCurrentDaytime(m_nowIsDay);
    }

    private void SetCurrentMode(GameInputManager.InputMap currentMode, bool shouldStickToBrother = true)
    {
        if(currentMode == GameInputManager.InputMap.Spectator)
        {
            m_carController.Deactivate();
            m_spectatorController.Activate();


            m_gameInputManager.SetCurrentMap(GameInputManager.InputMap.Spectator);
            m_cameraManager.ChangeActiveVirtualCamera(1, shouldStickToBrother);
        }
        else
        {
            m_spectatorController.Deactivate();
            m_carController.Activate();

            m_gameInputManager.SetCurrentMap(GameInputManager.InputMap.Car);
            m_cameraManager.ChangeActiveVirtualCamera(0);

        }
        
        m_currentMode = currentMode;
        
    }
    
    private void SetCurrentDaytime(bool nowIsDaytime) 
    {
        if (GameData.instance.currentWorld != null)
        {

            var statusForNow = nowIsDaytime ? NightyDaity.NightyDaityEnum.Daity : NightyDaity.NightyDaityEnum.Nighty;
            
            GameData.instance.currentWorld.GetRequest<ChangeDaytimeRequest>().Publish(new ChangeDaytimeRequest { statusForNow = statusForNow}, true);
            
            RenderSettings.skybox = nowIsDaytime ? m_skyboxDay : m_skyboxNight;

        }
    }
    
    IEnumerator WaitAndCall()
    {
        if (GameData.instance.currentWorld == null)
        {
            yield return new WaitForSeconds(.2f);
        }
        // suspend execution for 5 seconds
        SetCurrentDaytime(m_nowIsDay);
    }
}
