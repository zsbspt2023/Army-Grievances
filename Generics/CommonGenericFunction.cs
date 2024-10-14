
using ArmyGrievances.Models;
using ClosedXML.Excel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;
using System.Net.Http.Headers;


namespace ArmyGrievances.Generics
{
    public interface ICommonGenericFunction
    {
        Task<List<T>?> ExecuteGetListApi<T>(string MethodName, T? obj) where T : class, new();
        Task<T?> ExecutePostAPI<T>(string MethodName, T? obj) where T : class, new();
        Task<SelectionList?> ExecuteGetSelectionList(string MethodName, string par);
        Task<T?> ExecuteGetAPI<T>(string MethodName, string id) where T : class, new();
        string Uploadfile(IFormFile file, string folder);
        byte[] ExporttoExcel<T>(List<T>? table, string filename) where T : class, new();
        string ParseExcelFile(IFormFile formFile,string Action, int sheet);
    }
    public class CommonGenericFunction : ICommonGenericFunction
    {
        IConfiguration configuration;
        public readonly string Baseurl = "";
        private IWebHostEnvironment webHostEnvironment;
        public CommonGenericFunction(IConfiguration _config, IWebHostEnvironment _webHostEnvironment)
        {
            configuration = _config;
            webHostEnvironment = _webHostEnvironment;
        }

        public async Task<List<T>?> ExecuteGetListApi<T>(string MethodName, T? obj) where T : class, new()
        {
            var list = new List<T>();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Clear();
                var json = JsonConvert.SerializeObject(obj,
               new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var Res = await client.PostAsync(configuration.GetValue<string>("AppSettings:BaseUrl") + MethodName, content);
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<List<T>>(response);
                }
            }
            return list;
        }
        public async Task<T?> ExecutePostAPI<T>(string MethodName, T? obj) where T : class, new()
        {
            var genericresponse = default(T);
            using (var client = new HttpClient())
            {
                //Passing service base url

                client.DefaultRequestHeaders.Accept.Clear();
                var json = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var postResponse = await client.PostAsync(configuration.GetValue<string>("AppSettings:BaseUrl") + MethodName, content);

                if (postResponse.IsSuccessStatusCode)
                {
                    string response = postResponse.Content.ReadAsStringAsync().Result;
                    genericresponse = JsonConvert.DeserializeObject<T>(response);
                }
            }
            return genericresponse;
        }

        public async Task<T?> ExecuteGetAPI<T>(string MethodName, string id) where T : class, new()
        {
            var genericresponse = default(T);
            using (var client = new HttpClient())
            {
                //Passing service base url

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var postResponse = await client.GetAsync(configuration.GetValue<string>("AppSettings:BaseUrl") + MethodName + "?id=" + id);
                if (postResponse.IsSuccessStatusCode)
                {
                    string response = postResponse.Content.ReadAsStringAsync().Result;
                    genericresponse = JsonConvert.DeserializeObject<T>(response);
                }
            }
            return genericresponse;
        }
        public async Task<SelectionList?> ExecuteGetSelectionList(string MethodName, string par)
        {
            var obj = default(SelectionList);
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("nl-NL"));

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(configuration.GetValue<string>("AppSettings:BaseUrl") + MethodName + "?id=" + par);
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    obj = JsonConvert.DeserializeObject<SelectionList>(result);
                }
            }
            return obj;
        }

        public string Uploadfile(IFormFile file, string folder)
        {
            string UniqueFileName = $"{DateTime.Now:yyyyMMddTHHmmss}" + "_img.jpg";
            string uploadfolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
            string filepath = Path.Combine(uploadfolder, UniqueFileName);
            using (var filestream = new FileStream(filepath, FileMode.Open))
            {
                file.CopyTo(filestream);
            }

            return UniqueFileName;
        }
        public byte[] ExporttoExcel<T>(List<T>? table, string filename) where T : class, new()
        {
            using ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            var sheet = pack.Workbook.Worksheets.First();
            DataTable dt = new DataTable();
            ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.Light10);
            return pack.GetAsByteArray();
        }
        public string ParseExcelFile(IFormFile formFile, string Action, int sheet)
        {
            string jsonString = "";
            try
            {
                var grievances = new List<GrievanceModal>();
                var mailIn_Outs = new List<MainIn_OutModal>();
                var individuals = new List<IndividualModal>();
                var bytes = FormFileExtensions.GetBytes(formFile);
                Stream stream = new MemoryStream(bytes);
                using (var workbook = new XLWorkbook(stream))
                {
                    //Lets assume the First Worksheet contains the data
                    var worksheet = workbook.Worksheet(sheet);

                    //Lets assume first row contains the header, so skip the first row
                    var Allrows = worksheet.RowsUsed();
                    if (Allrows.Count() > 0)
                    {
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                        //Loop Through all the Rows except the first row which contains the header data
                        if (Action == "1")
                        {
                            foreach (var row in rows)
                            {
                                try
                                {
                                    var grievance = new GrievanceModal
                                    {
                                        Individual_Particular = row.Cell(2).GetValue<string>().Trim(),
                                        Grievance_ReceptDate = row.Cell(3).GetValue<string>().Trim(),
                                        Grienvance_Subject = row.Cell(4).GetValue<string>().Trim(),
                                        Sent_Area = row.Cell(5).GetValue<string>().Trim(),
                                        Regt_Record = row.Cell(6).GetValue<string>().Trim(),
                                        ZSB_MemoNo = row.Cell(7).GetValue<string>().Trim(),
                                        ZSB_MemoDate = row.Cell(8).GetValue<string>().Trim()
                                    };
                                    grievances.Add(grievance);
                                }
                                catch (Exception) { }
                            }
                            jsonString = JsonConvert.SerializeObject(grievances, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        }
                        else if (Action == "2")
                        {
                            foreach (var row in rows)
                            {
                                try
                                {
                                    var mailIn_Out = new MainIn_OutModal
                                    {
                                        Item = !string.IsNullOrWhiteSpace(row.Cell(1).GetValue<string>().Trim()) ? row.Cell(1).GetValue<string>().Trim() : "",
                                        MailOut_Date = !string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>().Trim()) ? row.Cell(2).GetValue<string>().Trim() : null,
                                        Subject = !string.IsNullOrWhiteSpace(row.Cell(3).GetValue<string>().Trim()) ? row.Cell(3).GetValue<string>().Trim() : "",
                                        ToWhom = !string.IsNullOrWhiteSpace(row.Cell(4).GetValue<string>().Trim()) ? row.Cell(4).GetValue<string>().Trim() : "",
                                        ZSB_MemoNo = !string.IsNullOrWhiteSpace(row.Cell(5).GetValue<string>().Trim()) ? row.Cell(5).GetValue<string>().Trim() : "",
                                        Diary_No = !string.IsNullOrWhiteSpace(row.Cell(6).GetValue<string>()) ? row.Cell(6).GetValue<long>() : 0,
                                        Service_No = !string.IsNullOrWhiteSpace(row.Cell(7).GetValue<string>().Trim()) ? row.Cell(7).GetValue<string>().Trim() : "",
                                        Name = !string.IsNullOrWhiteSpace(row.Cell(8).GetValue<string>().Trim()) ? row.Cell(8).GetValue<string>().Trim() : "",
                                        Recieving_Date = !string.IsNullOrWhiteSpace(row.Cell(9).GetValue<string>().Trim()) ? row.Cell(9).GetValue<string>().Trim() : null,
                                        Addressed_To = !string.IsNullOrWhiteSpace(row.Cell(10).GetValue<string>().Trim()) ? row.Cell(10).GetValue<string>().Trim() : "",
                                        Copy_To = !string.IsNullOrWhiteSpace(row.Cell(11).GetValue<string>().Trim()) ? row.Cell(11).GetValue<string>().Trim() : "",
                                        Letter_No = !string.IsNullOrWhiteSpace(row.Cell(12).GetValue<string>().Trim()) ? row.Cell(12).GetValue<string>().Trim() : "",
                                        Letter_Date = !string.IsNullOrWhiteSpace(row.Cell(13).GetValue<string>().Trim()) ? row.Cell(13).GetValue<string>().Trim() : null,
                                        LetterPlaced_File = false
                                    };
                                    mailIn_Outs.Add(mailIn_Out);
                                }
                                catch (Exception) { }
                            }
                            jsonString = JsonConvert.SerializeObject(mailIn_Outs);
                        }
                        else
                        {
                            foreach (var row in rows)
                            {
                                try
                                {
                                    var individual = new IndividualModal
                                    {
                                        Army_No = !string.IsNullOrWhiteSpace(row.Cell(1).GetValue<string>().Trim()) ? row.Cell(1).GetValue<string>().Trim() : "",
                                        Rank = !string.IsNullOrWhiteSpace(row.Cell(2).GetValue<string>().Trim()) ? row.Cell(2).GetValue<string>().Trim() : "",
                                        Name = !string.IsNullOrWhiteSpace(row.Cell(3).GetValue<string>().Trim()) ? row.Cell(3).GetValue<string>().Trim() : "",
                                        Address = !string.IsNullOrWhiteSpace(row.Cell(4).GetValue<string>().Trim()) ? row.Cell(4).GetValue<string>().Trim() : "",
                                        Mobile_No = !string.IsNullOrWhiteSpace(row.Cell(5).GetValue<string>().Trim()) ? row.Cell(5).GetValue<string>().Trim() : "",
                                        IdentityCardNo = !string.IsNullOrWhiteSpace(row.Cell(6).GetValue<string>().Trim()) ? row.Cell(6).GetValue<string>().Trim() : "",
                                    };
                                    individuals.Add(individual);
                                }
                                catch (Exception) { }
                            }
                            jsonString = JsonConvert.SerializeObject(individuals, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        }
                    }
                    else
                    {
                        jsonString = "ErrorMessage,No Record Found";
                    }
                }
                
                return jsonString;
            }
            catch (Exception e)
            {
                return "ErrorMessage," + e.Message;
            }

        }
    }
}
