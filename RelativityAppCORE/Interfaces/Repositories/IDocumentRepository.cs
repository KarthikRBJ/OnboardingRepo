using RelativityAppCore.Repositories;
using RelativityAppCore.DAL.Entities;
using System.Collections.Generic;

namespace RelativityAppCore.Interfaces.Repositories
{
    interface IDocumentRepository : IGenericRepository<Document>
    {
        bool Create(Dictionary<string, string> documents);
    }
}
