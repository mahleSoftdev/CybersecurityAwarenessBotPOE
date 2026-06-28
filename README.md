🛡️ Cybersecurity Awareness Chatbot

A desktop chatbot built in C# WPF that helps users improve their cybersecurity knowledge through interactive conversations, task management, quizzes, and Natural Language Processing (NLP) simulation.

📌 Project Overview

The Cybersecurity Awareness Chatbot is an intelligent desktop assistant developed for cybersecurity awareness. It provides users with educational information about online threats while offering productivity features such as task management and reminders.

The chatbot includes:

✔ Cybersecurity education

✔ Personalized conversations

✔ NLP command recognition

✔ Task Assistant

✔ Cybersecurity Quiz

✔ Activity Log

✔ MySQL database integration

✔ Modern WPF graphical interface

✨ Features
🔒 Cybersecurity Assistant

Learn about topics including:

Phishing
Malware
Ransomware
Password Safety
Social Engineering
Two-Factor Authentication
Safe Browsing
📋 Task Assistant

Users can:

Add tasks
View tasks
Complete tasks
Delete tasks
Store tasks permanently in MySQL

Example commands:

Add task

Remind me to submit my assignment

Show my tasks

Delete task
🧠 NLP Simulation

The chatbot recognizes multiple ways of asking the same question.

Examples:

User Input	Recognized Action
Remind me to study	Create Task
Create a task	Create Task
Show my tasks	View Tasks
List my tasks	View Tasks
Start quiz	Begin Quiz
Test me	Begin Quiz
🎮 Cybersecurity Quiz

Challenge your cybersecurity knowledge.

Commands:

quiz

start quiz

take quiz

challenge me

The chatbot:

Asks multiple-choice questions
Tracks your score
Displays your final result
📜 Activity Log

Every important action is recorded.

Commands:

activity log

show activity log

history

show log
💻 Software Requirements

Before running the project install:

Visual Studio 2022 or newer
.NET Desktop Development workload
.NET Framework
MySQL Server 8.0+
MySQL Workbench (Recommended)
MySQL Connector (MySql.Data)
🚀 Getting Started
Step 1

Extract the ZIP file.

Step 2

Open

Chatbot.slnx

using Visual Studio.

Step 3

Restore the NuGet packages.

Step 4

Open

DatabaseHelper.cs

Update the connection string.

Example:

Server=localhost;
Database=ChatbotDB;
Uid=root;
Pwd=yourpassword;
Step 5

Build the solution.

Step 6

Press

F5

or

Start

to run the application.

🗄 Database Setup

Create a new MySQL database.

CREATE DATABASE ChatbotDB;

Update your connection string inside

DatabaseHelper.cs

The chatbot automatically creates the required tables during the first launch.

📖 How to Use
Starting the Chatbot

Launch the application.

Enter your name when prompted.

Begin chatting naturally.

Task Assistant

Example commands:

Add task

Create task

Remind me to study

Show tasks

Complete task

Delete task
Quiz

Type:

quiz

or

start quiz

Follow the prompts.

Receive your final score.

NLP Testing

Try different phrases like:

Teach me about phishing

Explain ransomware

Create a task

I need to remember something

Show my tasks

Test me

Challenge me

The chatbot understands different sentence structures while performing the same action.

Activity Log

Type:

history

or

activity log

to view recent chatbot activity.

🔑 Login Information

No account is required.

The chatbot only asks for your name to personalize the conversation.

⚠ Important Notes
Ensure MySQL Server is running before launching the application.
Update the database connection string before building the project.
Internet access is not required after setup.
The chatbot automatically creates database tables when necessary.

📂 Project Structure
CybersecurityChatbot
│
├── Models
├── Services
├── Database
├── UI
├── Resources
├── MainWindow.xaml
├── DatabaseHelper.cs
└── README.md
🎥 Video Demonstration

Video Presentation



👨‍💻 Developed By

Asemahle Myeni

Cybersecurity Awareness Chatbot

C# WPF Desktop Application

MySQL Database Integration

Natural Language Processing Simulation

Cybersecurity Education Project
