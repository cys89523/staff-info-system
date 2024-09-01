using System.ComponentModel.DataAnnotations;



namespace WebApplicationE.ViewModels
{
   
    public class SectorViewModel
    {
        [Display(Name = "部門編號")]
        public string SecNo { get; set; }

        [Display(Name = "部門姓名")]
        public string SecName { get; set; }
    }

    public class AccountViewModel
    {
        [Display(Name = "員工編號")]
        public string SNo { get; set; }

        [Display(Name = "姓名")]
        public string SName { get; set; }

        [Display(Name = "密碼")]
        public string SPass { get; set; }
        [Display(Name = "登入時間")]
        public string SDate { get; set; }
    }
 
    public class Card
    {
        [Display(Name = "編號")]
        public int id { get; set; }
        [Display(Name = "姓名")]
        public string nam { get; set; }
        [Display(Name = "年齡")]
        public int age { get; set; }
        [Display(Name = "狀態")]
        public string sta { get; set; }
        [Display(Name = "身份證號")]
        public string bor { get; set; }

    }
    public class conn
    {
        public string OracleConn()
        {
            return "data Source =";
           
        }
    }
}