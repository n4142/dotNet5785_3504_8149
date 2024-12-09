
namespace Dal;
using DalApi;
using DO;


internal class VolunteerImplementation : IVolunteer
{
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

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int Id)
    {
        Volunteer? v = DataSource.Volunteers.FirstOrDefault(v => v.Id == Id);
        return v;
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }


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
