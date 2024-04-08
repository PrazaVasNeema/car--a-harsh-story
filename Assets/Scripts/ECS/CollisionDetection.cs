using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Scellecs.Morpeh;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] private bool m_splashDamageEnabled = false;
    [SerializeField] private List<Entity> m_entitiesWithHealth;
    public List<Entity> entitiesWithHealth => m_entitiesWithHealth;

    private void Start()
    {
        if (!m_splashDamageEnabled)
            return;
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            if (collider.TryGetComponent<HealthComponentProvider>(out var a))
            {
                m_entitiesWithHealth.Add(a.Entity);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        ContactPoint[] contacts = new ContactPoint[other.contactCount];
        int points = other.GetContacts(contacts);


        Entity targetEntity = null;
        Collider myCollider = other.GetContact(0).thisCollider;
        if (myCollider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider))
        {

            targetEntity = entityReverseProvider.GetEntity();

        }
        else if (myCollider.TryGetComponent<LocalMainframeRefForAServant>(out var refForMainframe))
        {
            if(refForMainframe.refForMainframe == null)
                return;
            if (refForMainframe.refForMainframe.TryGetComponent<EntityReverseProvider>(out entityReverseProvider))
            {

                targetEntity = entityReverseProvider.GetEntity();

            }
        }
        
        if (targetEntity != null)
            PublishCollisionEvent(other, targetEntity);

    }

    public void PublishCollisionEvent(Collision collision, Entity affectedEntity)
    {

        if (GameData.instance.currentWorld != null) 
        {
            GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = affectedEntity, contactPoint = collision.GetContact(0) , name = collision.GetContact(0).thisCollider.transform.name});
        }

    }

    public void RemoveEntityFromTheList(Entity entityToRemove)
    {
        m_entitiesWithHealth.Remove(entityToRemove);
    }
    
}
