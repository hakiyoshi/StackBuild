using UniRx;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace StackBuild
{
    public class PartsCore : MonoBehaviour
    {
        [SerializeField] private PartsSettings settings;
        public PartsSettings Settings => settings;

        private ReactiveProperty<bool> isActive = new();
        public IReadOnlyReactiveProperty<bool> IsActive => isActive;

        private ReactiveProperty<PartsId> partsId = new();
        public IReadOnlyReactiveProperty<PartsId> PartsID => partsId;

        [SerializeField] private PartsId initialId;

        public void Start()
        {
            isActive.AddTo(this);
            partsId.AddTo(this);

            if (initialId != PartsId.Default)
            {
                partsId.Value = initialId;
                isActive.Value = true;
            }
        }

        public PartsCore Spawn()
        {
            isActive.Value = true;
            return this;
        }

        public void Despawn()
        {
            isActive.Value = false;
        }

        public void Show()
        {
            isActive.Value = true;
        }

        public void Hide()
        {
            isActive.Value = false;
        }

        public void SetPartsID(PartsId id)
        {
            partsId.Value = id;
        }

        public PartsData GetPartsData()
        {
            return settings.PartsDataDictionary[partsId.Value];
        }
    }
}
