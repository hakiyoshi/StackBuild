using TMPro;
using UnityEngine;
using StackBuild.Game;

namespace StackBuild.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameModeText : MonoBehaviour
    {

        [SerializeField] private TMP_Text text;
        [SerializeField] private string pattern = "{}";

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            if (GameMode.Current == null) return;
            text.text = text.text.Replace(pattern, GameMode.Current.Name);
        }

    }
}
