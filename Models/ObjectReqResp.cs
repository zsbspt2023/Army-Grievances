using System.ComponentModel.DataAnnotations;

namespace ArmyGrievances.Models
{
    public class UserModel
    {
        public int id { get; set; } = 0;
        public string UserName { get; set; } = "";
        public string? lastname { get; set; } = "";
        public string? Password { get; set; } = "";
        public bool IsAdmin { get; set; } = false;
        public string? Email { get; set; }
        public string? Designation { get; set; } = "";
        public int Sno { get; set; } = 0;
        public string? Phone { get; set; } = "";
        public StatusCode? StatusCode { get; set; }
        public List<UserModel>? Users { get; set; }
        public string? IsPasswordReset { get; set; }
        public bool GlobalAdmin { get; set; } = false;
        public bool SuperAdmin { get; set; } = false;
    }
    public class ImportExcelFile
    {
        public string? JsonString { get; set; }
        public string? Action { get; set; }
        public StatusCode? Status { get; set; }
    }
    public class GrievanceModal
    {
        public int S_No { get; set; }
        public int Id { get; set; }
        public string? Individual_Particular { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Grievance_ReceptDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Year { get; set; } = "0";
        public string? Grienvance_Subject { get; set; }
        public string? Sent_Area { get; set; }
        public string? Regt_Record { get; set; } = "";
        public string? ZSB_MemoNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? ZSB_MemoDate { get; set; }
        public string? Month { get; set; } = "0";
        public int Action {  get; set; }
        public List<GrievanceModal> grievances { get; set; }
    }
    public class GrievanceExcel
    {
        public int S_No { get; set; }
        public string? Individual_Particular { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Grievance_Recept_Date { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Grienvance_Subject { get; set; }
        public string? Sent_To_Whom { get; set; }
        public string? Regt_Record { get; set; } = "";
        public string? ZSB_Memo_No { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? ZSB_Memo_Date { get; set; }
    }
    public class MainIn_OutModal
    {
        public int S_No { get; set; }
        public int Mail_Id { get; set; }
        public string? Item { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? MailOut_Date { get; set; }
        public string? Subject { get; set; }
        public string? ToWhom { get; set; }
        public string? ZSB_MemoNo { get; set; }
        public long Diary_No { get; set; }
        public string? Service_No { get; set; }
        public string? Name { get; set; }
        public string? Rank { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Recieving_Date { get; set; }
        public string? Addressed_To { get; set; }
        public string? Copy_To { get; set; }
        public string? Letter_No { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Letter_Date { get; set; }
        public bool LetterPlaced_File { get; set; }
        public string? Year { get; set; } = "0";
        public string? Month { get; set; } = "0";
        public int Action {  get; set; }
        public List<MainIn_OutModal> mainIn_Outs { get; set; }
    }
    public class MainIn_OutExcel
    {
        public int S_No { get; set; }
        public string? File_Name { get; set; }
        
        public string? Letter_No { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Letter_Date { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string? Mail_Out_Date { get; set; }
     
        public string? Service_No { get; set; }
        public string? Name { get; set; }
        public string? Rank { get; set; }
        public string? To_Whom { get; set; }
        public string? Subject { get; set; }
       
        //public string? ZSB_Memo_No { get; set; }
        //public long Diary_No { get; set; }
    
        
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        //public string? Recieving_Date { get; set; }
        //public string? Addressed_To { get; set; }
        
       
 
        //public bool Letter_Placed_File { get; set; }
    }
    public class IndividualModal
    {
        public int S_No { get; set; }
        public int Id { get; set; }
        public string?  Army_No { get; set; }
        public string? Rank { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Mobile_No { get; set; }
        public int Action { get; set; }
        public List<IndividualModal> individuals { get; set; }
    }
    public class IndividualExcel
    {
        public int S_No { get; set; }
        public string? Army_No { get; set; }
        public string? Rank { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Mobile_No { get; set; }
    }
    public class SelectionList
    {
        public string? Text { get; set; }
        public string? Value { get; set; }
        public List<SelectionList>? selectionLists { get; set; }
    }
    public class StatusCode
    {
        public string? Status { get; set; } = "";
        public string? ErrorMsg { get; set; } = "";
    }
}
