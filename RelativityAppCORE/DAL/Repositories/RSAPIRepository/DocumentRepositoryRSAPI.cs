using kCura.Relativity.Client;
using RelativityAppCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using DAL=RelativityAppCore.DAL.Entities;
using DTOs = kCura.Relativity.Client.DTOs;

namespace RelativityAppCore.DAL.Repositories.RSAPIRepository
{
    class DocumentRepositoryRSAPI:IDocumentRepository
    {
        public IRSAPIClient client;
        public DocumentRepositoryRSAPI(IRSAPIClient client)
        {
            this.client = client;
        }

        public bool Create(Dictionary<string, string> documents)
        {
            throw new NotImplementedException();
        }

        public int Create(Entities.Document artifact)
        {

            int result = 0;
            DTOs.Document documentToCreate = new DTOs.Document();
            documentToCreate.RelativityNativeFileLocation = artifact.RelativityNativeFileLocation;
            documentToCreate.TextIdentifier = artifact.Name;
            documentToCreate.ParentArtifact = new DTOs.Artifact(artifact.ParentArtifactId);

            try

            {
                result = client.Repositories.Document.CreateSingle(documentToCreate);
                
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public bool Delete(int artifactId)
        {
            try
            {
                client.Repositories.Document.DeleteSingle(artifactId);
                return true;
            }
            catch (Exception)
            {
                throw;  
            }
        }

        public Entities.Document Get(int artifactId)
        {
            Entities.Document document = new Entities.Document();
            try
            {
                DTOs.Document documentDTO = client.Repositories.Document.ReadSingle(artifactId);
                List<Entities.Comment> comments = new List<Entities.Comment>();
                CommentRepositoryRSAPI cr = new CommentRepositoryRSAPI(this.client);
                DTOs.FieldValue f1= new DTOs.FieldValue();
                document.amountComments = 0;
                foreach(var f in documentDTO.Fields)
                {
                    if(f.Name == "Commens - Document")
                    {
                        if(f.Value != null)
                        {
                            DTOs.FieldValueList<DTOs.Artifact> list = (DTOs.FieldValueList<DTOs.Artifact>)f.Value;
                            document.amountComments = list.Count;
                            foreach (var field in list)
                            {
                                comments.Add(cr.Get(field.ArtifactID));
                            }
                        }
                        
                    }
                }
                document.ArtifactId = documentDTO.ArtifactID;
                document.Name = documentDTO.TextIdentifier;
                document.Comments = comments;
                Entities.Artifact user= new Entities.Artifact(documentDTO.SystemCreatedBy.ArtifactID);
                user.Name = documentDTO.SystemCreatedBy.FullName;
                document.CreatedBy = user;
            }
            catch (Exception)
            {
                throw;
            }
            return document;
        }

        public IEnumerable<Entities.Document> GetAll()
        {
            DTOs.Query<DTOs.Document> query = new DTOs.Query<DTOs.Document>();
            DTOs.QueryResultSet<DTOs.Document> resultSet = new DTOs.QueryResultSet<DTOs.Document>();
            query.Fields.Add(new DTOs.FieldValue(DTOs.ArtifactFieldNames.TextIdentifier));
            List<DTOs.Document> documents = new List<DTOs.Document>();
            List<Entities.Document> documentsList = new List<Entities.Document>();
            try
            {
                resultSet = client.Repositories.Document.Query(query);
                foreach (var d in resultSet.Results)
                {
                    Entities.Document document  = Get(d.Artifact.ArtifactID);
                    documentsList.Add(document);
                }
                
            }
            catch (Exception)
            {
                throw;
            }

            return documentsList;


        }

        public bool Update(int artifactId, Entities.Document artifact)
        {
            bool result = false;
            DTOs.Document document = new DTOs.Document(artifactId);
            document.TextIdentifier = artifact.Name;
            document.ParentArtifact = new DTOs.Artifact(artifact.ParentArtifactId);
            try
            {
                client.Repositories.Document.UpdateSingle(document);
                result = true;
            }
            catch (Exception)
            {
                throw;
            } 
            return result;
        }
    }
}
