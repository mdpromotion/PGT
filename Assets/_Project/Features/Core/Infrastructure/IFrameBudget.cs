using System;

namespace _Project.Features.Core.Infrastructure
{
    public interface IFrameBudget
    {
        bool TryBeginOperation(
            out IFrameBudgetOperation operation);
    }


    public interface IFrameBudgetOperation : IDisposable
    {
    }
}