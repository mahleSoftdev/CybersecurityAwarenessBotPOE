using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Chatbot
{
  
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool IsCompleted { get; set; }
    }


   
    public static class DatabaseHelper
    {
        //Connection 
        private static readonly string DatabaseFileName = "ChatbotData.db";

        private static readonly string ConnectionString =
            $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFileName)};Version=3;";


        //Schema 
        private const string CreateUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserId       INTEGER  PRIMARY KEY AUTOINCREMENT,
                UserName     TEXT     NOT NULL,
                SessionStart DATETIME NOT NULL DEFAULT (datetime('now','localtime'))
            );";

        private const string CreateChatLogTable = @"
            CREATE TABLE IF NOT EXISTS ChatLog (
                LogId    INTEGER  PRIMARY KEY AUTOINCREMENT,
                UserId   INTEGER  NOT NULL,
                Sender   TEXT     NOT NULL CHECK(Sender IN ('User', 'Bot', 'Warning')),
                Message  TEXT     NOT NULL,
                SentAt   DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

        private const string CreateQuizResultsTable = @"
            CREATE TABLE IF NOT EXISTS QuizResults (
                ResultId    INTEGER  PRIMARY KEY AUTOINCREMENT,
                UserId      INTEGER  NOT NULL,
                Score       INTEGER  NOT NULL,
                TotalQ      INTEGER  NOT NULL,
                CompletedAt DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

        private const string CreateActivityLogTable = @"
            CREATE TABLE IF NOT EXISTS ActivityLog (
                ActivityId INTEGER  PRIMARY KEY AUTOINCREMENT,
                UserId     INTEGER,
                EventType  TEXT     NOT NULL,
                Detail     TEXT,
                OccurredAt DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

        private const string CreateTasksTable = @"
            CREATE TABLE IF NOT EXISTS Tasks (
                TaskId      INTEGER  PRIMARY KEY AUTOINCREMENT,
                Title       TEXT     NOT NULL,
                Description TEXT     NOT NULL DEFAULT '',
                Reminder    TEXT     NOT NULL DEFAULT '',
                IsCompleted INTEGER  NOT NULL DEFAULT 0,
                CreatedAt   DATETIME NOT NULL DEFAULT (datetime('now','localtime'))
            );";


        // Initialisation 
        public static void InitialiseDatabase()
        {
            using (var connection = OpenConnection())
            {
                ExecuteNonQuery(connection, CreateUsersTable);
                ExecuteNonQuery(connection, CreateChatLogTable);
                ExecuteNonQuery(connection, CreateQuizResultsTable);
                ExecuteNonQuery(connection, CreateActivityLogTable);
                ExecuteNonQuery(connection, CreateTasksTable);
            }
        }


        //Users 
        public static int AddUser(string userName)
        {
            const string sql = @"
                INSERT INTO Users (UserName)
                VALUES (@UserName);
                SELECT last_insert_rowid();";

            using (var connection = OpenConnection())
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserName", userName);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        //Chat Log 
        public static void LogMessage(int userId, string sender, string message)
        {
            const string sql = @"
                INSERT INTO ChatLog (UserId, Sender, Message)
                VALUES (@UserId, @Sender, @Message);";

            using (var connection = OpenConnection())
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Sender", sender);
                command.Parameters.AddWithValue("@Message", message);
                command.ExecuteNonQuery();
            }
        }


        //Quiz Results 
        public static void SaveQuizResult(int userId, int score, int totalQuestions)
        {
            const string sql = @"
                INSERT INTO QuizResults (UserId, Score, TotalQ)
                VALUES (@UserId, @Score, @TotalQ);";

            using (var connection = OpenConnection())
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Score", score);
                command.Parameters.AddWithValue("@TotalQ", totalQuestions);
                command.ExecuteNonQuery();
            }
        }


        //Activity Log   
        public static void LogActivity(int? userId, string eventType, string detail = null)
        {
            const string sql = @"
                INSERT INTO ActivityLog (UserId, EventType, Detail)
                VALUES (@UserId, @EventType, @Detail);";

            using (var connection = OpenConnection())
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                command.Parameters.AddWithValue("@EventType", eventType);
                command.Parameters.AddWithValue("@Detail", (object)detail ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
        }


        //Tasks  
       public static bool AddTask(string title, string description, string reminder)
        {
            const string sql = @"
                INSERT INTO Tasks (Title, Description, Reminder)
                VALUES (@Title, @Description, @Reminder);";

            try
            {
                using (var connection = OpenConnection())
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Title", title ?? "");
                    command.Parameters.AddWithValue("@Description", description ?? "");
                    command.Parameters.AddWithValue("@Reminder", reminder ?? "");
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        
        public static List<TaskItem> GetAllTasks()
        {
            const string sql = @"
                SELECT TaskId, Title, Description, Reminder, IsCompleted
                FROM   Tasks
                ORDER  BY CreatedAt ASC;";

            var tasks = new List<TaskItem>();

            try
            {
                using (var connection = OpenConnection())
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            Reminder = reader.GetString(3),
                            IsCompleted = reader.GetInt32(4) == 1
                        });
                    }
                }
            }
            catch { }

            return tasks;
        }


        public static bool DeleteTask(int taskId)
        {
            const string sql = "DELETE FROM Tasks WHERE TaskId = @TaskId;";

            try
            {
                using (var connection = OpenConnection())
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TaskId", taskId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

       
        public static bool MarkTaskComplete(int taskId)
        {
            const string sql = "UPDATE Tasks SET IsCompleted = 1 WHERE TaskId = @TaskId;";

            try
            {
                using (var connection = OpenConnection())
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TaskId", taskId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }


        //Private Helpers 
        private static SQLiteConnection OpenConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}