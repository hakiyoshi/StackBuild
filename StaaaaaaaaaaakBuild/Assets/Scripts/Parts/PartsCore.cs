using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace StackBuild
{
    public class PartsCore : MonoBehaviour
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
                .Where(pos => pos.y < -30 && isActive.Value)
                .Subscribe(_ => Parent.Return(this));
        }

        public PartsData GetPartsData()
        {
            return settings.PartsDataDictionary[partsId.Value];
        }
    }
}
