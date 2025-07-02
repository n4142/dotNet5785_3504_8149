

namespace Dal;

using System.Runtime.CompilerServices;
using DalApi;
using DO;


internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Call enteredCall)
    {
        int newId = Config.NextCallId;
        if (DataSource.Calls.Any(c => c.Id == newId))
        {
            throw new DalAlreadyExistException($"Call Object with Id {newId} already exists");
        }
        Call copyCall = enteredCall with { Id = newId };
        DataSource.Calls.Add(copyCall);
    }


    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        Call? c = Read(id);
        if (c is null)
        {
            throw new DalDoesNotExistException($"Call Object with {id} doesn't exist");
        }
        else
        {
            DataSource.Calls.Remove(c);
        }
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {

        DataSource.Calls.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(int Id)
    {
        Call? c = DataSource.Calls.FirstOrDefault(c => c.Id == Id);
        return c;
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Call updatedCall)
    {
        Call? c = Read(updatedCall.Id);
        if (c is null)
        {
            throw new DalDoesNotExistException($"Call Object with {updatedCall.Id} doesn't exist");
        }
        else
        {
            Delete(updatedCall.Id);
            DataSource.Calls.Add(updatedCall);
        }
    }
}
