using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementComponent : MonoBehaviour
{
    [FormerlySerializedAs("m_speed")] [SerializeField] private float m_maxSpeed = 5f;
    public float MaxSpeed => m_maxSpeed;

    [SerializeField] private float m_mouseSensitivity = 5f;
    
    private Vector3 m_currentVelocity = Vector3.zero;

    [SerializeField] private AttackManager m_attackManager;
    
    private float rotY;
    private float rotX;
    
    protected GameInputManager.ActorInputManagerAbstract m_actorInputManager;

    public void Init(GameInputManager.ActorInputManagerAbstract actorInputManager)
    {
        m_actorInputManager = actorInputManager;
        m_actorInputManager.OnFireAction += ActorInputManager_OnFireAction;

    }
    
    private void Awake()
    {
        // m_characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        // m_actorInputManager.OnFireAction += ActorInputManager_OnFireAction;
    }

    private void Start()
    {
        // m_actorInputManager.OnFireAction += ActorInputManager_OnFireAction;

    }

    private void OnDisable()
    {
        m_actorInputManager.OnFireAction -= ActorInputManager_OnFireAction;
    }

    private void Update()
    {
        Vector2 inputVector = m_actorInputManager.GetMovementVectorNormalized();
        Vector3 inputCorrect = new Vector3(inputVector.x, 0f, inputVector.y);
        Vector2 inputVectorMouse = m_actorInputManager.GetMouseVec();

        float inputUpDown = m_actorInputManager.GetMovementUpDown();
        float inputSpeedSlow = m_actorInputManager.GetMovementSpeedSlow();

        Move(inputVector, inputVectorMouse, inputUpDown, inputSpeedSlow);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Move(Vector3 movementVector, Vector2 mousePosition2D, float moveUpDown, float inputSpeedSlow)
    {
        Rect screenRect = new Rect(0,0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition))
            return;
        var camepraPos = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y);
        // Debug.Log(camepraPos);

        rotY = camepraPos.y;
        rotX = camepraPos.x;
        // Debug.Log($"pos x: {mousePosition2D.x}; pos y: {mousePosition2D.y}");

        rotY += mousePosition2D.x * m_mouseSensitivity;
        rotX -= mousePosition2D.y * m_mouseSensitivity;
        // Debug.Log($"rot x: {rotX}; rot y: {rotY}");

        Camera.main.transform.eulerAngles = new Vector3(rotX, rotY, 0);

        // Debug.Log($"x: {movementVector.x}; y: {movementVector.y}");
        
        var newPos1 = transform.position + transform.forward * (movementVector.y * 100) + transform.right * movementVector.x * 100 + transform.up * (moveUpDown * 100);
        
        float speedModifier = 1;
        if (inputSpeedSlow != 0)
        {
            speedModifier = (float) (inputSpeedSlow > 0 ? 2 : 0.5);
        }
        
        var newPos = Vector3.SmoothDamp(transform.localPosition, newPos1,
            ref m_currentVelocity, 0.5f, m_maxSpeed * speedModifier);
        transform.localPosition = newPos;

    }
    
    private void ActorInputManager_OnFireAction(object sender, System.EventArgs e)
    {
        Debug.Log($"test {gameObject.name}");
        if (m_attackManager != null)
        {
            m_attackManager.Attack();
        }
    }
    
    
}
