using Cinemachine;
using Movement.Components;
using InputSystems;
using Unity.Netcode;
using System.Numerics;
using System.Diagnostics;
using UnityEngine;
using Unity.Netcode.Components;
using UISystem.Managers;
using UnityEngine.UI;

namespace Netcode
{
    public class FighterNetworkConfig : NetworkBehaviour
    {

        public static FighterNetworkConfig LocalInstance { get; private set; }

        //[SerializeField] private GameObject publicLifebar;


        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            LocalInstance = this;

            //InstantiateFighterServerRpc(OwnerClientId);

            FighterMovement fighterMovement = GetComponent<FighterMovement>();
            InputSystem.Instance.Character = fighterMovement;
            ICinemachineCamera virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
            virtualCamera.Follow = transform;

            //InstantiateLifeBarPlayerServerRpc(OwnerClientId);
        }

        /*
        [ServerRpc]
        public void InstantiateLifeBarPlayerServerRpc(ulong clientId)
        {
            GameObject lifeBar = Instantiate(GameManager.Instance.GetSliderPrefabForLifebar().gameObject);
            lifeBar.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            lifeBar.transform.SetParent(transform, true);
        }*/



    }
}