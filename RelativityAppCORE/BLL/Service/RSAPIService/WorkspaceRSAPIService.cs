using RelativityAppCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;

namespace RelativityAppCore.BLL.Service.RSAPIService
{
    public class WorkspaceRSAPIService : IWorkspaceService
    {
        IRSAPIClient client;
        DAL.Repositories.RSAPIRepository.WorkspaceRepositoryRSAPI repository;

        public WorkspaceRSAPIService(IRSAPIClient client)
        {
            this.client = client;
            repository = new DAL.Repositories.RSAPIRepository.WorkspaceRepositoryRSAPI(client);
        }
        public int Create(Workspace artifact)
        {
            int result = 0;
            result = repository.Create(artifact);
            return result;
        }

        public bool Delete(int artifactId)
        {
            bool result = repository.Delete(artifactId);
            return result;
        }

        public Workspace Get(int artifactId)
        {
            Workspace workspace = repository.Get(artifactId);
            return workspace;
        }

        public IEnumerable<Workspace> GetAll()
        {
            return repository.GetAll();
            
        }

        public bool Update(int artifactId, Workspace artifact)
        {
            bool result;
           return result = repository.Update(artifactId, artifact);
        }
    }
}
