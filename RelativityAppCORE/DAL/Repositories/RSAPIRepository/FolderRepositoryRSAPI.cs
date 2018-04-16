using RelativityAppCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;
using DTOs = kCura.Relativity.Client.DTOs;

namespace RelativityAppCore.DAL.Repositories.RSAPIRepository
{
    class FolderRepositoryRSAPI : IFolderRepository
    {
        IRSAPIClient client;
        public FolderRepositoryRSAPI(IRSAPIClient client)
        {
            this.client = client;
        }

        public int Create(Folder artifact)
        {
            int result=0;
            DTOs.Folder folder = new DTOs.Folder();
            folder.Name = artifact.Name;
            folder.ParentArtifact = new DTOs.Artifact(artifact.ParentId);
            try
            {
                result=client.Repositories.Folder.CreateSingle(folder);
            }
            catch (Exception e)
            {
                Console.WriteLine( e.Message);
            }
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
