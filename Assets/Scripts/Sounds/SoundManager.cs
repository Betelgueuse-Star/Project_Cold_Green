using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private int poolSize = 16;

    private AudioSource[] pool;
    private int next;

    private void Awake()
    {
        if (Instance != null) Debug.LogError("Ja existe um AudioManager");
        Instance = this;

        // cria os AudioSources uma vez so, em vez de instanciar a cada som
        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject("PooledAudio");
            go.transform.SetParent(transform);
            pool[i] = go.AddComponent<AudioSource>();
            pool[i].playOnAwake = false;
        }
    }

    private void Start()
    {
        // aplica no Start: o mixer pode ignorar SetFloat feito no Awake
        LoadVolume("MasterVolume");
        LoadVolume("SFXVolume");
        LoadVolume("MusicVolume");
        LoadVolume("UIVolume");
    }

    public void Play2D(SoundSO sound) => Play(sound, Vector3.zero, 0f);       // ouve de qualquer lugar
    public void Play3D(SoundSO sound, Vector3 pos) => Play(sound, pos, 1f);   // depende da distancia

    private void Play(SoundSO sound, Vector3 pos, float spatialBlend)
    {
        if (sound == null || sound.clips.Length == 0) return;

        var src = pool[next];
        next = (next + 1) % pool.Length; // rodizio: se encher, corta o mais antigo

        src.transform.position = pos;
        src.clip = sound.GetClip();
        src.volume = sound.volume;       // o volume geral fica no mixer, nao aqui
        src.pitch = Random.Range(sound.pitchRange.x, sound.pitchRange.y);
        src.spatialBlend = spatialBlend; // 0 = 2D, 1 = 3D
        src.outputAudioMixerGroup = sound.mixerGroup;
        src.Play();
    }

    // slider 0..1 -> decibeis. O parametro precisa estar exposto no mixer
    public void SetVolume(string param, float value01)
    {
        mixer.SetFloat(param, Mathf.Log10(Mathf.Max(value01, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat(param, value01);
        PlayerPrefs.Save();
    }

    private void LoadVolume(string param)
    {
        SetVolume(param, PlayerPrefs.GetFloat(param, 1f)); // 1f = padrao se nada foi salvo
    }
}