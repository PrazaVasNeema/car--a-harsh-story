using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

public struct SpawnDamageDecalRequest : IRequestData
{
    public enum DecalDamageType
    {
        Low,
        High
    }
    public Entity targetEntity;
    public ContactPoint ContactPoint;
    public DecalDamageType damageType;
}