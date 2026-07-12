using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class TerrainUpdater : ITerrainUpdater
    {
        private readonly HashSet<TerrainData> _dirty =
            new();


        public void Register(
            TerrainData data)
        {
            _dirty.Add(data);
        }


        public void Apply()
        {
            foreach(var data in _dirty)
            {
                data.SyncHeightmap();
            }


            _dirty.Clear();
        }
    }
}