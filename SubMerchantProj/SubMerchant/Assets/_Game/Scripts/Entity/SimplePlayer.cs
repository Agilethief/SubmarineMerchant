using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Cleverous.VaultSystem;
using Cleverous.VaultInventory;
using Steamworks;

namespace CargoGame
{
    public class SimplePlayer : EntityPlayer, IMovable, IUseInventory
    {
        public static SimplePlayer localPlayerObject;

        [Header("Player Controller")]

        [SyncVar]
        public int playerID;

        private GameManager _GM;
        public GameManager gameManager
        {
            get
            {
                if (_GM == null) _GM = FindObjectOfType<GameManager>();

                return _GM;
            }
        }

        private SM_Movement _movementStateMachine;
        public SM_Movement movementStateMachine
        {
            get
            {
                if (_movementStateMachine == null) _movementStateMachine = transform.GetComponent<SM_Movement>();

                return _movementStateMachine;
            }
        }
        private SM_Hands _handsStateMachine;
        public SM_Hands handsStateMachine
        {
            get
            {
                if (_handsStateMachine == null) _handsStateMachine = transform.GetComponent<SM_Hands>();

                return _handsStateMachine;
            }
        }



        // This must be attached to the player prefab
        public CamRig camRig;

       [SerializeField]
       private PlayerUI playerUIPrefab;
       private PlayerUI _playerUI;
       public PlayerUI playerUI
       {
           get
           {
               if (_playerUI == null)
               {
                   _playerUI = _playerUI = FindObjectOfType<PlayerUI>();
                   _playerUI.SetupUI(this);
               }
               return _playerUI;
           }
       }

        [AssetDropdown(typeof(LootTable))]
        public LootTable StartingItems;
        public Inventory Inventory
        {
            get => m_inventory;
            set => m_inventory = value;
        }
        [SerializeField] private Inventory m_inventory;
        public Transform MyTransform => transform;


        [SerializeField]
        private TMP_Text nameLabel;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SerializeField]
        private GameObject artContainerFirstPerson, artContainerThirdPerson;

        [SerializeField]
        public LayerMask groundLayerMask;

        // Look at
        public float lookAtRange = 1.65f;
        public LayerMask lookAtRayLayerMask;
        RaycastHit lookAtRayHit;
        public GameObject currentLookAtObject;
        public InteractableBase currentlookAtInteractable;

        public Transform heldObjectSocket;
        public GameObject currentHeldItem;  // This is the item that is held by the player in the first person view. aka the item in their hands. This is not the item they are carrying
        public Int_Carryable currentCarryObject;
        public Transform carrySocket;

        public override void OnStartLocalPlayer()
        {
            camRig.transform.position = pos;
            camRig.simplePlayer = this;
            

            artContainerFirstPerson.transform.parent = camRig.GetCamTransform();
            artContainerFirstPerson.transform.localPosition = Vector3.zero;
            ToggleFirstPersonView(true);

            playerUI.crosshairPanel.ClearCrosshair();

            // Temp cursor lock
            playerUI.RevealMouse(false);

            gameManager.localPlayer = this;
            localPlayerObject = this;

             if(!SteamManager.Initialized) return;
             CmdSetupPlayer(SteamFriends.GetPersonaName());  // Send command to server to set this value
        }

        public override void OnStartServer()
        {
            ToggleFirstPersonView(false);

            gameManager.playerList.Add(this);
            playerID = gameManager.playerList.Count;


        }
        private void OnDestroy()
        {
            if (isServer)
            {
                if (gameManager == null) return;

                gameManager.playerList.Remove(this);
            }
        }

        public override void OnStartClient()
        {
            Inventory.Initialize(this, true);

            // If this is happening on the server, lets give our character some starting items using a LootTable!
            if (isServer)
            {
                for (int i = 0; i < StartingItems.Items.Length; i++)
                {
                    if (StartingItems.Items[i] == null) continue;
                    Inventory.DoAdd(new RootItemStack(StartingItems.Items[i], StartingItems.Amounts[i]));
                }
            }

            if (isLocalPlayer)
            {
                // Tell Vault and the UI that we are here and ready.
                VaultInventory.OnPlayerSpawn.Invoke(this);
                Inventory.CmdRefreshAllFromServer();


            }


        }

        // This is called by the game manaager to have the player do their initial setup work.
        public void InitialSetup()
        {
            // Load Player Data
            LoadPlayerData();
        }

        [Command]
        public void CmdSetupPlayer(string incomingPlayerName)
        {
            playerName = incomingPlayerName;
        }


        public override void OnStopClient()
        {
            base.OnStopClient();

            // Save player data
            //SavePlayerData();
        }

        [Command]
        public void SavePlayerData()
        {
            // take the player data and put it back over to the server
        }

        void LoadPlayerData()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer) return;

            LookAtScan();

            carrySocket.position = camRig.carrySocket.position;
            carrySocket.rotation = camRig.carrySocket.rotation;

            // Temp
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                playerUI.RevealMouse(!Cursor.visible);
            }

            // TEMP UI hotbar
            //Debug.Log("Mouse wheel delta: " + Input.GetAxis("Mouse ScrollWheel"));
            if (Input.GetAxis("Mouse ScrollWheel") >= 0.01f)
            {
                playerUI.hotbarUi.IncrementHotbar();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") <= -0.01f)
            {
                playerUI.hotbarUi.DecrementHotbar();
            }

            if(pos.y <= -1010)
            {
                gameManager.RespawnLocalPlayer();
            }
        }


        void ToggleFirstPersonView(bool firstPersonEnabled)
        {
            if (firstPersonEnabled)
            {
                artContainerThirdPerson.SetActive(false);
                artContainerFirstPerson.SetActive(true);
            }
            else
            {
                artContainerThirdPerson.SetActive(true);
                artContainerFirstPerson.SetActive(false);
            }
        }

        public void Move(Vector3 velocity)
        {

            // Translate the velocity into relative space based on the way the player is facing.
            velocity = transform.rotation * velocity;

            cc.Move(velocity * Time.deltaTime);
        }



        void OnNameChanged(string _Old, string _New)
        {
            nameLabel.text = playerName;
        }

        public bool IsGrounded()
        {
            if (cc.isGrounded)
                return true;

            // If it doesn't lets do a bigger beter check.
            return Physics.CheckSphere(pos, 0.1f, groundLayerMask);

        }

        void LookAtScan()
        {
            if(!handsStateMachine.handsFree) return; // Cannot look at while hands busy

            Debug.DrawLine(camRig.ownCam.transform.position, camRig.ownCam.transform.position + camRig.ownCam.transform.forward * lookAtRange, Color.red);

            if (Physics.Raycast(camRig.ownCam.transform.position, camRig.ownCam.transform.forward, out lookAtRayHit, lookAtRange, lookAtRayLayerMask, QueryTriggerInteraction.Ignore))
            {
                //Debug.Log("Look ray hit: " + lookAtRayHit.transform.name);
                if (lookAtRayHit.transform.gameObject != null)
                {
                    ChangeLookAtObject(lookAtRayHit.transform.gameObject);
                }
            }
            else
            {
                ChangeLookAtObject(null);   // Looking at nothing
            }
        }

        void ChangeLookAtObject(GameObject newLookAtObject)
        {

            // Start Prework clean up duties   ========
            if (currentLookAtObject != null)
            {
                if (newLookAtObject == currentLookAtObject)
                    return;

                // Clear look ats
                if (!currentLookAtObject.GetComponent<InteractableBase>())
                    currentlookAtInteractable = null;


            }

            // Finished all the prework  ========

            currentLookAtObject = newLookAtObject;


            if (currentLookAtObject == null)
            {
                playerUI.crosshairPanel.ClearCrosshair();
                return;
            }


            if (currentLookAtObject.GetComponent<InteractableBase>())
            {
                currentlookAtInteractable = currentLookAtObject.GetComponent<InteractableBase>();

                if (currentlookAtInteractable.crosshairSprite != null)
                    playerUI.crosshairPanel.SetCrossHairSprite(currentlookAtInteractable.crosshairSprite, Color.white);
            }

            // Begin postwork   ========

        }

        public void EquipItem(GameObject itemHeldPrefab)
        {
            RemoveEquippedItem();

            currentHeldItem = Instantiate(itemHeldPrefab, heldObjectSocket);
            currentHeldItem.transform.localPosition = Vector3.zero;
            currentHeldItem.transform.localRotation = Quaternion.identity;
            currentHeldItem.transform.localScale = Vector3.one;
        }

        void RemoveEquippedItem()
        {
            if (currentHeldItem == null) return;

            Destroy(currentHeldItem);
        }

        // Called remotely via RPC to tell the player they are holding something.
        public void PickedUpObject(Int_Carryable carryObject)
        {
            if(hasAuthority)
            { 
                handsStateMachine.ChangeState(handsStateMachine.carryingState);
                playerUI.crosshairPanel.HideCrosshair();
            }

            currentCarryObject = carryObject;
        }
        public void DroppedObject()
        {
            currentCarryObject = null;
        }
    }

}