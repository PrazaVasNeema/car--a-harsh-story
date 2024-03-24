using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct OnCollisionEnterEventKolhoz : IComponent {
    public Collision collision;
}

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public sealed class OnCollisionEnterEventKolhozProvider : MonoProvider<OnCollisionEnterEventKolhoz> {
    public void SetCollision(Collision collision)
    {
        this.GetData().collision = collision;
    }
}