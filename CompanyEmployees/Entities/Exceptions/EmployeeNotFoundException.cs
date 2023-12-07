namespace Entities.Exceptions
{
    public class EmployeeNotFoundException(Guid employeeId) : NotFoundException($"Employee with id: {employeeId} doesn't exist in the database.")
    {
    }
}