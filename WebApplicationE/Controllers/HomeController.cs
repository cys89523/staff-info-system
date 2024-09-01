using Dbmanger.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using WebApplicationE.ViewModels;
using ClosedXML.Excel;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using WebApplicationE;


namespace WebApplicationE.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        Dbmanager model = new Dbmanager();
        CRUDService.WebService1SoapClient Service = new CRUDService.WebService1SoapClient();
        #region index
        [Authorize]
        [Authorize(Users = "B0462")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            string bubble = Service.BubbleSort(6, 2, 9, 4);
            Dbmanager dbmanager = new Dbmanager();
            List<Card> cards = dbmanager.GetCards();
            ViewBag.cards = cards;
            return View();
           

        }
        #endregion

        #region innerjoin
        [Authorize]
        [Authorize(Users = "B0462,B0463")]
        public ActionResult join()
        {
            Dbmanager dbmanager = new Dbmanager();
            List<Card> cards = dbmanager.JoinCards();
            ViewBag.cards = cards;
            return View();

        }
        #endregion

        #region Leftjoin
        [Authorize]
        [Authorize(Users = "B0462")]
        public ActionResult Ljoin()
        {
            Dbmanager dbmanager = new Dbmanager();
            List<Card> cards = dbmanager.LJoinCards();
            ViewBag.cards = cards;
            return View();
        }
        #endregion

        #region Rightjoin
        [Authorize]
        [Authorize(Users = "B0462")]
        public ActionResult Rjoin()
        {
            Dbmanager dbmanager = new Dbmanager();
            List<Card> cards = dbmanager.RJoinCards();
            ViewBag.cards = cards;
            return View();
        }
        #endregion

        #region CreateRole
        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateRole(Card card)
        {
            Dbmanager dbmanager = new Dbmanager();
            try
            {
                dbmanager.Newchar(card);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditRole
        public ActionResult EditRole(int id)
        {
            Dbmanager dbmanager = new Dbmanager();
            Card card = dbmanager.GetCardById(id);
            return View(card);
        }
        [HttpPost]
        public ActionResult EditRole(Card card)
        {
            Dbmanager dbmanager = new Dbmanager();
            dbmanager.UpdateRole(card);
            return RedirectToAction("Index");
        }
        #endregion

        #region EditRoleAjax
        [HttpPost]
        public JsonResult EditRoleAjax(Card card)
        {
            try
            {

                Dbmanager dbmanager = new Dbmanager();
                dbmanager.UpdateRole(card);


                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region DeleteRole
        public ActionResult DeleteRole(int id)
        {
            Dbmanager dbmanager = new Dbmanager();
            dbmanager.DeleteRoleById(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteRoleAjax(int id)
        {
            try
            {
                Dbmanager dbmanager = new Dbmanager();
                dbmanager.DeleteRoleById(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region EmpData,Result
        public ActionResult EmpData()
        {
            Dbmanager dbManager = new Dbmanager();
            List<Card> idList = dbManager.GetCards(); // 從資料庫獲取 id 資料
            ViewBag.IdList = idList; // 將資料傳遞給 View
            return View();
        }

        [HttpPost]
        public ActionResult EmpDataResult(string empno)
        {
            Card EmpData = new Card();
            try
            {
                EmpData = model.GetEmpData(empno);
                return View(EmpData);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }

        #endregion


        public ActionResult SectorEmp()
        {
            ViewBag.SectorList = model.GetSectorSelectList();
            return View();
        }

        [HttpPost]
        public ActionResult SectorEmpResult(string sta)
        {
            IEnumerable<Card> SectorList = Enumerable.Empty<Card>();
            try
            {

                SectorList = model.GetSectionEmpList(sta);
                ViewBag.title = sta + " 狀態";
                return View(SectorList);
            }
            catch (Exception ex)
            {
                return Content(ex.Message.ToString());
            }
        }


        public ActionResult EmpName()
        {
            return View();
        }

        public string EmpNameResult(string empno)
        {
            string result = "";
            try
            {
                result = model.GetEmpName(empno);
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }

            return result;
        }



        #region Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            Dbmanager dbmanager = new Dbmanager();
            List<Card> cards = dbmanager.JoinCards();
            ViewBag.cards = cards;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(FormCollection post)
        {
            string account = post["account"];
            string password = post["password"];
            string[] uidAry = new string[] { "B0462", "B0463" };
            string[] pwdAry = new string[] { "12345", "456789" };


            //驗證密碼
            if (model.CheckUserData(account, password))
            {
                string nam = model.GetUserName(account);
                Session["NAM"] = nam;
                int index = -1;
                for (int i = 0; i < uidAry.Length; i++)
                {
                    if (uidAry[i] == account && pwdAry[i] == password)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == -1)
                {
                    ViewBag.Err = "帳密錯誤";
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(account, true);
                    return RedirectToAction("Account");
                }


                return View();

            }
            else
            {
                ViewBag.Msg = "登入失敗...";
                Dbmanager dbmanager = new Dbmanager();
                List<Card> cards = dbmanager.JoinCards();
                ViewBag.cards = cards;

                return View();
            }


        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["NAM"] = null;
            return RedirectToAction("Login");
        }

        #endregion
        public ActionResult Account()
        {

            List<AccountViewModel> cards = model.Getacc();
            ViewBag.cards = cards;
            return View();

        }


        #region excel
        private Dbmanager _dbManager = new Dbmanager();
        public FileResult ExportExcel()
        {

        

        var columnNameList = new List<string>
            {
                "id", "nam", "age", "sta"
            };

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "StaffReport.xlsx";

            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Staff");

            for (int i = 1; i <= columnNameList.Count(); i++)
            {
                worksheet.Cell(1, i).Value = columnNameList[i - 1];
                worksheet.Cell(1, i).Style.Font.SetBold();
            }

            List<Card> staffList = _dbManager.GetCards();

            for (int j = 1; j <= staffList.Count(); j++)
            {
                worksheet.Cell(j + 1, 1).Value = staffList[j - 1].id;
                worksheet.Cell(j + 1, 2).Value = staffList[j - 1].nam;
                worksheet.Cell(j + 1, 3).Value = staffList[j - 1].age;
                worksheet.Cell(j + 1, 4).Value = staffList[j - 1].sta;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, contentType, fileName);
            }
        }
        #endregion

        #region pdf
        public FileResult ExportPDF()
        {
            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            string fontPath = Server.MapPath("~/fonts/KAIU.TTF");

         
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, 12);
            document.Open();
        
            // 資料表標題
            PdfPTable table = new PdfPTable(4); 
            table.WidthPercentage = 100; 

            // 標題
            table.AddCell(new PdfPCell(new Phrase("ID", font)));
            table.AddCell(new PdfPCell(new Phrase("Name", font)));
            table.AddCell(new PdfPCell(new Phrase("年齡", font)));
            table.AddCell(new PdfPCell(new Phrase("狀態", font)));

            // 資料
            foreach (var staff in _dbManager.GetCards())
            {
                table.AddCell(new PdfPCell(new Phrase(staff.id.ToString(), font)));
                table.AddCell(new PdfPCell(new Phrase(staff.nam, font)));
                table.AddCell(new PdfPCell(new Phrase(staff.age.ToString(), font)));
                table.AddCell(new PdfPCell(new Phrase(staff.sta, font)));
            }

            document.Add(table);
            document.Close();

            byte[] content = memoryStream.ToArray();
            memoryStream.Close();

            return File(content, "application/pdf", "StaffReport.pdf");
        }
        #endregion

        public ActionResult CallWeb()
        {
            List<Card> cards = null;

            try
            {
                WebService1 webService = new WebService1();
                cards = webService.GetAllData();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            ViewBag.cards = cards;
            
            return View(); 
        }


    }

}
