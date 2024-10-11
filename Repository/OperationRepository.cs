using ArmyGrievances.Generics;
using ArmyGrievances.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
namespace ArmyGrievances.Repository
{
    public class OperationRepository : IOperationRepository
    {
        public async Task<UserModel> SP_ValidateUser(string? UserName, IConfiguration configuration, string? Password = "")
        {
            UserModel obj = new UserModel();
            StatusCode status = new StatusCode();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Phone", UserName),
                new SqlParameter("@EncryptPassword", Password)
            };
            DataSet ds = await SqlHelper.ExecuteSetAsync("SP_ValidateUser", 2, CommandType.StoredProcedure, parameters, configuration);
            DataTable dt = ds.Tables[0];
            status.Status = dt.Rows[0]["StatusCode"].ToString();
            status.ErrorMsg = dt.Rows[0]["ErrorMsg"].ToString();
            if (status.Status?.ToUpper().ToString() == "OK")
            {
                dt = ds.Tables[1];
                if (dt.Rows.Count > 0)
                {
                    //obj.UserName = Convert.ToString(dt.Rows[0]["Username"]);
                    obj.id = Convert.ToInt32(dt.Rows[0]["Applicant_Id"]);
                    obj.IsAdmin = (dt.Rows[0]["IsAdmin"].ToString() != "") ? (bool)dt.Rows[0]["IsAdmin"] : false;
                }
            }
            obj.StatusCode = status;
            return obj;
        }
        public async Task<StatusCode> SP_GrievanceManagement(GrievanceModal grievanceModal, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID",grievanceModal.Id),
                new SqlParameter("@Individual_Particular",grievanceModal.Individual_Particular),
                new SqlParameter("@Grievance_ReceptDate", grievanceModal.Grievance_ReceptDate),
                new SqlParameter("@Grienvance_Subject", grievanceModal.Grienvance_Subject),
                new SqlParameter("@Sent_Area", grievanceModal.Sent_Area),
                new SqlParameter("@Regt_Record",grievanceModal.Regt_Record),
                new SqlParameter("@ZSB_MemoNo",grievanceModal.ZSB_MemoNo),
                new SqlParameter("@ZSB_MemoDate",grievanceModal.ZSB_MemoDate),
                new SqlParameter("@Action", grievanceModal.Action),
            };
            bool isInsert = await SqlHelper.ExecuteNonQueryAsync("SP_GrievanceManagement", CommandType.StoredProcedure, parameters, configuration);
            if (isInsert)
            {
                obj.Status = "Ok";
                obj.ErrorMsg = "Grievance Record";
                obj.ErrorMsg += grievanceModal.Action == 1 ? " added Successfully!" : " updated Successfully!";
            }
            else
            {
                obj.Status = "Bad Request";
                obj.ErrorMsg = "Internal Request Error";
            }
            return obj;
        }
        public async Task<IndividualModal> SP_FetchIndividualRecords(IndividualModal individual, IConfiguration configuration)
        {
            IndividualModal obj1 = new IndividualModal();
            StatusCode obj2 = new StatusCode();
            List<IndividualModal> obj = new List<IndividualModal>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                   new SqlParameter("@Action",3),
                   new SqlParameter("@Id", individual.Id),
                   new SqlParameter("@Army_No", individual.Army_No)
            };

            DataTable dt = await SqlHelper.ExecuteTableAsync("SP_GETQueries", CommandType.StoredProcedure, parameters, configuration);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(new IndividualModal
                    {
                        S_No = i++,
                        Id = Convert.ToInt32(dr["Id"]),
                        Army_No = dr["Army_No"].ToString(),
                        Rank = dr["Rank"].ToString(),
                        Name = dr["Name"].ToString(),
                        Address = dr["Address"].ToString(),
                        Mobile_No = dr["Mobile_No"].ToString(),
                    });
                }
                obj1.individuals = obj;
            }
            else
            {
                obj2.ErrorMsg = "No Record Found";

            }
            return obj1;
        }
        public async Task<StatusCode> SP_IndividualManagement(IndividualModal individual, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID",individual.Id),
                new SqlParameter("@Army_No",individual.Army_No),
                new SqlParameter("@Rank", individual.Rank),
                new SqlParameter("@Name", individual.Name),
                new SqlParameter("@Address", individual.Address),
                new SqlParameter("@Regt_Record",individual.Mobile_No),
                new SqlParameter("@Action", individual.Action),
            };
            bool isInsert = await SqlHelper.ExecuteNonQueryAsync("Sp_IndividualManagement", CommandType.StoredProcedure, parameters, configuration);
            if (isInsert)
            {
                obj.Status = "Ok";
                obj.ErrorMsg = "ESM Individual Record";
                obj.ErrorMsg += individual.Action == 1 ? " added Successfully!" : " updated Successfully!";
            }
            else
            {
                obj.Status = "Bad Request";
                obj.ErrorMsg = "Internal Request Error";
            }
            return obj;
        }
        public async Task<GrievanceModal> SP_FetchGrievanceRecords(GrievanceModal grievance, IConfiguration configuration)
        {
            GrievanceModal obj1 = new GrievanceModal();
            StatusCode obj2 = new StatusCode();
            List<GrievanceModal> obj = new List<GrievanceModal>();

            //DateTime datevalue = (Convert.ToDateTime(grievance.Grievance_ReceptDate));

            //string dy = datevalue.Day.ToString();
            //string mn = datevalue.Month.ToString();
            //string yy = datevalue.Year.ToString();
            obj1.Year = grievance.Year;
            obj1.Month = grievance.Month;
            SqlParameter[] parameters = new SqlParameter[]
            {
                   new SqlParameter("@Action",1),
                   new SqlParameter("@Month", grievance.Month),
                   new SqlParameter("@Year",grievance.Year),
                   new SqlParameter("@Regt_Records",grievance.Regt_Record),
                   new SqlParameter("@Id", grievance.Id)
            };

            DataTable dt = await SqlHelper.ExecuteTableAsync("SP_GETQueries", CommandType.StoredProcedure, parameters, configuration);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(new GrievanceModal
                    {
                        S_No = i++,
                        Id = Convert.ToInt32(dr["id"]),
                        Individual_Particular = dr["Individual_Particular"].ToString(),
                        Grievance_ReceptDate = dr["ReceptDate"].ToString(),
                        Grienvance_Subject = dr["Grienvance_Subject"].ToString(),
                        Sent_Area = dr["Sent_Area"].ToString(),
                        Regt_Record = dr["Regt_Record"].ToString(),
                        ZSB_MemoNo = dr["ZSB_MemoNo"].ToString(),
                        ZSB_MemoDate = dr["MemoDate"].ToString(),
                    });
                }
                obj1.grievances = obj;
            }
            else
            {
                obj2.ErrorMsg = "No Record Found";

            }
            return obj1;
        }
        public async Task<StatusCode> SP_CheckExistArmyNo(string id, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            obj.Status = "false";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ArmyNo",id)

            };
            DataTable dt = await SqlHelper.ExecuteTableAsync("Select * from tbl_ESMIndividuals where Army_No like CONCAT('%',@ArmyNo,'%')", CommandType.Text, parameters, configuration);
            if (dt.Rows.Count > 0)
            {
                obj.Status = "true";
            }

            return obj;
        }
        public async Task<StatusCode> SP_CheckExistServiceNo(string id, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            obj.Status = "false";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Service_No",id)

            };
            DataTable dt = await SqlHelper.ExecuteTableAsync("Select * from tbl_MailIn_Out where Service_No like CONCAT('%',@Service_No,'%')", CommandType.Text, parameters, configuration);
            if (dt.Rows.Count > 0)
            {
                obj.Status = "true";
            }

            return obj;
        }
        public async Task<StatusCode> Sp_MailManagement(MainIn_OutModal main, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", main.Mail_Id),
                new SqlParameter("@Item",main.Item),
                new SqlParameter("@MailOut_Date", main.MailOut_Date),
                new SqlParameter("@Subject", main.Subject),
                new SqlParameter("@ToWhom", main.ToWhom),
                new SqlParameter("@Diary_No",main.Diary_No),
                new SqlParameter("@ZSB_MemoNo",main.ZSB_MemoNo),
                new SqlParameter("@Service_No",main.Service_No),
                new SqlParameter("@Name", main.Name),
                new SqlParameter("@Recieving_Date", main.Recieving_Date),
                new SqlParameter("@Addressed_To",main.Addressed_To),
                new SqlParameter("@Copy_To",main.Copy_To),
                new SqlParameter("@Letter_No",main.Letter_No),
                new SqlParameter("@Letter_Date",main.Letter_Date),
                new SqlParameter("@Rank",main.Rank),
                new SqlParameter("@Action", main.Action),
            };
            bool isInsert = await SqlHelper.ExecuteNonQueryAsync("Sp_MailManagement", CommandType.StoredProcedure, parameters, configuration);
            if (isInsert)
            {
                obj.Status = "Ok";
                obj.ErrorMsg = "In/Out Mail Record";
                obj.ErrorMsg += main.Action == 1 ? " added Successfully!" : " updated Successfully!";
            }
            else
            {
                obj.Status = "Bad Request";
                obj.ErrorMsg = "Internal Request Error";
            }
            return obj;
        }
        public async Task<MainIn_OutModal> SP_FetchMailInOutRecords(MainIn_OutModal mainIn_Out, IConfiguration configuration)
        {
            MainIn_OutModal obj1 = new MainIn_OutModal();
            StatusCode obj2 = new StatusCode();
            List<MainIn_OutModal> obj = new List<MainIn_OutModal>();
            obj1.Year = mainIn_Out.Year;
            obj1.Month = mainIn_Out.Month;
            obj1.Service_No = mainIn_Out.Service_No;
            obj1.Letter_No = mainIn_Out.Letter_No;
            obj1.Item = mainIn_Out.Item;
            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@Action",2),
                  new SqlParameter("@Month", mainIn_Out.Month),
                  new SqlParameter("@Year",mainIn_Out.Year),
                  new SqlParameter("@Id",mainIn_Out.Mail_Id),
                  new SqlParameter("@Service_No",mainIn_Out.Service_No?.Trim()),
                  new SqlParameter("@Letter_No",mainIn_Out.Letter_No?.Trim()),
                  new SqlParameter("@Item",mainIn_Out.Item),
            };

            DataTable dt = await SqlHelper.ExecuteTableAsync("SP_GETQueries", CommandType.StoredProcedure, parameters, configuration);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(new MainIn_OutModal
                    {
                        S_No = i++,
                        Mail_Id = Convert.ToInt32(dr["Mail_Id"]),
                        Rank = dr["Rank"].ToString(),
                        Item = dr["Item"].ToString(),
                        MailOut_Date = !string.IsNullOrEmpty(dr["MailOutDate"].ToString()) ? dr["MailOutDate"].ToString() : "",
                        Subject = dr["Subject"].ToString(),
                        ToWhom = dr["ToWhom"].ToString(),
                        ZSB_MemoNo = dr["ZSB_MemoNo"].ToString(),
                        Diary_No = Convert.ToInt64(dr["Diary_No"]),
                        Service_No = dr["Service_No"].ToString(),
                        Name = dr["Name"].ToString(),
                        Recieving_Date = !string.IsNullOrEmpty(dr["RecievingDate"].ToString()) ? dr["RecievingDate"].ToString() : "",
                        Addressed_To = dr["Addressed_To"].ToString(),
                        Copy_To = dr["Copy_To"].ToString(),
                        Letter_No = dr["Letter_No"].ToString(),
                        Letter_Date = !string.IsNullOrEmpty(dr["LetterDate"].ToString()) ? dr["LetterDate"].ToString() : "",
                        LetterPlaced_File = !string.IsNullOrEmpty(dr["LetterPlaced_File"].ToString()) ? Convert.ToBoolean(dr["LetterPlaced_File"]) : false,
                    });
                }
                obj1.mainIn_Outs = obj;
            }
            else
            {
                obj2.ErrorMsg = "No Record Found";

            }
            return obj1;
        }
        public async Task<StatusCode> SP_ExcelManagement(ImportExcelFile importExcel, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@JsonString",importExcel.JsonString),
                new SqlParameter("@Action", importExcel.Action)
                };
                DataTable dt = await SqlHelper.ExecuteTableAsync("Sp_RecordsManagement", CommandType.StoredProcedure, parameters, configuration);
                if (dt.Rows.Count > 0)
                {
                    obj.Status = "Ok";
                    obj.ErrorMsg = dt.Rows[0]["errormessage"].ToString();
                }
                else
                {
                    obj.Status = "Bad Request";
                    obj.ErrorMsg = "Internal Request Error";
                }
            }
            catch (Exception e)
            {
                obj.Status = "Internal Request Error";
                obj.ErrorMsg = e.Message;
            }
            return obj;
        }
        public async Task<StatusCode> SP_DeleteRecords(string ids, int Action, IConfiguration configuration)
        {
            StatusCode obj = new StatusCode();
            obj.Status = "false";
            var Recordids = ids.Split(",");
            string query = "";
            if (Action == 1) { query = "Delete from tbl_GrievanceRecords where id = @id"; }
            else if (Action == 2) { query = "Delete from tbl_MailIn_Out where Mail_Id = @id"; }
            else if (Action == 3) { query = "Delete from tbl_ESMIndividuals where Id = @id"; }
            foreach (var id in Recordids)
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                   new SqlParameter("@id",id)
                };

                bool isDelete = await SqlHelper.ExecuteNonQueryAsync(query, CommandType.Text, parameters, configuration);
                if (isDelete)
                {
                    obj.Status = "true";
                }
            }

            return obj;
        }

        public static string GetMD5Hash(string theInput)
        {
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 hasher = MD5.Create())
            {
                byte[] dbytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(theInput));
                for (int n = 0; n <= dbytes.Length - 1; n++)
                    sBuilder.Append(dbytes[n].ToString("X2"));
            }
            return sBuilder.ToString();
        }

        public string EncryptMD5(string strPlain)
        {
            string strPassword = "";
            for (int i = 0; i <= 9; i++)
            {
                Random rnd = new Random();
                int ofs = rnd.Next(0, 2147483647);
                strPassword = strPassword + ofs.ToString();
            }
            string strSalt = GetMD5Hash(strPassword).Substring(0, 2);
            strPassword = GetMD5Hash(strSalt + strPlain) + ":" + strSalt;
            return strPassword;
        }
        public bool Validate_Password(string? strplain, string strencrypted)
        {
            string[] arr = strencrypted.Split(':');
            if (arr.Length != 2)
                return false;
            string strSalt = arr[1];
            string strCalculated = GetMD5Hash(strSalt + strplain);
            return (strCalculated.ToUpper() == arr[0].ToUpper());
        }
    }
}
