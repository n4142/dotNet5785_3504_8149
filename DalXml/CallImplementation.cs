namespace Dal;
using DO;
using DalApi;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Call enteredCall)
    {
        int newId = Config.NextCallId;
        Call copyCall = enteredCall with
        {
            Id = newId,
            OpeningTime = enteredCall.OpeningTime == default
        ? Config.Clock
        : enteredCall.OpeningTime
        };

        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Calls.Add(copyCall);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);

    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? c = Calls.FirstOrDefault(c => c.Id == id);
        return c;
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public List<Call> ReadAll()
    {
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
}
