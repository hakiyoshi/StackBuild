using UnityEngine;

namespace StackBuild.UI
{
    [CreateAssetMenu(menuName = "ScriptableObject/UI/StackbuildButtonStyle")]
    public class StackbuildButtonStyle : ScriptableObject
    {

        [field: SerializeField] public Color NormalBackgroundColor { get; private set; }
        [field: SerializeField] public Color NormalBorderColor { get; private set; }
        [field: SerializeField] public Color NormalTextColor { get; private set; }
        [field: SerializeField] public Color HoveredBackgroundColor { get; private set; }
        [field: SerializeField] public Color PressedBackgroundColor { get; private set; }
        [field: SerializeField] public Color DisabledBackgroundColor { get; private set; }
        [field: SerializeField] public Color DisabledBorderColor { get; private set; }
        [field: SerializeField] public Color DisabledTextColor { get; private set; }

    }
}
