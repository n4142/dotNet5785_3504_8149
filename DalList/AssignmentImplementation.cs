

namespace Dal;

using System.Runtime.CompilerServices;
using DalApi;
using DO;

internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Assignment enteredAssignment)
    {
        int newId = Config.NextAssignmentId;
        if (DataSource.Assignments.Any(a => a.Id == newId))
        {
            throw new DalAlreadyExistException($"Assignment Object with Id {newId} already exists");
        }
        Assignment newAssignment = enteredAssignment with { Id = newId };
        DataSource.Assignments.Add(newAssignment);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
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
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Assignment? Read(int Id)
    {
        Assignment? a = DataSource.Assignments.FirstOrDefault(a => a.Id == Id);
        return a;
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
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
