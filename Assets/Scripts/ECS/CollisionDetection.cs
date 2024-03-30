using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Scellecs.Morpeh;

public class CollisionDetection : MonoBehaviour
{
    private bool test = false;

    private void Start()
    {
        
    test = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!test)
            return;
        // foreach (var collider)
        Collider myCollider = other.GetContact(0).thisCollider;
        if (myCollider.TryGetComponent<PartOfTheSystem>(out var part))
        {



            var targetEntity = myCollider.GetComponent<HealthComponentProvider>().GetEntity();

            //GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = targetEntity, collision = other });
            Debug.Log("test");


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
