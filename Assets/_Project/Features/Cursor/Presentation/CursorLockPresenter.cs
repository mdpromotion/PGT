using UnityEngine;

namespace _Project.Features.Cursor.Presentation
{
    public interface ICursorService
    {
        void LockCursor(bool state);
    }
    
    public sealed class CursorLockService : ICursorService
    {
        public void LockCursor(bool state)
        {
            UnityEngine.Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            UnityEngine.Cursor.visible = !state;
        }
    }
}