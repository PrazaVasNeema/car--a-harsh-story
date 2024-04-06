using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLayerCollidersInRange : MonoBehaviour
{
    [SerializeField] private LayerMask m_layersToDisable;
    [SerializeField] private Collider m_targetCollider;

    private void OnTriggerEnter(Collider other)
    {
        if(((1<<other.gameObject.layer) & m_layersToDisable) != 0)
        {
            Debug.Log("TESTTT");
            m_targetCollider.enabled = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(((1<<other.gameObject.layer) & m_layersToDisable) != 0)
        {
            m_targetCollider.enabled = true;
        }
    }
}
