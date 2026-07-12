using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain.World
{
    [System.Serializable]
    public class WorldNoiseSettings
    {
        [Header("Temperature")]

        public float TemperatureScale = 5000f;

        public int TemperatureOctaves = 3;

        [Range(0f,1f)]
        public float TemperaturePersistence = 0.5f;


        [Header("Moisture")]

        public float MoistureScale = 5000f;

        public int MoistureOctaves = 3;

        [Range(0f,1f)]
        public float MoisturePersistence = 0.5f;
    }
}