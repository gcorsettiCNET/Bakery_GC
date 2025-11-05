namespace Bakery.Core.Common;

/// <summary>
/// Rappresenta il risultato di un'operazione che può avere successo o fallire
/// Elimina l'uso di eccezioni per il flow control
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Successful result cannot have an error");
        
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Crea un Result di successo
    /// </summary>
    public static Result Success() => new(true, string.Empty);

    /// <summary>
    /// Crea un Result di errore
    /// </summary>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Crea un Result di errore da eccezione
    /// </summary>
    public static Result Failure(Exception exception) => new(false, exception.Message);
}

/// <summary>
/// Result generico con valore di ritorno
/// </summary>
public class Result<T> : Result
{
    public T Value { get; }

    protected Result(bool isSuccess, T value, string error) : base(isSuccess, error)
    {
        if (isSuccess && value == null)
            throw new InvalidOperationException("Successful result must have a value");

        Value = value;
    }

    /// <summary>
    /// Crea un Result di successo con valore
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, string.Empty);

    /// <summary>
    /// Crea un Result di errore
    /// </summary>
    public static new Result<T> Failure(string error) => new(false, default(T)!, error);

    /// <summary>
    /// Crea un Result di errore da eccezione
    /// </summary>
    public static new Result<T> Failure(Exception exception) => new(false, default(T)!, exception.Message);

    /// <summary>
    /// Converte implicitamente il valore in Result<T>
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);
}