
using ArmyGrievances.Models;

namespace ArmyGrievances.Repository
{
    public interface IOperationRepository
    {

        Task<UserModel> SP_ValidateUser(string? UserName, IConfiguration configuration, string? Password = "");
        Task<GrievanceModal> SP_FetchGrievanceRecords(GrievanceModal grievance, IConfiguration configuration);
        Task<StatusCode> SP_ExcelManagement(ImportExcelFile importExcel, IConfiguration configuration);
        Task<MainIn_OutModal> SP_FetchMailInOutRecords(MainIn_OutModal mainIn_Out, IConfiguration configuration);
        Task<StatusCode> SP_GrievanceManagement(GrievanceModal grievanceModal, IConfiguration configuration);
        Task<StatusCode> Sp_MailManagement(MainIn_OutModal main, IConfiguration configuration);
        Task<StatusCode> SP_DeleteRecords(string ids, int Action, IConfiguration configuration);
        Task<IndividualModal> SP_FetchIndividualRecords(IndividualModal individual, IConfiguration configuration);
        Task<StatusCode> SP_CheckExistArmyNo(string id, IConfiguration configuration);
        Task<StatusCode> SP_IndividualManagement(IndividualModal individual, IConfiguration configuration);
        Task<StatusCode> SP_CheckExistServiceNo(string id, IConfiguration configuration);
    }
}
