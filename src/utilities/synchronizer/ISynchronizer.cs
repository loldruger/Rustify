using Rustify.Monads;
using System;

namespace Rustify.Utilities.Synchronizer
{
    public enum SynchronizerErrorKind
    {
        Locked,
        Failed,
        UnknownError,
        RecursionError,
        Disposed,
        Timeout,
        Cancelled
    }

    public readonly struct SynchronizerError : IEquatable<SynchronizerError>
    {
        public SynchronizerErrorKind Kind { get; }
        public string? Message { get; }
        public Exception? InnerException { get; }

        public SynchronizerError(SynchronizerErrorKind kind, string? message = null, Exception? innerException = null)
        {
            this.Kind = kind;
            this.Message = message;
            this.InnerException = innerException;
        }

        public static SynchronizerError Locked(string? message = null) =>
            new(SynchronizerErrorKind.Locked, message);

        public static SynchronizerError Failed(string? message = null, Exception? inner = null) =>
            new(SynchronizerErrorKind.Failed, message, inner);

        public static SynchronizerError Disposed(string? message = null) =>
            new(SynchronizerErrorKind.Disposed, message);

        public static SynchronizerError Timeout(string? message = null) =>
            new(SynchronizerErrorKind.Timeout, message);

        public static SynchronizerError Cancelled(string? message = null, Exception? inner = null) =>
            new(SynchronizerErrorKind.Cancelled, message, inner);

        public static SynchronizerError UnknownError(string? message = null, Exception? inner = null) =>
            new(SynchronizerErrorKind.UnknownError, message, inner);

        public static SynchronizerError RecursionError(string? message = null) =>
            new(SynchronizerErrorKind.RecursionError, message);

        public bool Equals(SynchronizerError other) =>
            this.Kind == other.Kind && this.Message == other.Message;

        public override bool Equals(object? obj) =>
            obj is SynchronizerError error && this.Equals(error);

        public override int GetHashCode() =>
            HashCode.Combine(this.Kind, this.Message);

        public override string ToString() =>
            this.Message != null ? $"{this.Kind}: {this.Message}" : this.Kind.ToString();

        public static bool operator ==(SynchronizerError left, SynchronizerError right) =>
            left.Equals(right);

        public static bool operator !=(SynchronizerError left, SynchronizerError right) =>
            !left.Equals(right);

        public static implicit operator SynchronizerError(SynchronizerErrorKind kind) =>
            new(kind);
    }

    [Obsolete("Use SynchronizerErrorKind instead")]
    public enum ISynchronizerError
    {
        Locked,
        Failed,
        UnknownError,
        RecursionError
    }

    public interface ISynchronizer<T> where T : notnull
    {
        Result<T, ISynchronizerError> GetValue();
    }
}