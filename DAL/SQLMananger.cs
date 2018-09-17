using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SQLite;
using System.IO;

namespace DAL
{
    public class SQLMananger : ISQLMananger
    {
        SQLiteConnection m_dbConnection;
        const string SQL_INIT =
            "CREATE TABLE IF NOT EXISTS Video ( id   INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL, name TEXT UNIQUE);" +
            "CREATE TABLE IF NOT EXISTS Frame( id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,frameNumber INT,azureResult TEXT NOT NULL,idVideo INT  REFERENCES Video(id));" +
            "CREATE UNIQUE INDEX IF NOT EXISTS UN_frameNumber_idVideo ON Frame ( frameNumber,idVideo);";
        const string DB_NAME = "LieDetectorSaveFile.db";
        public SQLMananger()
        {
            if(!File.Exists(DB_NAME))
                SQLiteConnection.CreateFile(DB_NAME);
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
            using (SQLiteCommand command = new SQLiteCommand(SQL_INIT, m_dbConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void saveVideo(string videoName)
        {
            using (SQLiteCommand command = new SQLiteCommand("INSERT INTO Video (name) values ('" + videoName + "');",m_dbConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void saveFrame(string videoName, int frameNumber, string resultAzure)
        {

            using (SQLiteCommand command = new SQLiteCommand("SELECT id FROM Video WHERE name = '" + videoName + "';", m_dbConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                if (!reader.HasRows)
                {
                    saveVideo(videoName);
                }
            }
            using (SQLiteCommand command = new SQLiteCommand("SELECT id FROM Video WHERE name = '" + videoName + "';", m_dbConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();

                using (SQLiteCommand command2 = new SQLiteCommand("INSERT INTO Frame (frameNumber,azureResult,idVideo) values ('" + frameNumber + "', '" + resultAzure + "', '" + reader["id"] + "');", m_dbConnection))
                {
                    command2.ExecuteNonQuery();
                }

            }
        }

        public string getFrame(string videoName, int frameNumber)
        {
            using (SQLiteCommand command = new SQLiteCommand("SELECT azureResult FROM Frame JOIN Video ON Frame.idVideo=Video.id WHERE Video.Name='"+videoName+"' AND Frame.frameNumber="+frameNumber+";", m_dbConnection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                        return reader[0].ToString();
                    return null;
                }
            }

        }
    }
}
