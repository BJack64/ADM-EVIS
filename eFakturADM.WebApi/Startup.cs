using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System;
using System.Web.Http;
using System.Configuration;
using Hangfire.SqlServer;
using Hangfire.MemoryStorage;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using Hangfire.Dashboard;
using System.Text;
using Hangfire.JobsLogger;

[assembly: OwinStartup(typeof(eFakturADM.WebApi.Startup))]

namespace eFakturADM.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                Console.WriteLine("Memulai konfigurasi aplikasi...");

                // Membaca koneksi string
                string connectionString = ConfigurationManager.AppSettings["eFakturADM.Connection.String"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'eFakturADM.Connection.String' tidak ditemukan atau kosong.");
                }
                Console.WriteLine($"Connection string berhasil dibaca: {connectionString}");

                // Konfigurasi Hangfire
                Console.WriteLine("Mengkonfigurasi Hangfire...");
                Hangfire.GlobalConfiguration.Configuration
                    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        SchemaName = "EVIS",
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(15),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(15),
                        QueuePollInterval = TimeSpan.FromSeconds(30),
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                        CountersAggregateInterval = TimeSpan.FromMinutes(1),
                        JobExpirationCheckInterval = TimeSpan.FromMinutes(5),
                    })
                    .WithJobExpirationTimeout(TimeSpan.FromDays(14))
                    .UseColouredConsoleLogProvider()
                    .UseJobsLogger();

                Console.WriteLine("Konfigurasi Hangfire selesai.");

                // Menentukan jumlah worker
                Console.WriteLine("Membaca konfigurasi worker...");
                int workerCount = int.TryParse(
                    GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.WorkerCount)?.ConfigValue,
                    out var result) ? result : 5;

                Console.WriteLine($"Jumlah worker: {workerCount}");

                var options = new BackgroundJobServerOptions
                {
                    WorkerCount = Environment.ProcessorCount * workerCount,
                    ServerName = "EVIS",
                    ServerTimeout = TimeSpan.FromMinutes(5),
                    ShutdownTimeout = TimeSpan.FromMinutes(5),
                    HeartbeatInterval = TimeSpan.FromSeconds(15)
                };

                Console.WriteLine("Mengkonfigurasi dashboard Hangfire...");
                // Menambahkan otentikasi untuk dashboard Hangfire
                var dashboardOptions = new DashboardOptions
                {
                    DashboardTitle = "Jobs Dashboard",
                    Authorization = new[]
                    {
                new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    SslRedirect = false,
                    RequireSsl = false,
                    LoginCaseSensitive = true,
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = "admin",
                            PasswordClear = "password"
                        }
                    }
                })
            }
                };

                app.UseHangfireDashboard("/dashboard", dashboardOptions);
                app.UseHangfireServer(options);

                Console.WriteLine("Konfigurasi Hangfire server selesai.");

                // Menjadwalkan ulang job yang gagal
                Console.WriteLine("Menjadwalkan ulang semua job yang gagal...");
                RequeueAllFailedJobs();
                Console.WriteLine("Semua job yang gagal telah dijadwalkan ulang.");

                // Konfigurasi Web API
                Console.WriteLine("Mengkonfigurasi Web API...");
                HttpConfiguration config = new HttpConfiguration();
                WebApiConfig.Register(config);
                app.UseWebApi(config);
                Console.WriteLine("Konfigurasi Web API selesai.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Kesalahan selama konfigurasi: {ex.Message}");
                Console.Error.WriteLine($"Detail stack trace: {ex.StackTrace}");
                throw; // Jangan lupa melempar ulang exception untuk investigasi lebih lanjut
            }
        }

        // Anda bisa menambahkan konfigurasi lain di sini
        // Misalnya konfigurasi autentikasi
        private void ConfigureAuth(IAppBuilder app)
        {
            // Konfigurasi autentikasi Anda di sini
            // Contoh: app.UseOAuthAuthorizationServer(); dll.
        }

        private void RequeueAllFailedJobs()
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                int offset = 0;
                var failedJobs = monitoringApi.FailedJobs(offset, 100); // Ambil job gagal dalam batch

                while (failedJobs.Count > 0)
                {
                    foreach (var failedJob in failedJobs)
                    {
                        BackgroundJob.Requeue(failedJob.Key); // Menjadwalkan ulang setiap job
                        Console.WriteLine($"Job {failedJob.Key} dijadwalkan ulang.");
                    }

                    // Update offset untuk mengambil batch berikutnya
                    offset += 100;
                    failedJobs = monitoringApi.FailedJobs(offset, 100); // Ambil batch berikutnya
                }

                Console.WriteLine("Semua job gagal telah dijadwalkan ulang.");
            }
        }

    }

}
