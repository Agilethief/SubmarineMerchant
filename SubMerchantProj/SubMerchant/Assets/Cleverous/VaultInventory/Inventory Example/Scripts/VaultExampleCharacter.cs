//// (c) Copyright Cleverous 2020. All rights reserved.

using Cleverous.VaultSystem;
using Cleverous.NetworkImposter;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using Random = System.Random;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
using Command = FishNet.Object.ServerRpcAttribute;
using NetworkIdentity = FishNet.Object.NetworkObject;
using FishNet.Object.Synchronizing;
#endif

namespace Cleverous.VaultInventory.Example
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class VaultExampleCharacter : NetworkBehaviour, IUseInventory
    {
        [AssetDropdown(typeof(LootTable))]
        public LootTable StartingItems;
        public CharacterController Controller;
        public Animator Animator;
        public MeshRenderer PlayerNode;
        [SyncVar] private Color m_playerColor;
        [SyncVar] private bool m_isMoving;

        // we satisfy the interface here and provide a serialized backing field below.
        public Inventory Inventory
        {
            get => m_inventory;
            set => m_inventory = value;
        }
        [SerializeField] private Inventory m_inventory;

        public Transform MyTransform => transform;
        private Vector3 m_inputLast;

        public void Awake()
        {
            m_inputLast = Vector3.forward;
        }

        public override void OnStartServer()
        {
#if FISHNET
            base.OnStartServer();
#endif
            // The server should initialize the Colors and set the SyncVar value for clients.
            Random rng = new Random();
            m_playerColor = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble(), 1);
        }
        public override void OnStartClient()
        {
#if FISHNET
            base.OnStartClient();
#endif
            Inventory.Initialize(this, true);

            // If this is happening on the server, lets give our character some starting items using a LootTable!
            if (this.IsServer())
            {
                for (int i = 0; i < StartingItems.Items.Length; i++)
                {
                    if (StartingItems.Items[i] == null) continue;
                    Inventory.DoAdd(new RootItemStack(StartingItems.Items[i], StartingItems.Amounts[i]));
                }
            }

            if (this.IsLocalPlayer())
            {
                // Tell Vault and the UI that we are here and ready.
                VaultInventory.OnPlayerSpawn.Invoke(this);
            }

            PlayerNode.material.color = m_playerColor;

            if (this.IsLocalPlayer())
                Inventory.CmdRefreshAllFromServer();
        }

        public void Update()
        {
            // We don't need to do anything in Update() for the inventory. It is event based.
            MovementAndAnimation();
        }

        /// <summary>
        /// A basic movement function, not for production. Just an example.
        /// </summary>
        public void MovementAndAnimation()
        {
            if (!this.IsLocalPlayer())
            {
                Animator.SetBool("IsMoving", m_isMoving);
                return;
            }

            Vector3 input = new Vector3();

#if ENABLE_INPUT_SYSTEM
            bool a = InputSystem.GetDevice<Keyboard>()[Key.A].isPressed;
            bool d = InputSystem.GetDevice<Keyboard>()[Key.D].isPressed;
            bool w = InputSystem.GetDevice<Keyboard>()[Key.W].isPressed;
            bool s = InputSystem.GetDevice<Keyboard>()[Key.S].isPressed;
#else
            bool a = Input.GetKey(KeyCode.A);
            bool d = Input.GetKey(KeyCode.D);
            bool w = Input.GetKey(KeyCode.W);
            bool s = Input.GetKey(KeyCode.S);
#endif
            if (a) input.x -= 1;
            if (d) input.x += 1;
            if (w) input.z += 1;
            if (s) input.z -= 1;

            input.Normalize();
            bool movement = input.magnitude > 0;

            Animator.SetBool("IsMoving", movement);
            transform.rotation = Quaternion.LookRotation(new Vector3(m_inputLast.x, 0, m_inputLast.z), Vector3.up);
            Controller.Move(input * 5 * Time.deltaTime);

            if (input != Vector3.zero) m_inputLast = input;
            if (movement != m_isMoving) CmdIsMoving(movement);
        }

        [Command]
        public void CmdIsMoving(bool state)
        {
            m_isMoving = state;
        }
    }
}