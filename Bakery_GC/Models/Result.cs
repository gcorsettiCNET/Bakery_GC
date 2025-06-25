using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HL7Analyzer.Models
{
    public sealed class Result
    {
        public readonly string Ok = "Ok";

        private Result()
        {
            Status = "Ok";
        }

        private Result(Error error)
        {
            Error = error;
            ErrorType = error.Type;
            Status = error.Status;
        }

        public Error Error { get; }
        public ErrorType? ErrorType { get; }
        public string Status { get; private set; }

        [JsonIgnore]
        public bool IsSuccess { get { return Status == Ok; } }


        public static Result Success() => new Result();
        public static Result Failure(Error error) => new Result(error);
    }

    public sealed class Result<T>
    {
        public readonly string Ok = "Ok";

        private Result(T value)
        {
            Value = value;
            Status = Ok;
        }

        private Result(Error error)
        {
            Error = error;
            Status = error.Status;
        }

        public T Value { get; }

        [JsonIgnore]
        public bool IsSuccess { get { return Status == Ok; } }

        public Error Error { get; }
        public string Status { get; private set; }

        public static Result<T> Success(T value) => new Result<T>(value);
        public static Result<T> Failure(Error error) => new Result<T>(error);
    }

    public record Error(ErrorType Type, string Description, string Status)
    {
        public static Error UnexpectedError = new(ErrorType.Error, "Unexpected error occurred", nameof(UnexpectedError));
        public static Error DuplicatedEntry = new(ErrorType.Error, "The record you are trying to insert is already present", nameof(DuplicatedEntry));
        public static Error ArgumentError = new(ErrorType.Error, "An argument error occurred", nameof(ArgumentError));
        public static Error NotFound = new(ErrorType.Error, "The item you are trying to find has not been found", nameof(NotFound));
        public static Error InvalidPaging = new(ErrorType.Error, "The paging options provided are not valid", nameof(InvalidPaging));

        public static Error FromException(Exception exception)
        {
            if (exception is ArgumentException)
                return ArgumentError;
            else if (exception is ArgumentNullException)
                return ArgumentNull;
            else if (exception is PathTooLongException)
                return PathTooLong;
            else if (exception is DirectoryNotFoundException)
                return DirectoryNotFound;
            else if (exception is UnauthorizedAccessException)
                return UnauthorizedAccess;
            else if (exception is ArgumentOutOfRangeException)
                return ArgumentOutOfRange;
            else if (exception is IOException)
                return IO;
            else if (exception is NotSupportedException)
                return NotSupported;
            else if (exception is System.Security.SecurityException)
                return SecurityViolation;
            else
                return new(ErrorType.Error, "Unexpected error occurred", nameof(UnexpectedError));
        }
    }

    public enum ErrorType
    {
        Error = 0,
        Warning = 1
    }
}
