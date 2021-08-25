using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlotMachine
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private Rigidbody myRigidbody;

        private void Start()
        {
            transform.localRotation = Quaternion.Euler(new Vector3(0,0,1)*Random.value*360);
        }

        public void ExplodeCoin(float minForce,float maxForce, Vector3 explosionPos,float explosionRadius, float upwardModifier)
        {
            myRigidbody.AddExplosionForce(Random.Range(minForce,maxForce),explosionPos,explosionRadius,upwardModifier,ForceMode.VelocityChange);
            Destroy(gameObject,5f);
        }
    }
}