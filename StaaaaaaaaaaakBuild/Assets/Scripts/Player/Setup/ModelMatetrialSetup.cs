using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public class ModelMatetrialSetup : MonoBehaviour
    {
        enum SetupType
        {
            ParticleSystemRenderer,
            SkinnedMeshRenderer,
        }

        [System.Serializable]
        struct SetupMaterial
        {
            public SetupType type;
            public Material[] setMaterials;
            public List<GameObject> setupObjects;

            public SetupMaterial(Material[] set)
            {
                setMaterials = new Material[2];
                setupObjects = null;
                type = SetupType.SkinnedMeshRenderer;
            }
        }

        [SerializeField] private PlayerManagerProperty playerManagerProperty;
        [SerializeField] private List<SetupMaterial> setupMaterials = new List<SetupMaterial>();

        private void Start()
        {
            //自分の本当の親を探す旅に出る
            var player = RootPlayerParent(this.transform, this.transform);

            //自分のプレイヤーインデックスを取得
            var playerIndex = playerManagerProperty.playerManager.GetPlayerIndex(player.gameObject);

            //マテリアルを書き換える
            foreach (var set in setupMaterials)
            {
                foreach (var obj in set.setupObjects)
                {
                    SetupTypeMaterial(set.type, obj, set.setMaterials[playerIndex]);
                }
            }

            Destroy(this);
        }

        void SetupTypeMaterial(SetupType type, GameObject setupObject, Material setMaterial)
        {
            switch (type)
            {
                case SetupType.ParticleSystemRenderer:
                    setupObject.GetComponent<ParticleSystemRenderer>().material = setMaterial;
                    break;
                case SetupType.SkinnedMeshRenderer:
                    setupObject.GetComponent<SkinnedMeshRenderer>().material = setMaterial;
                    break;
                default:
                    return;
            }
        }

        Transform RootPlayerParent(Transform parent, Transform child)
        {
            if (parent.parent == null)
                return child;

            return RootPlayerParent(parent.parent, parent);
        }
    }
}