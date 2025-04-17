using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eFakturADM.Logic.Objects;
using eFakturADM.Logic.Collections;
using eFakturADM.Web.Models;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using eFakturADM.Logic.Core;
using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml.Style;
using System.IO;
using System.Text;
using PdfiumViewer;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Globalization;
using DocumentFormat.OpenXml.Drawing;
using eFakturADM.Logic.Utilities;
using DocumentFormat.OpenXml.Vml.Office;
using System.Web.Configuration;

namespace eFakturADM.Web.Controllers
{
    public class CoretaxController : BaseController
    {
        [AuthActivity("47")]
        public ActionResult ListFakturPajak()
        {
            return View();
        }


        //[HttpGet]

        //public JsonResult GetListFakturPajak()
        //{
        //    List<FakturPajakFromCoretax> fakturPajakFromCoretaxes = FakturPajakFromCoretaxes.GetFakturPajakFromCoretax();

        //    // Get total records
        //    var recordsTotal = fakturPajakFromCoretaxes.Count();
        //    var recordsFiltered = recordsTotal;

        //    // Apply pagination (start and length values)
        //    var start = Convert.ToInt32(Request.QueryString["start"]);
        //    var length = Convert.ToInt32(Request.QueryString["length"]);
        //    var pagedData = fakturPajakFromCoretaxes.Skip(start).Take(length).ToList();

        //    // Return the data in the correct format for DataTables
        //    return Json(new
        //    {
        //        draw = Request.QueryString["draw"].FirstOrDefault(),
        //        recordsTotal = recordsTotal,
        //        recordsFiltered = recordsFiltered,
        //        data = pagedData
        //    }, JsonRequestBehavior.AllowGet);

        //    //return Json(new
        //    //{
        //    //    aaData = fakturPajakFromCoretaxes
        //    //},
        //    //JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult GetListFakturPajak(string namaPenjual = null, string npwpPenjual = null, string nomorFakturStart = null, string nomorFakturEnd = null, DateTime? tanggalStart = null, DateTime? tanggalEnd = null, int? masaPajak = null, int? tahun = null, string statusFaktur = null)
        {
            try
            {
                // Simulasi mendapatkan data dari sumber
                List<FakturPajakFromCoretax> fakturPajakFromCoretaxes = FakturPajakFromCoretaxes.GetFakturPajakFromCoretax();

                // Filter data berdasarkan parameter yang diterima
                var filteredFaktur = fakturPajakFromCoretaxes.Where(f =>
                    (string.IsNullOrEmpty(namaPenjual) || f.Nama_Penjual.ToLower().Contains(namaPenjual.ToLower())) &&
                    (string.IsNullOrEmpty(npwpPenjual) || f.NPWP_Penjual.ToLower().Contains(npwpPenjual.ToLower())) &&
                    (string.IsNullOrEmpty(nomorFakturStart) || string.IsNullOrEmpty(nomorFakturEnd) ||
                        (string.Compare(f.Nomor_Faktur_Pajak, nomorFakturStart) >= 0 &&
                         string.Compare(f.Nomor_Faktur_Pajak, nomorFakturEnd) <= 0)) &&
                    (!tanggalStart.HasValue || !tanggalEnd.HasValue ||
                        (f.Tanggal_Faktur_Pajak.Date >= tanggalStart.Value.Date &&
                         f.Tanggal_Faktur_Pajak.Date <= tanggalEnd.Value.Date)) &&
                    (!masaPajak.HasValue || f.Masa_Pajak == masaPajak) &&
                    (!tahun.HasValue || f.Tahun == tahun.Value) &&
                    (string.IsNullOrEmpty(statusFaktur) || f.Status_Faktur.ToLower().Contains(statusFaktur.ToLower()))
                ).ToList();

                // Get total records
                int recordsTotal = fakturPajakFromCoretaxes.Count;
                int recordsFiltered = filteredFaktur.Count;

                // Ambil nilai start dan length dari query string untuk paginasi
                int start = 0;
                int length = 10;
                int.TryParse(Request.QueryString["start"], out start);
                int.TryParse(Request.QueryString["length"], out length);

                // Apply pagination
                var pagedData = filteredFaktur.Skip(start).Take(length).ToList();

                // Return the data in the correct format for DataTables
                return Json(new
                {
                    draw = Request.QueryString["draw"] ?? "1", // Pastikan draw selalu ada
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = pagedData ?? new List<FakturPajakFromCoretax>(), // Jaminan array kosong jika tidak ada data
                    excel = filteredFaktur ?? new List<FakturPajakFromCoretax>() // Jaminan array kosong jika tidak ada data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle error
                return Json(new
                {
                    error = true,
                    message = ex.Message,
                    data = new List<FakturPajakFromCoretax>() // Tambahkan data kosong agar tidak error di DataTables
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public string ConvertMonthToIndonesian(int? month)
        {
            // Array dengan nama bulan dalam Bahasa Indonesia
            string[] months = new string[]
            {
        "Januari", "Februari", "Maret", "April", "Mei", "Juni",
        "Juli", "Agustus", "September", "Oktober", "November", "Desember"
            };

            // Cek jika bulan null atau tidak valid (kurang dari 1 atau lebih dari 12)
            if (!month.HasValue || month < 1 || month > 12)
            {
                return ""; // Mengembalikan string kosong jika bulan tidak valid atau null
            }

            // Mengembalikan nama bulan sesuai dengan angka
            return months[month.Value - 1]; // Menggunakan .Value untuk mengambil nilai bulan
        }


        [HttpPost]
        public JsonResult ConvertExcelToJson(HttpPostedFileBase file)
        {
            var validData = new List<FakturPajakFromCoretax>();
            var invalidData = new List<object>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;  // or LicenseContext.Commercial if you have a commercial license

            var userNameLogin = Session["UserName"] as string;


            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "File tidak ditemukan" });
                }

                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    // Loop through the rows in the worksheet
                    for (int row = 2; row <= rowCount; row++)  // Assuming the first row is headers
                    {
                        var rowData = new FakturPajakFromCoretax
                        {
                            NPWP_Penjual = worksheet.Cells[row, 1].Text,
                            Nama_Penjual = worksheet.Cells[row, 2].Text,
                            Nomor_Faktur_Pajak = worksheet.Cells[row, 3].Text,
                            Tanggal_Faktur_Pajak = DateTime.TryParse(worksheet.Cells[row, 4].Text, out var tanggal) ? tanggal.Date : DateTime.MinValue.Date,
                            Masa_Pajak = ConvertBulanToInt(worksheet.Cells[row, 5].Text),
                            Tahun = int.TryParse(worksheet.Cells[row, 6].Text, out var tahun) ? tahun : 0,
                            Masa_Pajak_Pengkreditan = ConvertBulanToInt(worksheet.Cells[row, 7].Text),
                            Tahun_Pengkreditan = int.TryParse(worksheet.Cells[row, 8].Text, out var tahun_pengkreditan) ? tahun_pengkreditan : 0,
                            Harga_Jual = decimal.TryParse(worksheet.Cells[row, 9].Text, out var hargaJual) ? hargaJual : 0,
                            DPP_Nilai_Lain = decimal.TryParse(worksheet.Cells[row, 10].Text, out var dpp) ? dpp : 0,
                            PPN = decimal.TryParse(worksheet.Cells[row, 11].Text, out var ppn) ? ppn : 0,
                            PPnBM = decimal.TryParse(worksheet.Cells[row, 12].Text, out var ppnbm) ? ppnbm : 0,
                            Status_Faktur = worksheet.Cells[row, 13].Text,
                            Perekam = worksheet.Cells[row, 14].Text,
                            Nomor_SP2D = worksheet.Cells[row, 15].Text,
                            Valid = bool.TryParse(worksheet.Cells[row, 16].Text, out var valid) && valid,
                            Dilaporkan = bool.TryParse(worksheet.Cells[row, 17].Text, out var dilaporkan) && dilaporkan,
                            Dilaporkan_oleh_Penjual = bool.TryParse(worksheet.Cells[row, 18].Text, out var dilaporkanPenjual) && dilaporkanPenjual,
                            CreatedOn = DateTime.Now,
                            CreatedBy = userNameLogin,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = userNameLogin
                        };

                        // Perform validation for each row
                        var errors = new List<string>();
                        if (string.IsNullOrEmpty(rowData.NPWP_Penjual)) errors.Add("NPWP Penjual is required.");
                        if (string.IsNullOrEmpty(rowData.Nama_Penjual)) errors.Add("Nama Penjual is required.");
                        if (string.IsNullOrEmpty(rowData.Nomor_Faktur_Pajak)) errors.Add("Nomor Faktur Pajak is required.");
                        if (rowData.Tanggal_Faktur_Pajak == DateTime.MinValue) errors.Add("Tanggal Faktur Pajak is invalid.");
                        if (rowData.Tahun == 0) errors.Add("Tahun is required.");
                        // Add other validations as needed

                        if (errors.Count > 0)
                        {
                            invalidData.Add(new { data = rowData, errors = errors });
                        }
                        else
                        {
                            validData.Add(rowData);
                        }
                    }
                }

                return Json(new { success = true, validData = validData, invalidData = invalidData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ScanSatuanPDF(HttpPostedFileBase file, string Dikreditkan)
        {
            // Konversi string Dikreditkan menjadi boolean
            bool isDikreditkan = false;
            if (!string.IsNullOrEmpty(Dikreditkan))
            {
                isDikreditkan = Dikreditkan.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            var userNameLogin = Session["UserName"] as string;

            //FakturPajakFromCoretax fakturPajakFromCoretax = new FakturPajakFromCoretax();
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "File tidak ditemukan" });
                }

                string stringPDF = ExtractTextFromPdf(file.InputStream);

                FakturPajakFromCoretax fakturPajakFromCoretax = ConvertStringToFakturPajak(stringPDF);
                fakturPajakFromCoretax.Dikreditkan = isDikreditkan;
                fakturPajakFromCoretax.Nama_Pembeli = "ASTRA DAIHATSU MOTOR";
                fakturPajakFromCoretax.NPWP_Pembeli = "0010005718092000";
                fakturPajakFromCoretax.Status_Faktur = "APPROVED";


                return SaveScanSatuan(fakturPajakFromCoretax);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ScanBulkPDF(HttpPostedFileBase[] files, string Dikreditkan)
        {
            // Konversi string Dikreditkan menjadi boolean
            bool isDikreditkan = false;
            if (!string.IsNullOrEmpty(Dikreditkan))
            {
                isDikreditkan = Dikreditkan.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            var userNameLogin = Session["UserName"] as string;

            // List untuk menyimpan hasil setiap file
            var results = new List<FakturPajakFromCoretax>();

            try
            {
                if (files == null || files.Length == 0)
                {
                    return Json(new { success = false, message = "Tidak ada file yang diunggah" });
                }

                foreach (var file in files)
                {

                    string stringPDF = ExtractTextFromPdf(file.InputStream);

                    FakturPajakFromCoretax fakturPajakFromCoretax = ConvertStringToFakturPajak(stringPDF);
                    fakturPajakFromCoretax.Dikreditkan = isDikreditkan;
                    fakturPajakFromCoretax.Nama_Pembeli = "ASTRA DAIHATSU MOTOR";
                    fakturPajakFromCoretax.NPWP_Pembeli = "0010005718092000";
                    fakturPajakFromCoretax.Status_Faktur = "APPROVED";

                    // Simpan dan tambahkan hasil ke list
                    results.Add(fakturPajakFromCoretax);
                }

                return Json(new { success = true, data = results });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        #region ScanPDF
        private string ExtractTextFromPdf(Stream pdfStream)
        {
            StringBuilder textBuilder = new StringBuilder();

            using (var document = PdfDocument.Load(pdfStream))
            {
                int pageCount = document.PageCount;

                for (int i = 0; i < pageCount; i++)
                {
                    string pageText = document.GetPdfText(i); // Extract text from each page
                    textBuilder.Append(pageText);
                }
            }

            return textBuilder.ToString();
        }

        private string ExtractField(string text, string start, string end)
        {

            // If no special regex characters, use the original IndexOf method
            int startIndex = text.IndexOf(start);
            if (startIndex == -1)
            {
                return string.Empty;
            }
            startIndex += start.Length;

            int endIndex = text.IndexOf(end, startIndex);
            if (endIndex == -1)
            {
                return string.Empty;
            }

            return text.Substring(startIndex, endIndex - startIndex).Trim();

        }

        private string ExtractField(string text, string pattern)
        {
            // Define the regex pattern
            //string pattern = @"#(\d+)\r\nNPWP : (\d+)\r\nNIK :";

            // Perform regex match
            Match match = Regex.Match(text, pattern, RegexOptions.Singleline);

            // If a match is found, return the NPWP value (second capture group)
            if (match.Success)
            {
                return match.Groups[2].Value.Trim(); // Return the NPWP number (the second capture group)
            }
            else
            {
                return string.Empty; // If no match is found, return empty
            }
        }

        private string CleanString(string input)
        {
            // Remove extra line breaks and spaces, replace with a single space
            string cleaned = input.Replace("\r", "").Replace("\n", "").Trim();
            return cleaned;
        }

        public static DateTime ConvertTanggalToDate(string tanggalString)
        {
            var culture = new CultureInfo("id-ID");
            DateTime tanggal;

            // Default tanggal jika parsing gagal
            DateTime defaultTanggal = DateTime.MinValue;

            if (!DateTime.TryParseExact(
                tanggalString,
                "dd MMMM yyyy",
                culture,
                DateTimeStyles.None,
                out tanggal))
            {
                tanggal = defaultTanggal;  // Set nilai default
            }

            return tanggal;
        }

        private FakturPajakFromCoretax ConvertStringToFakturPajak(string text)
        {

            string nomorFakturPajak = ExtractField(text, "Kode dan Nomor Seri Faktur Pajak: ", "\nPengusaha Kena Pajak");

            // Pengusaha Kena Pajak
            var namaPengusaha = ExtractField(text, "Pengusaha Kena Pajak:\r\nNama : ", "\r\nAlamat :");
            var alamatPengusaha = CleanString(ExtractField(text, "Alamat : ", "\r\nNPWP :"));
            var npwpPengusaha = ExtractField(text, "NPWP : ", "\nPembeli Barang Kena Pajak");
            var nitkuPengusaha = ExtractField(alamatPengusaha, "#(\\d{19})");

            // Pembeli Barang Kena Pajak
            var namaPembeli = ExtractField(text, "Pembeli Barang Kena Pajak/Penerima Jasa Kena Pajak:\r\nNama : ", "\r\nAlamat :");
            var alamatPembeli = CleanString(ExtractField(text, "Alamat : ", "\r\nNPWP :").Trim());
            var nitkuPembeli = ExtractField(alamatPembeli, "#(\\d{19})");
            var npwpPembeli = ExtractField(text, @"#(\d+)\r\nNPWP : (\d+)\r\nNIK :");
            var nikPembeli = ExtractField(text, "NIK : ", "\r\nNomor Paspor :") == "-" ? string.Empty : ExtractField(text, "NIK : ", "\r\nNomor Paspor :");
            var nomorPasporPembeli = ExtractField(text, "Nomor Paspor : ", "\r\nIdentitas Lain :") == "-" ? string.Empty : ExtractField(text, "Nomor Paspor : ", "\r\nIdentitas Lain :");
            var identitasLainPembeli = ExtractField(text, "Identitas Lain : ", "\r\nEmail :") == "-" ? string.Empty : ExtractField(text, "Identitas Lain : ", "\r\nEmail :");
            var emailPembeli = ExtractField(text, "Email: ", "\r\nNo.") == "-" ? string.Empty : ExtractField(text, "Email: ", "\r\nNo.");
            var dpp = ExtractField(text, "Dasar Pengenaan Pajak", "\nJumlah PPN");

            // Total
            var hargaJual = ExtractField(text, "Harga Jual / Penggantian / Uang Muka / Termin ", "\nDikurangi Potongan Harga");
            var jumlahPPN = ExtractField(text, "Jumlah PPN (Pajak Pertambahan Nilai) ", "\nJumlah PPnBM");
            var jumlahPPnBM = ExtractField(text, "Jumlah PPnBM (Pajak Penjualan atas Barang Mewah) ", "\nSesuai dengan ketentuan");

            // Tanggal
            var tanggal = ConvertTanggalToDate(CleanString(ExtractField(text, "^(.*?),\\s*(\\d{2} [A-Za-z]+ \\d{4})")));
            /*
                Nama Penjual
                NPWP Penjual
                Nama Pembeli
                NPWP Pembeli
                Nomor Faktur Pajak
                Tanggal Faktur Pajak
                DPP
                PPN
                PPNBM
                APPROVE
             */
            FakturPajakFromCoretax fakturPajakFromCoretax = new FakturPajakFromCoretax();
            fakturPajakFromCoretax.Nomor_Faktur_Pajak = nomorFakturPajak;
            fakturPajakFromCoretax.Nama_Penjual = namaPengusaha;
            fakturPajakFromCoretax.NPWP_Penjual = npwpPengusaha;
            fakturPajakFromCoretax.Tanggal_Faktur_Pajak = tanggal;
            fakturPajakFromCoretax.Masa_Pajak = tanggal.Month;
            fakturPajakFromCoretax.Tahun = tanggal.Year;
            fakturPajakFromCoretax.Harga_Jual = ParseDecimal(hargaJual);
            fakturPajakFromCoretax.DPP_Nilai_Lain = ParseDecimal(dpp);
            fakturPajakFromCoretax.PPN = ParseDecimal(jumlahPPN);
            fakturPajakFromCoretax.PPnBM = ParseDecimal(jumlahPPnBM);
            fakturPajakFromCoretax.Status_Faktur = "";

            return fakturPajakFromCoretax;

        }

        // Fungsi untuk mengonversi string nominal ke decimal dengan mendeteksi format
        static decimal ParseDecimal(string value)
        {
            // Cek apakah string mengandung koma dan titik
            if (value.Contains(",") && value.Contains("."))
            {
                // Jika terdapat koma dan titik, kita perlu menentukan formatnya
                // Cek apakah koma berada di bagian terakhir (kemungkinan sebagai pemisah desimal, seperti format Indonesia)
                if (value.LastIndexOf(",") > value.LastIndexOf("."))
                {
                    // Format Indonesia (titik sebagai pemisah ribuan, koma sebagai desimal)
                    value = value.Replace(".", "").Replace(",", ".");
                }
                else
                {
                    // Format Amerika (koma sebagai pemisah ribuan, titik sebagai desimal)
                    value = value.Replace(",", "").Replace(".", ",");
                }
            }
            else if (value.Contains(","))
            {
                // Jika hanya ada koma, maka kemungkinan format Amerika
                value = value.Replace(",", "");
            }
            else if (value.Contains("."))
            {
                // Jika hanya ada titik, maka kemungkinan format Indonesia
                value = value.Replace(".", "").Replace(",", ".");
            }

            // Coba mengonversi string ke decimal
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            return 0; // Nilai default jika parsing gagal
        }

        public static int ConvertBulanToInt(string namaBulan)
        {
            var bulanIndonesia = new Dictionary<string, int>
        {
            { "Januari", 1 },
            { "Februari", 2 },
            { "Maret", 3 },
            { "April", 4 },
            { "Mei", 5 },
            { "Juni", 6 },
            { "Juli", 7 },
            { "Agustus", 8 },
            { "September", 9 },
            { "Oktober", 10 },
            { "November", 11 },
            { "Desember", 12 }
        };

            // Menghapus spasi tambahan dan mengonversi ke format standar (case-insensitive)
            namaBulan = namaBulan.Trim();

            // Mengembalikan bulan yang sesuai jika ada, jika tidak mengembalikan 0
            return bulanIndonesia.ContainsKey(namaBulan) ? bulanIndonesia[namaBulan] : 0;
        }

        #endregion

        [HttpPost]
        public JsonResult UploadFileFakturPajak(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return Json(new { success = false, message = "File tidak ditemukan" });
            }

            var model = new RequestResultModel()
            {
                InfoType = RequestResultInfoType.Success,
                Message = "Upload Coretax Success"
            };

            var userNameLogin = Session["UserName"] as string;

            // Validate file extension
            if (file == null || file.ContentLength == 0 || !file.FileName.EndsWith(".xlsx"))
            {
                model.InfoType = RequestResultInfoType.ErrorOrDanger;
                model.Message = "Invalid file type. Please upload an Excel file.";
                return Json(new { Html = model }, JsonRequestBehavior.AllowGet);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;  // or LicenseContext.Commercial if you have a commercial license


            // Read the Excel file
            // Create a list to store the extracted data
            List<FakturPajakFromCoretax> fakturPajakFromCoretaxes = new List<FakturPajakFromCoretax>();
            List<object> validationErrors = new List<object>();

            // Read the Excel file
            try
            {
                int totalBarisBerhasil = 0;
                int totalBarisGagal = 0;

                using (var package = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)  // Baris pertama dianggap sebagai header
                    {
                        string npwpPenjual = worksheet.Cells[row, 1].Text; //
                        string nomorFaktur = worksheet.Cells[row, 3].Text; //
                        string hargaJualText = worksheet.Cells[row, 10].Text; // 
                        string dppText = worksheet.Cells[row, 11].Text; //
                        string ppnText = worksheet.Cells[row, 12].Text; //
                        string ppnbmText = worksheet.Cells[row, 13].Text; //

                        List<string> rowErrors = new List<string>();

                        // Validasi NPWP Penjual harus angka
                        if (!npwpPenjual.All(char.IsDigit))
                        {
                            rowErrors.Add("NPWP hanya boleh menggunakan angka");
                        }

                        // Validasi Nomor Faktur Pajak harus angka
                        if (!nomorFaktur.All(char.IsDigit))
                        {
                            rowErrors.Add("No. Faktur hanya boleh menggunakan angka");
                        }

                        int help = 0;

                        // Validasi Harga Jual, DPP Nilai Lain, PPN, dan PPnBM harus angka
                        if (!decimal.TryParse(hargaJualText, out var hargaJual))
                        {
                            rowErrors.Add("Harga Jual/DPP/PPN/PPnBM hanya boleh menggunakan angka");
                            help = 1;
                        }
                        if (!decimal.TryParse(dppText, out var dpp) && help != 1)
                        {
                            rowErrors.Add("Harga Jual/DPP/PPN/PPnBM hanya boleh menggunakan angka");
                            help = 1;
                        }
                        if (!decimal.TryParse(ppnText, out var ppn) && help != 1)
                        {
                            rowErrors.Add("Harga Jual/DPP/PPN/PPnBM hanya boleh menggunakan angka");
                            help = 1;
                        }
                        if (!decimal.TryParse(ppnbmText, out var ppnbm) && help != 1)
                        {
                            rowErrors.Add("Harga Jual/DPP/PPN/PPnBM hanya boleh menggunakan angka");
                        }


                        // Jika ada error, simpan ke list error dan lanjutkan ke baris berikutnya
                        if (rowErrors.Count > 0)
                        {
                            totalBarisGagal++;
                            validationErrors.Add(new
                            {
                                Baris = row,
                                Errors = rowErrors.Select((error, index) => $"{index + 1}. {error}").ToList()
                            });
                            continue;
                        }

                        // Jika validasi lolos, buat objek dan tambahkan ke daftar faktur pajak
                        var faktur = new FakturPajakFromCoretax
                        {
                            NPWP_Penjual = npwpPenjual, //
                            Nama_Penjual = worksheet.Cells[row, 2].Text, //
                            Nomor_Faktur_Pajak = nomorFaktur, //
                            Tanggal_Faktur_Pajak = DateTime.TryParse(worksheet.Cells[row, 4].Text, out var tanggal) ? tanggal.Date : DateTime.MinValue, //
                            Masa_Pajak = ConvertBulanToInt(worksheet.Cells[row, 5].Text), //
                            Tahun = int.TryParse(worksheet.Cells[row, 6].Text, out var tahun) ? tahun : 0, //
                            Masa_Pajak_Pengkreditan = ConvertBulanToInt(worksheet.Cells[row, 7].Text), //
                            Tahun_Pengkreditan = int.TryParse(worksheet.Cells[row, 8].Text, out var tahun_pengkreditan) ? tahun_pengkreditan : 0, //
                            Harga_Jual = hargaJual, //
                            DPP_Nilai_Lain = dpp, //
                            PPN = ppn, //
                            PPnBM = ppnbm, //
                            Status_Faktur = worksheet.Cells[row, 9].Text, //
                            Perekam = worksheet.Cells[row, 14].Text, // 
                            Nomor_SP2D = string.IsNullOrEmpty(worksheet.Cells[row, 15].Text) ? "" : worksheet.Cells[row, 15].Text,                     //
                            Valid = bool.TryParse(worksheet.Cells[row, 16].Text, out var valid) && valid, //
                            Dilaporkan = bool.TryParse(worksheet.Cells[row, 17].Text, out var dilaporkan) && dilaporkan, //
                            Dilaporkan_oleh_Penjual = bool.TryParse(worksheet.Cells[row, 18].Text, out var dilaporkanPenjual) && dilaporkanPenjual, //
                            CreatedOn = DateTime.Now,
                            CreatedBy = userNameLogin,
                            UpdatedOn = DateTime.Now,
                            UpdatedBy = userNameLogin,
                            IsDeleted = bool.TryParse("false", out bool isDeleted)
                        };

                        fakturPajakFromCoretaxes.Add(faktur);
                        totalBarisBerhasil++;
                    }

                    if (fakturPajakFromCoretaxes.Count > 0)
                    {
                        FakturPajakFromCoretaxes.SaveList(fakturPajakFromCoretaxes);
                        bool update = UpdateStatusFaktur();
                    }

                    if (validationErrors.Count > 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"{totalBarisBerhasil} baris berhasil dimasukkan, {totalBarisGagal} baris terkena validasi.",
                            errors = validationErrors
                        });
                    }
                }
                return Json(new { success = true, message = $"{totalBarisBerhasil} baris berhasil dimasukkan, {totalBarisGagal} baris terkena validasi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        public static bool UpdateStatusFaktur()
        {
            try
            {
                var sp = new SpBase(@"EXEC dbo.sp_UpdateStatusFaktur");
                int result = sp.ExecuteNonQuery();

                if (result == 0)
                {
                    return false; // False jika error (return 0 dari SP)
                }

                return true; // True selama hasilnya 1 (sukses)
            }
            catch (Exception)
            {
                return false; // False kalau error di sisi C#
            }
        }

        [AuthActivity("47")]
        public ActionResult ImportFromExcel()
        {
            return View();
        }

        [AuthActivity("47")]
        public ActionResult ScanFakturPajak()
        {
            return View();
        }


        private List<string> ErrorCheckFakturPajak(FakturPajakFromCoretax fakturPajakFromCoretax)
        {
            var errorMessages = new List<string>();  // List to accumulate error messages

            // Check for required fields and format validation
            // Validasi NPWP Penjual (panjang harus 15, 16, 20, atau 22 karakter dan hanya angka)  
            if (string.IsNullOrWhiteSpace(fakturPajakFromCoretax.NPWP_Penjual) ||
                !new[] { 15, 16, 20, 22 }.Contains(fakturPajakFromCoretax.NPWP_Penjual.Length) ||
                !fakturPajakFromCoretax.NPWP_Penjual.All(char.IsDigit)) // Hanya angka  
            {
                errorMessages.Add("NPWP Penjual tidak valid.");
            }

            // Validasi Nama Penjual  
            if (string.IsNullOrWhiteSpace(fakturPajakFromCoretax.Nama_Penjual))
            {
                errorMessages.Add("Nama Penjual tidak boleh kosong.");
            }

            // Validasi Nomor Faktur Pajak (harus panjang 17 dan hanya angka)  
            if (string.IsNullOrWhiteSpace(fakturPajakFromCoretax.Nomor_Faktur_Pajak) ||
                fakturPajakFromCoretax.Nomor_Faktur_Pajak.Length != 17 ||
                !fakturPajakFromCoretax.Nomor_Faktur_Pajak.All(char.IsDigit)) // Hanya angka  
            {
                errorMessages.Add("Nomor Faktur Pajak tidak valid.");
            }

            else if (fakturPajakFromCoretax.Nomor_Faktur_Pajak.Any(char.IsPunctuation))
            {
                errorMessages.Add("No. Faktur Tidak Boleh Menggunakan Tanda Baca.");
            }

            // Validasi Tanggal Faktur Pajak  
            if (fakturPajakFromCoretax.Tanggal_Faktur_Pajak == default(DateTime))
            {
                errorMessages.Add("Tanggal Faktur Pajak tidak valid.");
            }

            // Validasi Status Faktur Harus APPROVED  
            if (fakturPajakFromCoretax.Status_Faktur != "APPROVED")
            {
                errorMessages.Add("Status Faktur harus APPROVED.");
            }

            // Validasi Nama dan NPWP Pembeli Bukan ASTRA DAIHATSU MOTOR  
            if (!fakturPajakFromCoretax.Nama_Pembeli.Contains("ASTRA DAIHATSU MOTOR") ||
                fakturPajakFromCoretax.NPWP_Pembeli != "0010005718092000")
            {
                errorMessages.Add("Nama atau NPWP Pembeli bukan ASTRA DAIHATSU MOTOR atau 0010005718092000.");
            }

            // Validasi NPWP Tidak Boleh Menggunakan Tanda Baca  
            if (!fakturPajakFromCoretax.NPWP_Penjual.All(char.IsDigit))
            {
                errorMessages.Add("NPWP hanya boleh berisi angka.");
            }

            // Validasi Harga Jual/DPP/PPN/PPnBM Tidak Boleh Menggunakan Tanda Baca  
            if (!decimal.TryParse(fakturPajakFromCoretax.Harga_Jual.ToString(), out _) ||
                !decimal.TryParse(fakturPajakFromCoretax.DPP_Nilai_Lain.ToString(), out _) ||
                !decimal.TryParse(fakturPajakFromCoretax.PPN.ToString(), out _) ||
                !decimal.TryParse(fakturPajakFromCoretax.PPnBM.ToString(), out _))
            {
                errorMessages.Add("Harga Jual/DPP/PPN/PPnBM harus berupa angka desimal yang valid.");
            }


            // Validasi PPN dan PPNBM tidak negatif  
            if (fakturPajakFromCoretax.PPN < 0 || fakturPajakFromCoretax.PPnBM < 0)
            {
                errorMessages.Add("PPN dan PPNBM tidak boleh negatif.");
            }

            return errorMessages;
        }

        [HttpPost]
        public JsonResult SaveScanSatuan(FakturPajakFromCoretax fakturPajakFromCoretax)
        {
            // Check Validasi and return any errors
            List<string> errorMessages = ErrorCheckFakturPajak(fakturPajakFromCoretax);

            // Get the username login
            var userNameLogin = Session["UserName"] as string;

            // Get the current month and year in Indonesian
            var currentDate = DateTime.Now;

            // Get the current year
            var year = currentDate.Year;

            // Assign the values
            //fakturPajakFromCoretax.Tahun = year;
            fakturPajakFromCoretax.CreatedBy = userNameLogin;
            fakturPajakFromCoretax.UpdatedBy = userNameLogin;
            fakturPajakFromCoretax.CreatedOn = DateTime.Now;
            fakturPajakFromCoretax.UpdatedOn = DateTime.Now;

            FakturPajak fakturPajak = FakturPajaks.GetFakturPajakByNoFaktur(fakturPajakFromCoretax.Nomor_Faktur_Pajak);

            if (!string.IsNullOrEmpty(fakturPajak.NoFakturPajak))
            {
                return Json(new
                {
                    success = false,
                    messages = "Sudah discan pada " +
                                    ConvertHelper.DateTimeConverter.ToLongDateString(fakturPajak.Created) + " oleh " +
                    fakturPajak.CreatedBy +
                    ". No FP " + fakturPajak.FormatedNoFaktur + " sudah ada di Masa Pajak " + fakturPajak.MasaPajakName +
                    " " + fakturPajak.TahunPajak
                                    + ", Nomor Filling Index " + fakturPajak.FillingIndex
                });
            }

            FakturPajakFromCoretax getFakturPajak = FakturPajakFromCoretaxes.GetByNoFaktur(fakturPajakFromCoretax);

            if (getFakturPajak != null)
            {
                // Validasi HargaJual/PPN/PPnBM Tidak Sesuai dengan Data Penampung  
                if (Math.Round(fakturPajakFromCoretax.Harga_Jual, 2) != Math.Round(getFakturPajak.Harga_Jual, 2) ||
                    Math.Round(fakturPajakFromCoretax.PPN, 2) != Math.Round(getFakturPajak.PPN, 2) ||
                    Math.Round(fakturPajakFromCoretax.PPnBM, 2) != Math.Round(getFakturPajak.PPnBM, 2))
                {
                    errorMessages.Add("Nilai DPP/PPN/PPnBM Tidak Sesuai dengan Data Penampung.");
                }

                // Validasi NPWP Penjual Tidak Sesuai dengan Data Penampung  
                if (fakturPajakFromCoretax.NPWP_Penjual != getFakturPajak.NPWP_Penjual)
                {
                    errorMessages.Add("NPWP Penjual Tidak Sesuai dengan Data Penampung.");
                }

            }
            else
            {
                errorMessages.Add($"Nomor Faktur {fakturPajakFromCoretax.Nomor_Faktur_Pajak} tidak ada dalam Tabel Penampung dari Coretax");
            }

            // If there are any errors, return them
            if (errorMessages.Any())
            {
                return Json(new { success = false, messages = errorMessages });
            }

            if (getFakturPajak != null && errorMessages.Count == 0)
            {
                // FakturPajakFromCoretaxes.Save(fakturPajakFromCoretax); // Assuming Save method handles database saving
                FakturPajaks.SaveCoretax(new FakturPajak
                {
                    NPWPPenjual = getFakturPajak.NPWP_Penjual,
                    NamaPenjual = getFakturPajak.Nama_Penjual,
                    NamaLawanTransaksi = string.IsNullOrEmpty(getFakturPajak.Nama_Pembeli)
    ? "Astra Daihatsu Motor"
    : getFakturPajak.Nama_Pembeli,
                    NPWPLawanTransaksi = string.IsNullOrEmpty(getFakturPajak.NPWP_Pembeli)
    ? "0010005718092000"
    : getFakturPajak.NPWP_Pembeli,
                    Dikreditkan = fakturPajakFromCoretax.Dikreditkan,
                    NoFakturPajak = getFakturPajak.Nomor_Faktur_Pajak,
                    TglFaktur = getFakturPajak.Tanggal_Faktur_Pajak,
                    KdJenisTransaksi = getFakturPajak.Nomor_Faktur_Pajak.Length >= 2
    ? getFakturPajak.Nomor_Faktur_Pajak.Substring(0, 2)
    : "01",
                    FgPengganti = getFakturPajak.Nomor_Faktur_Pajak.Substring(1, 3) != "100" ? "1" : "0",
                    AlamatLawanTransaksi = "Jl. Gaya Motor III Sunter II, 5, Sungai Bambu, Tanjung Priok, Kota ADM. Jakarta Utara, DKI Jakarta, 14330",
                    MasaPajak = getFakturPajak.Masa_Pajak,
                    TahunPajak = getFakturPajak.Tahun,
                    MasaPajakPengkreditan = getFakturPajak.Masa_Pajak_Pengkreditan,
                    TahunPajakPengkreditan = getFakturPajak.Tahun_Pengkreditan,
                    JumlahHargaJual = getFakturPajak.Harga_Jual,
                    JumlahDPP = getFakturPajak.DPP_Nilai_Lain,
                    JumlahPPN = getFakturPajak.PPN,
                    JumlahPPNBM = getFakturPajak.PPnBM,
                    StatusFaktur = getFakturPajak.Status_Faktur,
                    Created = DateTime.Now,
                    CreatedBy = userNameLogin,
                    Modified = DateTime.Now,
                    ModifiedBy = userNameLogin,
                    IsCoretax = true,
                    Status = (int)ApplicationEnums.StatusFakturPajak.Success,
                    /*
                        IWS = 1,
                        NON_IWS = 2,
                        KHUSUS = 3,
                        EXTERNAL = 4,
                     */
                    FPType = 2,

                    /*
                        SCAN_SATUAN = 1,
                        SCAN_BULK = 2,
                        MANUAL = 3,
                        EXTERNAL = 4,
                    */
                    ScanType = 1,
                    Source = userNameLogin,
                    FCode = ApplicationConstant.FCodeFm ?? "FM",
                });
            }

            fakturPajak = FakturPajaks.GetFakturPajakByNoFaktur(fakturPajakFromCoretax.Nomor_Faktur_Pajak);

            return Json(new { success = true, messages = $"No. Faktur Pajak {fakturPajak.NoFakturPajak} Berhasil Disimpan", data = fakturPajak });
        }

        [AuthActivity("47")]
        public ActionResult ScanBulkFakturPajak()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveScanBulk(List<FakturPajakFromCoretax> fakturPajakList)
        {
            //var resultEntries = new List<(string nomorFaktur, string fillingIndex, List<string> errorMessages)>();
            var resultEntries = new List<object>();

            var userNameLogin = Session["UserName"] as string;


            // Validate each faktur and collect errors with their indices
            for (int i = 0; i < fakturPajakList.Count; i++)
            {
                var faktur = fakturPajakList[i];
                var errors = ErrorCheckFakturPajak(faktur);
                var errorMessages = new List<string>();

                if (errors.Any())
                {
                    errorMessages.AddRange(errors);
                }
                else
                {
                    // Assign values
                    faktur.Masa_Pajak = faktur.Tanggal_Faktur_Pajak.Month;
                    faktur.Tahun = faktur.Tanggal_Faktur_Pajak.Year;
                    faktur.CreatedBy = userNameLogin;
                    faktur.UpdatedBy = userNameLogin;
                    faktur.CreatedOn = DateTime.Now;
                    faktur.UpdatedOn = DateTime.Now;

                    // Attempt to save the valid FakturPajak
                    try
                    {
                        FakturPajakFromCoretax getFakturPajak = FakturPajakFromCoretaxes.GetByNoFaktur(faktur);
                        if (getFakturPajak != null)
                        {
                            FakturPajaks.SaveCoretax(new FakturPajak
                            {
                                NPWPPenjual = getFakturPajak.NPWP_Penjual,
                                NamaPenjual = getFakturPajak.Nama_Penjual,
                                NamaLawanTransaksi = string.IsNullOrEmpty(getFakturPajak.Nama_Pembeli)
                                    ? "Astra Daihatsu Motor"
                                    : getFakturPajak.Nama_Pembeli,
                                NPWPLawanTransaksi = string.IsNullOrEmpty(getFakturPajak.NPWP_Pembeli)
                                    ? "0010005718092000"
                                    : getFakturPajak.NPWP_Pembeli,
                                Dikreditkan = faktur.Dikreditkan,
                                NoFakturPajak = getFakturPajak.Nomor_Faktur_Pajak,
                                TglFaktur = getFakturPajak.Tanggal_Faktur_Pajak,
                                KdJenisTransaksi = getFakturPajak.Nomor_Faktur_Pajak.Length >= 2
                                    ? getFakturPajak.Nomor_Faktur_Pajak.Substring(0, 2)
                                    : "01",
                                FgPengganti = getFakturPajak.Nomor_Faktur_Pajak.Substring(1, 3) != "100" ? "1" : "0",
                                AlamatLawanTransaksi = "Jl. Gaya Motor III Sunter II, 5, Sungai Bambu, Tanjung Priok, Kota ADM. Jakarta Utara, DKI Jakarta, 14330",
                                MasaPajak = getFakturPajak.Masa_Pajak,
                                TahunPajak = getFakturPajak.Tahun,
                                MasaPajakPengkreditan = getFakturPajak.Masa_Pajak_Pengkreditan,
                                TahunPajakPengkreditan = getFakturPajak.Tahun_Pengkreditan,
                                JumlahHargaJual = getFakturPajak.Harga_Jual,
                                JumlahDPP = getFakturPajak.DPP_Nilai_Lain,
                                JumlahPPN = getFakturPajak.PPN,
                                JumlahPPNBM = getFakturPajak.PPnBM,
                                StatusFaktur = getFakturPajak.Status_Faktur,
                                Created = DateTime.Now,
                                CreatedBy = userNameLogin,
                                Modified = DateTime.Now,
                                ModifiedBy = userNameLogin,
                                IsCoretax = true,
                                Status = (int)ApplicationEnums.StatusFakturPajak.Success,
                                FPType = 2,
                                ScanType = 1,
                                Source = userNameLogin,
                                FCode = ApplicationConstant.FCodeFm ?? "FM",
                            });

                            var fakturPajak = FakturPajaks.GetFakturPajakByNoFaktur(getFakturPajak.Nomor_Faktur_Pajak);
                            var entry = new Dictionary<string, object>
                                {
                                    { "nomorFaktur", faktur.Nomor_Faktur_Pajak },
                                    { "fillingIndex", "" },
                                    { "errorMessages", new List<string>() }
                                };

                            if (fakturPajak == null)
                            {
                                ((List<string>)entry["errorMessages"]).Add($"Nomor Faktur Pajak {getFakturPajak.Nomor_Faktur_Pajak} tidak ditemukan di Tabel Penampung");
                            }
                            else
                            {
                                entry["fillingIndex"] = fakturPajak.FillingIndex;
                            }

                            // Prepare the entry for the current faktur

                            // Add successful entry
                            resultEntries.Add(entry);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add($"Error saving faktur with NoFakturPajak: {faktur.Nomor_Faktur_Pajak}. Error: {ex.Message}");
                    }
                }

                // Add entry with errors if any
                if (errorMessages.Any())
                {
                    //var entry = new
                    //{
                    //    nomorFaktur = faktur.Nomor_Faktur_Pajak,
                    //    fillingIndex = "", // Jika error maka Filling Index nya kosong
                    //    errorMessages = errorMessages
                    //};

                    var entry = new Dictionary<string, object>
                                {
                                    { "nomorFaktur", faktur.Nomor_Faktur_Pajak },
                                    { "fillingIndex", "" },
                                    { "errorMessages", errorMessages }
                                };
                    resultEntries.Add(entry);
                }
            }

            // Determine overall success
            bool overallSuccess = true;

            return Json(new
            {
                success = overallSuccess,
                data = resultEntries
            });
        }

        [HttpPost]
        public ActionResult DownloadExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Baca request body JSON manual
            string requestBody;
            using (var reader = new StreamReader(Request.InputStream))
            {
                requestBody = reader.ReadToEnd();
            }

            // Parse JSON ke List<List<string>>
            var fakturData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(requestBody);

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Faktur Pajak");

                // Header
                string[] headers = { "NPWP Penjual", "Nama Penjual", "Nomor Faktur Pajak", "Tanggal Faktur Pajak", "Masa Pajak", "Tahun", "Masa Pajak Pengkreditan", "Tahun Pajak Pengkreditan", "Status Faktur", "Harga Jual/Pengganti/DPP", "DPP Nilai Lain/DPP", "PPN", "PPnBM", "Perekam", "Nomor SP2D", "Valid", "Dilaporkan" , "Dilaporkan Oleh Penjual" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Styling Header
                using (var range = worksheet.Cells["A1:R1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Isi Data dari Frontend
                int row = 2;
                foreach (var item in fakturData)
                {
                    for (int col = 0; col < item.Count; col++)
                    {
                        worksheet.Cells[row, col + 1].Value = item[col];
                    }
                    row++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Simpan dalam memory stream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = "FakturPajak.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

        //public JsonResult SaveScanBulk(List<FakturPajakFromCoretax> fakturPajakList)
        //{
        //    var errorMessages = new List<string>();
        //    var userNameLogin = Session["UserName"] as string;

        //    // Get the current month and year in Indonesian
        //    var currentDate = DateTime.Now;
        //    //var monthIndonesian = currentDate.ToString("MMMM", new System.Globalization.CultureInfo("id-ID"));
        //    var year = currentDate.Year;

        //    foreach (var faktur in fakturPajakList)
        //    {
        //        errorMessages = ErrorCheckFakturPajak(faktur);

        //        // Assign values
        //        //faktur.Masa_Pajak = monthIndonesian;
        //        faktur.Masa_Pajak = faktur.Tanggal_Faktur_Pajak.Month;

        //        faktur.Tahun = year;
        //        faktur.CreatedBy = userNameLogin;
        //        faktur.UpdatedBy = userNameLogin;
        //        faktur.CreatedOn = DateTime.Now;
        //        faktur.UpdatedOn = DateTime.Now;
        //    }

        //    // If there are errors, return them without saving
        //    if (errorMessages.Any())
        //    {
        //        return Json(new { success = false, messages = errorMessages });
        //    }

        //    List<string> errors = new List<string>();
        //    // Save all valid FakturPajak
        //    foreach (var faktur in fakturPajakList)
        //    {
        //        FakturPajakFromCoretax getFakturPajak = FakturPajakFromCoretaxes.GetByNoFaktur(faktur);

        //        if (getFakturPajak != null && errorMessages.Count == 0)
        //        {
        //            // FakturPajakFromCoretaxes.Save(fakturPajakFromCoretax); // Assuming Save method handles database saving
        //            FakturPajaks.SaveCoretax(new FakturPajak
        //            {
        //                NPWPPenjual = getFakturPajak.NPWP_Penjual,
        //                NamaPenjual = getFakturPajak.Nama_Penjual,
        //                NamaLawanTransaksi = string.IsNullOrEmpty(getFakturPajak.Nama_Pembeli)
        //? "Astra Daihatsu Motor"
        //: getFakturPajak.Nama_Pembeli,
        //                NPWPLawanTransaksi = string.IsNullOrEmpty(getFakturPajak.NPWP_Pembeli)
        //? "0010005718092000"
        //: getFakturPajak.NPWP_Pembeli,
        //                Dikreditkan = faktur.Dikreditkan,
        //                NoFakturPajak = getFakturPajak.Nomor_Faktur_Pajak,
        //                TglFaktur = getFakturPajak.Tanggal_Faktur_Pajak,
        //                KdJenisTransaksi = getFakturPajak.Nomor_Faktur_Pajak.Length >= 2
        //? getFakturPajak.Nomor_Faktur_Pajak.Substring(0, 2)
        //: "01",
        //                FgPengganti = getFakturPajak.Nomor_Faktur_Pajak.Substring(1, 3) != "100" ? "1" : "0",
        //                AlamatLawanTransaksi = "Jl. Gaya Motor III Sunter II, 5, Sungai Bambu, Tanjung Priok, Kota ADM. Jakarta Utara, DKI Jakarta, 14330",
        //                MasaPajak = getFakturPajak.Masa_Pajak,
        //                TahunPajak = getFakturPajak.Tahun,
        //                JumlahHargaJual = getFakturPajak.Harga_Jual,
        //                JumlahDPP = getFakturPajak.DPP_Nilai_Lain,
        //                JumlahPPN = getFakturPajak.PPN,
        //                JumlahPPNBM = getFakturPajak.PPnBM,
        //                StatusFaktur = getFakturPajak.Status_Faktur,
        //                Created = DateTime.Now,
        //                CreatedBy = userNameLogin,
        //                Modified = DateTime.Now,
        //                ModifiedBy = userNameLogin,
        //                IsCoretax = true,
        //                Status = (int)ApplicationEnums.StatusFakturPajak.Success,
        //                /*
        //                    IWS = 1,
        //                    NON_IWS = 2,
        //                    KHUSUS = 3,
        //                    EXTERNAL = 4,
        //                 */
        //                FPType = 2,

        //                /*
        //                    SCAN_SATUAN = 1,
        //                    SCAN_BULK = 2,
        //                    MANUAL = 3,
        //                    EXTERNAL = 4,
        //                */
        //                ScanType = 1,
        //                Source = userNameLogin,
        //                FCode = ApplicationConstant.FCodeFm ?? "FM",
        //            });
        //        }
        //    }

        //    if (errors.Any())
        //    {
        //        return Json(new { success = false, data = errors, message = "Data ada yang tidak ada." });

        //    }
        //    else
        //    {
        //        return Json(new { success = true, message = "Semua data berhasil disimpan." });
        //    }

        //}


    }
}