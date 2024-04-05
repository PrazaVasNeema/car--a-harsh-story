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
        Debug.Log($"Damage amount loop NAME: {other.transform.name}, contactCount: {other.contactCount}");
        for (int i = 0; i < points; i++)
        {
            Debug.Log($"Damage amount loop: {i}, {contacts[i].impulse.magnitude}, this collider NAME: {contacts[i].thisCollider.transform.name}");
            // ContactPoint cp = contact*;*
            // if (GetComponent().gameObject != gameObject)
                // continue;
        }

        Collider myCollider = other.GetContact(0).thisCollider;
        if (myCollider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider))
        {
            Debug.Log($"Damage amount loop:  this collider NAME: {myCollider.transform.name}");

            var targetEntity = entityReverseProvider.GetEntity();
            //GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = targetEntity, collision = other });

            PublishCollisionEvent(other, targetEntity);
        }
    }

    public void PublishCollisionEvent(Collision collision, Entity affectedEntity)
    {

        if (GameData.instance.currentWorld != null) 
        {
            Debug.Log($"Damage amount loop: {collision.GetContact(0).thisCollider.transform.name}, contactCount: {collision.contactCount}");
            GameData.instance.currentWorld.GetEvent<OnCollisionEnterEvent>().NextFrame(new OnCollisionEnterEvent { targetEntity = affectedEntity, contactPoint = collision.GetContact(0) , name = collision.GetContact(0).thisCollider.transform.name});
        }

    }

    public void RemoveEntityFromTheList(Entity entityToRemove)
    {
        m_entitiesWithHealth.Remove(entityToRemove);
    }
    
}
