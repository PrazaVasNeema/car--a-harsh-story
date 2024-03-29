using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCarBehaviourComponent : MonoBehaviour
{
    [SerializeField]
    private PrometeoCarController m_prometeoCarController;

    public void Move(Vector2 moveVector)
    {
        //Debug.Log($"MoveVector: {moveVector}");
    }

    public void Handbrake()
    {
        Debug.Log("Handbreak pressed");
    }
}
