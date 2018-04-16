using RelativityAppCore.Interfaces.Services;
using System;
using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;

namespace RelativityAppCore.BLL.Service.RSAPIService
{
    public class FolderRSAPIService : IFolderService
    {
        IRSAPIClient client;
        DAL.Repositories.RSAPIRepository.FolderRepositoryRSAPI repository;

        public FolderRSAPIService()
        {
        }

        public FolderRSAPIService(IRSAPIClient client)
        {
            this.client = client;
            repository = new DAL.Repositories.RSAPIRepository.FolderRepositoryRSAPI(client);
        }

        public int Create(Folder artifact)
        {
            int result = 0;
            result = repository.Create(artifact);
            return result;
        }

        public bool Delete(int artifactId)
        {
            throw new NotImplementedException();
        }

        public Folder Get(int artifactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Folder> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Update(int artifactId, Folder artifact)
        {
            throw new NotImplementedException();
        }
    }
}
