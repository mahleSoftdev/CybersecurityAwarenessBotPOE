using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Chatbot
{
    public partial class MainWindow : Window
    {
        private static readonly SolidColorBrush BrushPrimary   = new SolidColorBrush(Color.FromRgb(0,   255, 255));
        private static readonly SolidColorBrush BrushAccent    = new SolidColorBrush(Color.FromRgb(0,   255, 127));
        private static readonly SolidColorBrush BrushWarning   = new SolidColorBrush(Color.FromRgb(255, 215,   0));
        private static readonly SolidColorBrush BrushSubtle    = new SolidColorBrush(Color.FromRgb(184, 134,  11));
        private static readonly SolidColorBrush BrushBody      = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        private static readonly SolidColorBrush BrushError     = new SolidColorBrush(Color.FromRgb(255,  68,  68));
        private static readonly SolidColorBrush BrushUserBg    = new SolidColorBrush(Color.FromRgb( 28,  33,  40));
        private static readonly SolidColorBrush BrushBotBg     = new SolidColorBrush(Color.FromRgb( 22,  27,  34));
        private static readonly SolidColorBrush BrushBorder    = new SolidColorBrush(Color.FromRgb( 48,  54,  61));

        private ChatBotEngine _bot = new ChatBotEngine();
        private bool _chatStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitialiseDatabase();
            NameBox.Focus();
        }

     
        private void NameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) StartChat_Click(null, null);
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "User";

            _bot.SetUserName(name);
            _chatStarted = true;

            // Hide name panel
            NamePanel.Visibility = Visibility.Collapsed;

            // Enable input
            InputBox.IsEnabled  = true;
            SendButton.IsEnabled = true;
            InputBox.Focus();

            // Play greeting WAV (non-blocking; gracefully skips if not found)
            PlayGreeting();

            // Welcome messages
            AppendDivider(heavy: true);
            AppendBotMessage($"Welcome, {name}! Great to have you here. " +
                             "I'm your Cybersecurity Awareness Assistant, and I'm here to help you stay safe online.");
            AppendBotMessage($"Think of me as your personal digital bodyguard, {name}. " +
                             "You can ask me about passwords, phishing, malware, privacy, and much more. " +
                             "Click a topic chip below, or just type your question!");
            AppendDivider();

            StatusBar.Text = $"Chatting as {name}  •  Type a question or click a topic chip.";
            
        }



        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(InputBox.Text))
                ProcessInput(InputBox.Text.Trim());
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputBox.Text))
                ProcessInput(InputBox.Text.Trim());
        }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            if (!_chatStarted) return;
            string tag = ((Button)sender).Tag?.ToString() ?? "";
            ProcessInput(tag);
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SendButton.IsEnabled = _chatStarted && !string.IsNullOrWhiteSpace(InputBox.Text);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
        }

        private void ProcessInput(string rawInput)
        {
            // Validate
            if (!_bot.ValidateInput(rawInput, out string validationError))
            {
                AppendWarningMessage(validationError);
                return;
            }

            // Echo user message
            AppendUserMessage(rawInput);
            InputBox.Clear();

            // Check for exit
            if (_bot.IsExitCommand(rawInput))
            {
                AppendBotMessage($"It was a pleasure helping you, {_bot.UserName}! " +
                                 "Stay safe and keep your digital life secure. Goodbye!");
                AppendGoodbyeBanner();
                InputBox.IsEnabled   = false;
                SendButton.IsEnabled = false;
                StatusBar.Text = "Session ended. Stay safe online! 🔒";
                return;
            }

            // Get and display response
            string response = _bot.GetResponse(rawInput);
            AppendBotMessage(response);
            AppendDivider();

            // Update topic label
            TopicLabel.Text = _bot.CurrentTopic != null
                ? $"Topic: {_bot.CurrentTopic}"
                : string.Empty;

            // Update status bar to reflect current bot state
            if (_bot.InQuiz)
                StatusBar.Text = "🎮 Quiz in progress — type A, B, C, or D to answer.";
            else if (_bot.AwaitingReminder)
                StatusBar.Text = "⏰ Awaiting reminder — type a date/timeframe or 'no' to skip.";
            else
                StatusBar.Text = $"Chatting as {_bot.UserName}  •  Type a question or click a topic chip.";

            ScrollToBottom();
        }

        private void AppendUserMessage(string text)
        {
            Border bubble = CreateBubble(
                prefix: $"👤 [{_bot.UserName}]",
                prefixBrush: BrushPrimary,
                text: text,
                textBrush: BrushBody,
                bgBrush: BrushUserBg,
                alignRight: true);
            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        private void AppendBotMessage(string text)
        {
            Border bubble = CreateBubble(
                prefix: "🤖 [BOT]",
                prefixBrush: BrushAccent,
                text: text,
                textBrush: BrushBody,
                bgBrush: BrushBotBg,
                alignRight: false);
            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        private void AppendWarningMessage(string text)
        {
            Border bubble = CreateBubble(
                prefix: "⚠  [BOT]",
                prefixBrush: BrushWarning,
                text: text,
                textBrush: BrushWarning,
                bgBrush: BrushBotBg,
                alignRight: false);
            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        private void AppendGoodbyeBanner()
        {
            Border banner = new Border
            {
                Background      = BrushBotBg,
                BorderBrush     = BrushPrimary,
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Padding         = new Thickness(14, 10, 14, 10),
                Margin          = new Thickness(4, 6, 4, 6)
            };
            StackPanel sp = new StackPanel();
            sp.Children.Add(MakeRun("═══════════════════════════════════════════", BrushPrimary, bold: false));
            sp.Children.Add(MakeRun("  Thank you for using the Cybersecurity Awareness Bot!", BrushWarning, bold: true));
            sp.Children.Add(MakeRun("  Stay vigilant — your security starts with awareness.", BrushSubtle, bold: false));
            sp.Children.Add(MakeRun("═══════════════════════════════════════════", BrushPrimary, bold: false));
            banner.Child = sp;
            ChatPanel.Children.Add(banner);
            ScrollToBottom();
        }

        private void AppendDivider(bool heavy = false)
        {
            TextBlock tb = new TextBlock
            {
                Text       = heavy
                    ? new string('═', 72)
                    : new string('─', 72),
                FontFamily = new FontFamily("Consolas"),
                FontSize   = 9,
                Foreground = heavy ? BrushPrimary : BrushBorder,
                Margin     = new Thickness(4, 2, 4, 2)
            };
            ChatPanel.Children.Add(tb);
        }

        private Border CreateBubble(string prefix, SolidColorBrush prefixBrush,
                                    string text, SolidColorBrush textBrush,
                                    SolidColorBrush bgBrush, bool alignRight)
        {
            StackPanel sp = new StackPanel { Orientation = Orientation.Vertical };

            // Prefix label
            TextBlock prefixTb = new TextBlock
            {
                Text       = prefix,
                FontFamily = new FontFamily("Consolas"),
                FontSize   = 11,
                FontWeight = FontWeights.Bold,
                Foreground = prefixBrush,
                Margin     = new Thickness(0, 0, 0, 3)
            };
            sp.Children.Add(prefixTb);

            //AI Assited 
            // Message body — support multi-line 
            foreach (string line in text.Split('\n'))
            {
                TextBlock lineTb = new TextBlock
                {
                    Text            = line,
                    FontFamily      = new FontFamily("Consolas"),
                    FontSize        = 12,
                    Foreground      = textBrush,
                    TextWrapping    = TextWrapping.Wrap
                };
                sp.Children.Add(lineTb);
            }

            Border bubble = new Border
            {
                Background      = bgBrush,
                BorderBrush     = BrushBorder,
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Padding         = new Thickness(12, 8, 12, 8),
                Margin          = alignRight
                    ? new Thickness(60, 4, 4, 4)
                    : new Thickness(4, 4, 60, 4),
                Child           = sp,
                HorizontalAlignment = alignRight
                    ? HorizontalAlignment.Right
                    : HorizontalAlignment.Left
            };
            return bubble;
        }

        // Simple helper for goodbye banner lines
        private TextBlock MakeRun(string text, SolidColorBrush brush, bool bold)
        {
            return new TextBlock
            {
                Text       = text,
                FontFamily = new FontFamily("Consolas"),
                FontSize   = 12,
                Foreground = brush,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                Margin     = new Thickness(0, 1, 0, 1)
            };
        }

        private void ScrollToBottom()
        {
            ChatScroller.ScrollToEnd();
        }

       
        //AI Asssited 
        private void PlayGreeting()
        {
            try
            {
                // Look for Greeting.wav next to the executable
                string path = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "C:\\Users\\asema\\source\\repos\\Chatbot\\Chatbot\\Greeting.wav");

                if (!System.IO.File.Exists(path)) return;

                SoundPlayer player = new SoundPlayer(path);
                player.Load();
                player.Play();   
            }
            catch { }
        }
    }
}
