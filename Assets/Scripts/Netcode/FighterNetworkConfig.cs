using Cinemachine;
using Movement.Components;
using InputSystems;
using Unity.Netcode;
using System.Numerics;
using System.Diagnostics;
using UnityEngine;
using Unity.Netcode.Components;
using UISystem.Managers;

namespace Netcode
{
    public class FighterNetworkConfig : NetworkBehaviour
    {

        public static FighterNetworkConfig LocalInstance { get; private set; }


        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            LocalInstance = this;

            //InstantiateFighterServerRpc(OwnerClientId);

            FighterMovement fighterMovement = GetComponent<FighterMovement>();
            InputSystem.Instance.Character = fighterMovement;
            ICinemachineCamera virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
            virtualCamera.Follow = transform;

            
        }

        [ServerRpc]
        public void InstantiateFighterServerRpc(ulong id)
        {
            Transform fighter = Instantiate(LocalInstance.transform);
            fighter.GetComponent<NetworkObject>().SpawnWithOwnership(id);
            fighter.transform.SetParent(transform, false);
        }



    }
}