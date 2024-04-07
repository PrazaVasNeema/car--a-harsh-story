using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpectatorMovementComponent : MonoBehaviour
{
    [SerializeField] private float m_maxSpeed = 5f;
    public float MaxSpeed => m_maxSpeed;
    [Range(1,10)]
    [SerializeField] private float m_shiftspeedModifier = 3f;
    public float shiftspeedModifier => m_shiftspeedModifier;
    [Range(0,1)]
    [SerializeField] private float m_ctrlspeedModifier = 2f;
    public float ctrlspeedModifier => m_ctrlspeedModifier;

    [SerializeField] private float m_mouseSensitivity = 5f;

    [SerializeField] private Transform m_test;

    private Vector3 m_currentVelocity = Vector3.zero;
    
    private float rotY;
    private float rotX;

    public void Move(Vector3 movementVector, Vector2 mousePosition2D, float moveUpDown, float inputSpeedSlow)
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
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

        //Camera.main.transform.eulerAngles = new Vector3(rotX, rotY, 0);

        // Debug.Log($"x: {movementVector.x}; y: {movementVector.y}");

        var newPos1 = transform.position + transform.forward * (movementVector.y * 100) + transform.right * movementVector.x * 100 + transform.up * (moveUpDown * 100);

        float speedModifier = 1;
        if (inputSpeedSlow != 0)
        {
            speedModifier = (float)(inputSpeedSlow > 0 ? m_shiftspeedModifier : m_ctrlspeedModifier);
        }

        var newPos = Vector3.SmoothDamp(transform.localPosition, newPos1, ref m_currentVelocity, 0.5f, m_maxSpeed * speedModifier);
        //transform.localPosition = newPos;
        m_test.localPosition = newPos;

    }
}
