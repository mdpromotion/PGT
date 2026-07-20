namespace _Project.Features.Player.Domain
{
    public interface IPlayerStanceState
    {
        bool IsCrouching { get; }
        float CrouchBlend { get; }
    }
}