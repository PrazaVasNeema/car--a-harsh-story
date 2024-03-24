using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public struct IsBreakableChange : IComponent {
    [Serializable]
    public struct DestructionStates
    {
        public int HPThreshold;
        public GameObject NewStateGO;
    }

    public List<DestructionStates> DestructionStatesList;
    public GameObject CurGO;
}

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
public sealed class IsBreakableChangeProvider : MonoProvider<IsBreakableChange> {
    private void Awake()
    {
        this.GetData().CurGO = gameObject;
    }
}