using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scellecs.Morpeh;

public class CollisionDetectionOutpost : MonoBehaviour
{
    private Entity m_correspondingEntity;
    private CollisionDetection m_collisionDetectionHQ;

    private bool isInitialized = false;

    private void Awake()
    {


    }

    private void Start()
    {
        if (TryGetComponent<EntityReverseProvider>(out var entityReverseProvider))
        {
            m_correspondingEntity = entityReverseProvider.GetEntity();
            m_collisionDetectionHQ = GetComponentInParent<CollisionDetection>();
            isInitialized = true;
        }
        else
            Destroy(this);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isInitialized)
            m_collisionDetectionHQ.PublishCollisionEvent(other, m_correspondingEntity);
    }
    
    
}
