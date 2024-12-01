using OneOf;

namespace quotifyai.Core.Common;

public sealed class Result<TSuccess, TError> : OneOfBase<TSuccess, TError>
{
    public Result(TSuccess success) : base(success) { }
    public Result(TError error) : base(error) { }

    public bool IsSuccess => IsT0;
    public bool IsError => IsT1;

    public TSuccess ToSuccess() =>
        IsT0 ? AsT0 : throw new InvalidOperationException("Expected success result.");
    public TError ToError() =>
        IsT1 ? AsT1 : throw new InvalidOperationException("Expected error result.");

    public static implicit operator Result<TSuccess, TError>(TSuccess success) => new(success);

    public static implicit operator Result<TSuccess, TError>(TError error) => new(error);
}