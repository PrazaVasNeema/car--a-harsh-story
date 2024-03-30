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
            if (change.collision.contactCount == 0)
                continue;
            float damageAmount = change.collision.impulse.magnitude;
                damageAmount = change.collision.GetContact(0).impulse.magnitude;
            //Debug.Log($"EntityName: {change.targetEntity.GetComponent<TransformRef>().transform.name}");
            if (damageAmount >= m_settingsData.spawnDecalsHPThreshold && change.targetEntity.Has<IsDamageDecalReceiver>())
            {
                // List<ContactPoint> contact = new List<ContactPoint>();
                // Debug.Log("Test1");
                // foreach (var VARIABLE in change.collision.contacts)
                // {
                //     
                // }
                this.World.GetRequest<SpawnDamageDecalRequest>().Publish(new SpawnDamageDecalRequest { targetEntity = change.targetEntity, ContactPoint = change.collision.GetContact(0) }, true);
            }
            if (change.targetEntity.Has<HealthComponent>())
                this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest { targetEntity = change.targetEntity, damageAmount = damageAmount }, true);
            if (damageAmount >= m_settingsData.gigaSplashHPThreshold)
            {
                var a = change.targetEntity.GetComponent<TransformRef>().transform;
                var b = Physics.SphereCastAll(a.position, m_settingsData.gigaSplashSphereRadius,
                    change.collision.GetContact(0).normal,
                    m_settingsData.gigaSplashCastDistance, m_settingsData.gigaSplashLayerMask);
                foreach (var c in b)
                {
                    if (!c.collider.TryGetComponent<HealthComponentProvider>(out var healthComponent))
                        continue;

                    //Debug.Log($"EntityName3 {c.collider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider)}");

                    if (c.collider.TryGetComponent<EntityReverseProvider>(out var entityReverseProvider))
                    {
                            if (c.collider.name == "RightDoor")
                            {
                                Debug.Log($"Damage: {m_settingsData.gigaSplashSphereRadius + m_settingsData.gigaSplashCastDistance}");
                                Debug.Log($"Damage2: {(change.collision.transform.position - c.collider.transform.position).magnitude}");
                                Debug.Log($"Damage3: {Mathf.Lerp(0, m_settingsData.gigaSplashSphereRadius + m_settingsData.gigaSplashCastDistance,(change.collision.transform.position - c.collider.transform.position).magnitude)}");
                                Debug.Log($"DamageTrue: {damageAmount}");

                            }
                            var damageMultiplier = Mathf.Lerp(0, m_settingsData.gigaSplashSphereRadius + m_settingsData.gigaSplashCastDistance,
                            (change.collision.transform.position - c.collider.transform.position).magnitude);

                            var damageAmountSplash = damageAmount * damageMultiplier;
                            this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest
                            {
                                targetEntity = entityReverseProvider.GetEntity(),
                                damageAmount = damageAmountSplash
                            }, true);
                        }
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