using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelativityAppCore.DAL.Connection.SqlConnection
{
    public class Connection : IHelper
    {
        public string server;
        public string database;
        public string login;
        public string password;

        public Connection(string server, string database, string login, string password)
        {
            this.server = server;
            this.database = database;
            this.login = login;
            this.password = password;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDBContext GetDBContext(int caseID)
        {
            kCura.Data.RowDataGateway.Context context;
            string db = "EDDS";
            try
            {
                if (caseID.Equals(-1))
                {
                    context = new kCura.Data.RowDataGateway.Context(server, db, login, password);
                }
                else
                {
                    context = new kCura.Data.RowDataGateway.Context(server, database, login, password);
                }
            }
            catch (Exception)
            {

                throw;
            }
            

            return new DBContext(context);
        }

        public Guid GetGuid(int workspaceID, int artifactID)
        {
            throw new NotImplementedException();
        }

        public ILogFactory GetLoggerFactory()
        {
            throw new NotImplementedException();
        }

        public string GetSchemalessResourceDataBasePrepend(IDBContext context)
        {
            throw new NotImplementedException();
        }

        public IServicesMgr GetServicesManager()
        {
            throw new NotImplementedException();
        }

        public IUrlHelper GetUrlHelper()
        {
            throw new NotImplementedException();
        }

        public string ResourceDBPrepend()
        {
            throw new NotImplementedException();
        }

        public string ResourceDBPrepend(IDBContext context)
        {
            throw new NotImplementedException();
        }
    }
}
