using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Datamodel;
using DataModel;
using DataModel.Interfaces;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Web.Common;
using Services.Interfaces;
using SteamApp;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof(NinjectWebCommon), "Stop")]

namespace SteamApp
{
    /// <summary>
    ///     Handles Dependency Injection using Ninject
    /// </summary>
    public class NinjectWebCommon : IDependencyResolver
    {
        /// <summary>
        ///     Initialises the Ninject container
        /// </summary>
        private static readonly Bootstrapper _bootstrapper = new Bootstrapper();

        private static StandardKernel _kernel;

        /// <summary>
        ///     Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>
        ///     The requested service or object.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return _kernel.Get(serviceType);
        }

        /// <summary>
        ///     Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>
        ///     The requested services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            _bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            _bootstrapper.ShutDown();
        }

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            // Prevent nullable parameters from being injected
            kernel.Settings.AllowNullInjection = true;

            RegisterServices(kernel);
            _kernel = kernel;
            return kernel;
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(x => x.FromAssemblyContaining<ISteamService>()
                .SelectAllClasses()
                .BindDefaultInterfaces());


            kernel.Rebind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            kernel.Bind<SteamAppEntities>().ToSelf().InRequestScope();
        }

        /// <summary>
        ///     Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <typeparam name="T">typeof service</typeparam>
        /// <returns>The requested service or object.</returns>
        public T GetService<T>()
        {
            return _kernel.Get<T>();
        }

        /// <summary>
        ///     Rebinds the specified instance.
        /// </summary>
        /// <typeparam name="T">typeof instance</typeparam>
        /// <param name="instance">instance to bind to</param>
        public void Rebind<T>(T instance)
        {
            _kernel.Rebind<T>().ToConstant(instance);
        }

        /// <summary>
        ///     Rebinds object creation to self instance
        /// </summary>
        /// <typeparam name="T">typeof object</typeparam>
        public void RebindToSelf<T>()
        {
            _kernel.Rebind<T>().ToSelf();
        }

        /// <summary>
        ///     Rebinds object creation to method that returns instance
        /// </summary>
        /// <typeparam name="T">typeof object</typeparam>
        /// <param name="func">function that returns instance</param>
        public void RebindToMethod<T>(Func<T> func)
        {
            _kernel.Rebind<T>().ToMethod(ctx => func());
        }
    }
}