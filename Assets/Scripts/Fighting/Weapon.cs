using Movement.Components;
using Netcode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

namespace Fighting
{
    public class Weapon : NetworkBehaviour
    {
        public Animator effectsPrefab;
        private static readonly int Hit03 = Animator.StringToHash("hit03");

        [SerializeField] private int Damage;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject otherObject = collision.gameObject;
            // Debug.Log($"Sword collision with {otherObject.name}");

            Animator effect = Instantiate(effectsPrefab);
            effect.transform.position = collision.GetContact(0).point;
            effect.SetTrigger(Hit03);

            
            // TODO: Review if this is the best way to do this
            IFighterReceiver enemy = otherObject.GetComponent<IFighterReceiver>();
            PlayerData playerData;
            if (enemy != null)
            {
                enemy.TakeHit();

                //Buscamos al cliente que recibe el daño
                NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();
                playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(networkObject.OwnerClientId);

                //PlayerData playerData = otherObject.GetComponent<PlayerData>();
                Debug.Log($"El Jugador {OwnerClientId} ha dado al jugador {playerData.clientId}");
                TakeDamageFromCollisionServerRpc(playerData);
            }    

            //OnCollisionEnter2DServerRpc(collision);
        }
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageFromCollisionServerRpc(PlayerData playerData, ServerRpcParams serverRpcParams = default)
        {
            Debug.Log(playerData.clientId + ": Auch");
            
            Debug.Log("Initial life: " + playerData.playerLife);
            playerData.TakeHit(Damage);
            GameMultiplayer.Instance.UpdatePlayerDataServerRpc(playerData);
            Debug.Log("Life after damage: " + playerData.playerLife);

        }
    }
}
