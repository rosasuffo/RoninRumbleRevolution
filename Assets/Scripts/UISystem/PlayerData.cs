using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int characterIdFromList;
    public int playerLife;
    //public string playerName;

    public bool Equals(PlayerData other)
    {
        bool clientCheck = clientId == other.clientId;
        bool characterIdCheck = characterIdFromList == other.characterIdFromList;
        bool playerLifeCheck = playerLife == other.playerLife;
        //bool playerNameCheck = playerName == other.playerName;
        return clientCheck && characterIdCheck && playerLifeCheck;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterIdFromList);
        serializer.SerializeValue(ref playerLife);
        //serializer.SerializeValue(ref playerName);
    }

    public void TakeHit(int damage)
    {
        playerLife -= damage;
    }
}
