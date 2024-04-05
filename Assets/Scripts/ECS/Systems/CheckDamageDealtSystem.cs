using System.Collections.Generic;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CheckDamageDealtSystem))]
public sealed class CheckDamageDealtSystem : UpdateSystem {

    protected Filter filter;
    private DamageSystemSettingSO.Data m_settingsData;
    
    public override void OnAwake()
    {
        m_settingsData = GameData.instance.damageSystemSetting.data;

        var onCollisionEnterEvent = this.World.GetEvent<OnCollisionEnterEvent>().Subscribe(changes =>
        {
        foreach (var change in changes)
        {
                Debug.Log($"TEST: 1");
                // Debug.Log($"damage amount before test name: {change.collision.GetContact(0).thisCollider.name}, point: {change.collision.contactCount}");
            // Debug.Log($"Damage amount IN name: {change.name}, contactCount: {change.collision.contactCount}");
            // if (change.collision.contactCount == 0)
            //     continue;
            float damageAmount = change.contactPoint.impulse.magnitude;
                damageAmount = change.contactPoint.impulse.magnitude;
                Debug.Log($"TEST: 2");
                Debug.Log($"Damage amount: {damageAmount}, name: {change.contactPoint.thisCollider.name}");

                //Debug.Log($"EntityName: {change.targetEntity.GetComponent<TransformRef>().transform.name}");
                if (damageAmount >= m_settingsData.spawnDecalsHPThreshold && change.targetEntity.Has<IsDamageDecalReceiver>())
            {
                    // List<ContactPoint> contact = new List<ContactPoint>();
                    // Debug.Log("Test1");
                    // foreach (var VARIABLE in change.collision.contacts)
                    // {
                    //     
                    // }
                    Debug.Log($"TEST: 3");

                    this.World.GetRequest<SpawnDamageDecalRequest>().Publish(new SpawnDamageDecalRequest { targetEntity = change.targetEntity, ContactPoint = change.contactPoint }, true);
            }
                Debug.Log($"TEST: 4");

                if (change.targetEntity.Has<HealthComponent>())
                {
                    Debug.Log($"TEST: 5");

                    this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest { targetEntity = change.targetEntity, damageAmount = damageAmount }, true);
                }
                Debug.Log($"TEST: 6");

                if (damageAmount >= m_settingsData.gigaSplashHPThreshold)
            {
                    Debug.Log($"TEST: 7");

                    var a = change.targetEntity.GetComponent<TransformRef>().transform;
                    Debug.Log($"TEST: 8");

                    if (a.GetComponentInParent<CollisionDetection>() == null)
                        break;
                    var entitiesList = a.GetComponentInParent<CollisionDetection>().entitiesWithHealth;
                    Debug.Log($"TEST: 9");

                    Debug.Log($"Affected: {entitiesList.Count}");
                    Debug.Log($"TEST: 10");

                    Debug.Log($"AffectedNAMEE: {a.name}");
                    foreach (var entity in entitiesList)
                    {
                        var entityTransform = entity.GetComponent<TransformRef>().transform;
                        Debug.Log($"Name: {entityTransform.name}");
                        var entitiesDistance = (change.contactPoint.point - entityTransform.position).magnitude;
                        var damageMultiplier = Mathf.InverseLerp(0, 5, entitiesDistance);
                        var damageAmountSplash = damageAmount * -Mathf.Log10(damageMultiplier)/2;
                        Debug.Log($"Distance: {entitiesDistance}");
                        Debug.Log($"Initial damage: {damageAmount}; damageMultiplier: {damageMultiplier}; log: {-Mathf.Log10(damageMultiplier)}; Total damage: {damageAmountSplash}");
                        this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest
                        {
                            targetEntity = entity,
                            damageAmount = damageAmountSplash
                        }, true);

                    }
                //var b = Physics.SphereCastAll(a.position, m_settingsData.gigaSplashSphereRadius,
                //    change.collision.GetContact(0).normal,
                //    m_settingsData.gigaSplashCastDistance, m_settingsData.gigaSplashLayerMask);
                //foreach (var c in b)
                //{
                //    if (!c.collider.TryGetComponent<HealthComponentProvider>(out var healthComponent))
                //        continue;

                //    //Debug.Log($"EntityName3 {c.collider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider)}");

                //    if (c.collider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider))
                //    {
                //            if (c.collider.name == "RightDoor")
                //            {
                //                Debug.Log($"Damage: {m_settingsData.gigaSplashSphereRadius + m_settingsData.gigaSplashCastDistance}");
                //                Debug.Log($"Damage2: {(change.collision.transform.position - c.collider.transform.position).magnitude}");
                //                Debug.Log($"Damage3: {Mathf.Lerp(0, m_settingsData.gigaSplashSphereRadius + m_settingsData.gigaSplashCastDistance,(change.collision.transform.position - c.collider.transform.position).magnitude)}");
                //                Debug.Log($"DamageTrue: {damageAmount}");

                //            }
                //            var damageMultiplier = Mathf.Lerp(0, m_settingsData.gigaSplashSphereRadius + m_settingsData.gigaSplashCastDistance,
                //            (change.collision.transform.position - c.collider.transform.position).magnitude);

                //            var damageAmountSplash = damageAmount * damageMultiplier;
                //            this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest
                //            {
                //                targetEntity = entityReverseProvider.GetEntity(),
                //                damageAmount = damageAmountSplash
                //            }, true);
                //        }
                //    }
                }
            }
        });
    }

    public override void OnUpdate(float deltaTime) {
        // foreach (var entity in this.filter)
        // {
        //     ref var eventComponent = ref entity.GetComponent<OnCollisionEnterEventKolhoz>();
        //     Collider myCollider = eventComponent.collision.GetContact(0).thisCollider;
        //     float damageAmount = eventComponent.collision.impulse.sqrMagnitude;
        //     if (damageAmount >= m_settingsData.spawnDecalsHPThreshold)
        //     {
        //         World.GetRequest<SpawnDamageDecalRequest>().Publish(new SpawnDamageDecalRequest { ContactPoint = eventComponent.collision.GetContact(0) });
        //     }
        //     World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest {damageAmount = damageAmount});
        //
        //     entity.RemoveComponent<OnCollisionEnterEventKolhoz>();
        // }
    }
}