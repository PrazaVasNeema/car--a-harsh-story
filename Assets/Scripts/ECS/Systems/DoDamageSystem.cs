using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DoDamageSystem))]
public sealed class DoDamageSystem : UpdateSystem {
    protected Filter filter;
    private Request<DoDamageRequest> doDamageRequest;
    
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<HealthComponent>().Build();
        this.doDamageRequest = this.World.GetRequest<DoDamageRequest>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var doDamageRequest in doDamageRequest.Consume())
        {
            if (doDamageRequest.targetEntity.Has<IsDisabledMarker>())
                continue;
            Debug.Log(doDamageRequest.targetEntity);
            Debug.Log(doDamageRequest.damageAmount);
            ref var healthComponent = ref doDamageRequest.targetEntity.GetComponent<HealthComponent>();
            // healthComponent.HP = (int)Mathf.Max(0, healthComponent.HP - doDamageRequest.damageAmount);
            healthComponent.HP = (int)(healthComponent.HP - doDamageRequest.damageAmount);
            if (healthComponent.HP <= 0)
            {
                this.World.GetRequest<BreakThisRequest>().Publish(new BreakThisRequest {targetEntity = doDamageRequest.targetEntity, breakCompletely = true}, true);
                doDamageRequest.targetEntity.AddComponent<IsDisabledMarker>();

            }
            else
            {
                if (doDamageRequest.targetEntity.Has<IsHingeJoint>() && !doDamageRequest.targetEntity.GetComponent<IsHingeJoint>().hasBeenDone && healthComponent.HP <= doDamageRequest.targetEntity.GetComponent<IsHingeJoint>().HPThreshold)
                {
                    this.World.GetRequest<BreakThisRequest>().Publish(new BreakThisRequest {targetEntity = doDamageRequest.targetEntity, breakCompletely = false}, true);
                }
            }
        }
    }
}