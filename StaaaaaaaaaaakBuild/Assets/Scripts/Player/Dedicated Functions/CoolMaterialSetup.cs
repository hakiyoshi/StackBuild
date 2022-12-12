using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace StackBuild.Dedicated_Functions
{
    public class CoolMaterialSetup : MonoBehaviour
    {
        [SerializeField] private Material[] bania_mtl = new Material[2];
        [SerializeField] private Material[] cool_mtl = new Material[2];
        [SerializeField] private PlayerManagerProperty playerManager;

        [SerializeField] private List<ParticleSystemRenderer> bania_mtls;
        [SerializeField] private List<SkinnedMeshRenderer> cool_mtls;

        private void Start()
        {
            var player = RootPlayerParent(this.transform, this.transform);

            //自分のプレイヤーインデックスを取得
            var playerIndex = playerManager.playerManager.GetPlayerIndex(player.gameObject);

            if (bania_mtls != null)
            {
                for (var i = 0; i < bania_mtls.Count; i++)
                {
                    bania_mtls[i].material = bania_mtl[playerIndex];
                }
            }

            if (cool_mtls != null)
            {
                for (var i = 0; i < cool_mtls.Count; i++)
                {
                    cool_mtls[i].material = cool_mtl[playerIndex];
                }
            }

            Destroy(this);
        }

        Transform RootPlayerParent(Transform parent, Transform child)
        {
            if (parent.parent == null)
                return child;

            return RootPlayerParent(parent.parent, parent);
        }
    }
}