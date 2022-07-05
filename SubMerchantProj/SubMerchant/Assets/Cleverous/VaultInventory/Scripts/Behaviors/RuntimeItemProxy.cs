// (c) Copyright Cleverous 2020. All rights reserved.

using System.Collections;
using Cleverous.VaultSystem;
using Cleverous.NetworkImposter;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
using FishNet.Object.Synchronizing;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cleverous.VaultInventory
{
    [RequireComponent(typeof(Collider))]
    public class RuntimeItemProxy : NetworkBehaviour
    {
        public RootItemStack Data;
        public bool AutoPickup;
        public float LockedForSeconds;
        public LayerMask CanPickMeUp;
        public bool PopSpawn;
        protected float SpawnedAtTime;
        protected GameObject GraphicsObject; // Keep this object simple, and network free.
        protected bool IsBusy;

        [SyncVar] protected Vector3 OriginPosition;
        [SyncVar] protected Vector3 OffsetSpawnPosition;
        [SyncVar] protected int ItemDbKey;
        [SyncVar] protected int StackSize;

        public static AnimationCurve MoundCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

#if MIRROR && !FISHNET || !MIRROR && !FISHNET
        protected virtual void Reset()
        {
#elif FISHNET
        protected override void Reset()
        {
            base.Reset();
#endif
            Data = null;
            AutoPickup = true;
            LockedForSeconds = 1;
            CanPickMeUp = 1;
            PopSpawn = true;
            IsBusy = false;
        }
        protected virtual void OnEnable()
        {
            IsBusy = false;
            SpawnedAtTime = Time.time;
        }
        protected virtual void OnTriggerEnter(Collider col)
        {
            if (!this.IsServer()) return;

            if (SpawnedAtTime + LockedForSeconds > Time.time) return;
            bool validTouch = ((1 << col.gameObject.layer) & CanPickMeUp) != 0;
            if (!validTouch) return;
            if (!AutoPickup) return;

            IUseInventory touchySubject = col.GetComponent<IUseInventory>();
            if (touchySubject != null) TryPickup(touchySubject);
        }

        public override void OnStartClient()
        {
#if FISHNET
            base.OnStartClient();
#endif            
            if (GraphicsObject != null) Destroy(GraphicsObject); // cleanup if necessary.

            RootItem source = (RootItem)Vault.Get(ItemDbKey);

            //Debug.Log($"<color=cyan>Spawned as [{m_itemId}] : ({m_stackSize}) {source.Title}</color>", this);

            Data = new RootItemStack(source, StackSize);
            GraphicsObject = Instantiate(Data.Source.artPrefabWorld, transform, false);

            // Server did this already.
            // If not running headless, the server is a client (Host) and reaches this code too, so we'd have to skip it.
            if (PopSpawn && !this.IsServer() && OffsetSpawnPosition != Vector3.zero) StartCoroutine(SpawnPop(OffsetSpawnPosition));
        }

        /// <summary>
        /// This is called from VaultInventory when spawning the item on the Server. It will setup the syncvars.
        /// </summary>
        /// <param name="sourceItem">The item being spawned</param>
        /// <param name="stackSize">Total number of items in this stack</param>
        public virtual void SvrInitialize(RootItem sourceItem, int stackSize)
        {
            if (!NetworkPipeline.StaticIsServer()) return;

            float mag = Random.value * 5;
            Vector2 rng = Random.insideUnitCircle;
            
            StackSize = stackSize;
            ItemDbKey = sourceItem.GetDbKey();
            OriginPosition = transform.position;
            OffsetSpawnPosition = new Vector3(rng.x * mag, transform.position.y, rng.y * mag);

            // Need to do this here for headless, so it's here as well as the client method (bypassed there if server).
            if (PopSpawn && OffsetSpawnPosition != Vector3.zero) StartCoroutine(SpawnPop(OffsetSpawnPosition));
        }

        /// <summary>
        /// Try to pick up the item. Typically called from an OnTrigger method and only on the server.
        /// Clients cannot demand pickups, so running this on a client would cause them to be out of sync.
        /// </summary>
        /// <param name="requestor">The Inventory interface trying to claim the item.</param>
        public virtual void TryPickup(IUseInventory requestor)
        {
            // If the Inventory can't fit the content, it'll reflect back the amount.
            // We have to keep this object and update the stack size here in that case.
            if (IsBusy) return;
            IsBusy = true;

            int remainder = VaultInventory.TryGiveItem(requestor.Inventory, Data);
            if (remainder <= 0) DeSpawn();
            else Data.StackSize = remainder;

            IsBusy = false;
        }

        /// <summary>
        /// Modifies the Transform of the object locally to create a "pop" movement from the origin position to the origin+offset position.
        /// This is the reason we syncvar the OffsetSpawnPosition variable, so we don't need NetworkTransform components to sync the transform for
        /// all of the items floating around in the scene.
        /// </summary>
        /// <param name="offset">3d world space offset in units</param>
        protected virtual IEnumerator SpawnPop(Vector3 offset)
        {
            Vector3 goal = OriginPosition + offset;

            const float totalTime = 0.5f;
            float time = 0;
            while (time < totalTime)
            {
                time += Time.deltaTime;
                Vector3 lerp = Vector3.Lerp(OriginPosition, goal, time);
                lerp.y = OriginPosition.y + MoundCurve.Evaluate(time / totalTime);
                transform.position = lerp;
                yield return null;
            }
        }

        /// <summary>
        /// Destroy or pool the object.
        /// </summary>
        public virtual void DeSpawn()
        {
            // todo pooling
            Destroy(gameObject);
        }
    }
}