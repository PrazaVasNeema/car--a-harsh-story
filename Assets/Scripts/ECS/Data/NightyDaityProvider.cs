using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct NightyDaity : IComponent
{
    public enum NightyDaityEnum
    {
        Nighty,
        Daity
    }

    public NightyDaityEnum theeStatus;
}

[RequireComponent(typeof(TransformRefProvider))]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public sealed class NightyDaityProvider : MonoProvider<NightyDaity> {
    
}