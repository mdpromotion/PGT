using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Player.Presentation
{
    public interface IPlayerInputReader
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool JumpPressed { get; }
        bool SprintPressed { get; }
        bool CrouchPressed { get; }
    }

    public sealed class PlayerInputReader :
        IPlayerInputReader,
        IInitializable,
        System.IDisposable
    {
        private readonly InputSystem_Actions _inputActions;

        public PlayerInputReader(InputSystem_Actions inputActions)
        {
            _inputActions = inputActions;
        }

        public Vector2 Move => _inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 Look => _inputActions.Player.Look.ReadValue<Vector2>();
        public bool JumpPressed => _inputActions.Player.Jump.IsPressed();
        public bool SprintPressed => _inputActions.Player.Sprint.IsPressed();
        public bool CrouchPressed => _inputActions.Player.Crouch.IsPressed();

        public void Initialize()
        {
            _inputActions.Enable();
        }

        public void Dispose()
        {
            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}