using System;
using System.Collections.Generic;
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
        Instance = this;
        m_inputMapsDictionary.Add(InputMap.Spectator, new SpectatorActorInputManager(InputMap.Spectator.ToString(), m_playerInput.actions));
        m_inputMapsDictionary.Add(InputMap.Car, new CarActorInputManager(InputMap.Car.ToString(), m_playerInput.actions));
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
        public event EventHandler OnChangeModeAction;


        protected const string MOVE = "MOVE";
        protected const string CHANGE_MODE = "CHANGE_MODE";




        protected InputActionMap m_inputActionMap;

        protected InputAction m_moveAction;
        protected InputAction m_changeMode;



        public ActorInputManagerAbstract(string mapName, InputActionAsset inputActionAsset)
        {
            m_inputActionMap = inputActionAsset.FindActionMap(mapName);
            m_moveAction = m_inputActionMap.FindAction(MOVE);
            m_changeMode = m_inputActionMap.FindAction(CHANGE_MODE);

        }

        virtual public void Enable()
        {
            m_changeMode.started += M_changeMode_started;
        }

        virtual public void Disable()
        {
            m_changeMode.started -= M_changeMode_started;
        }

        public Vector2 GetMovementVector()
        {
            Vector2 inputVector = m_moveAction.ReadValue<Vector2>();
            return inputVector;
        }

        public Vector2 GetMovementVectorNormalized()
        {
            Vector2 inputVector = m_moveAction.ReadValue<Vector2>();
            inputVector = inputVector.normalized;
            return inputVector;
        }

        private void M_changeMode_started(InputAction.CallbackContext obj)
        {
            OnChangeModeAction?.Invoke(this, EventArgs.Empty);
        }

    }

    public class SpectatorActorInputManager : ActorInputManagerAbstract
    {
        public event EventHandler OnInteractAction;
        public event EventHandler OnFireAction;
        public event EventHandler OnFireHeavyAction;
        public event EventHandler OnFireSuperAction;

        protected const string INTERACT = "INTERACT";
        protected const string MOUSE = "MOUSE";
        protected const string MOVE_UPDOWN = "MOVE_UPDOWN";
        protected const string MOVE_SPEEDSLOW = "MOVE_SPEEDSLOW";
        protected const string FIRE = "FIRE";
        protected const string FIRE_HEAVY = "FIRE_HEAVY";
        protected const string FIRE_SUPER = "FIRE_SUPER";

        protected InputAction m_interactAction;
        protected InputAction m_mouseAction;
        protected InputAction m_moveUpDownAction;
        protected InputAction m_moveSpeedSlowAction;
        protected InputAction m_fire;
        protected InputAction m_fireHeavy;
        protected InputAction m_fireSuper;

        public SpectatorActorInputManager(string mapName, InputActionAsset inputActionAsset) : base(mapName, inputActionAsset)
        {

            m_interactAction = m_inputActionMap.FindAction(INTERACT);
            m_mouseAction = m_inputActionMap.FindAction(MOUSE);
            m_moveUpDownAction = m_inputActionMap.FindAction(MOVE_UPDOWN);
            m_moveSpeedSlowAction = m_inputActionMap.FindAction(MOVE_SPEEDSLOW);
            m_fire = m_inputActionMap.FindAction(FIRE);
            m_fireHeavy = m_inputActionMap.FindAction(FIRE_HEAVY);
            m_fireSuper = m_inputActionMap.FindAction(FIRE_SUPER);

            Enable();

        }

        public override void Enable()
        {
            base.Enable();
            m_interactAction.started += InteractAction_performed;
            m_fire.started += FireAction_performed;
            m_fireHeavy.started += FireHeavyAction_performed;
            m_fireSuper.started += FireSuperAction_performed;
        }

        public override void Disable()
        {
            base.Disable();
            m_interactAction.started -= InteractAction_performed;
            m_fire.started -= FireAction_performed;
            m_fireHeavy.started -= FireHeavyAction_performed;
            m_fireSuper.started -= FireSuperAction_performed;
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

        private void InteractAction_performed(InputAction.CallbackContext obj)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }

        private void FireAction_performed(InputAction.CallbackContext obj)
        {
            OnFireAction?.Invoke(this, EventArgs.Empty);
        }
        
        private void FireHeavyAction_performed(InputAction.CallbackContext obj)
        {
            OnFireHeavyAction?.Invoke(this, EventArgs.Empty);
        }
        
        private void FireSuperAction_performed(InputAction.CallbackContext obj)
        {
            OnFireSuperAction?.Invoke(this, EventArgs.Empty);
        }


    }

    public class CarActorInputManager : ActorInputManagerAbstract
    {

        public event EventHandler OnHandbreakAction;
        public event EventHandler OnHandbreakActionCanceled;
        public event EventHandler OnFrontLightsAction;

        protected const string HANDBREAK = "HANDBREAK";
        protected const string FRONT_LIGHTS = "FRONT_LIGHTS";

        protected InputAction m_handbreak;
        protected InputAction m_frontLights;


        public CarActorInputManager(string mapName, InputActionAsset inputActionAsset) : base(mapName, inputActionAsset)
        {
            m_handbreak = m_inputActionMap.FindAction(HANDBREAK);
            m_frontLights = m_inputActionMap.FindAction(FRONT_LIGHTS);
            Enable();

        }

        public override void Enable()
        {
            base.Enable();
            m_handbreak.started += M_handbreak_started;
            m_handbreak.canceled += M_handbreak_canceled;
            m_frontLights.started += M_frontLights_started;
        }



        public override void Disable()
        {
            base.Disable();
            m_handbreak.started -= M_handbreak_started;
            m_handbreak.canceled -= M_handbreak_canceled;
        }

        private void M_handbreak_started(InputAction.CallbackContext obj)
        {
            OnHandbreakAction?.Invoke(this, EventArgs.Empty);
        }

        private void M_handbreak_canceled(InputAction.CallbackContext obj)
        {
            OnHandbreakActionCanceled?.Invoke(this, EventArgs.Empty);
        }

        private void M_frontLights_started(InputAction.CallbackContext obj)
        {
            OnFrontLightsAction?.Invoke(this, EventArgs.Empty);
        }
    }
}

