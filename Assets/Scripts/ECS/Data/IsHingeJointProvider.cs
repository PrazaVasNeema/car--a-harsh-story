using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct IsHingeJoint : IComponent {
    public int HPThreshold;
    public float minAngle;
    public float maxAngle;
    [HideInInspector]
    public HingeJoint hingeJoint;
}

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public sealed class IsHingeJointProvider : MonoProvider<IsHingeJoint> {
    private void Awake()
    {
        this.GetData().hingeJoint = GetComponent<HingeJoint>();
        // hingeJoint.limits = new JointLimits() { min = 0, max = 0 };
        // this.GetData().hingeJoint = GetComponent<HingeJoint>();
        // hingeJoint.limits = joinLimit;
        this.GetData().hingeJoint.limits = new JointLimits() { min = 0, max = 0 };
    }
}