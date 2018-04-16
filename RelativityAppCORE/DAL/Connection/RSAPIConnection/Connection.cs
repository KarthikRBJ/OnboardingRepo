using kCura.Relativity.Client;
using System;
using System.Configuration;

namespace RelativityAppCore.DAL.Connection.RSAPIConnection
{
    public class Connection
    {
        string baseAddres;
        string user;
        string password;
        public Connection(string baseAddres, string user, string password)
        {
            this.baseAddres = baseAddres;
            this.user = user;
            this.password = password;
        }
        public IRSAPIClient getConection(int workspaceId)
        {
            IRSAPIClient client = new RSAPIClient(new Uri(String.Format($"{baseAddres}/relativity.services/")),
                                                  new UsernamePasswordCredentials(user, password));
            client.APIOptions.WorkspaceID = workspaceId;
            return client;
        }
    }
}
