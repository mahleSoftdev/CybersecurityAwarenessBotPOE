
CREATE TABLE IF NOT EXISTS Users (
    UserId       INTEGER  PRIMARY KEY AUTOINCREMENT,
    UserName     TEXT     NOT NULL,
    SessionStart DATETIME NOT NULL DEFAULT (datetime('now', 'localtime'))
);


CREATE TABLE IF NOT EXISTS ChatLog (
    LogId    INTEGER  PRIMARY KEY AUTOINCREMENT,
    UserId   INTEGER  NOT NULL,
    Sender   TEXT     NOT NULL CHECK (Sender IN ('User', 'Bot', 'Warning')),
    Message  TEXT     NOT NULL,
    SentAt   DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')),

    FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE TABLE IF NOT EXISTS QuizResults (
    ResultId    INTEGER  PRIMARY KEY AUTOINCREMENT,
    UserId      INTEGER  NOT NULL,
    Score       INTEGER  NOT NULL,
    TotalQ      INTEGER  NOT NULL,
    CompletedAt DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')),

    FOREIGN KEY (UserId) REFERENCES Users (UserId)
);



CREATE TABLE IF NOT EXISTS ActivityLog (
    ActivityId INTEGER  PRIMARY KEY AUTOINCREMENT,
    UserId     INTEGER,
    EventType  TEXT     NOT NULL,
    Detail     TEXT,
    OccurredAt DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')),

    FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE INDEX IF NOT EXISTS idx_chatlog_user     ON ChatLog     (UserId);
CREATE INDEX IF NOT EXISTS idx_quizresults_user ON QuizResults (UserId);
CREATE INDEX IF NOT EXISTS idx_activitylog_user ON ActivityLog (UserId);
