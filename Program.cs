﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace dcrpt_miner
{
    public unsafe class Unmanaged
    {
        [DllImport("sha256_lib", ExactSpelling = true)]
        [SuppressGCTransition]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static extern void SHA256Ex(byte* buffer, byte* output);
    }

    class Program
    {
        public static int Shares;
        public static int AcceptedShares;
        public static int RejectedShares;
        public static object ConsoleLock = new object();

        private static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) => {
                    configuration.Sources.Clear();

                    configuration.AddJsonFile("config.json");
                    configuration.AddCommandLine(args);
                })
                .ConfigureServices(ConfigureServices)
                .Build();

            Console.Title = "dcrptd miner";
            await host.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddConnectionProvider();
            services.AddSingleton<Channels>();
            services.AddHostedService<WorkerManager>();
            services.AddHostedService<ConnectionManager>();
            services.AddHostedService<StatusManager>();
        }
    }
}