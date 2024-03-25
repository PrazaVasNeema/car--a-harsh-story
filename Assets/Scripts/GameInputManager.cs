using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{

    public static GameInputManager Instance { get; set; }
    
    public enum InputMap
    {
        Spectator,
        Car
    }
    
    [SerializeField] private PlayerInput m_playerInput;

    private Dictionary<InputMap, ActorInputManagerAbstract> m_inputMapsDictionary = new Dictionary<InputMap, ActorInputManagerAbstract>();
    private InputMap? m_currentInputMap;

    private void Awake()
    {
        Instance= this;
        m_inputMapsDictionary.Add(InputMap.Spectator, new SpectatorActorInputManager(InputMap.Spectator.ToString(), m_playerInput.actions));
       // m_inputMapsDictionary.Add(InputMap.Car, new CarActorInputManager(InputMap.Car.ToString(), m_playerInput.actions));
    }
    
    public void SetCurrentMap(InputMap inputMap)
    {
        m_playerInput.currentActionMap.Disable();
        m_playerInput.SwitchCurrentActionMap(inputMap.ToString());
        m_playerInput.currentActionMap.Enable();
    }
    
    public ActorInputManagerAbstract GetActorInputMapManager(InputMap inputMap)
    {
        return m_inputMapsDictionary[inputMap];
    }
    
    public abstract class ActorInputManagerAbstract
    {
        public event EventHandler OnInteractAction;
        public event EventHandler OnFireAction;

        protected const string MOVE = "MOVE";
        protected const string INTERACT = "INTERACT";
        protected const string MOUSE = "MOUSE";
        protected const string MOVE_UPDOWN = "MOVE_UPDOWN";
        protected const string MOVE_SPEEDSLOW = "MOVE_SPEEDSLOW";
        protected const string FIRE = "FIRE";

        
        protected InputActionMap m_inputActionMap;

        protected InputAction m_moveAction;
        protected InputAction m_interactAction;
        protected InputAction m_mouseAction;
        protected InputAction m_moveUpDownAction;
        protected InputAction m_moveSpeedSlowAction;
        protected InputAction m_fire;


        public ActorInputManagerAbstract(string mapName, InputActionAsset inputActionAsset)
        {
            m_inputActionMap = inputActionAsset.FindActionMap(mapName);
            m_moveAction = m_inputActionMap.FindAction(MOVE);
            m_interactAction = m_inputActionMap.FindAction(INTERACT);
            m_mouseAction = m_inputActionMap.FindAction(MOUSE);
            m_moveUpDownAction = m_inputActionMap.FindAction(MOVE_UPDOWN);
            m_moveSpeedSlowAction = m_inputActionMap.FindAction(MOVE_SPEEDSLOW);
            m_fire = m_inputActionMap.FindAction(FIRE);
            Enable();
        }

        public void Enable()
        {
            m_interactAction.started += InteractAction_performed;
            m_fire.started += FireAction_performed;
            Debug.Log(20);

        }

        public void Disable()
        {
            m_interactAction.started -= InteractAction_performed;
            m_fire.started -= FireAction_performed;

        }

        private void InteractAction_performed(InputAction.CallbackContext obj)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }
        
        private void FireAction_performed(InputAction.CallbackContext obj)
        {
            Debug.Log(10);
            OnFireAction?.Invoke(this, EventArgs.Empty);
        }

        public Vector2 GetMovementVectorNormalized()
        {
            Vector2 inputVector = m_moveAction.ReadValue<Vector2>();
            inputVector = inputVector.normalized;
            return inputVector;
        }
        
        public float GetMovementUpDown()
        {
            float inputfloat = m_moveUpDownAction.ReadValue<float>();
            return inputfloat;
        }
        
        public float GetMovementSpeedSlow()
        {
            float inputfloat = m_moveSpeedSlowAction.ReadValue<float>();
            return inputfloat;
        }
        
        public Vector2 GetMouseVec()
        {
            Vector2 inputVector = m_mouseAction.ReadValue<Vector2>();
            return inputVector;
        }

       
    }
    
    public class SpectatorActorInputManager : ActorInputManagerAbstract
    {
        public SpectatorActorInputManager(string mapName, InputActionAsset inputActionAsset) : base(mapName, inputActionAsset)
        {

        }


    }
        
    public class CarActorInputManager : ActorInputManagerAbstract
    {

        public CarActorInputManager(string mapName, InputActionAsset inputActionAsset) : base(mapName, inputActionAsset)
        {

        }

    }
}

