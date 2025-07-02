
namespace Dal;

using System.Runtime.CompilerServices;
using DalApi;
using DO;


internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Volunteer vol)
    {
        Volunteer? v = Read(vol.Id);
        if (v is not null)
        {
            throw new DalAlreadyExistException($"Volunteer Object with {vol.Id} already exists");
        }
        else
        {
            DataSource.Volunteers.Add(vol);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {

        Volunteer? v = Read(id);
        if (v is null)
        {
            throw new DalDoesNotExistException($"Volunteer Object with {id} doesn't exist");
        }
        else
        {
            DataSource.Volunteers.Remove(v);
        }
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Volunteer? Read(int Id)
    {
        Volunteer? v = DataSource.Volunteers.FirstOrDefault(v => v.Id == Id);
        return v;
    }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Volunteer vol)
    {
        Volunteer? v = Read(vol.Id);
        if (v is null)
        {
            throw new DalDoesNotExistException($"Volunteer Object with {vol.Id} doesn't exist");
        }
        else
        {
            Delete(vol.Id);
            DataSource.Volunteers.Add(vol);
        }
    }
}
