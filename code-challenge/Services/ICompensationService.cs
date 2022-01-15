using challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Services
{
    public interface ICompensationService
    {
        
        Compensation GetById(String compensationId);

        ISet<Compensation> GetByEmployee(string employeeId);
        Compensation Create(Compensation compensation);
        Compensation Replace(Compensation originalCompensation, Compensation newCompensation);
    }
}
