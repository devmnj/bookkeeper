using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace accounts
{
    public class BackupService
    {
        private readonly SqlConnection _connection;
        private readonly string _backupFolderFullPath;
        private readonly string[] _systemDatabaseNames = { "master", "tempdb", "model",public_members.database };

        public BackupService(SqlConnection connection , string backupFolderFullPath)
        {
            _connection = connection;
            public_members.AutoCreateDirectory(backupFolderFullPath);
            _backupFolderFullPath = backupFolderFullPath;
        }

        public void BackupAllUserDatabases()
        {
            foreach (string databaseName in GetAllUserDatabases())
            {
                BackupDatabase(databaseName);
            }
        }

        public void BackupDatabase(string databaseName)
        {
            string filePath = BuildBackupPathWithFilename(databaseName);

           
                var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", databaseName, filePath);

                using (var command = new SqlCommand(query, _connection))
                {
                   // _connection.Open();
                    command.ExecuteNonQuery();
                }
             
        }

        private IEnumerable<string> GetAllUserDatabases()
        {
            var databases = new List<String>();

            DataTable databasesTable;

             
               // _connection.Open();

                databasesTable = _connection.GetSchema("Databases");

               // _connection.Close();
            

            foreach (DataRow row in databasesTable.Rows)
            {
                string databaseName = row["database_name"].ToString();

                if (_systemDatabaseNames.Contains(databaseName))
                    continue;

                databases.Add(databaseName);
            }

            return databases;
        }

        private string BuildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now);

            return Path.Combine(_backupFolderFullPath, filename);
        }
    }
}
