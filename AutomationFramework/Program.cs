namespace automationframework
{
    class Program
    {
        public static void Main(string[] args) {
            Console.WriteLine("Starting MES...");
            var mes = new MES();
            mes.Start();
        }
    }
}
