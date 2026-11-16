using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Sound")]
public class SoundSO : ScriptableObject
{
    public AudioClip[] clips;                              // varios clipes = variacao aleatoria
    [Range(0f, 1f)] public float volume = 1f;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f); // evita som repetitivo
    public AudioMixerGroup mixerGroup;                     // SFX, UI etc.

    public AudioClip GetClip() => clips[Random.Range(0, clips.Length)];
}