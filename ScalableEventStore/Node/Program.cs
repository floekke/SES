using Serilog;
using Topshelf;

namespace Node
{
    class Program
    {
        static void Main(string[] args)
        {
            SetUpLogging();

            HostFactory.Run(x =>                                
            {
                x.Service<NodeRunner>(s =>                        
                {
                  
                    s.ConstructUsing(name => new NodeRunner(int.Parse(name.InstanceName)));     
                    s.WhenStarted(tc => tc.Start());              
                    s.WhenStopped(tc => tc.Stop());       
                    
                });
                x.UseSerilog();

                x.RunAsLocalSystem();                            
                x.SetServiceName("Event Store Node");                    
            });

        }

        static void SetUpLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }
    }
}
