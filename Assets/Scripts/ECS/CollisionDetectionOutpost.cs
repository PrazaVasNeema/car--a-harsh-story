using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scellecs.Morpeh;

public class CollisionDetectionOutpost : MonoBehaviour
{
    private Entity m_correspondingEntity;
    private CollisionDetection m_collisionDetectionHQ;
    private bool test = false;

    private void Awake()
    {


    }

    private void Start()
    {
        m_correspondingEntity = GetComponent<HealthComponentProvider>().GetEntity();
        m_collisionDetectionHQ = GetComponentInParent<CollisionDetection>();
        Debug.Log(m_correspondingEntity.ID);
        test = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (test)
            m_collisionDetectionHQ.PublishCollisionEvent(other, m_correspondingEntity);
        
    }
    
    
}
