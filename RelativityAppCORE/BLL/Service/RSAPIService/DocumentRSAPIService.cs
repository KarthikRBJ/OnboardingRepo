using RelativityAppCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;
using RelativityAppCore.Interfaces.Services;

namespace RelativityAppCore.BLL.Service.RSAPIService
{
    public class DocumentRSAPIService : IDocumentService
    {
        IRSAPIClient client;
        DAL.Repositories.RSAPIRepository.DocumentRepositoryRSAPI repository;
        public DocumentRSAPIService(IRSAPIClient client)
        {
            this.client = client;
            repository = new DAL.Repositories.RSAPIRepository.DocumentRepositoryRSAPI(client);
        }

        public bool Create(Dictionary<string, string> documents)
        {
            bool result;
            result = repository.Create(documents);
            return result;
        }

        public int Create(Document artifact)
        {
            int result = 0;
            result = repository.Create(artifact);
            return result;
        }

        public bool Delete(int artifactId)
        {
            bool result;
            result = repository.Delete(artifactId);
            return result;
        }

        public Document Get(int artifactId)
        {
            return repository.Get(artifactId);
        }

        public IEnumerable<Document> GetAll()
        {
            return repository.GetAll();
        }

        public bool Update(int artifactId, Document artifact)
        {
            bool result = false;
            result = repository.Update(artifactId, artifact);
            return result;
        }
    }
}
