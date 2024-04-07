using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;

public struct ChangeDaytimeRequest : IRequestData
{
    public NightyDaity.NightyDaityEnum statusForNow;
}