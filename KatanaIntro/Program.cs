using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace KatanaIntro
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

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
            app.Use(async (env, next) =>
            {
                Console.WriteLine("Requesting : " + env.Request.Path);
                await next();
                Console.WriteLine("Respone: " + env.Response.StatusCode);
            });
            
            // This is is ugly code
            app.Use(async (environment, next) =>
            {
                var color = Console.ForegroundColor;
                foreach (var pair in environment.Environment)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("{0}: ", pair.Key);

                    Console.ForegroundColor = color;
                    if (pair.Key == "owin.ResponseStatusCode") {
                        if(pair.Value.ToString() == "200") 
                            Console.ForegroundColor = ConsoleColor.Green;
                        else if (pair.Value.ToString().StartsWith("5"))
                            Console.ForegroundColor = ConsoleColor.Red;
                        else
                            Console.ForegroundColor = ConsoleColor.Yellow;
                    }


                    var subs = pair.Value as IDictionary<string, object>;
                    if (subs != null)
                    {
                        foreach (var sub in subs)
                        {
                            Console.WriteLine("\t{0}:{1}", sub.Key, sub.Value);
                        }

                    }
                    else
                        Console.WriteLine("{0}", pair.Value);
                }
                Console.ForegroundColor = color;
                await next();
            });

            ConfigureWebApi(app);

            app.UseHelloWorld();
            //app.UseWelcomePage();
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
                );
            
            app.UseWebApi(config);
        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class HelloWorldComponent
    {
        private AppFunc _nextComponent;

        public HelloWorldComponent(AppFunc nextComponent)
        {
            _nextComponent = nextComponent;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            _nextComponent(environment);

            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Hello!!");
            }
            
        }
    }
}
