using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;

public struct DoDamageRequest : IRequestData
{
    public Entity targetEntity;
    public float damageAmount;
}