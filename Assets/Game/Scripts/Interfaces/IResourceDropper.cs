using HexaClash.Game.Scripts.Data.ValueData;
using UnityEngine;

namespace HexaClash.Game.Scripts.Interfaces
{
    public interface IResourceDropper
    {
        ResourceData GetResourceData();
        Vector3 GetDropPosition();
    }
}