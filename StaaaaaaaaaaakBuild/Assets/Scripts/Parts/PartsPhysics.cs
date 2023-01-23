using UniRx;
using UnityEngine;

namespace StackBuild
{
    [RequireComponent(typeof(PartsCore))]
    [RequireComponent(typeof(Rigidbody))]
    public class PartsPhysics : MonoBehaviour
    {
        [field: SerializeField] public PartsCore PartsCore { get; private set; }

        private Rigidbody rb;

        private void Awake()
        {
            TryGetComponent(out rb);
        }

        private void Start()
        {
            PartsCore.isActive
                .Subscribe(SetActive).AddTo(this);
        }

        private void SetActive(bool isActive)
        {
            if (isActive)
            {
                rb.WakeUp();
                rb.useGravity = true;
            }
            else
            {
                rb.useGravity = false;
                rb.Sleep();
            }
        }

        public void Shoot(Vector3 pos, Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            transform.position = pos;
            rb.velocity = Vector3.zero;
            rb.position = pos;
            PartsCore.isActive.Value = true;
            rb.mass = PartsCore.Settings.PartsDataDictionary[PartsCore.partsId.Value].mass;
            rb.AddForce(force, forceMode);
            rb.AddTorque(force);
        }
    }
}