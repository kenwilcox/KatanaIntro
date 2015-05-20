using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace KatanaIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = "http://localhost:8080";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Starting...");
                Console.WriteLine(uri);
                Console.ReadKey();
                Console.WriteLine("Stopping...");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWelcomePage();
//             app.Run(ctx =>
//             {
//                 return ctx.Response.WriteAsync("Hello World!");
//             });
        }
    }
}
