using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

public struct OnCollisionEnterEvent : IEventData
{
    public Entity targetEntity;
    public ContactPoint contactPoint;
    public string name;
}