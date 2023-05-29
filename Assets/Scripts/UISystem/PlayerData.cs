using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int characterIdFromList;
    public int playerLifeBar;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && characterIdFromList == other.characterIdFromList;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterIdFromList);
        serializer.SerializeValue(ref playerLifeBar);
    }
}
