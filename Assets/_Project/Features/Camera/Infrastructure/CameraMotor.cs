using System;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Features.Camera.Infrastructure
{
    public interface ICameraMotor
    {
        float GetCurrentHeight();
        Vector3 Position { get; }
        Vector3 FollowOffset { get; }

        void SetRotation(Quaternion rotation);
        void SetFollowOffset(Vector3 offset);
        void ApplyOriginShift(Vector3 delta);
    }
    
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(CinemachineFollow))]
    [RequireComponent(typeof(CinemachineCamera))]
    public sealed class CameraMotor : MonoBehaviour, ICameraMotor
    {
        public Vector3 Position => transform.localPosition;

        private CinemachineFollow _follow;
        private CinemachineCamera _vcam;

        public Vector3 FollowOffset => _follow.FollowOffset;

        public float GetCurrentHeight() => transform.localPosition.y;

        private void Awake()
        {
            _follow = GetComponent<CinemachineFollow>();
            _vcam = GetComponent<CinemachineCamera>();
        }

        public void SetRotation(Quaternion rotation)
        {
            transform.localRotation = rotation;
        }

        public void SetFollowOffset(Vector3 offset)
        {
            _follow.FollowOffset = offset;
        }

        public void ApplyOriginShift(Vector3 delta)
        {
            Transform trackingTarget = _vcam.Target.TrackingTarget;

            if (trackingTarget)
            {
                _vcam.OnTargetObjectWarped(trackingTarget, delta);
            }
        }
    }
}