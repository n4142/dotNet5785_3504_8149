namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment enteredAssignment)
    {
        int newId = Config.NextAssignmentId;
        Assignment newAssignment = enteredAssignment with { Id = newId };
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? a = Assignments.FirstOrDefault(a => a.Id == id);
        return a;
    }

    public List<Assignment> ReadAll()
    {
        return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
    }

    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
}