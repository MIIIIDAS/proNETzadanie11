using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace proNETzadanie11
{
    public partial class MainForm : Form
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private EventLog eventLog;

        public MainForm()
        {
            InitializeComponent();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            eventLog = new EventLog("Application");
            eventLog.Source = "SystemMonitorApp";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float cpuUsage = cpuCounter.NextValue();
            float availableRAM = ramCounter.NextValue();
            cpuLabel.Text = $"CPU Usage: {cpuUsage}%";
            ramLabel.Text = $"Available RAM: {availableRAM} MB";
            CheckAndLogConditions(cpuUsage, availableRAM);
        }

        private void CheckAndLogConditions(float cpuUsage, float availableRAM)
        {
            if (cpuUsage > 90)
            {
                LogEvent($"High CPU Usage detected: {cpuUsage}%");
            }

            if (availableRAM < 100)
            {
                LogEvent($"Low Available RAM detected: {availableRAM} MB");
            }
        }

        private void LogEvent(string message)
        {
            eventLog.WriteEntry(message, EventLogEntryType.Warning);
        }

        private void btnSaveToLogFile_Click(object sender, EventArgs e)
        {
            string logFilePath = "system_monitor_log.txt";

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now}] CPU Usage: {cpuCounter.NextValue()}%");
                    writer.WriteLine($"[{DateTime.Now}] Available RAM: {ramCounter.NextValue()} MB");
                    writer.WriteLine();
                }

                MessageBox.Show($"Dane zapisano do pliku: {logFilePath}", "Zapisano do pliku", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisu do pliku: {ex.Message}", "Błąd zapisu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
