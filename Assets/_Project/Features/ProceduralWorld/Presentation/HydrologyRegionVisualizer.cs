using _Project.Features.Player.Domain;
using _Project.Features.ProceduralWorld.Application.Hydrology;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using Unity.Mathematics;
using UnityEngine;
using VContainer;


namespace _Project.Features.ProceduralWorld.Presentation
{
    public sealed class HydrologyRegionVisualizer : MonoBehaviour
    {
        [SerializeField]
        private bool enabledVisualization = true;


        [SerializeField]
        private int viewRadius = 1;


        [SerializeField]
        private float drawHeight = 20f;


        private IHydrologyProvider _hydrology;

        private IPlayerReadOnly _player;



        [Inject]
        public void Construct(
            IHydrologyProvider hydrology,
            IPlayerReadOnly player)
        {
            _hydrology = hydrology;

            _player = player;
        }



        private void OnDrawGizmos()
        {
            if (!enabledVisualization ||
                _hydrology == null ||
                _player == null)
            {
                return;
            }


            Vector3 playerPosition =
                _player.Position;


            float2 worldPosition =
                new float2(
                    playerPosition.x,
                    playerPosition.z);



            if (!_hydrology.TryGetRegion(
                    worldPosition,
                    out HydrologyRegion currentRegion))
            {
                return;
            }



            DrawRegions(
                currentRegion.Coordinate);


            DrawPlayer(
                playerPosition);
        }





        private void DrawRegions(
            HydrologyRegionCoordinate center)
        {
            for (int x = -viewRadius; x <= viewRadius; x++)
            {
                for (int y = -viewRadius; y <= viewRadius; y++)
                {
                    HydrologyRegionCoordinate coordinate =
                        new HydrologyRegionCoordinate(
                            center.X + x,
                            center.Y + y);



                    float size = 1024f;

                    float2 position =
                        new float2(
                            coordinate.X * size,
                            coordinate.Y * size);



                    if (_hydrology.TryGetRegion(
                            position,
                            out HydrologyRegion region))
                    {
                        DrawRegion(
                            region);
                    }
                    else
                    {
                        DrawEmptyRegion(
                            coordinate);
                    }
                }
            }
        }





        private void DrawRegion(
            HydrologyRegion region)
        {
            float size =
                1024f;


            Vector3 center =
                new Vector3(
                    region.Coordinate.X * size + size * 0.5f,
                    drawHeight,
                    region.Coordinate.Y * size + size * 0.5f);



            Gizmos.DrawWireCube(
                center,
                new Vector3(
                    size,
                    0,
                    size));



#if UNITY_EDITOR

            UnityEditor.Handles.Label(
                center,
                $"Region {region.Coordinate.X}:{region.Coordinate.Y}\n" +
                $"State: {region.State}\n" +
                $"Sources: {region.Sources.Length}");

#endif
        }





        private void DrawEmptyRegion(
            HydrologyRegionCoordinate coordinate)
        {
            float size =
                1024f;


            Vector3 center =
                new Vector3(
                    coordinate.X * size + size * 0.5f,
                    drawHeight,
                    coordinate.Y * size + size * 0.5f);



            Gizmos.DrawWireCube(
                center,
                new Vector3(
                    size,
                    0,
                    size));


#if UNITY_EDITOR

            UnityEditor.Handles.Label(
                center,
                $"Region {coordinate.X}:{coordinate.Y}\n" +
                $"Not generated");

#endif
        }





        private void DrawPlayer(
            Vector3 position)
        {
            Gizmos.DrawSphere(
                new Vector3(
                    position.x,
                    drawHeight,
                    position.z),
                10f);
        }
    }
}