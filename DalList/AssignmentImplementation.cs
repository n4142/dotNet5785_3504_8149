

namespace Dal;
using DalApi;
using DO;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment enteredAssignment)
    {
        int newId = Config.NextAssignmentId;
        Assignment newAssignment = enteredAssignment with { Id = newId };
        DataSource.Assignments.Add(newAssignment);
    }

    public void Delete(int id)
    {
        Assignment? a = Read(id);
        if (a is null)
        {
            throw new DalDoesNotExistException($"Assignment Object with {id} doesn't exist");
        }
        else
        {
            DataSource.Assignments.Remove(a);
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int Id)
    {
        Assignment? a = DataSource.Assignments.FirstOrDefault(a => a.Id == Id);
        return a;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment updatedAssignment)
    {
        Assignment? a = Read(updatedAssignment.Id);
        if (a is null)
        {
            throw new DalDoesNotExistException($"Assignment Object with {updatedAssignment.Id} doesn't exist");
        }
        else
        {
            Delete(updatedAssignment.Id);
            DataSource.Assignments.Add(updatedAssignment);
        }
    }
}
