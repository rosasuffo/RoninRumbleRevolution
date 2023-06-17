using InputSystems;
using UISystem.Managers;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UISystem.GameSceneUI;

namespace Movement.Components
{
    [RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(Animator)),
     RequireComponent(typeof(NetworkObject))]
    public sealed class FighterMovement : NetworkBehaviour, IMoveableReceiver, IJumperReceiver, IFighterReceiver
    {
        public float speed = 1.0f;
        public float jumpAmount = 1.0f;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private NetworkAnimator _networkAnimator;
        private Transform _feet;
        private LayerMask _floor;
        private Slider _lifebar;

        private Vector3 _direction = Vector3.zero;
        private bool _grounded = true;

        //Authoritative server
        //NetworkVariable<bool> myGrounded = new();
        NetworkVariable<Vector3> myDirection = new();

        private static readonly int AnimatorSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorVSpeed = Animator.StringToHash("vspeed");
        private static readonly int AnimatorGrounded = Animator.StringToHash("grounded");
        private static readonly int AnimatorAttack1 = Animator.StringToHash("attack1");
        private static readonly int AnimatorAttack2 = Animator.StringToHash("attack2");
        private static readonly int AnimatorHit = Animator.StringToHash("hit");
        private static readonly int AnimatorDie = Animator.StringToHash("die");

        public void Awake()
        {
            //myTransform.OnValueChanged += TransformValueChanged;
            //myGrounded.OnValueChanged += GroundedValueChanged;
            myDirection.OnValueChanged += DirectionValueChanged;
        }

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _networkAnimator = GetComponent<NetworkAnimator>();
            _lifebar = GetComponentInChildren<Canvas>().GetComponentInChildren<Slider>();

            _feet = transform.Find("Feet");
            _floor = LayerMask.GetMask("Floor");

            if (IsOwner)
            {
                GameManager.Instance.OnPlayerPaused += GameManager_OnPlayerPaused;
                GameManager.Instance.OnPlayerUnpaused += GameManager_OnPlayerUnpaused;
            }

        }



        void Update()
        {
            _grounded = Physics2D.OverlapCircle(_feet.position, 0.1f, _floor);
            _animator.SetFloat(AnimatorSpeed, this.myDirection.Value.magnitude);
            _animator.SetFloat(AnimatorVSpeed, this._rigidbody2D.velocity.y);
            _animator.SetBool(AnimatorGrounded, this._grounded);

        }

        void FixedUpdate()
        {
            if (!IsServer || !IsSpawned) return;

            _rigidbody2D.velocity = new Vector2(myDirection.Value.x, _rigidbody2D.velocity.y);

            //UpdateVelocityClientRpc(_rigidbody2D.velocity);
            //myTransform.Value = transform.position;
        }


        private void TransformValueChanged(Vector3 previous, Vector3 newValue)
        {
            transform.position = newValue;
        }

        private void DirectionValueChanged(Vector3 previous, Vector3 newValue)
        {
            _direction = newValue;
        }

        /*
        public void Move(IMoveableReceiver.Direction direction)
        {
            if (direction == IMoveableReceiver.Direction.None)
            {
                this._direction = Vector3.zero;
                return;
            }

            bool lookingRight = direction == IMoveableReceiver.Direction.Right;
            _direction = (lookingRight ? 1f : -1f) * speed * Vector3.right;
            transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
        }*/

        public void Move(IMoveableReceiver.Direction direction)
        {

            MoveServerRpc(direction);
        }

        [ServerRpc]
        public void MoveServerRpc(IMoveableReceiver.Direction direction)
        {
            if (direction == IMoveableReceiver.Direction.None)
            {
                this.myDirection.Value = Vector3.zero;
                return;
            }

            bool lookingRight = direction == IMoveableReceiver.Direction.Right;
            myDirection.Value = (lookingRight ? 1f : -1f) * speed * Vector3.right;
            //transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);

            //myTransform.Value = transform.position;

            UpdateDirectionClientRpc(lookingRight);
        }


        [ClientRpc]
        public void UpdateDirectionClientRpc(bool rightDir)
        {
            transform.localScale = new Vector3(rightDir ? 1 : -1, 1, 1);
        }

        /*
        public void Jump(IJumperReceiver.JumpStage stage)
        {
            switch (stage)
            {
                case IJumperReceiver.JumpStage.Jumping:
                    if (_grounded)
                    {
                        float jumpForce = Mathf.Sqrt(jumpAmount * -2.0f * (Physics2D.gravity.y * _rigidbody2D.gravityScale));
                        _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                    break;
                case IJumperReceiver.JumpStage.Landing:
                    break;
            }
        }*/

        public void Jump(IJumperReceiver.JumpStage stage)
        {
            JumpServerRpc(stage);
        }

        [ServerRpc]
        public void JumpServerRpc(IJumperReceiver.JumpStage stage)
        {
            switch (stage)
            {
                case IJumperReceiver.JumpStage.Jumping:
                    if (_grounded)
                    {
                        float jumpForce = Mathf.Sqrt(jumpAmount * -2.0f * (Physics2D.gravity.y * _rigidbody2D.gravityScale));
                        _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                    break;
                case IJumperReceiver.JumpStage.Landing:
                    break;
            }
        }

        public void Attack1()
        {
            Attack1ServerRpc();
        }
        [ServerRpc]
        public void Attack1ServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorAttack1);
        }

        public void Attack2()
        {
            Attack1ServerRpc();
        }
        [ServerRpc]
        public void Attack2ServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorAttack2);
        }

        public void TakeHit()
        {
            TakeHitServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void TakeHitServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorHit);
            //_lifebar.value -= 5;
            UpdateLifebarClientRpc();
        }

        public void Die()
        {
            DieServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void DieServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorDie);
        }
        
        [ClientRpc]
        private void UpdateLifebarClientRpc()
        {
            PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
            _lifebar.value = playerData.playerLife;
        }


        private void GameManager_OnPlayerPaused(object sender, System.EventArgs e)
        {
            InputSystem.Instance.Move.Disable();
        }
        private void GameManager_OnPlayerUnpaused(object sender, System.EventArgs e)
        {
            InputSystem.Instance.Move.Enable();
        }

    }
}