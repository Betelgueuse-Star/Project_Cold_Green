using UnityEngine;

[CreateAssetMenu(menuName = "Audio/UI Sounds")]
public class UISoundsSO : ScriptableObject
{
    public SoundSO click;
    public SoundSO actionBlocked;
    public SoundSO hover;
    public SoundSO confirm;
}