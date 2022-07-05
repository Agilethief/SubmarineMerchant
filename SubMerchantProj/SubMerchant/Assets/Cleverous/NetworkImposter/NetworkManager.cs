// (c) Copyright Cleverous 2022. All rights reserved.

#if !MIRROR && !FISHNET
using UnityEngine;

namespace Cleverous.NetworkImposter
{
    public class NetworkManager : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public bool SpawnHeroAtStart;
        public GameObject SpawnPoint;

        public void Start()
        {
            if (SpawnHeroAtStart) SpawnHero();
        }

        public void SpawnHero()
        {
            Instantiate(PlayerPrefab, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        }
    }
}
#endif