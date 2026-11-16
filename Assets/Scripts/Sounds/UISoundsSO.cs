using UnityEngine;

[CreateAssetMenu(menuName = "Audio/UI Sounds")]
public class UISoundsSO : ScriptableObject
{
    public SoundSO click;
    public SoundSO blocked;
    public SoundSO hover;
    public SoundSO confirm;
}