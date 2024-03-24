using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Scellecs.Morpeh;

public class CollisionDetection : MonoBehaviour
{

    private void OnCollisionEnter(Collision other)
    {

        // foreach (var collider)
        Collider myCollider = other.GetContact(0).thisCollider;

        var targetEntity = myCollider.GetComponent<HealthComponentProvider>().GetEntity();
        
        GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = targetEntity, collision = other });
        Debug.Log("test");
        
        PublishCollisionEvent(other, targetEntity);
    }

    public void PublishCollisionEvent(Collision collision, Entity affectedEntity)
    {
        
        GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = affectedEntity, collision = collision });
        
    }
    
}
