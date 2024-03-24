using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

public struct SpawnDamageDecalRequest : IRequestData
{
    public Entity targetEntity;
    public ContactPoint ContactPoint;
}