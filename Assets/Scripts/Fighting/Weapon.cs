using Movement.Components;
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

            //Encontrar a que personaje esta dando
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromGameObjectCharacter(otherObject);
            //PlayerData playerData = otherObject.GetComponent<PlayerData>();
            Debug.Log($"El Jugador {NetworkManager.Singleton.LocalClientId} ha dado al jugador {playerData.clientId}");
            TakeDamageFromCollisionServerRpc(playerData);

            Animator effect = Instantiate(effectsPrefab);
            effect.transform.position = collision.GetContact(0).point;
            effect.SetTrigger(Hit03);

            // TODO: Review if this is the best way to do this
            IFighterReceiver enemy = otherObject.GetComponent<IFighterReceiver>();
            if (enemy != null)
                enemy.TakeHit();

            //OnCollisionEnter2DServerRpc(collision);
        }
        [ServerRpc(RequireOwnership = false)]
        public void TakeDamageFromCollisionServerRpc(PlayerData playerData, ServerRpcParams serverRpcParams = default)
        {
            Debug.Log(playerData.clientId + ": Auch");
            
            Debug.Log("Initial life: " +playerData.playerLifeBar);
            playerData.playerLifeBar -= Damage;
            Debug.Log("Life after damage: " + playerData.playerLifeBar);

        }
    }
}
