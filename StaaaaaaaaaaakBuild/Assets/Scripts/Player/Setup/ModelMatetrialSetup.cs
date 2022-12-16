using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

#if UNITY_EDITOR
        //シェーダープロパティ名のショートカット
        enum ShaderColorNameShortcut
        {
            None,
            Color,
            Emission,
        }

        private readonly string[] ShaderColorName =
        {
            "",
            "_Color",
            "_EmissionColor",
        };
#endif

        [System.Serializable]
        struct SetupMaterial
        {
            public SetupType type;
#if UNITY_EDITOR
            public ShaderColorNameShortcut shaderColorNameShortcut;
#endif
            public string colorName;
            [ColorUsage(true, true)]public Color[] setColors;

            public List<GameObject> setupObjects;



            public SetupMaterial(int n = 0)
            {
                setupObjects = null;
                type = SetupType.SkinnedMeshRenderer;

                colorName = "";
                setColors = new Color[2];

#if UNITY_EDITOR
                shaderColorNameShortcut = ShaderColorNameShortcut.Color;
#endif
            }
        }

        [SerializeField] private PlayerManagerProperty playerManagerProperty;
        [SerializeField] private List<SetupMaterial> setupMaterials = new List<SetupMaterial>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = 0; i < setupMaterials.Count; i++)
            {
                var material = setupMaterials[i];
                material.colorName =ShaderColorName[(int) material.shaderColorNameShortcut];
                setupMaterials[i] = material;
            }
        }
#endif

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
                    if(set.setColors.Length <= playerIndex)
                        continue;

                    SetupTypeMaterial(set.type, obj, set.colorName,
                        set.setColors[playerIndex]);
                }
            }

            Destroy(this);
        }

        void SetupTypeMaterial(SetupType type, GameObject setupObject, string colorName, Color setColor)
        {
            switch (type)
            {
                case SetupType.ParticleSystemRenderer:
                    setupObject.GetComponent<ParticleSystemRenderer>().material.SetColor(colorName, setColor);
                    break;
                case SetupType.SkinnedMeshRenderer:
                    setupObject.GetComponent<SkinnedMeshRenderer>().material.SetColor(colorName, setColor);
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