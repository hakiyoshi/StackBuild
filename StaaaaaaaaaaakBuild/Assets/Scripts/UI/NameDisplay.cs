using TMPro;
using UnityEngine;

namespace StackBuild.UI
{
    public class NameDisplay : MonoBehaviour
    {

        [SerializeField] private TMP_Text text;
        [SerializeField] private PlayerProperty player;

        private void Awake()
        {
            text.text = player.characterProperty.CompanyName;
        }

    }
}
