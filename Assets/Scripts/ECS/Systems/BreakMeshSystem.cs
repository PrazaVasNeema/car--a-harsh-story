using System.Collections.Generic;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Unity.VisualScripting;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BreakMeshSystem))]
public sealed class BreakMeshSystem : UpdateSystem {
    private Request<BreakThisRequest> breakThisRequest;
    
    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();
    
    public override void OnAwake() {
        this.breakThisRequest = this.World.GetRequest<BreakThisRequest>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var breakThisRequest in breakThisRequest.Consume())
        {
            var entity = breakThisRequest.targetEntity;
            if (breakThisRequest.breakCompletely)
            {
                if (entity.Has<IsBreakableMesh>())
                {
                    entity.GetComponent<TransformRef>().transform.GetComponentInParent<CollisionDetection>().RemoveEntityFromTheList(entity);
                    var a = entity.GetComponent<IsBreakableMesh>();
                    a.whole.SetActive(false);
                    GameData.instance.AddBrokenGlass(a.broken);
                    a.broken.SetActive(true);
                   
                    entity.Dispose();
                }
                else if (entity.Has<IsDetachable>())
                {
                    entity.GetComponent<TransformRef>().transform.GetComponentInParent<CollisionDetection>().RemoveEntityFromTheList(entity);
                    GameData.instance.AddBrokenDetail(entity.GetComponent<TransformRef>().transform.gameObject);
                    var transform = entity.GetComponent<TransformRef>().transform;
                    if (transform.TryGetComponent(out Rigidbody body))
                    {
                        if (transform.TryGetComponent(out HingeJoint hingeJoint))
                        {
                            Destroy(hingeJoint);
                        }
                        if (transform.TryGetComponent(out CollisionDetectionOutpost collisionDetectionOutpost))
                        {
                            Destroy(collisionDetectionOutpost);
                        }
                    }
                    else
                    {
                        transform.AddComponent<Rigidbody>();
                    }
                }
            }
            else
            {
                if (entity.Has<IsHingeJoint>())
                {
                    // hingeJoint.limits.min = 
                    var isHingeJoint = entity.GetComponent<IsHingeJoint>();
                    
                    isHingeJoint.hingeJoint.limits = new JointLimits() { min = isHingeJoint.minAngle, max = isHingeJoint.maxAngle, 
                    bounciness = isHingeJoint.bounciness, bounceMinVelocity = isHingeJoint.bounceMinVelocity};
                    
                }
            }
        }
    }

   
}