using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework
{
    public class Logger
    {
        private readonly string logPath;

        public Logger(string moduleName)
        {
            string moduleDir = Path.Combine("Logs", moduleName);
            Directory.CreateDirectory(moduleDir);

            logPath = Path.Combine(moduleDir, $"{moduleName}_logs");
        }

        public void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[{timestamp}] {message}";

            File.AppendAllText(logPath, logEntry + Environment.NewLine);
        }
    }
}
