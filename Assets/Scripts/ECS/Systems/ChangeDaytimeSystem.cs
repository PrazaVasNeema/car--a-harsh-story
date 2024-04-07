using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ChangeDaytimeSystem))]
public sealed class ChangeDaytimeSystem : UpdateSystem {
    protected Filter filter;
    private Request<ChangeDaytimeRequest> changeDaytimeRequest;
    
    public override void OnAwake()
    {
        this.filter = this.World.Filter.With<NightyDaity>().With<TransformRef>().Build();
        this.changeDaytimeRequest = this.World.GetRequest<ChangeDaytimeRequest>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var changeDaytimeRequest in changeDaytimeRequest.Consume())
        {
            Debug.Log("NightDaity1");
            foreach (var entity in this.filter)
            {
                Debug.Log("NightDaity2");

                ref var nightyDaityComp = ref entity.GetComponent<NightyDaity>();
                ref var transformRefComp = ref entity.GetComponent<TransformRef>();
                
                transformRefComp.transform.gameObject.SetActive(nightyDaityComp.theeStatus == changeDaytimeRequest.statusForNow);
            }
           
        }
    }
}