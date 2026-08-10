using System;

namespace _Project.Features.Shared.Application
{
    public interface ITick
    {
        public event Action Tick;
    }
}
