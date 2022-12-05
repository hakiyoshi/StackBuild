using UniRx.Toolkit;
using UnityEngine;

namespace StackBuild
{
    public class PartsPool : ObjectPool<PartsCore>
    {
        private readonly PartsCore prefab;
        private readonly Transform parent;

        public PartsPool(PartsCore prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        protected override PartsCore CreateInstance()
        {
            return GameObject.Instantiate(prefab, parent, true).Spawn();
        }

        protected override void OnBeforeRent(PartsCore instance)
        {
            instance.Hide();
        }

        protected override void OnBeforeReturn(PartsCore instance)
        {
            instance.Hide();
        }

        protected override void OnClear(PartsCore instance)
        {
            base.OnClear(instance);
            instance.Despawn();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
