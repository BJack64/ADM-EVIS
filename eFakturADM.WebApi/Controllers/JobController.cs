using eFakturADM.DJPLib.Models;
using eFakturADM.DJPLib;
using eFakturADM.DJPService;
using eFakturADM.Web.Helpers;
using eFakturADM.WebApi.Attributes;
using Hangfire;
using System;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StructureMap.Query;
using eFakturADM.WebApi.Controllers.Base;
using System.IO;
using System.Web.Http.Results;
using System.Linq;
using eFakturADM.DJPLib.Objects;
using eFakturADM.Logic.Objects;
using System.Security.Policy;
using eFakturADM.Logic.Collections;
using eFakturADM.Logic.Core;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Hangfire.JobsLogger;
using eFakturADM.Shared.Utility;
using Microsoft.Extensions.Logging;
using Hangfire.Logging;
using Hangfire.Server;
using Swashbuckle.Swagger;
using System.Collections.Generic;


namespace eFakturADM.WebApi.Controllers
{
    /// <summary>
    /// The job to be processed asynchronously.
    /// </summary>
    /// <returns>
    /// A response indicating whether the Faktur Pajak was successfully added to the validation queue
    /// </returns>
    [RoutePrefix("api/job")]
    public class JobController : BaseApiController
    {
        /// <summary>
        /// Validates and processes the Faktur Pajak based on the provided URL and FPdjpID.
        /// The method schedules the Faktur validation to be processed asynchronously.
        /// </summary>
        /// <param name="url">The URL for the Faktur Pajak to be validated.</param>
        /// <param name="fpdjpID">The ID of the Faktur Pajak for validati  on.</param>
        /// <returns>
        /// A response indicating whether the Faktur Pajak was successfully added to the validation queue,
        /// or an error if the URL or FPdjpID is empty or invalid.
        /// </returns>
        [CustomAuthorize]
        [CustomExceptionFilter]
        [Log]
        [HttpGet]
        [Route("Validasi")]
        public IHttpActionResult ValidasiFakturPajak(string url, string fpdjpID)
        {
            if (!string.IsNullOrWhiteSpace(url) && url.ToLower() != "null" && !string.IsNullOrWhiteSpace(fpdjpID) && fpdjpID.ToLower() != "null" && IsValidEfakturUrl(url)) 
            {
                var userInitial = UserAuth(Request)?.UserInitial;
                var userName = UserAuth(Request)?.UserName;

                try
                {
                    var monitoringApi = JobStorage.Current.GetMonitoringApi();

                    // Mengecek pekerjaan yang sedang antri
                    var enqueuedJobs = monitoringApi.EnqueuedJobs("default", 0, int.MaxValue)
                        .Any(job => job.Value?.Job?.Args?.Contains(fpdjpID) == true);

                    // Mengecek pekerjaan yang sedang diproses
                    var processingJobs = monitoringApi.ProcessingJobs(0, int.MaxValue)
                        .Any(job => job.Value?.Job?.Args?.Contains(fpdjpID) == true);

                    // Jika pekerjaan dengan fpdjpID sudah ada di antrian atau sedang diproses
                    if (enqueuedJobs || processingJobs)
                    {
                        var duplicateResponse = new
                        {
                            Status = "Duplicate",
                            StatusCode = "409",
                            FPDJPID = fpdjpID,
                            URL = url,
                            Message = $"Faktur ID : {fpdjpID} sudah ada dalam antrian atau sedang dalam proses validasi."
                        };

                        return Content(HttpStatusCode.Conflict, duplicateResponse);
                    }

                    // Jika belum ada, tambahkan pekerjaan baru ke antrian
                    var faktur = BackgroundJob.Enqueue(() => ProcessFaktur(null,url, fpdjpID, userInitial, userName));


                    var successResponse = new
                    {
                        Status = "OK",
                        StatusCode = "200",
                        FPDJPID = fpdjpID,
                        URL = url,
                        Message = $"Faktur ID : {fpdjpID} berhasil ditambahkan ke dalam antrian untuk diproses validasi"
                    };

                    return Content(HttpStatusCode.OK, successResponse);
                }
                catch (Exception ex)
                {
                    var errorResponse = new
                    {
                        Status = "Error",
                        StatusCode = "500",
                        FPDJPID = fpdjpID,
                        URL = url,
                        Message = $"Terjadi kesalahan saat menambahkan faktur ID: {fpdjpID} ke dalam antrian. ExceptionMessage : {ex.Message}"
                    };

                    return Content(HttpStatusCode.InternalServerError, errorResponse);
                }
            }
            else
            {
                // Use Ok() or BadRequest with a serialized object or return directly the response object
                var errorResponse = new
                {
                    Status = "Error",
                    StatusCode = "400",
                    FPDJPID = fpdjpID,
                    URL = url,
                    Message = "URL dan FPdjpID tidak boleh kosong atau tidak valid`"
                };

                return Content(HttpStatusCode.BadRequest, errorResponse);
            }
        }
       
        /// <summary>
        /// Memvalidasi URL apakah sesuai dengan format http://efaktur.pajak.go.id/validasi/faktur/{id}/{kode}/{signature}
        /// </summary>
        /// <param name="url">URL yang akan divalidasi.</param>
        /// <returns>True jika valid, False jika tidak.</returns>
        public static bool IsValidEfakturUrl(string url)
        {
            if ((string.IsNullOrWhiteSpace(url)) || url.Trim().ToLower() == "null")
                return false;

            // Pola regex untuk memvalidasi URL
            string pattern = @"^http:\/\/svc\.efaktur\.pajak\.go\.id\/.*$";

            // Validasi menggunakan Regex
            return Regex.IsMatch(url, pattern);
        }

        //public bool GetFakturPajakFromDB(string url, out FakturPajak fakturDB)
        //{
        //    fakturDB = ValidateFakturLib.GetFakturByUrl(url);
        //    return true;
        //}

        //[AutomaticRetry(Attempts = 20)]

        /// <summary>
        /// Memproses faktur pajak dengan validasi data menggunakan API DJP dan mencatat log proses ke HangfireJobsLogger.
        /// </summary>
        /// <param name="context">
        /// Objek <see cref="PerformContext"/> yang menyediakan informasi tentang pekerjaan yang sedang berjalan di Hangfire.
        /// </param>
        /// <param name="url">URL endpoint yang digunakan untuk validasi data Faktur Pajak.</param>
        /// <param name="fpdjpID">ID unik Faktur Pajak yang sedang diproses.</param>
        /// <param name="userInitial">Inisial pengguna yang memulai pekerjaan ini.</param>
        /// <param name="userName">Nama lengkap pengguna yang memulai pekerjaan ini.</param>
        /// <returns>
        /// Sebuah <see cref="Task"/> yang mewakili operasi asynchronous.
        /// </returns>
        /// <remarks>
        /// Metode ini menggunakan konfigurasi dari <see cref="DjpServiceConfiguration"/> untuk menentukan pengaturan proxy, waktu tunggu (timeout),
        /// dan interval permintaan. Jika validasi gagal, pekerjaan akan diulang hingga batas maksimal retry tercapai.
        /// </remarks>
        /// <exception cref="WebException">
        /// Dilemparkan jika terjadi masalah koneksi atau API timeout selama proses validasi data Faktur Pajak.
        /// </exception>
        /// <exception cref="Exception">
        /// Dilemparkan jika terjadi kesalahan lain yang tidak ditangani selama pemrosesan.
        /// </exception>
        /// <example>
        /// Contoh penggunaan metode:
        /// <code>
        /// string url = "https://api.djp.example.com/faktur";
        /// string fpdjpID = "FP123456789";
        /// string userInitial = "JS";
        /// string userName = "John Smith";
        /// PerformContext context = // didapatkan dari Hangfire.
        /// await ProcessFaktur(context, url, fpdjpID, userInitial, userName);
        /// </code>
        /// </example>
        [DisplayName("[Validate] FP DJP ID : {2}")]
        [AutomaticRetry(Attempts = 2, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ProcessFaktur(PerformContext context, string url, string fpdjpID, string userInitial, string userName)
        {
            var jobId = context.BackgroundJob.Id;
            //var retryCount = context.GetJobParameter<int>("RetryCount");
            //string retryKey = $"RetryCount_{fpdjpID}";

            //// Ambil retry count dari parameter Hangfire
            //int retryCount = context.GetJobParameter<int?>(retryKey) ?? 1;
            //int retryCountHangfire = context?.GetJobParameter<int?>("RetryCount") ?? 0;
            //context.SetJobParameter("RetryCount", retryCountHangfire + 1);
            //var maxRetryCount = context.GetJobParameter<int>("MaxRetryCount");
            //int retryCount = retryCountHangfire + 1;
            int retryCountHangfire = context?.GetJobParameter<int?>("RetryCount") ?? 0;
            int retryCount = retryCountHangfire + 1;
            int maxRetryCount = 3;

            try
            {

                // Log job execution start menggunakan fpdjpID
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.None, "Job Started");
                //HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"Attempt {retryCount} of {maxRetryCount}");
                if ((retryCount) % maxRetryCount != 0)
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information,
                        $"Attempt {retryCount}");
                }
                else
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information,
                        $"Max retry attempt reached. No further retries.");
                }


                // Logika proses faktur
                bool isValid = true;
                string errMsg = string.Empty;

                #region Validation Empty Field
                if (isValid && string.IsNullOrEmpty(url))
                {
                    isValid = false;
                    errMsg = "Url is required.";
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Warning, $"Validation failed: {errMsg}");
                }
                #endregion

                if (isValid)
                {
                    WebExceptionStatus eStatus;
                    string msgError;
                    string logKey;

                    DjpServiceConfiguration.LoadConfig();

                    var inetProxy = DjpServiceConfiguration.InternetProxy;
                    var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
                    var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
                    var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
                    var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
                    bool isUseProxy = !string.IsNullOrEmpty(inetProxy);

                    ResValidateFakturPm objXml = new ResValidateFakturPm();

                    // Log URL and other parameters being used in the API request
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"Validating FakturPajak with fpdjpID: {fpdjpID}, url: {url}, user: {userInitial}");
                    var logMessage = new StringBuilder();

                    //logMessage.AppendLine("=== Log Faktur Validation ===\n");
                    //logMessage.AppendLine($"URL: {url}\n");
                    //logMessage.AppendLine($"User Initial: {userInitial}\n");
                    //logMessage.AppendLine($"User Name: {userName}\n");
                    logMessage.AppendLine($"[Timeout Setting]: {itimeoutsetting} ms\n");
                    logMessage.AppendLine($"[Use Proxy]: {isUseProxy}\n");
                    logMessage.AppendLine($"[Proxy Address]: {inetProxy}\n");
                    logMessage.AppendLine($"[Proxy Port]: {inetProxyPort}\n");
                    logMessage.AppendLine($"[Proxy Use Credential]: {inetProxyUseCredential}\n");

                    // Log informasi utama menggunakan HangfireJobsLogger
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug,
                        $"FakturPajak with fpdjpID: {fpdjpID} will validate using this config. \n{logMessage}");

                    FakturPajakResultModel result = ValidateFakturLib.ValidateFakturObjectAPIJob(url, userInitial, userName, fpdjpID,
                            itimeoutsetting, isUseProxy, inetProxy, inetProxyPort, inetProxyUseCredential, out msgError, out eStatus, out logKey);

             
                    if (!string.IsNullOrWhiteSpace(msgError))
                    {
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug, $"[Validate] Hit DJP Status : {eStatus} [Message] : {msgError}");

                    }
                    else
                    {
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug, $"[Validate] Hit DJP Status : {eStatus}");
                    }
                    if (eStatus == WebExceptionStatus.Success)
                    {
                        // Log successful validation
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"Faktur Pajak with fpdjpID: {fpdjpID} validated successfully.");
                        //await PostFakturAsync(jobId, result);
                        //Console.WriteLine("OK");
                        try
                        {
                            //await PostFakturAsync(jobId, result);
                            BackgroundJob.Enqueue(() => PostFakturAsync(null,fpdjpID, result));

                        }
                        catch (Exception ex)
                        {
                            HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Error,
                                $"Error in PostFakturAsync for fpdjpID: {fpdjpID}. Error: {ex.Message}");
                            // Cek jika ini adalah retry terakhir

                            //if (retryCount >= maxRetryCount)
                            //{
                            //    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Warning, 
                            //        $"Execute [Post failed job] after reaching maximum attempts. Attempt {retryCount} of {maxRetryCount}.");

                            //   await PostFailed(null, url, fpdjpID, userInitial, userName);

                            //    //BackgroundJob.Enqueue(() => PostFailed(null, url, fpdjpID, userInitial, userName));
                            //}

                            if ((retryCount) % maxRetryCount == 0)
                            {
                                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Warning,
                                    $"Execute [Post failed job] after reaching maximum attempts.");

                                await PostFailed(null, url, fpdjpID, userInitial, userName);

                                //BackgroundJob.Enqueue(() => PostFailed(null, url, fpdjpID, userInitial, userName));
                            }
                            throw ex;
                        }
                    }
                    else
                    {
                        // Log failure if status is not success
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Error, $"Failed to process  FP DJP ID {fpdjpID} because invalid data");
                        throw new Exception($"Failed to process faktur with ID {fpdjpID} because DJP timeout. {msgError}");
                    }
                }
            }
            catch (Exception ex)
            {
                retryCount++;

                // Simpan retry count ke dalam parameter Hangfire dengan key unik
                //context.SetJobParameter(retryKey, retryCount);
                // Log error details
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Error, $"Error processing FakturPajak with ID {fpdjpID}: {ex.Message}");

                // Logika jika terjadi kesalahan
                // Setelah 20 kali gagal, eksekusi PostFailed
                // Cek jika ini adalah retry terakhir
                //var retryCount = context.GetJobParameter<int>("RetryCount");
                //var maxRetryCount = context.GetJobParameter<int>("MaxRetryCount");
                if (retryCount >= maxRetryCount)
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Warning,
                        $"[Post failed job] after reaching maximum attempts.");

                    BackgroundJob.Enqueue(() => PostFailed(null, url, fpdjpID, userInitial, userName));
                }
                // Re-throw the exception
                throw ex;
            }
            finally
            {
                // Log job completion or final status
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"Job completed for FakturPajak ID: {fpdjpID}");
            }

        }

        /// <summary>
        /// Mengirimkan data hasil Faktur Pajak ke endpoint API tertentu menggunakan permintaan HTTP POST.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        /// <param name="fpdjpID"></param>
        /// <param name="userInitial"></param>
        /// <param name="userName"></param>
        /// <param name="jobId">ID unik dari pekerjaan Hangfire.</param>
        /// <param name="model">Objek <see cref="FakturPajakResultModel"/> yang berisi data Faktur Pajak yang akan dikirim.</param>
        /// <returns>Sebuah task yang merepresentasikan operasi asynchronous.</returns>
        /// <exception cref="Exception">
        /// Dilempar jika permintaan HTTP POST gagal atau API mengembalikan status kode yang bukan keberhasilan.
        /// </exception>
        /// <remarks>
        /// Metode ini bertanggung jawab untuk melakukan serialisasi data Faktur Pajak ke dalam format JSON,
        /// mengirimkannya ke endpoint API yang telah dikonfigurasi, dan mencatat detail permintaan serta responsnya.
        /// </remarks>
        /// <example>
        /// Contoh penggunaan:
        /// <code>
        /// var fakturModel = new FakturPajakResultModel
        /// {
        ///     FPdjpID = "12345",
        ///     StatusValidasi = "Valid",
        ///     KeteranganValidasi = "Validasi Berhasil"
        /// };
        /// await PostFakturAsync("job123", fakturModel);
        /// </code>
        /// </example>
        [DisplayName("[POST FailedAsync] FP DJP ID : {2}")]
        [AutomaticRetry(Attempts = 2, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public Task PostFailed(PerformContext context, string url, string fpdjpID, string userInitial, string userName)
        {
            var jobId = context.BackgroundJob.Id;
            int retryCountHangfire = context?.GetJobParameter<int?>("RetryCount") ?? 0;
            int retryCount = retryCountHangfire + 1;
            int maxRetryCount = 3;
            try
            {
                // Log job execution start menggunakan fpdjpID
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.None, "POST Failed started.");
                //HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"Attempt {retryCount} of {maxRetryCount}");
                if ((retryCount) % maxRetryCount != 0)
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information,
                        $"Attempt {retryCount}");
                }
                else
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information,
                        $"Max retry attempt reached. No further retries.");
                }
                FakturPajakResultModel result = new FakturPajakResultModel();
                result.FPdjpID = fpdjpID;

                // Simulate error handling logic (email notification example)
                // var mh = new MailHelper();
                // var logkey2 = "";
                // mh.DjpRequestErrorSendMail(out bool isErrorSendMail, url, logkey2);

                // Example condition for error handling
                result.StatusValidasi = "true";
                result.KeteranganValidasi = "Failed Get Data From DJP";
                result.DataFakturPajak = new List<DataFakturPajak>();

                // Log before posting result
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug, logMessage: $"[Posting failed] result to for FP DJP ID: {fpdjpID} with request : {result}");

                //await PostFakturAsync(jobId, result);
                BackgroundJob.Enqueue(() => PostFakturAsync(null, fpdjpID, result));


                // Log after posting result
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"[POST Failed] result processed for FP DJP ID: {fpdjpID}.");
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug, $"{ result}");

            }
            catch (Exception ex)
            {
                // Log error details
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Error, $"Error in PostFailed for FP DJP ID: {fpdjpID}. Error: {ex.Message}");
                throw;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Mengirimkan data hasil Faktur Pajak ke endpoint API yang ditentukan menggunakan permintaan HTTP POST.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobId">Identifikasi unik untuk pekerjaan Hangfire.</param>
        /// <param name="fpdjpID"></param>
        /// <param name="model"><see cref="FakturPajakResultModel"/> yang berisi data Faktur Pajak yang akan dikirim.</param>
        /// <returns>Sebuah task yang merepresentasikan operasi asynchronous.</returns>
        /// <exception cref="Exception">
        /// Dilempar jika permintaan HTTP POST gagal atau API mengembalikan status kode selain keberhasilan.
        /// </exception>
        /// <remarks>
        /// Metode ini bertanggung jawab untuk melakukan serialisasi data Faktur Pajak ke format JSON, 
        /// mengirimkannya ke endpoint API yang telah dikonfigurasi, dan mencatat detail permintaan serta respons.
        /// </remarks>
        /// <example>
        /// Contoh penggunaan:
        /// <code>
        /// var fakturModel = new FakturPajakResultModel
        /// {
        ///     FPdjpID = "12345",
        ///     StatusValidasi = "Valid",
        ///     KeteranganValidasi = "Validasi Berhasil"
        /// };
        /// await PostFakturAsync("job123", fakturModel);
        /// </code>
        /// </example>
        [DisplayName("[POST FakturAsync] FP DJP ID : {1}")]
        [AutomaticRetry(Attempts = 2, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task PostFakturAsync(PerformContext context, string fpdjpID, FakturPajakResultModel model)
        {
            var jobId = context.BackgroundJob.Id;
            //int retryCountHangfire = context?.GetJobParameter<int?>("RetryCount") ?? 0;
            //int retryCount = retryCountHangfire + 1;
            // Buat key unik untuk RetryCount berdasarkan fpdjpid
            //string retryKey = $"POSTFakturRetryCount_{fpdjpID}";

            //// Ambil retry count dari parameter Hangfire
            //int retryCount = context.GetJobParameter<int?>(retryKey) ?? 1;
            int retryCountHangfire = context?.GetJobParameter<int?>("RetryCount") ?? 0;
            int retryCount = retryCountHangfire + 1;
            int maxRetryCount = 3;
            try
            {
                // Log job execution start
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.None, "POST FakturAsync started.");
                //HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"Attempt {retryCount} of {maxRetryCount}");
                if ((retryCount) % maxRetryCount != 0)
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information,
                        $"Attempt {retryCount}");
                }
                else
                {
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information,
                        $"Max retry attempt reached. No further retries.");
                }

                string delviAPI = GeneralConfigs.GetById((int)ApplicationEnums.GeneralConfig.DelviAPIPostFaktur).ConfigValue.ToString();
                var url = delviAPI;

                using (var httpClient = new HttpClient())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.Indented);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Log request content
                    HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug, $"[Posting data] to {url} for FakturPajak ID: {model.FPdjpID}. \n [Request body]: \n{json}");

                    // Kirim POST request
                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Information, $"[POST] to {url} Success for FP DJP ID: {model.FPdjpID}. \n[Response] : \n{responseContent}");
                        Console.WriteLine("Response: " + responseContent);
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var errorMessage = $"Error POST to {url}: {response.StatusCode}";
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Error, errorMessage);
                        HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Debug, $"[Status Code] : {response.StatusCode}. \n [Response] : \n{responseContent}");

                        Console.WriteLine(errorMessage);
                        //retryCount++;

                        // Simpan retry count ke dalam parameter Hangfire dengan key unik
                        //context.SetJobParameter(retryKey, retryCount);
                        throw new Exception(errorMessage);
                    }
                }
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.None, "POSTFakturAsync finished.");

            }
            catch (Exception ex)
            {
                // Log error details
                HangfireJobsLogger.Log(jobId, Microsoft.Extensions.Logging.LogLevel.Error, $"Error in PostFakturAsync for FP DJP ID: {model.FPdjpID}. Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Menerima data Faktur Pajak dari permintaan HTTP, menyimpannya sebagai file JSON di sistem file, 
        /// dan mengembalikan respons HTTP sesuai hasil proses.
        /// </summary>
        /// <param name="model">
        /// Model <see cref="FakturPajakResultModel"/> yang berisi data Faktur Pajak untuk diproses. 
        /// Jika model null, metode ini akan mengembalikan respons <c>BadRequest</c>.
        /// </param>
        /// <returns>
        /// Sebuah task yang menghasilkan <see cref="IHttpActionResult"/> yang merepresentasikan respons HTTP:
        /// <list type="bullet">
        /// <item><description><c>Ok</c> jika data berhasil disimpan sebagai file JSON.</description></item>
        /// <item><description><c>BadRequest</c> jika data tidak valid atau terjadi kesalahan internal.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="Exception">
        /// Dilempar jika terjadi kesalahan saat menyimpan data ke file atau selama proses lainnya.
        /// </exception>
        /// <remarks>
        /// Metode ini melakukan langkah-langkah berikut:
        /// <list type="number">
        /// <item>Mengecek apakah data model valid (tidak null).</item>
        /// <item>Melakukan serialisasi data model ke format JSON menggunakan <see cref="JsonConvert.SerializeObject"/>.</item>
        /// <item>Menyimpan data JSON ke lokasi direktori "C:\DELVI" dengan nama file yang berisi ID Faktur Pajak dan timestamp saat ini.</item>
        /// <item>Mengembalikan status HTTP <c>Ok</c> jika berhasil, atau <c>BadRequest</c> jika terjadi kesalahan.</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// Contoh penggunaan:
        /// <code>
        /// var model = new FakturPajakResultModel
        /// {
        ///     FPdjpID = "12345",
        ///     StatusValidasi = "Valid",
        ///     KeteranganValidasi = "Data validasi berhasil",
        ///     DataFakturPajak = new List<DataFakturPajak>()
        /// };
        /// var result = await DelviAsync(model);
        [HttpPost]
        [Route("delvi")]
        public Task<IHttpActionResult> Delvi([FromBody] FakturPajakResultModel model)
        {
            if (model == null)
            {
                // Mengembalikan respons JSON untuk BadRequest
                return Task.FromResult<IHttpActionResult>(BadRequest("Invalid data."));
            }

            try
            {
                // Serialisasi model ke JSON
                //var json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var json = JsonConvert.SerializeObject(model, Formatting.Indented);


                // Tentukan path untuk menyimpan file JSON
                var filePath = Path.Combine("C:\\DELVI", $"Faktur_{model.FPdjpID}_{DateTime.Now:yyyyMMddHHmmss}.json");

                // Simpan JSON ke file
                System.IO.File.WriteAllText(filePath, json);

                // Mengembalikan respons JSON untuk Ok
                return Task.FromResult<IHttpActionResult>(Ok(new { message = "Faktur received and saved as JSON.", filePath }));
            }
            catch (Exception ex)
            {
                // Mengembalikan respons JSON untuk Internal Server Error
                return Task.FromResult<IHttpActionResult>(BadRequest($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Mengembalikan status berdasarkan kode yang diberikan.
        /// </summary>
        /// <param name="code">Kode status HTTP yang diinginkan.</param>
        /// <param name="model">Objek model faktur yang dikirimkan dalam request body.</param>
        /// <returns>Response JSON berisi status, pesan, timestamp, dan data model yang dikirimkan.</returns>
        [HttpPost]
        [Route("TestStatus/{code:int}")]
        public Task<IHttpActionResult> TestStatus(int code, [FromBody] FakturPajakResultModel model)
        {
            if (model == null)
            {
                return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.BadRequest, new
                {
                    status = 400,
                    message = "Bad Request - Model cannot be null",
                    timestamp = DateTime.UtcNow
                }));
            }

            var response = new
            {
                status = code,
                message = GetMessage(code),
                timestamp = DateTime.UtcNow,
                data = model // Mengembalikan model yang dikirimkan dalam response
            };

            switch (code)
            {
                case 200: return Task.FromResult<IHttpActionResult>(Ok(response));
                case 201: return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.Created, response));
                case 204: return Task.FromResult<IHttpActionResult>(StatusCode(HttpStatusCode.NoContent));
                case 400: return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.BadRequest, response));
                case 401: return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.Unauthorized, response));
                case 403: return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.Forbidden, response));
                case 404: return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.NotFound, response));
                case 500: return Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.InternalServerError, response));
                default: return Task.FromResult<IHttpActionResult>(Ok(response)); // Default ke 200 jika tidak ada dalam daftar
            }
        }

        private string GetMessage(int code)
        {
            switch (code)
            {
                case 200: return "OK - Success";
                case 201: return "Created - Resource successfully created";
                case 204: return "No Content - No data to return";
                case 400: return "Bad Request - Invalid request";
                case 401: return "Unauthorized - Authentication required";
                case 403: return "Forbidden - Access denied";
                case 404: return "Not Found - Resource not found";
                case 500: return "Internal Server Error - An unexpected error occurred";
                default: return "Success";
            }
        }

        #region Tidak Dipakai
        //private void SaveFakturToDatabase(string url, string fpdjpID, string userInitial, string userName, FakturPajak fakturDB, ResValidateFakturPm fakturDJP)
        //{
        //    Exception ex = null;
        //    ValidateFakturLib.SaveDetailTransaksiToDatabase(fakturDJP, url, out ex);
        //}

        /// <summary>
        /// Mengambil data Faktur Pajak dari DJP berdasarkan URL yang diberikan.
        /// </summary>
        /// <param name="url">URL endpoint yang digunakan untuk mendapatkan data Faktur Pajak.</param>
        /// <param name="fakturDJP">Objek output yang berisi data validasi Faktur Pajak.</param>
        /// <returns>
        /// Mengembalikan nilai <c>true</c> jika operasi berhasil, 
        /// meskipun terdapat kemungkinan pesan kesalahan atau status yang harus diperiksa.
        /// </returns>
        /// <remarks>
        /// Metode ini menggunakan konfigurasi dari <see cref="DjpServiceConfiguration"/> untuk menentukan pengaturan proxy, 
        /// waktu tunggu (timeout), dan interval permintaan.
        /// </remarks>
        /// <exception cref="WebException">
        /// Dilemparkan jika terjadi masalah koneksi selama proses pengambilan data.
        /// </exception>
        /// <example>
        /// Contoh penggunaan metode:
        /// <code>
        /// string url = "https://api.djp.example.com/faktur";
        /// ResValidateFakturPm faktur;
        /// bool result = GetFakturPajakFromDJP(url, out faktur);
        /// if (result)
        /// {
        ///     // Proses faktur sesuai kebutuhan
        /// }
        /// </code>
        /// </example>
        //public bool GetFakturPajakFromDJP(string url, out ResValidateFakturPm fakturDJP)
        //{
        //    WebExceptionStatus eStatus;
        //    string msgError;
        //    string logKey;

        //    DjpServiceConfiguration.LoadConfig();

        //    var inetProxy = DjpServiceConfiguration.InternetProxy;
        //    var inetProxyPort = DjpServiceConfiguration.InternetProxyPort;
        //    var inetProxyUseCredential = DjpServiceConfiguration.UseDefaultCredential;
        //    var itimeoutsetting = DjpServiceConfiguration.DJPRequestTimeOutSetting;
        //    var reqinterval = DjpServiceConfiguration.ServiceRequestDetailFakturPajakDjpRequestInterval;
        //    bool isUseProxy = !string.IsNullOrEmpty(inetProxy);
        //    //bool isUseProxy = false;
        //    fakturDJP = ValidateFakturLib.GetValidateFakturObjectV3(url, itimeoutsetting, isUseProxy, inetProxy, inetProxyPort
        //                    , inetProxyUseCredential, out msgError, out eStatus, out logKey);
        //    return true;
        //}
        #endregion Tidak Dipakai
    }
}
