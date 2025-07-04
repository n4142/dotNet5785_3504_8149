﻿
//namespace BO
//{
//    /// <summary>
//    /// Exception thrown when a required property is null or missing.
//    /// </summary>
//    [Serializable]
//    public class BlNullPropertyException : Exception
//    {
//        public BlNullPropertyException(string? message) : base(message) { }
//        public BlNullPropertyException(string message, Exception innerException)
//            : base(message, innerException) { }
//    }

//    /// <summary>
//    /// Exception thrown when an invalid time unit is encountered.
//    /// </summary>
//    [Serializable]
//    public class BlInvalidTimeUnitException : Exception
//    {
//        public BlInvalidTimeUnitException(string? message) : base(message) { }
//    }
//    [Serializable]
//    public class BlProgramException : Exception
//    {
//        public BlProgramException(string? message) : base(message) { }
//    }

//    /// <summary>
//    /// Exception thrown when a user attempts an unauthorized action.
//    /// </summary>
//    [Serializable]
//    public class BlUnauthorizedAccessException : Exception
//    {
//        public BlUnauthorizedAccessException(string? message) : base(message) { }
//    }

//    /// <summary>
//    /// Exception thrown when a requested entity does not exist in the system.
//    /// </summary>
//    [Serializable]
//    public class BlDoesNotExistException : Exception
//    {
//        public BlDoesNotExistException(string? message) : base(message) { }
//        public BlDoesNotExistException(string message, Exception innerException)
//            : base(message, innerException) { }
//    }

//    /// <summary>
//    /// Exception thrown when an invalid format is encountered in input data.
//    /// </summary>
//    [Serializable]
//    public class BlFormatException : Exception
//    {
//        public BlFormatException(string? message) : base(message) { }
//    }

//    /// <summary>
//    /// Exception thrown when a general database error occurs.
//    /// </summary>
//    [Serializable]
//    public class BlGeneralDatabaseException : Exception
//    {
//        public BlGeneralDatabaseException(string? message) : base(message) { }
//    }

//    /// <summary>
//    /// Exception thrown when an entity that is being created already exists.
//    /// </summary>
//    [Serializable]
//    public class BlAlreadyExistsException : Exception
//    {
//        public BlAlreadyExistsException(string? message) : base(message) { }
//    }

//    /// <summary>
//    /// Exception thrown when ArgumentException occurs.
//    /// </summary>
//    [Serializable]
//    public class BlArgumentException : Exception
//    {
//        public BlArgumentException(string? message) : base(message) { }
//    }
//}
namespace BO
{
    /// <summary>
    /// Exception thrown when a required property is null or missing.
    /// </summary>
    [Serializable]
    public class BlNullPropertyException : Exception
    {
        public BlNullPropertyException(string? message) : base(message) { }
        public BlNullPropertyException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when an invalid time unit is encountered.
    /// </summary>
    [Serializable]
    public class BlInvalidTimeUnitException : Exception
    {
        public BlInvalidTimeUnitException(string? message) : base(message) { }
        public BlInvalidTimeUnitException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown for general program errors.
    /// </summary>
    [Serializable]
    public class BlProgramException : Exception
    {
        public BlProgramException(string? message) : base(message) { }
        public BlProgramException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when a user attempts an unauthorized action.
    /// </summary>
    [Serializable]
    public class BlUnauthorizedAccessException : Exception
    {
        public BlUnauthorizedAccessException(string? message) : base(message) { }
        public BlUnauthorizedAccessException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when a requested entity does not exist in the system.
    /// </summary>
    [Serializable]
    public class BlDoesNotExistException : Exception
    {
        public BlDoesNotExistException(string? message) : base(message) { }
        public BlDoesNotExistException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when an invalid format is encountered in input data.
    /// </summary>
    [Serializable]
    public class BlFormatException : Exception
    {
        public BlFormatException(string? message) : base(message) { }
        public BlFormatException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when a general database error occurs.
    /// </summary>
    [Serializable]
    public class BlGeneralDatabaseException : Exception
    {
        public BlGeneralDatabaseException(string? message) : base(message) { }
        public BlGeneralDatabaseException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when an entity that is being created already exists.
    /// </summary>
    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message) : base(message) { }
        public BlAlreadyExistsException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when ArgumentException occurs.
    /// </summary>
    [Serializable]
    public class BlArgumentException : Exception
    {
        public BlArgumentException(string? message) : base(message) { }
        public BlArgumentException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
    [Serializable]
    public class BLTemporaryNotAvailableException : Exception
    {
        public BLTemporaryNotAvailableException(string? message) : base(message) { }
        public BLTemporaryNotAvailableException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
