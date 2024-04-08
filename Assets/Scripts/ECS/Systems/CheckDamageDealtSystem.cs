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

    private Filter filter;
    private DamageSystemSettingSO.Data m_settingsData;
    
    public override void OnAwake()
    {
        m_settingsData = GameData.instance.damageSystemSetting.data;

        var onCollisionEnterEvent = this.World.GetEvent<OnCollisionEnterEvent>().Subscribe(changes =>
        {
        foreach (var change in changes)
        {
            if(change.targetEntity.IsDisposed())
                continue;

            float damageAmount = change.contactPoint.impulse.magnitude;
                damageAmount = change.contactPoint.impulse.magnitude;
                if (damageAmount >= m_settingsData.spawnDecalsHPThresholdLow && change.targetEntity.Has<IsDamageDecalReceiver>())
            {

                    SpawnDamageDecalRequest.DecalDamageType damageType =
                        damageAmount >= m_settingsData.spawnDecalsHPThresholdHigh
                            ? SpawnDamageDecalRequest.DecalDamageType.High
                            : SpawnDamageDecalRequest.DecalDamageType.Low;
                    this.World.GetRequest<SpawnDamageDecalRequest>().Publish(new SpawnDamageDecalRequest { targetEntity = change.targetEntity, ContactPoint = change.contactPoint, damageType = damageType}, true);
            }
 
            if (change.targetEntity.Has<HealthComponent>())
            {

                this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest { targetEntity = change.targetEntity, damageAmount = damageAmount }, true);
            }

            if (damageAmount >= m_settingsData.gigaSplashHPThreshold)
            {

                    ref var  a = ref  change.targetEntity.GetComponent<TransformRef>().transform;

                    if (a.GetComponentInParent<CollisionDetection>() == null)
                        break;
                    var entitiesList = a.GetComponentInParent<CollisionDetection>().entitiesWithHealth;
                    foreach (var entity in entitiesList)
                    {
                        ref var entityTransform = ref entity.GetComponent<TransformRef>().transform;
                        float entitiesDistance = (change.contactPoint.point - entityTransform.position).magnitude;
                        float damageMultiplier = Mathf.InverseLerp(0, 5f, entitiesDistance);
                        float damageAmountSplash = damageAmount * -Mathf.Log10(damageMultiplier)/2;
                        this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest
                        {
                            targetEntity = entity,
                            damageAmount = damageAmountSplash
                        }, true);

                    }
            
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