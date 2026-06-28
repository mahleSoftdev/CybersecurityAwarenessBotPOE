using System;
using System.Collections.Generic;

namespace Chatbot
{
    /*AI Assited to an extent, but mostly hand-crafted responses to ensure accuracy and relevance for South African users. 
      * The bot is designed to be friendly, informative, and easy to understand, even for those new to cybersecurity. 
      * It can detect user sentiment and provide empathetic responses to help users feel supported while learning about online safety. 
      * The bot also remembers the user's favourite topic to personalise future interactions and make the learning experience more engaging. 
      * public class ChatBotEngine*/
    public class ChatBotEngine
    {
        //User memory
        public  string UserName      { get; private set; } = "User";
        public  string CurrentTopic  { get; private set; } = null;
        private string _favouriteTopic = null;

        // Task 1: Task assistant state
        public  bool   AwaitingReminder     { get; private set; } = false;
        private string _pendingTaskTitle       = null;
        private string _pendingTaskDescription = null;

        // Task 2: Quiz state
        public  bool InQuiz      { get; private set; } = false;
        private int  _quizIndex  = 0;
        private int  _quizScore  = 0;

        // Task 4: Activity log (stores last 10 actions)
        private readonly List<string> _activityLog = new List<string>();

        //keywords
        private static readonly string[] SentimentWorried    = { "worried", "scared", "afraid", "nervous", "anxious", "fear", "terrified" };
        private static readonly string[] SentimentCurious    = { "curious", "wonder", "interested", "tell me more", "how does", "what is", "explain", "i want to learn", "i want to know", "teach me" };
        private static readonly string[] SentimentFrustrated = { "frustrated", "confused", "don't understand", "lost", "overwhelmed", "difficult", "hard", "complicated", "stuck" };
        private static readonly string[] FollowUp            = { "another tip", "more", "tell me more", "give me more", "continue", "go on", "next", "elaborate", "details", "more details", "another", "again" };
        private static readonly string[] ExitWords           = { "bye", "exit", "quit", "goodbye", "see you", "later", "close", "done" };

        // Task 3: NLP keyword groups — flexible phrasing for all intents
        private static readonly string[] NlpAddTask      = { "add task", "add a task", "new task", "create task", "create a task", "i need to", "remind me to", "can you remind me", "set a task", "make a task", "i want to", "schedule task", "log a task" };
        private static readonly string[] NlpShowTasks    = { "show tasks", "view tasks", "my tasks", "list tasks", "what are my tasks", "show my tasks", "display tasks", "see my tasks", "get my tasks", "all tasks" };
        private static readonly string[] NlpDeleteTask   = { "delete task", "remove task", "delete a task", "remove a task", "get rid of task", "cancel task" };
        private static readonly string[] NlpCompleteTask = { "complete task", "mark task", "mark as done", "mark complete", "finish task", "done with task", "completed task", "task done", "i finished", "i completed" };
        private static readonly string[] NlpStartQuiz    = { "quiz", "start quiz", "take quiz", "test me", "cyber quiz", "begin quiz", "play quiz", "i want a quiz", "challenge me", "test my knowledge", "trivia" };
        private static readonly string[] NlpActivityLog  = { "show activity log", "activity log", "what have you done", "what have you done for me", "show log", "recent actions", "history", "what did you do", "log", "past actions" };
        private static readonly string[] NlpShowMoreLog  = { "show more log", "show more", "full log", "full history", "show all log", "show all actions", "more actions", "see all" };
        private static readonly string[] NlpTellMeAbout  = { "tell me about", "i want to learn about", "i want to know about", "teach me about", "explain to me", "information about", "info on", "help me understand" };

        //Random for response variation
        private static readonly Random _rng = new Random();

        private readonly Dictionary<string[], List<string>> _responses;

        // Task 2: Quiz questions (question, options, correct answer index)
        private static readonly List<QuizQuestion> _quizQuestions = new List<QuizQuestion>
        {
            new QuizQuestion(
                "What does 'phishing' mean in cybersecurity?",
                new[] { "A) Catching fish online", "B) Tricking users into revealing sensitive info", "C) A type of firewall", "D) Encrypting data" },
                1),
            new QuizQuestion(
                "True or False: You should use the same password for all your accounts.",
                new[] { "A) True", "B) False" },
                1),
            new QuizQuestion(
                "What does '2FA' stand for?",
                new[] { "A) Two-Factor Authentication", "B) Two-File Access", "C) Twice-Filtered Application", "D) Two-Firewall Approach" },
                0),
            new QuizQuestion(
                "True or False: HTTPS in a website URL means the connection is encrypted.",
                new[] { "A) True", "B) False" },
                0),
            new QuizQuestion(
                "Which of these is the strongest password?",
                new[] { "A) password123", "B) John1990", "C) Coffee!Sunrise#42Table", "D) abc123" },
                2),
            new QuizQuestion(
                "What is ransomware?",
                new[] { "A) Software that speeds up your PC", "B) Malware that encrypts your files and demands payment", "C) A type of antivirus", "D) A VPN service" },
                1),
            new QuizQuestion(
                "True or False: Public Wi-Fi networks are generally safe to use for banking.",
                new[] { "A) True", "B) False" },
                1),
            new QuizQuestion(
                "What is 'social engineering' in cybersecurity?",
                new[] { "A) Building social media apps", "B) Manipulating people into revealing confidential info", "C) Engineering social networks", "D) A type of malware" },
                1),
            new QuizQuestion(
                "Which South African law protects your personal data?",
                new[] { "A) GDPR", "B) RICA", "C) POPIA", "D) FICA" },
                2),
            new QuizQuestion(
                "True or False: You should click links in unexpected emails from your bank to verify your account.",
                new[] { "A) True", "B) False" },
                1),
            new QuizQuestion(
                "What is the 3-2-1 backup rule?",
                new[] { "A) 3 passwords, 2 devices, 1 antivirus", "B) 3 copies, on 2 different media, 1 offsite/cloud", "C) 3 backups per day, 2 per week, 1 per month", "D) None of the above" },
                1),
            new QuizQuestion(
                "True or False: App-based 2FA is more secure than SMS-based 2FA.",
                new[] { "A) True", "B) False" },
                0)
        };

        // Task 2: Varied correct-answer feedback messages
        private static readonly string[] CorrectFeedback =
        {
            "✅ Correct! Well done!",
            "✅ Spot on! You know your stuff.",
            "✅ Excellent! That's exactly right.",
            "✅ Great work! Keep it up.",
            "✅ That's correct! Cybersecurity expert in the making."
        };

        // Task 2: Varied incorrect-answer feedback messages
        private static readonly string[] IncorrectFeedback =
        {
            "❌ Not quite.",
            "❌ Incorrect — but now you know!",
            "❌ That wasn't right — every mistake is a learning opportunity.",
            "❌ Wrong answer this time.",
            "❌ Nope — don't worry, this is how we learn!"
        };

        public ChatBotEngine()
        {
            _responses = new Dictionary<string[], List<string>>(new KeyArrayComparer())
            {
                {
                    new[] { "how are you", "how r u", "you okay", "you good", "how do you feel", "hello" },
                    new List<string>
                    {
                        "I'm running smoothly and staying secure! Always on guard against cyber threats. How can I assist you today?",
                        "All systems online and threat-free! What cybersecurity topic can I help you with?",
                        "Fully patched and ready to help! What would you like to know about staying safe online?"
                    }
                },
                {
                    new[] { "your purpose", "what do you do", "why are you here", "what are you", "who are you" },
                    new List<string>
                    {
                        "I'm the Cybersecurity Awareness Assistant — your digital safety guide! I cover topics like phishing, passwords, malware, privacy, and much more. Just ask away!",
                        "Think of me as your personal digital bodyguard. I'm here to make sure you and your data stay safe online. Ask me anything cyber-related!",
                        "I'm a cybersecurity awareness bot built to help South Africans stay safe in a connected world. Type 'help' to see all my topics."
                    }
                },
                {
                    new[] { "what can i ask", "topics", "help", "what can you do", "what do you know", "menu", "options" },
                    new List<string>
                    {
                        "Here's what I can help you with:\n" +
                        "  🔑  Password safety\n" +
                        "  🎣  Phishing scams\n" +
                        "  🌐  Safe browsing\n" +
                        "  🧠  Social engineering\n" +
                        "  🦠  Malware and viruses\n" +
                        "  🔐  Two-factor authentication (2FA)\n" +
                        "  📶  Public Wi-Fi risks\n" +
                        "  💾  Data backups\n" +
                        "  🕵️  Privacy and tracking\n" +
                        "  💳  Online scams and fraud\n" +
                        "  📋  Task assistant — type 'add task [title]'\n" +
                        "  🎮  Cybersecurity quiz — type 'quiz'\n" +
                        "  📜  Activity log — type 'show activity log'\n" +
                        "  Just type a topic or click a chip button above!"
                    }
                },
                {
                    new[] { "password", "passwords", "strong password", "password safety", "passphrase" },
                    new List<string>
                    {
                        "Password Safety Tips:\n" +
                        "  ✅  Use at least 12 characters — longer is stronger.\n" +
                        "  ✅  Mix uppercase, lowercase, numbers, and symbols.\n" +
                        "  ✅  Never reuse the same password across different sites.\n" +
                        "  ✅  Use a reputable password manager (e.g. Bitwarden, 1Password).\n" +
                        "  ✅  Enable two-factor authentication (2FA) wherever possible.\n" +
                        "  ❌  Avoid personal info like birthdays or pet names.\n" +
                        "  ❌  Never share your password — not even with IT support!",

                        "Quick password tip: A passphrase like 'Coffee!Sunrise#42Table' is far stronger than a short complex password AND easier to remember. Try it!",

                        "Did you know? Over 80% of data breaches involve weak or reused passwords. A password manager solves this problem automatically — give Bitwarden (free) a try."
                    }
                },
                {
                    new[] { "phishing", "phish", "fake email", "suspicious email", "scam email", "email scam" },
                    new List<string>
                    {
                        "Phishing Awareness:\n" +
                        "  🎣  Phishing emails trick you into revealing personal information.\n" +
                        "  ✅  Always check the sender's actual email address carefully.\n" +
                        "  ✅  Look for spelling errors, urgency, and suspicious links.\n" +
                        "  ✅  Navigate directly to websites — don't click email links.\n" +
                        "  ✅  When in doubt, contact the organisation by phone directly.\n" +
                        "  ❌  Never enter passwords or banking details via email links.\n" +
                        "  💡  If it feels urgent and unexpected, it's almost certainly a scam.",

                        "Spot a phishing email: Hover over links before clicking — the real URL often looks nothing like the display text. Legitimate banks never ask for your PIN by email.",

                        "Phishing tip: Attackers often spoof trusted brands like SARS, FNB, or Standard Bank. When in doubt, call the institution directly using a number from their official website."
                    }
                },
                {
                    new[] { "safe browsing", "browsing", "internet safety", "safe internet", "browse safely", "web safety" },
                    new List<string>
                    {
                        "Safe Browsing Tips:\n" +
                        "  ✅  Always look for HTTPS (the padlock icon) in the address bar.\n" +
                        "  ✅  Keep your browser and its extensions up to date.\n" +
                        "  ✅  Use a reputable VPN on public or untrusted networks.\n" +
                        "  ✅  Install an ad-blocker to reduce exposure to malicious ads.\n" +
                        "  ❌  Avoid downloading files from unknown or untrusted sources.\n" +
                        "  ❌  Do not ignore browser security warnings — they exist for a reason.",

                        "Pro tip: Use Firefox or Brave as your daily browser — both have strong built-in privacy protections. Add the uBlock Origin extension for extra ad and tracker blocking."
                    }
                },
                {
                    new[] { "malware", "virus", "ransomware", "spyware", "trojan", "worm", "keylogger" },
                    new List<string>
                    {
                        "Malware Protection:\n" +
                        "  🦠  Malware includes viruses, ransomware, spyware, trojans, and worms.\n" +
                        "  ✅  Install and regularly update reputable antivirus software.\n" +
                        "  ✅  Keep your operating system and all apps fully updated.\n" +
                        "  ✅  Back up your data regularly to an offline or cloud location.\n" +
                        "  ❌  Never open email attachments from unknown senders.\n" +
                        "  ❌  Avoid pirated software — it often contains hidden malware.\n" +
                        "  💡  Ransomware encrypts your files and demands payment — backups are your best defence.",

                        "Ransomware tip: The average ransom paid by South African organisations in 2023 exceeded R1 million. Regular offline backups are your single best protection — no backup, no recovery.",

                        "Free antivirus options: Windows Defender (built into Windows 10/11) is surprisingly effective. Keep it enabled and up to date!"
                    }
                },
                {
                    new[] { "social engineering", "manipulation", "pretexting", "baiting", "vishing", "smishing" },
                    new List<string>
                    {
                        "Social Engineering Awareness:\n" +
                        "  🧠  Social engineering exploits human psychology, not software.\n" +
                        "  ✅  Be sceptical of any unsolicited request for sensitive information.\n" +
                        "  ✅  Always verify a caller's identity through official channels before sharing anything.\n" +
                        "  ✅  Trust your instincts — if something feels wrong, it probably is.\n" +
                        "  ❌  No legitimate organisation will ever ask for your password.\n" +
                        "  💡  Vishing = voice phishing (phone calls). Smishing = SMS phishing. Both are common in SA.",

                        "Remember: Even a confident, friendly caller may be a scammer. Social engineers are trained to sound trustworthy. When in doubt, hang up and call back on the official number."
                    }
                },
                {
                    new[] { "2fa", "two factor", "two-factor", "mfa", "multi factor", "authenticator", "otp" },
                    new List<string>
                    {
                        "Two-Factor Authentication (2FA):\n" +
                        "  🔐  2FA adds a critical second layer of security beyond your password.\n" +
                        "  ✅  Use an authenticator app (e.g. Google Authenticator, Authy) — safer than SMS.\n" +
                        "  ✅  Enable 2FA on email, banking, social media, and cloud accounts.\n" +
                        "  ❌  Never share your OTP or 2FA code with anyone — ever.\n" +
                        "  💡  Even if a hacker steals your password, 2FA stops them from logging in.",

                        "Quick tip: App-based 2FA (Google Authenticator, Microsoft Authenticator) is more secure than SMS codes, which can be intercepted via SIM-swapping — a growing crime in South Africa."
                    }
                },
                {
                    new[] { "public wifi", "public wi-fi", "wifi", "wi-fi", "hotspot", "free wifi" },
                    new List<string>
                    {
                        "Public Wi-Fi Risks:\n" +
                        "  📶  Public Wi-Fi networks are often unsecured and easily intercepted.\n" +
                        "  ✅  Always use a VPN when connected to public Wi-Fi.\n" +
                        "  ✅  Avoid accessing banking, email, or sensitive accounts on public Wi-Fi.\n" +
                        "  ❌  Do not connect to networks with generic names like 'Free_WiFi'.\n" +
                        "  💡  Attackers can set up fake hotspots to steal your data — known as an 'Evil Twin' attack.",

                        "If you must use public Wi-Fi: Use a trusted VPN (ProtonVPN has a solid free tier), avoid logging into banking apps, and turn off auto-connect in your phone settings."
                    }
                },
                {
                    new[] { "backup", "backups", "data backup", "back up", "data loss" },
                    new List<string>
                    {
                        "Data Backup Best Practices:\n" +
                        "  💾  Regular backups protect you from ransomware, hardware failure, and accidents.\n" +
                        "  ✅  Follow the 3-2-1 rule: 3 copies, on 2 different media, 1 offsite/cloud.\n" +
                        "  ✅  Test your backups periodically to ensure they can actually be restored.\n" +
                        "  ✅  Use reputable cloud services (e.g. OneDrive, Google Drive, Backblaze).\n" +
                        "  ❌  Do not keep your only backup on the same device as the original files.",

                        "Backup tip: Windows 10/11 includes built-in backup via 'File History'. Set it up today — it's free and automatic!"
                    }
                },
                {
                    new[] { "privacy", "tracking", "data privacy", "personal data", "cookies", "gdpr", "popia" },
                    new List<string>
                    {
                        "Privacy and Data Protection:\n" +
                        "  🕵️  Your personal data is valuable — treat it that way.\n" +
                        "  ✅  Review app permissions and disable any that seem unnecessary.\n" +
                        "  ✅  Use a privacy-focused browser or browser extensions (e.g. uBlock Origin).\n" +
                        "  ✅  South Africa's POPIA law gives you rights over your personal data.\n" +
                        "  ❌  Avoid oversharing personal details on social media.\n" +
                        "  💡  Regularly review which apps have access to your camera, microphone, and location.",

                        "POPIA tip: Under South Africa's Protection of Personal Information Act, you can request that companies delete your data or correct inaccurate records. You have rights — use them!",

                        "Privacy quick-win: Go to your phone's Settings → Apps → Permissions and audit which apps can access your location, camera, and microphone. You may be surprised."
                    }
                },
                {
                    new[] { "scam", "fraud", "online fraud", "419", "advance fee", "romance scam", "investment scam" },
                    new List<string>
                    {
                        "Online Scams and Fraud:\n" +
                        "  💳  Online scams cost South Africans billions of rands each year.\n" +
                        "  ✅  If an offer sounds too good to be true, it almost certainly is.\n" +
                        "  ✅  Verify online sellers and use secure payment platforms.\n" +
                        "  ❌  Never transfer money to someone you have only met online.\n" +
                        "  ❌  Legitimate lotteries and competitions never ask for upfront fees.\n" +
                        "  💡  Report scams to the South African Police Service (SAPS) and the SAFPS.",

                        "Scam alert: The SAFPS (South African Fraud Prevention Service) runs a free fraud alert system. Register at safps.org.za to protect your identity.",

                        "Romance scam tip: Scammers often target people on dating apps and social media. If someone you've never met in person asks you for money — that is a scam, every time."
                    }
                }
            };
        }

        public void SetUserName(string name)
        {
            UserName = string.IsNullOrWhiteSpace(name) ? "User" : name.Trim();
        }

        public bool ValidateInput(string input, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "It looks like you didn't type anything. Please enter a question or type 'help'.";
                return false;
            }
            if (input.Trim().Length > 500)
            {
                errorMessage = "That message is a little long! Please keep your question under 500 characters.";
                return false;
            }
            errorMessage = null;
            return true;
        }

        public bool IsExitCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            string norm = input.ToLower().Trim();
            foreach (string w in ExitWords)
                if (norm == w) return true;
            return false;
        }

        // Task 4: Log an action with a timestamp, keeping only the last 10
        private void LogAction(string description)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] {description}";
            _activityLog.Add(entry);
            if (_activityLog.Count > 10)
                _activityLog.RemoveAt(0);
        }

        // Task 4: Return the last 5 activity log entries (default view)
        public string GetActivityLog()
        {
            if (_activityLog.Count == 0)
                return "📜 No actions recorded yet. Start chatting to build your activity log!";

            int    start  = Math.Max(0, _activityLog.Count - 5);
            string result = "📜 Recent Activity (last 5 actions):\n";
            int    num    = 1;
            for (int i = start; i < _activityLog.Count; i++)
            {
                result += $"  {num}. {_activityLog[i]}\n";
                num++;
            }
            if (_activityLog.Count > 5)
                result += "\n💡 Type 'show more log' to see your full activity history.";
            return result.TrimEnd();
        }

        // Task 4: Return all activity log entries (show more)
        public string GetFullActivityLog()
        {
            if (_activityLog.Count == 0)
                return "📜 No actions recorded yet. Start chatting to build your activity log!";

            string result = $"📜 Full Activity Log ({_activityLog.Count} actions):\n";
            for (int i = 0; i < _activityLog.Count; i++)
                result += $"  {i + 1}. {_activityLog[i]}\n";
            return result.TrimEnd();
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I didn't receive any input. Could you try typing something?";

            string norm = input.ToLower().Trim();

            // Task 2: Route to quiz handler if a quiz is in progress
            if (InQuiz)
                return ProcessQuizAnswer(norm);

            // Task 1: Route to reminder handler if awaiting reminder response
            if (AwaitingReminder)
                return ProcessReminderResponse(input);

            // Detect sentiment
            string sentimentPrefix = DetectSentiment(norm);

            // Task 4: Show more log
            foreach (string kw in NlpShowMoreLog)
            {
                if (norm.Contains(kw))
                {
                    LogAction("User viewed full activity log");
                    return GetFullActivityLog();
                }
            }

            // Task 4: Activity log command detection (NLP)
            foreach (string kw in NlpActivityLog)
            {
                if (norm.Contains(kw))
                {
                    LogAction("User viewed activity log");
                    return GetActivityLog();
                }
            }

            // Task 2: Quiz start detection (NLP)
            foreach (string kw in NlpStartQuiz)
            {
                if (norm.Contains(kw))
                    return StartQuiz();
            }

            // Task 1: Show tasks command (NLP)
            foreach (string kw in NlpShowTasks)
            {
                if (norm.Contains(kw))
                {
                    LogAction("User viewed task list");
                    return GetFormattedTaskList();
                }
            }

            // Task 1: Delete task command (NLP)
            foreach (string kw in NlpDeleteTask)
            {
                if (norm.Contains(kw))
                {
                    LogAction("User requested task deletion");
                    return "To delete a task, type: delete task [number]\nExample: 'delete task 2'\nType 'show tasks' to see your task numbers.";
                }
            }

            // Task 1: Complete task command (NLP)
            foreach (string kw in NlpCompleteTask)
            {
                if (norm.Contains(kw))
                {
                    LogAction("User requested task completion");
                    return "To mark a task as complete, type: complete task [number]\nExample: 'complete task 1'\nType 'show tasks' to see your task numbers.";
                }
            }

            // Task 1: Handle "delete task N" directly
            if (norm.StartsWith("delete task "))
            {
                string numPart = norm.Replace("delete task ", "").Trim();
                if (int.TryParse(numPart, out int delId))
                    return HandleDeleteTask(delId);
                return "Please specify the task number. Type 'show tasks' to see the list.";
            }

            // Task 1: Handle "complete task N" directly
            if (norm.StartsWith("complete task "))
            {
                string numPart = norm.Replace("complete task ", "").Trim();
                if (int.TryParse(numPart, out int compId))
                    return HandleCompleteTask(compId);
                return "Please specify the task number. Type 'show tasks' to see the list.";
            }

            // Task 3: NLP — "tell me about X" / "i want to learn about X" intent routing
            foreach (string kw in NlpTellMeAbout)
            {
                if (norm.Contains(kw))
                {
                    string subject = norm.Replace(kw, "").Trim();
                    string topicResponse = FindTopicResponse(subject);
                    if (topicResponse != null)
                    {
                        LogAction($"NLP intent matched: '{kw}' → topic: {subject}");
                        return sentimentPrefix + topicResponse;
                    }
                }
            }

            // Task 1 + Task 3: Add task detection (NLP — flexible phrasing)
            foreach (string kw in NlpAddTask)
            {
                if (norm.Contains(kw))
                {
                    string title = ExtractTaskTitle(input, kw);
                    return BeginAddTask(title);
                }
            }

            // Follow-up detection
            if (CurrentTopic != null && IsFollowUp(norm))
            {
                string followUpResponse = GetResponseForTopic(CurrentTopic);
                return sentimentPrefix + followUpResponse;
            }

            // Favourite topic storage
            if (norm.Contains("favourite topic") || norm.Contains("favorite topic") ||
                norm.Contains("my favourite") || norm.Contains("i like"))
            {
                _favouriteTopic = input;
                return sentimentPrefix +
                       $"Got it, {UserName}! I'll remember that your favourite topic is related to: \"{input}\". " +
                       "I'll keep that in mind to personalise our conversation. What would you like to know more about?";
            }

            // Keyword scan through response dictionary
            foreach (var entry in _responses)
            {
                foreach (string keyword in entry.Key)
                {
                    if (norm.Contains(keyword))
                    {
                        CurrentTopic = entry.Key[0];
                        string picked = entry.Value[_rng.Next(entry.Value.Count)];

                        if (_favouriteTopic != null && picked.Contains("Just ask away"))
                            picked += $" (I know you're particularly interested in {_favouriteTopic}!)";

                        LogAction($"User asked about: {CurrentTopic}");
                        return sentimentPrefix + picked;
                    }
                }
            }

            // Default response
            CurrentTopic = null;
            return sentimentPrefix +
                   $"I'm not sure I understood that, {UserName}. " +
                   "Try asking about a specific topic, or click one of the topic buttons above. " +
                   "Type 'help' to see everything I can cover.";
        }

        // Task 3: Find a topic response by scanning keywords — used for "tell me about X" NLP intent
        private string FindTopicResponse(string subject)
        {
            foreach (var entry in _responses)
            {
                foreach (string keyword in entry.Key)
                {
                    if (subject.Contains(keyword))
                    {
                        CurrentTopic = entry.Key[0];
                        return entry.Value[_rng.Next(entry.Value.Count)];
                    }
                }
            }
            return null;
        }

        // Task 1: Begin adding a task — store title and ask for reminder
        private string BeginAddTask(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                title = "Cybersecurity Task";

            _pendingTaskTitle       = title;
            _pendingTaskDescription = GetTaskDescription(title);
            AwaitingReminder        = true;

            LogAction($"Task started: \"{_pendingTaskTitle}\"");
            return $"📋 Task added: \"{_pendingTaskTitle}\"\n" +
                   $"Description: \"{_pendingTaskDescription}\"\n\n" +
                   "Would you like to set a reminder? If yes, say something like:\n" +
                   "  • \"Remind me in 3 days\"\n" +
                   "  • \"Remind me on 2025-12-01\"\n" +
                   "Or type 'no' to skip the reminder.";
        }

        // Task 1: Handle the reminder response after task creation
        private string ProcessReminderResponse(string input)
        {
            AwaitingReminder = false;
            string norm = input.ToLower().Trim();

            if (norm == "no" || norm == "no thanks" || norm == "skip" || norm == "none")
            {
                bool saved = DatabaseHelper.AddTask(_pendingTaskTitle, _pendingTaskDescription, "");
                LogAction($"Task saved without reminder: \"{_pendingTaskTitle}\"");
                _pendingTaskTitle       = null;
                _pendingTaskDescription = null;
                return saved
                    ? "✅ Got it! Task saved without a reminder. Type 'show tasks' to view all your tasks."
                    : "⚠ Task could not be saved to the database. Please check your database connection.";
            }

            string reminder  = input.Trim();
            bool   dbSaved   = DatabaseHelper.AddTask(_pendingTaskTitle, _pendingTaskDescription, reminder);
            string savedTitle = _pendingTaskTitle;
            LogAction($"Task saved with reminder \"{reminder}\": \"{savedTitle}\"");
            _pendingTaskTitle       = null;
            _pendingTaskDescription = null;
            return dbSaved
                ? $"✅ Got it! I'll remind you: {reminder}\nTask \"{savedTitle}\" has been saved. Type 'show tasks' to view all your tasks."
                : "⚠ Task could not be saved to the database. Please check your database connection.";
        }

        // Task 1: Generate a relevant description based on the task title keywords
        private string GetTaskDescription(string title)
        {
            string t = title.ToLower();
            if (t.Contains("password"))                                         return "Update or strengthen your passwords to protect your accounts.";
            if (t.Contains("2fa") || t.Contains("two factor") || t.Contains("authentication")) return "Enable two-factor authentication to add a second layer of security.";
            if (t.Contains("privacy") || t.Contains("settings"))                return "Review account privacy settings to ensure your data is protected.";
            if (t.Contains("backup"))                                            return "Set up or verify your data backup to prevent data loss.";
            if (t.Contains("antivirus") || t.Contains("malware"))               return "Install or update antivirus software to protect against malware.";
            if (t.Contains("vpn"))                                               return "Configure a VPN for secure browsing on public networks.";
            if (t.Contains("phishing"))                                          return "Learn to identify and avoid phishing emails and links.";
            return $"Complete the cybersecurity task: {title}.";
        }

        // Task 1: Retrieve and format all tasks from the database
        private string GetFormattedTaskList()
        {
            List<TaskItem> tasks = DatabaseHelper.GetAllTasks();
            if (tasks.Count == 0)
                return "📋 You have no tasks yet. Type 'add task [title]' to add one!";

            string result = "📋 Your Cybersecurity Tasks:\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                TaskItem t      = tasks[i];
                string   status = t.IsCompleted ? "✅" : "⬜";
                result += $"\n  {i + 1}. {status} {t.Title}\n";
                result += $"     📝 {t.Description}\n";
                if (!string.IsNullOrWhiteSpace(t.Reminder))
                    result += $"     ⏰ Reminder: {t.Reminder}\n";
            }
            result += "\nType 'delete task [number]' or 'complete task [number]' to manage tasks.";
            return result;
        }

        // Task 1: Delete a task by its list position (1-based)
        private string HandleDeleteTask(int listNumber)
        {
            List<TaskItem> tasks = DatabaseHelper.GetAllTasks();
            if (listNumber < 1 || listNumber > tasks.Count)
                return $"⚠ Task number {listNumber} not found. Type 'show tasks' to see your list.";

            string title   = tasks[listNumber - 1].Title;
            int    dbId    = tasks[listNumber - 1].Id;
            bool   deleted = DatabaseHelper.DeleteTask(dbId);
            if (deleted)
            {
                LogAction($"Task deleted: \"{title}\"");
                return $"🗑 Task {listNumber} (\"{title}\") has been deleted successfully.";
            }
            return "⚠ Could not delete the task. Please check your database connection.";
        }

        // Task 1: Mark a task complete by its list position (1-based)
        private string HandleCompleteTask(int listNumber)
        {
            List<TaskItem> tasks = DatabaseHelper.GetAllTasks();
            if (listNumber < 1 || listNumber > tasks.Count)
                return $"⚠ Task number {listNumber} not found. Type 'show tasks' to see your list.";

            string title     = tasks[listNumber - 1].Title;
            int    dbId      = tasks[listNumber - 1].Id;
            bool   completed = DatabaseHelper.MarkTaskComplete(dbId);
            if (completed)
            {
                LogAction($"Task marked complete: \"{title}\"");
                return $"✅ Task {listNumber} (\"{title}\") marked as completed. Well done, {UserName}!";
            }
            return "⚠ Could not update the task. Please check your database connection.";
        }

        // Task 3: Extract task title from flexible user input after the matched keyword
        private string ExtractTaskTitle(string input, string matchedKeyword)
        {
            string norm  = input.ToLower();
            int    index = norm.IndexOf(matchedKeyword);
            if (index < 0) return input.Trim();

            string after = input.Substring(index + matchedKeyword.Length).Trim();

            string[] connectors = { " to ", " -", ":", " about ", " for ", " - " };
            foreach (string c in connectors)
            {
                if (after.ToLower().StartsWith(c.Trim()))
                    after = after.Substring(c.Trim().Length).Trim();
            }

            return string.IsNullOrWhiteSpace(after) ? "Cybersecurity Task" : after;
        }

        // Task 2: Start the quiz
        public string StartQuiz()
        {
            InQuiz     = true;
            _quizIndex = 0;
            _quizScore = 0;
            LogAction("Quiz started");
            return "🎮 Welcome to the Cybersecurity Quiz!\n" +
                   $"You'll answer {_quizQuestions.Count} questions. Type A, B, C, or D to answer.\n" +
                   "Let's begin!\n\n" +
                   GetCurrentQuestion();
        }

        // Task 2: Get the current quiz question formatted
        private string GetCurrentQuestion()
        {
            if (_quizIndex >= _quizQuestions.Count)
                return FinishQuiz();

            QuizQuestion q      = _quizQuestions[_quizIndex];
            string       result = $"❓ Question {_quizIndex + 1} of {_quizQuestions.Count}:\n{q.Question}\n";
            foreach (string opt in q.Options)
                result += $"  {opt}\n";
            return result.TrimEnd();
        }

        // Task 2: Process the user's answer during the quiz — varied feedback
        private string ProcessQuizAnswer(string norm)
        {
            QuizQuestion q = _quizQuestions[_quizIndex];

            int answerIndex = -1;
            if (norm.StartsWith("a"))      answerIndex = 0;
            else if (norm.StartsWith("b")) answerIndex = 1;
            else if (norm.StartsWith("c")) answerIndex = 2;
            else if (norm.StartsWith("d")) answerIndex = 3;

            if (answerIndex < 0)
                return "Please answer with A, B, C, or D.\n\n" + GetCurrentQuestion();

            string feedback;
            if (answerIndex == q.CorrectIndex)
            {
                _quizScore++;
                feedback = CorrectFeedback[_rng.Next(CorrectFeedback.Length)] + "\n";
            }
            else
            {
                feedback = IncorrectFeedback[_rng.Next(IncorrectFeedback.Length)] +
                           $" The correct answer was: {q.Options[q.CorrectIndex]}\n";
            }

            _quizIndex++;

            if (_quizIndex >= _quizQuestions.Count)
                return feedback + "\n" + FinishQuiz();

            return feedback + "\n" + GetCurrentQuestion();
        }

        // Task 2: Finish the quiz and show results with grade
        private string FinishQuiz()
        {
            InQuiz = false;
            int    total = _quizQuestions.Count;
            double pct   = (double)_quizScore / total;
            string grade;

            if (pct == 1.0)       grade = "🏆 Perfect score! You're a cybersecurity expert!";
            else if (pct >= 0.75) grade = "🌟 Great job! You have strong cybersecurity knowledge.";
            else if (pct >= 0.50) grade = "👍 Good effort! Keep learning to stay secure.";
            else                  grade = "📚 Keep practising — cybersecurity knowledge is your best defence!";

            LogAction($"Quiz completed — Score: {_quizScore}/{total}");
            return $"🎮 Quiz Complete!\n" +
                   $"Your score: {_quizScore} / {total}\n" +
                   $"{grade}\n\n" +
                   "Type 'quiz' to play again, or ask me anything about cybersecurity!";
        }

        private string DetectSentiment(string norm)
        {
            foreach (string w in SentimentWorried)
                if (norm.Contains(w))
                    return $"I understand this can feel worrying, {UserName} — but knowledge is the best protection! ";

            foreach (string w in SentimentFrustrated)
                if (norm.Contains(w))
                    return $"Don't worry, {UserName} — cybersecurity can be complex, but let's break it down together. ";

            foreach (string w in SentimentCurious)
                if (norm.Contains(w))
                    return $"Great question, {UserName}! I love the curiosity — here's what you need to know: ";

            return string.Empty;
        }

        private bool IsFollowUp(string norm)
        {
            foreach (string w in FollowUp)
                if (norm.Contains(w)) return true;
            return false;
        }

        private string GetResponseForTopic(string topicKeyword)
        {
            foreach (var entry in _responses)
            {
                foreach (string kw in entry.Key)
                {
                    if (kw == topicKeyword)
                    {
                        return entry.Value[_rng.Next(entry.Value.Count)] +
                               "\n\n💬 Feel free to ask for even more details on this topic!";
                    }
                }
            }
            return "I don't have more details on that topic right now. Try asking about something else!";
        }
    }

    // Task 2: Quiz question model
    public class QuizQuestion
    {
        public string   Question     { get; }
        public string[] Options      { get; }
        public int      CorrectIndex { get; }

        public QuizQuestion(string question, string[] options, int correctIndex)
        {
            Question     = question;
            Options      = options;
            CorrectIndex = correctIndex;
        }
    }

    public class KeyArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y) => ReferenceEquals(x, y);
        public int GetHashCode(string[] obj) =>
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}
