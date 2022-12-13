using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace StackBuild
{
    public class PartsCore : NetworkBehaviour
    {
        [SerializeField] private PartsSettings settings;
        public PartsSettings Settings => settings;

        public PartsManager Parent => transform.GetComponentInParent<PartsManager>();

        public ReactiveProperty<bool> isActive = new(false);

        public ReactiveProperty<PartsId> partsId = new();

        private void Awake()
        {
            isActive.AddTo(this);
            partsId.AddTo(this);
        }

        private void Start()
        {
            this.OnTriggerEnterAsObservable()
                .Where(collider => collider.CompareTag("Goal"))
                .Subscribe(_ => Parent.Return(this));

            this.UpdateAsObservable()
                .Select(_ => transform.position)
                .DistinctUntilChanged()
                .Skip(1)
                .Where(pos => pos.y < -30)
                .Subscribe(_ => Parent.Return(this));
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"Spawn {gameObject.name}");
        }

        public PartsData GetPartsData()
        {
            return settings.PartsDataDictionary[partsId.Value];
        }
    }
}
