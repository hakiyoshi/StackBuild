using UnityEngine;
using StackBuild.Audio;

public class AudioTest : MonoBehaviour
{

    [SerializeField] private AudioCue cue;
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioEffectChannel[] effects;

    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            channel.Request(cue);
        }
        if (GUILayout.Button("Stop"))
        {
            channel.RequestStop();
        }

        for (int i = 0; i < effects.Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Effect #{(i + 1).ToString()}");
            if (GUILayout.Button("On"))
            {
                effects[i].RequestEffect(true);
            }
            if (GUILayout.Button("Off"))
            {
                effects[i].RequestEffect(false);
            }
            GUILayout.EndHorizontal();
        }
    }

}
