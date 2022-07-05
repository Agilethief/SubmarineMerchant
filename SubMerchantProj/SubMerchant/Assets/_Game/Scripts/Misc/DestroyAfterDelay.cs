using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photo
{
    public class DestroyAfterDelay : MonoBehaviour
    {

        public float delay = 1;
        // Start is called before the first frame update
        void Start()
        {
            Destroy(this.gameObject, 1);

        }
        
    }
}