using System.Numerics;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpawnDamageDecalSystem))]
public sealed class SpawnDamageDecalSystem : UpdateSystem {
    protected Filter filter;
    private Request<SpawnDamageDecalRequest> spawnDecalRequest;
    public override void OnAwake() {
        // this.filter = this.World.Filter.With<HealthComponent>().Build();
        this.spawnDecalRequest = this.World.GetRequest<SpawnDamageDecalRequest>();
        
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var spawnDecalRequest in spawnDecalRequest.Consume())
        {
            Debug.Log("Test2");
            // Debug.Log(GameData.instance.damageSystemSetting.data.decalPrefabs[0]);
            var contactPoint = spawnDecalRequest.ContactPoint;
            
            
            
            Vector3 meshForward = spawnDecalRequest.targetEntity.GetComponent<TransformRef>().transform.forward;
            float angle = Mathf.Acos(Vector3.Dot(meshForward, contactPoint.normal));
            angle = Vector3.Dot(meshForward, contactPoint.normal);
            angle = Vector3.Angle(meshForward, contactPoint.normal);

            // Vector3 angles = Vector3.Angle(meshForward, contactPoint.normal);


            Debug.Log(angle);

            // Vector3 off = thisRoom.RoomSpawnPoint.rotation.eulerAngles - _currentEnvironment.transform.rotation.eulerAngles;
            Quaternion a = Quaternion.LookRotation(-contactPoint.normal, Vector3.up); 
            Debug.Log(a);
            var targetPosition = contactPoint.point + (float).1 * contactPoint.normal;
            DamageSystemSettingSO.Data data = GameData.instance.damageSystemSetting.data;
            targetPosition = contactPoint.point;
            GameObject decalPrefab = spawnDecalRequest.damageType == SpawnDamageDecalRequest.DecalDamageType.Low
                ? data.decalLowDamagePrefabs[Random.Range((int)0, data.decalLowDamagePrefabs.Count)]
                : data.decalHighDamagePrefabs[Random.Range((int)0, data.decalHighDamagePrefabs.Count)];
            var b = Instantiate(decalPrefab, targetPosition, a);
            b.transform.parent =    spawnDecalRequest.targetEntity.GetComponent<TransformRef>().transform;
        }
    }
}