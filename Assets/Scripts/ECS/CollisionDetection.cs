using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Scellecs.Morpeh;

public class CollisionDetection : MonoBehaviour
{

    private void Start()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {

        // foreach (var collider)
        Collider myCollider = other.GetContact(0).thisCollider;
        if (myCollider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider))
        {

            var targetEntity = entityReverseProvider.GetEntity();
            //GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = targetEntity, collision = other });

            PublishCollisionEvent(other, targetEntity);
        }
    }

    public void PublishCollisionEvent(Collision collision, Entity affectedEntity)
    {

        if (GameData.instance.currentWorld != null) 
        { 
            GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = affectedEntity, collision = collision });
        }

    }
    
}
