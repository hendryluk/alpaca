﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using Cormo.Catch;
using Cormo.Contexts;
using Cormo.Data.EntityFramework.Api.Events;
using Cormo.Injects;
using Cormo.Interceptions;
using Cormo.Web.Api;
using Owin;

[assembly:EnableHttpSessionState]

namespace SampleWebApp
{
    [RestController]
    public class MyController
    // Inheriting ApiController or IHttpController is optional. Cormo.Web will inject that for you.
    // This promotes DI principle and lightweight components.
    {
        [Inject] UpperCaseGreeter _stringService;                // -> Resolves to UpperCaseGreeter
        [Inject] IInstance<CountGreeter> _countServiceInstance;                // -> Resolves to UpperCaseGreeter
        [Inject] IGreeter<IEnumerable<int>> _integersService;     // -> Resolves to EnumerableGreeter<int>
        [Inject] private IEvents<string> _someEvents;
            
        [Route("test/{id}"), HttpGet]
        public string TestWithId(HttpRequestMessage msg)
        {
            return _stringService.Greet("World") + "/" + GetHashCode();
        }

        [Route("test"), HttpGet, HttpStatusCode(HttpStatusCode.Accepted)]
        public virtual string Test(HttpRequestMessage msg)
        {
            _someEvents.Fire("Hello");

            //throw new Exception("xxx");

            var x = "";
            for (var i = 0; i < 10; i++)
            {
                x += _countServiceInstance.Value.Greet("World");
            }
            //return _stringService.Greet("World");
            //return _stringService.Greet("World") + "/" + GetHashCode();
            return x;
        }

        

        [Route("testMany"), HttpGet]
        public string TestMany([Value(Default = 100)]int aaa)
        {
            return _integersService.Greet(new[] { 1, 2, 3, 4, 5 });
        }
    }

    // ============= SERVICES BELOW ===============
    public interface IGreeter<T>
    {
        [Logged]
        string Greet(T val);
    }

    [Value("limit", Default = 50)]
    public class LimitAttribute : StereotypeAttribute
    {
    }

    public class LoggedAttribute : InterceptorBindingAttribute
    {
    }

    [Interceptor, Logged]
    public class LoggingInterceptor: IAroundInvokeInterceptor
    {
        public async Task<object> AroundInvoke(IInvocationContext invocationContext)
        {
            return "abc" + await invocationContext.Proceed();
        }
    }

    public class Haha
    {
        [Inject] private UpperCaseGreeter _greeter;
    }

    public class SheepAttribute : QualifierAttribute
    {
        
    }

    public class HendersException : Exception
    {

    }

    public class HendersHandler
    {
        [HttpStatusCode(HttpStatusCode.Unauthorized)]
        public virtual void OnHenders([Handles] ICaughtException<HendersException> e)
        {
        }

        [HttpStatusCode(HttpStatusCode.Unauthorized)]
        public void OnHenders(
            [Handles] ICaughtException<HendersException> e,
            [CatchResource]HttpResponseMessage message)
        {
            message.Headers.RetryAfter = new RetryConditionHeaderValue(DateTime.Now.AddMinutes(5));
        }
    }

    //[RequestScoped]
    public class CountGreeter : IGreeter<string>
    {
        private static int i=0;
        private int _index;
        public CountGreeter()
        {
            _index = i++;
        }

        public string Greet(string val)
        {
            return val + " " + i;
        }
    }

    public class UpperCaseGreeter : IGreeter<string>, IDisposable
    {
        [Inject] private Haha _haha;
        [Inject, HeaderParam] string Accept;
        [Inject] IDbSet<Person> _persons;
        [Inject] private IPrincipal _principal;

        //[Inject, RouteParam]
        private int id;

        [Inject, Limit] private int xxxx;
        
        //[ExceptionsHandled]
        public virtual string Greet(string val)
        {
            //ExceptionsHandled.Throw(new HendersException(), new SheepAttribute());
            return string.Format("Hello {0} ({1}). Count: {2}. Accept: {3}", val.ToUpper(), 
                _principal.Identity,
                _persons.Count(), 
                Accept) + "/" + id;
        }

        public void OnHello([Observes] string str )
        {
            Debug.WriteLine("Hello " + str);
        }

        [Logged]
        public virtual string Hello
        {
            get
            {
                return "Hahaha";
            }
        }

        public void Dispose()
        {
            // Clear some resources here
            Debug.WriteLine("Disposed EnumerableeGreeter: " + this);
        }
    }

    public class BlahAttribute : QualifierAttribute
    {
    }

    [FromBody]
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    [SessionScoped]
    public class EnumerableeGreeter<T>: IGreeter<IEnumerable<T>>
    {
        public string Greet(IEnumerable<T> vals)
        {
            return string.Format("Hello many {0} ({1})", string.Join(",", vals), GetHashCode());
        }
    }

    [Configuration, Singleton]
    public class MyConfig
    {
        [Inject]
        void GetConfig(HttpConfiguration config, IAppBuilder builder)
        {
            config.Routes.MapHttpRoute("api", "api/{controller}");
            builder.UseWebApi(config);
        }
    }
}