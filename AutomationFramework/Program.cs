namespace AutomationFramework
{
    // This serves as 'Main' in the diagram (see the issue)
    // https://github.com/NikoBK/qc-automation-framework/issues/3
    class Program
    {
        public static void Main(string[] args) {
            Console.WriteLine("Starting MES...");
            var mes = new MES();
            mes.Start();
        }
    }
}
