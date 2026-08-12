using UnityEngine;

namespace _Project.Features.Camera.Infrastructure
{
    [CreateAssetMenu(menuName = "Project/Player/Camera Config")]
    public class PlayerCameraConfig : ScriptableObject
    {
        public float sensitivity = 0.08f;
        public bool invertY = false;
        public float standingHeight = 1.7f;
        public float crouchingHeight = 1.15f;
        public float heightSmoothSpeed = 12f;
        public float lookSmoothTime = 0.03f;
    }
}