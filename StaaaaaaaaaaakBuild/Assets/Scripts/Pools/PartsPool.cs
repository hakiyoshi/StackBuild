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
            // 生成時の処理
            return GameObject.Instantiate(prefab, parent, true).Spawn();
        }

        protected override void OnBeforeRent(PartsCore instance)
        {
            // 貸出前の処理
            base.OnBeforeRent(instance);
            instance.Hide();
        }

        protected override void OnBeforeReturn(PartsCore instance)
        {
            // 返却前の処理
            base.OnBeforeReturn(instance);
            instance.Hide();
        }

        protected override void OnClear(PartsCore instance)
        {
            // 削除時の処理
            base.OnClear(instance);
            instance.Despawn();
        }

        protected override void Dispose(bool disposing)
        {
            // 後処理
            base.Dispose(disposing);
        }
    }
}
