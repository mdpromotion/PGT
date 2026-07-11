using UnityEngine;
using VContainer.Unity;

namespace _Project.Features.Player.Presentation
{
    public interface IPlayerInputReader
    {
        Vector2 Move { get; }
        Vector2 Look { get; }
        bool JumpPressedThisFrame { get; }
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
        public bool JumpPressedThisFrame => _inputActions.Player.Jump.WasPressedThisFrame();

        public void Initialize()
        {
            Debug.Log("[PlayerInputReader] Initialize");
            _inputActions.Enable();
        }

        public void Dispose()
        {
            Debug.Log("[PlayerInputReader] Dispose");
            _inputActions.Disable();
            _inputActions.Dispose();
        }
    }
}