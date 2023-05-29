using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int characterIdFromList;
    public int hp;
    public string playerName;

    /*public PlayerData(ulong clientId, int characterIdFromList, int hp, string playerName)
    {
        this.clientId = clientId;
        this.characterIdFromList = characterIdFromList;
        this.hp = hp;
        this.playerName = playerName;
    }*/
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && characterIdFromList == other.characterIdFromList && hp == other.hp && playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterIdFromList);
        serializer.SerializeValue(ref hp);
        serializer.SerializeValue(ref playerName);
    }
   // public static int GetHP() { return hp; }
   // public static void SetHP(int newHp) { hp = newHp; }
}
