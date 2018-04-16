using RelativityAppCore.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using RelativityAppCore.DAL.Entities;
using kCura.Relativity.Client;
using DTOs = kCura.Relativity.Client.DTOs;

namespace RelativityAppCore.DAL.Repositories.RSAPIRepository
{
    class WorkspaceRepositoryRSAPI : IWorkspaceRepository
    {
        IRSAPIClient client;

        public WorkspaceRepositoryRSAPI(IRSAPIClient client)
        {
            this.client = client;
        }
        public int Create(Workspace artifact)
        {
            int response = 0;
            int? templateArtifactID = null;

            DTOs.Query<DTOs.Workspace> query = new DTOs.Query<DTOs.Workspace>();
            query.Condition = new TextCondition(DTOs.FieldFieldNames.Name, TextConditionEnum.EqualTo, "kCura Starter Template");
            query.Fields = DTOs.FieldValue.AllFields;
            DTOs.QueryResultSet<DTOs.Workspace> results = client.Repositories.Workspace.Query(query, 0);

            if (results.Success)
            {
                templateArtifactID = results.Results.FirstOrDefault().Artifact.ArtifactID;
            }
            else
            {
                return response;
            }
            DTOs.Workspace workspaceToCreate = new DTOs.Workspace();
            workspaceToCreate.Name = artifact.Name;
            workspaceToCreate.Client = client.Repositories.Client.ReadSingle(artifact.ClientId);
            workspaceToCreate.MatterID = artifact.MatterId;
            ProcessOperationResult result = new ProcessOperationResult();
            ProcessInformation process = new ProcessInformation();
            try
            {
                result = client.Repositories.Workspace.CreateAsync(templateArtifactID.Value, workspaceToCreate);
                response = 1;
            }
            catch (Exception)
            {
                throw;
            }
            return response;
        }

        public bool Delete(int artifactId)
        {
            bool result = false;
            try
            {
                client.Repositories.Workspace.DeleteSingle(artifactId);
                result = true;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public Workspace Get(int artifactId)
        {
            Workspace workspace = new Workspace();
            try
            {
                DTOs.Workspace workspaceDTO = client.Repositories.Workspace.ReadSingle(artifactId);
                workspace.ArtifactId = workspaceDTO.ArtifactID;
                workspace.Name = workspaceDTO.Name;
                workspace.MatterId = workspaceDTO.MatterID.Value;
                workspace.ClientId = workspaceDTO.Client.ArtifactID;
                Entities.Artifact user = new Entities.Artifact(workspaceDTO.SystemCreatedBy.ArtifactID);
                user.Name = workspaceDTO.SystemCreatedBy.FullName;
                workspace.CreatedBy = user;
            }
            catch (Exception)
            {

                throw;
            }
            return workspace;
        }

        public IEnumerable<Workspace> GetAll()
        {
            List<Workspace> workspacesList = new List<Workspace>();
            DTOs.QueryResultSet<DTOs.Workspace> resultSet = new DTOs.QueryResultSet<DTOs.Workspace>();
            DTOs.Query<DTOs.Workspace> query = new DTOs.Query<DTOs.Workspace>();
            DTOs.QueryResultSet<DTOs.Workspace> results = new DTOs.QueryResultSet<DTOs.Workspace>();
            query.Fields = DTOs.FieldValue.AllFields;
            try
            {
                resultSet = client.Repositories.Workspace.Query(query);

                foreach (var w in resultSet.Results)
                {
                    Workspace workspace = Get(w.Artifact.ArtifactID);
                    workspacesList.Add(workspace);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return workspacesList;

        }

        public bool Update(int artifactId, Workspace artifact)
        {
            bool result = false;
            DTOs.Workspace workspace = new DTOs.Workspace(artifactId);
            workspace.Name = artifact.Name;
            workspace.MatterID = artifact.MatterId;
            workspace.Client = client.Repositories.Client.ReadSingle(artifact.ClientId);
            try
            {
                client.Repositories.Workspace.UpdateSingle(workspace);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
