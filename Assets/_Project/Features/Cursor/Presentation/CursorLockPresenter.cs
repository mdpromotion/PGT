using UnityEngine;

namespace _Project.Features.Cursor.Presentation
{
    public sealed class CursorLockPresenter : MonoBehaviour
    {
        private void Awake()
        {
            LockCursor(true);
        }

        private void LockCursor(bool state)
        {
            UnityEngine.Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            UnityEngine.Cursor.visible = !state;
        }
    }
}