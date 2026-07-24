using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class MacroZoneHydrologyCache
    {
        private sealed class Entry
        {
            public int RefCount;
            public JobHandle GenerationHandle;

            public NativeArray<float> Heights;
            public NativeArray<sbyte> FlowDirection;
            public NativeArray<float> Accumulation;
            public NativeArray<int> Order;

            public int Resolution;
            public float2 CellSize;
            public float2 OriginWorld;
        }

        private readonly Dictionary<MacroZoneCoordinate, Entry> _entries = new();
        
        public MacroZoneHandle Acquire(
            MacroZoneCoordinate zone,
            MacroZoneGrid macroGrid,
            HydrologySettings settings,
            TerrainNoiseSettingsProvider noiseProvider)
        {
            if (_entries.TryGetValue(zone, out Entry existing))
            {
                existing.RefCount++;
                return ToHandle(existing);
            }

            Entry entry = CreateAndSchedule(zone, macroGrid, settings, noiseProvider);
            entry.RefCount = 1;
            _entries[zone] = entry;
            return ToHandle(entry);
        }

        public void Release(MacroZoneCoordinate zone)
        {
            if (!_entries.TryGetValue(zone, out Entry entry))
            {
                return;
            }

            entry.RefCount--;
            if (entry.RefCount > 0)
                return;
            
            entry.GenerationHandle.Complete();

            if (entry.Heights.IsCreated) entry.Heights.Dispose();
            if (entry.FlowDirection.IsCreated) entry.FlowDirection.Dispose();
            if (entry.Accumulation.IsCreated) entry.Accumulation.Dispose();
            if (entry.Order.IsCreated) entry.Order.Dispose();

            _entries.Remove(zone);
        }

        private static MacroZoneHandle ToHandle(Entry entry)
        {
            return new MacroZoneHandle(
                entry.GenerationHandle,
                entry.Accumulation,
                entry.FlowDirection,
                entry.Resolution,
                entry.CellSize,
                entry.OriginWorld);
        }

        private static Entry CreateAndSchedule(
            MacroZoneCoordinate zone,
            MacroZoneGrid macroGrid,
            HydrologySettings settings,
            TerrainNoiseSettingsProvider noiseProvider)
        {
            int interiorResolution = macroGrid.ZoneSizeInChunks * settings.CellsPerChunk;
            int resolution = interiorResolution + settings.HaloCells * 2;
            int cellCount = resolution * resolution;

            float2 cellSize = new float2(
                macroGrid.ZoneWorldSizeX / interiorResolution,
                macroGrid.ZoneWorldSizeZ / interiorResolution);

            Vector2 zoneOrigin = macroGrid.ToZoneWorldOrigin(zone);
            float2 originWorld = new float2(zoneOrigin.x, zoneOrigin.y)
                                  - cellSize * settings.HaloCells;

            var entry = new Entry
            {
                Resolution = resolution,
                CellSize = cellSize,
                OriginWorld = originWorld,
                Heights = new NativeArray<float>(cellCount, Allocator.Persistent),
                FlowDirection = new NativeArray<sbyte>(cellCount, Allocator.Persistent),
                Accumulation = new NativeArray<float>(cellCount, Allocator.Persistent),
                Order = new NativeArray<int>(cellCount, Allocator.Persistent),
            };
            
            var heightsJob = new ComputeMacroHeightsJob
            {
                Settings = noiseProvider.Create(),
                OctaveOffsets = noiseProvider.GetOctaveOffsets(noiseProvider.Octaves),
                Resolution = resolution,
                CellSize = cellSize,
                OriginWorld = originWorld,
                Heights = entry.Heights,
            };
            JobHandle h1 = heightsJob.Schedule(cellCount, 64);

            var flowJob = new ComputeFlowDirectionJob
            {
                Heights = entry.Heights,
                Resolution = resolution,
                MinSlopeEpsilon = settings.MinSlopeEpsilon,
                FlowDirection = entry.FlowDirection,
            };
            JobHandle h2 = flowJob.Schedule(cellCount, 64, h1);

            var accumulationJob = new ComputeAccumulationJob
            {
                Heights = entry.Heights,
                FlowDirection = entry.FlowDirection,
                Resolution = resolution,
                Order = entry.Order,
                Accumulation = entry.Accumulation,
            };
            JobHandle h3 = accumulationJob.Schedule(h2);

            entry.GenerationHandle = h3;
            return entry;
        }
    }
    
    public readonly struct MacroZoneHandle
    {
        public readonly JobHandle GenerationHandle;
        public readonly NativeArray<float> Accumulation;
        public readonly NativeArray<sbyte> FlowDirection;
        public readonly int Resolution;
        public readonly float2 CellSize;
        public readonly float2 OriginWorld;

        public MacroZoneHandle(
            JobHandle generationHandle,
            NativeArray<float> accumulation,
            NativeArray<sbyte> flowDirection,
            int resolution,
            float2 cellSize,
            float2 originWorld)
        {
            GenerationHandle = generationHandle;
            Accumulation = accumulation;
            FlowDirection = flowDirection;
            Resolution = resolution;
            CellSize = cellSize;
            OriginWorld = originWorld;
        }
    }
}