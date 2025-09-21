
/// <summary>
/// Custom exception for handling bad requests.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}

/// <summary>
/// Custom exception for handling not found errors.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
/// <summary>
/// Custom exception for handling conflict errors.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}