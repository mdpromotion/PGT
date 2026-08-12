using UnityEngine;

namespace _Project.Features.Camera.Infrastructure
{
    public interface ICameraMotor
    {
        float GetCurrentHeight();
        Vector3 Position { get; }

        void SetRotation(Quaternion rotation);
        void SetPosition(Vector3 position);
    }
    [RequireComponent(typeof(Transform))]
    public sealed class CameraMotor : MonoBehaviour, ICameraMotor
    {
        public Vector3 Position => transform.localPosition;

        public float GetCurrentHeight() => transform.localPosition.y;
        
        public void SetRotation(Quaternion rotation)
        {
            transform.localRotation = rotation;
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;
        }
        
    }
}