using UnityEngine;

namespace _Project.Features.Core.Infrastructure
{
    [CreateAssetMenu(fileName = "FrameBudgetConfig", menuName = "Project/Frame Budget Config")]
    public sealed class FrameBudgetConfig : ScriptableObject
    {
        [Tooltip("Target FPS used as the reference for the frame budget.")]
        [Min(1f)]
        [SerializeField] private float targetFrameRate = 60f;

        [Tooltip("Fraction of the target frame time available for budgeted work.")]
        [Range(0.01f, 0.5f)]
        [SerializeField] private float budgetRatio = 0.2f;

        [Tooltip("Minimum time budget allowed per frame, in milliseconds.")]
        [Min(0f)]
        [SerializeField] private float minBudgetMilliseconds = 0.25f;

        [Tooltip("Maximum time budget allowed per frame, in milliseconds.")]
        [Min(0f)]
        [SerializeField] private float maxBudgetMilliseconds = 2f;

        [Tooltip("How quickly the budget adapts to changes in frame time.")]
        [Range(0.001f, 1f)]
        [SerializeField] private float smoothing = 0.1f;


        public float TargetFrameRate =>
            targetFrameRate;

        public float BudgetRatio =>
            budgetRatio;

        public float MinBudgetMilliseconds =>
            minBudgetMilliseconds;

        public float MaxBudgetMilliseconds =>
            maxBudgetMilliseconds;

        public float Smoothing =>
            smoothing;
    }
}