using UnityEngine;
using Random = UnityEngine.Random;

namespace SlotMachine
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private Rigidbody myRigidbody;
        
        public void ExplodeCoin(float minForce,float maxForce, Vector3 explosionPos,float explosionRadius, float upwardModifier)
        {
            myRigidbody.AddExplosionForce(Random.Range(minForce,maxForce),explosionPos,explosionRadius,upwardModifier,ForceMode.VelocityChange);
            Destroy(gameObject,5f);
        }
    }
}