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
        this.filter = this.World.Filter.With<OnCollisionEnterEventKolhoz>().Build();
        m_settingsData = GameData.instance.damageSystemSetting.data;

        var onCollisionEnterEvent = this.World.GetEvent<OnCollisionEnterEvent>().Subscribe(changes =>
        {
            foreach (var change in changes)
            {
                if (change.collision.contactCount == 0)
                    continue;
                float damageAmount = change.collision.impulse.sqrMagnitude;
                Debug.Log($"1: {damageAmount >= m_settingsData.spawnDecalsHPThreshold}, 2: {change.targetEntity.Has<IsDamageDecalReceiver>()}");
                Debug.Log($"change.collision.GetContact(0): {change.collision.GetContact(0)}");
                Debug.Log($"change.collision.GetContacts(0): {change.collision.contactCount}");
                if (damageAmount >= m_settingsData.spawnDecalsHPThreshold && change.targetEntity.Has<IsDamageDecalReceiver>())
                {
                    // List<ContactPoint> contact = new List<ContactPoint>();
                    // Debug.Log("Test1");
                    // foreach (var VARIABLE in change.collision.contacts)
                    // {
                    //     
                    // }
                    this.World.GetRequest<SpawnDamageDecalRequest>().Publish(new SpawnDamageDecalRequest { targetEntity = change.targetEntity, ContactPoint = change.collision.GetContact(0)}, true);
                }
                this.World.GetRequest<DoDamageRequest>().Publish(new DoDamageRequest { targetEntity = change.targetEntity, damageAmount = damageAmount}, true);
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