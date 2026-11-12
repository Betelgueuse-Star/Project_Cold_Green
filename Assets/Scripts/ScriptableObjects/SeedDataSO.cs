using UnityEngine;

[CreateAssetMenu(fileName = "New Seed", menuName = "Game/Data/Seed Data")]
public class SeedDataSO : ScriptableObject
{
    [Header("Informações")]
    public string seedName;

    [Header("Status")]
    [Min(1)]
    public float baseGrowthTime = 120f;
    public float waterMarginError = 2f;
    public SoilDataSO recommendedSoil;

    [Header("Água por Estágio")]
    [Min(0)]
    public float seedRequiredWater = 10f;
    [Min(0)]
    public float germinatingRequiredWater = 10f;
    [Min(0)]
    public float growingRequiredWater = 10f;

    [Header("Estágios Visuais")]
    public GameObject seedModel;
    public GameObject germinatingModel;
    public GameObject growingModel;

    [Header("Resultado")]
    public GameObject saplingPrefab;

    // MUDANÇA: antes o switch por estágio existia em dois lugares (PlantingArea para a
    // água e PlantingAreaVisual para o prefab). Como os dados estão aqui, as
    // consultas também ficam aqui: um lugar só para mexer ao criar um novo estágio.

    // Prefab visual correspondente ao estágio (null se não houver).
    public GameObject GetStagePrefab(PlantGrowthState state)
    {
        switch (state)
        {
            case PlantGrowthState.Seed:
                return seedModel;

            case PlantGrowthState.Germinating:
                return germinatingModel;

            case PlantGrowthState.Growing:
                return growingModel;

            case PlantGrowthState.Ready:
                return saplingPrefab;

            default:
                return null;
        }
    }

    // Quantidade de água necessária no estágio (0 nos estágios sem exigência).
    public float GetRequiredWater(PlantGrowthState state)
    {
        switch (state)
        {
            case PlantGrowthState.Seed:
                return seedRequiredWater;

            case PlantGrowthState.Germinating:
                return germinatingRequiredWater;

            case PlantGrowthState.Growing:
                return growingRequiredWater;

            default:
                return 0f;
        }
    }
}