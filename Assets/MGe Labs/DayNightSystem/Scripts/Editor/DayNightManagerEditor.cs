#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MGeLabs.DayNightSystem
{
    [CustomEditor(typeof(DayNightManager))]
    public class DayNightManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DayNightManager manager = (DayNightManager)target;
            if (!Application.isPlaying) return;

            EditorGUILayout.LabelField($"Current Time: {manager.DisplayTime} (Day {manager.CurrentDay})");
            EditorGUILayout.LabelField($"Time Scale: {manager.TimeScale}x");
            EditorGUILayout.LabelField($"Day Percentage: {manager.NormalizedTime:P2}");
            GUILayout.Space(10);

            if (GUILayout.Button("Set to Noon")) manager.SetTimeByHour(12);
            if (GUILayout.Button("Set to Midnight")) manager.SetTimeByHour(0);
            if (GUILayout.Button("Reset Time")) manager.ResetTime();
            if (GUILayout.Button("Reset Time Limit")) manager.ResetTimeLimit();
        }
    }
}
#endif