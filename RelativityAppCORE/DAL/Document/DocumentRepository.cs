using System;
using System.Collections.Generic;
using RelativityAppCORE.Interfaces;
using RelativityAppCORE.INTERFACES.Entities;
using kCura.Relativity.Client;
using RelativityAppCORE.Repositories;

namespace RelativityAppCORE.BLL.Repositories.Document
{
    public class DocumentRepositoryRsapi : IDocumentRepository
    {

        public IRSAPIClient client;
        public DocumentRepositoryRsapi(IRSAPIClient client)
        {
            this.client = client;
        }

        public bool Create(INTERFACES.Entities.Document artifact)
        {
            throw new NotImplementedException();
        }

        public bool Create(Document artifact)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int artifactId)
        {
            throw new NotImplementedException();
        }

        public DTOs.Document Get(int artifactId)
        {
            DTOs.Document documentDTO;
            try
            {
                documentDTO = client.Repositories.Document.ReadSingle(artifactId);

            }
            catch (Exception e)
            {
                throw;
            }
            return documentDTO;
        }

        public IEnumerable<DTOs.Document> GetAll()
        {
            DTOs.Query<DTOs.Document> query = new DTOs.Query<DTOs.Document>();
            DTOs.QueryResultSet<DTOs.Document> resultSet = new DTOs.QueryResultSet<DTOs.Document>();
            query.Fields.Add(new DTOs.FieldValue(DTOs.ArtifactFieldNames.TextIdentifier));
            List<DTOs.Document> documents = new List<DTOs.Document>();
            try
            {
                resultSet = client.Repositories.Document.Query(query);
                foreach (var d in resultSet.Results)
                {
                    documents.Add(d.Artifact);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return documents;
        }

        public bool Update(int artifactId, INTERFACES.Entities.Document artifact)
        {
            throw new NotImplementedException();
        }

        public bool Update(int artifactId, DTOs.Document artifact)
        {
            throw new NotImplementedException();
        }

        INTERFACES.Entities.Document IGenericRepository<INTERFACES.Entities.Document>.Get(int artifactId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<INTERFACES.Entities.Document> IGenericRepository<INTERFACES.Entities.Document>.GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
