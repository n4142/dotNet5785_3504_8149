using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    /// <summary>
    /// Interface for administrative operations in the system.
    /// </summary>
    public interface IAdmin
    {
        void InitializeDB();/// Initializes the database.
        void ResetDB();/// Resets the database to its initial state.
        TimeSpan GetMaxRange();/// Retrieves the maximum allowed range.
        void SetMaxRange(TimeSpan maxRange);/// Sets the maximum allowed range.
        DateTime GetClock();/// Retrieves the current system clock.
        void ForwardClock(BO.TimeUnit unit);/// Advances the system clock by a specified time unit.
    }
}
