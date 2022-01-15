using System.Collections.Generic;
using challenge.Models;
using challenge.Repositories;
using Microsoft.Extensions.Logging;

namespace challenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }
        public Compensation Create(Compensation compensation)
        {
            if(compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }
            return compensation;
        }

        public ISet<Compensation> GetByEmployee(string employeeId)
        {
            if (!string.IsNullOrEmpty(employeeId))
            {
                return _compensationRepository.GetByEmployeeId(employeeId);
            }

            return null;
        }

        public Compensation GetById(string compensationId)
        {
            if (!string.IsNullOrEmpty(compensationId))
            {
                return _compensationRepository.GetById(compensationId);
            }

            return null;
        }

        public Compensation Replace(Compensation originalCompensation, Compensation newCompensation)
        {
            throw new System.NotImplementedException();/*
            if (originalCompensation != null)
            {
                _compensationRepository.Remove(originalCompensation);
                if (newCompensation != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _compensationRepository.SaveAsync().Wait();

                    _compensationRepository.Add(newCompensation);
                    // overwrite the new id with previous employee id
                    newCompensation.CompensationId = originalCompensation.CompensationId;
                }
                _compensationRepository.SaveAsync().Wait();
            }

            return newCompensation;*/
        }
    }
}
