using BO;
using DalApi;
using Helpers;
using System;

namespace BlImplementation;
internal class AdminImplementation : BlApi.IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        switch (unit)
        {
            case BO.TimeUnit.Minute:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case BO.TimeUnit.Hour:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case BO.TimeUnit.Day:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case BO.TimeUnit.Month:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case BO.TimeUnit.Year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unit), "Invalid time unit");
        }
    }

    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    public TimeSpan GetMaxRange()
    {
        return AdminManager.RiskRange;
    }

    public void InitializeDB()
    {
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Now);
    }

    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.RiskRange = maxRange;
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;

    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;

    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;

    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

    public void StopSimulator()
    => AdminManager.Stop(); //stage 7

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

}
