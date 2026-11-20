using UnityEngine;

public class PlantingAreaAudio : MonoBehaviour
{
    [SerializeField] private PlantingArea plantingArea;

    [Header("Sons")]
    [SerializeField] private SoundSO plantSound;    // sfx_Incubator_Plant
    [SerializeField] private SoundSO growSound;     // sfx_Incubator_Grow (_01 e _02 são clipes do mesmo SoundSO)
    [SerializeField] private SoundSO readySound;    // sfx_Incubator_Ready
    [SerializeField] private SoundSO harvestSound;  // sfx_Incubator_Harvest

    [Header("Loop")] //opcional
    [SerializeField] private AudioSource runningLoop; // amb_Incubator_Running

    private void Awake()
    {
        if (plantingArea == null)
            plantingArea = GetComponent<PlantingArea>();

        if (plantingArea == null)
            Debug.LogError($"{name}: PlantingAreaAudio sem referência à PlantingArea.", this);
    }

    private void OnEnable()
    {
        if (plantingArea == null)
            return;

        plantingArea.StateChanged += HandleStateChanged;
        plantingArea.PlantHarvested += HandlePlantHarvested;
    }

    private void OnDisable()
    {
        if (plantingArea == null)
            return;

        plantingArea.StateChanged -= HandleStateChanged;
        plantingArea.PlantHarvested -= HandlePlantHarvested;
    }

    private void HandleStateChanged(PlantGrowthState state)
    {
        switch (state)
        {
            case PlantGrowthState.Seed:
                Play(plantSound);
                SetLoop(true);
                break;

            case PlantGrowthState.Germinating:
            case PlantGrowthState.Growing:
                Play(growSound);
                break;

            case PlantGrowthState.Ready:
                Play(readySound);
                SetLoop(false);
                break;

            case PlantGrowthState.Empty:
                // Empty também dispara quando a planta morre ou a pá remove tudo:
                // aqui só para o loop. Cada um desses casos tem som próprio, em outro lugar.
                SetLoop(false);
                break;
        }
    }

    private void HandlePlantHarvested() => Play(harvestSound);

    private void Play(SoundSO sound)
    {
        AudioManager.Instance.Play3D(sound, transform.position);
    }

    private void SetLoop(bool on)
    {
        if (runningLoop == null)
            return;

        if (on && !runningLoop.isPlaying) runningLoop.Play();
        else if (!on) runningLoop.Stop();
    }
}