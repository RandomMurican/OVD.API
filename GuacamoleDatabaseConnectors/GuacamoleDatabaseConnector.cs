using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public class GuacamoleDatabaseConnector : IDisposable
    {

        private MySqlConnection connection;
        private const string SERVER = "10.100.3.1";     //Guacamole SQL Docker Container IP
        private const string PORT = "3306";             //Guacamole SQL Docker Port
        private const string DATABASE = "guacamole_db";
        private const string USER = "root";
        private const string PASSWORD = "secret";

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/> class.
        /// </summary>
        public GuacamoleDatabaseConnector(ref List<Exception> excepts)
        {
            initialize();
            openConnection(ref excepts);
        }


        /// <summary>
        /// Releases all resource used by the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/>. The
        /// <see cref="Dispose"/> method leaves the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/> in an unusable
        /// state. After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/> so the garbage
        /// collector can reclaim the memory that the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/> was occupying.</remarks>
        public void Dispose()
        {
            ReleaseResources(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Releases the resources.
        /// </summary>
        /// <param name="isFromDispose">If set to <c>true</c> is from dispose.</param>
        protected void ReleaseResources(bool isFromDispose)
        {
            if (!isDisposed)
            {
                if (isFromDispose)
                {
                    closeConnection();
                }
            }
            isDisposed = true;
        }


        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:test_OVD_clientless.GuacamoleDatabaseConnectors.GuacamoleDatabaseConnector"/> is reclaimed by
        /// garbage collection.
        /// </summary>
        ~GuacamoleDatabaseConnector()
        {
            ReleaseResources(false);
        }


        /// <summary>
        /// Gets the MySqlDatabase connection.
        /// </summary>
        /// <returns>The Guacamole database connection.</returns>
        public MySqlConnection getConnection()
        {
            return this.connection;
        }


        /// <summary>
        /// Initialize this instance of the Guacamole mysql connection.
        /// </summary>
        private void initialize()
        {
            string connectionString;
            connectionString = "Server=" + SERVER + ";" + "Port=" + PORT + ";" + "Database=" +
            DATABASE + ";" + "UID=" + USER + ";" + "Password=" + PASSWORD + ";";

            connection = new MySqlConnection(connectionString);
        }


        /// <summary>
        /// Opens a connection to the Guacamole mysql database.
        /// </summary>
        /// <returns><c>true</c>, if connection was opened, <c>false</c> otherwise.</returns>
        private bool openConnection(ref List<Exception> excepts)
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                excepts.Add(e);
                return false;
            }
        }


        /// <summary>
        /// Closes the connection to the Guacamole mysql database.
        /// </summary>
        /// <returns><c>true</c>, if connection was closed, <c>false</c> otherwise.</returns>
        private void closeConnection()
        {
            connection.Close();
        }
    }
}