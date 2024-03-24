using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(SetGameDataInitializer))]
public sealed class SetGameDataInitializer : Initializer {
    public override void OnAwake()
    {
        GameData.instance.currentWorld = this.World;
    }

    public override void Dispose() {
    }
}