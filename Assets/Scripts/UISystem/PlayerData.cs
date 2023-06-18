using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Movement.Components;
using Unity.Services.Lobbies.Models;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    //public static PlayerData Instance { get; private set; }

    //public event EventHandler OnPlayerLifeToZero;
    public ulong clientId;
    public FixedString64Bytes playerName;
    public int characterIdFromList;
    public int playerLife;

    public bool Equals(PlayerData other)
    {
        bool clientCheck = clientId == other.clientId;
        bool playerNameCheck = playerName == other.playerName;
        bool characterIdCheck = characterIdFromList == other.characterIdFromList;
        bool playerLifeCheck = playerLife == other.playerLife;
        return clientCheck && playerNameCheck && characterIdCheck && playerLifeCheck;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref characterIdFromList);
        serializer.SerializeValue(ref playerLife);
        //serializer.SerializeValue(ref playerName);
    }

    public void TakeHit(int damage)
    {
        playerLife -= damage;
    }
}
