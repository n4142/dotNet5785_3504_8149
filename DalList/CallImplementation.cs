

namespace Dal;
using DalApi;
using DO;


internal class CallImplementation : ICall
{
    public void Create(Call enteredCall)
    {
        int newId = Config.NextCallId;
        Call copyCall = enteredCall with { Id = newId };
        DataSource.Calls.Add(copyCall);
    }


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

    public void DeleteAll()
    {

        DataSource.Calls.Clear();
    }


    public Call? Read(int Id)
    {
        Call? c = DataSource.Calls.FirstOrDefault(c => c.Id == Id);
        return c;
    }
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

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
