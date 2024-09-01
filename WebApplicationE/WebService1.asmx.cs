using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Dbmanger.Models;
using System.Web.Mvc;
using System.Web.Security;
using WebApplicationE.ViewModels;


namespace WebApplicationE
{
    /// <summary>
    ///WebService1 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        #region 泡泡排序法
        [WebMethod(Description = "泡泡排序法 ")]
        public string BubbleSort(int A, int B, int C, int D)
        {

            int[] arr = new int[] { A, B, C, D };

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3 - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }

            }
            string ar = arr[0] + " ," + arr[1] + " ," + arr[2] + " ," + arr[3];

            return ar;
        }
        #endregion

        #region 費氏數列
        [WebMethod(Description = "費氏數列 ")]
        public string Fib(int x)
        {
            int[] b = new int[x];
            string FIB = " ";
            b[0] = 0;
            b[1] = 1;


            for (int i = 2; i <= x - 1; i++)
            {
                b[i] = b[i - 1] + b[i - 2];
                FIB = FIB + b[i] + ",";
            }

            return FIB;

        }
        #endregion

        #region BMI
        [WebMethod(Description = "BMI")]
        public float Bmi(float kg, float m)
        {

            float x = kg / (m * m);

            return x;
        }
        #endregion

        #region 取得員工姓名
        [WebMethod(Description = "取得員工姓名")]
        public string GetEmpName(string empno)
        {
            string result = "";
            try
            {
                result = new Dbmanager().GetEmpName(empno);
            }
            catch { }
            return result;
        }
        #endregion

        #region 刪除員工姓名
        [WebMethod(Description = "刪除員工姓名")]
        public List<Card> DeleteName(int id)
        {
            try
            {
                new Dbmanager().DeleteRoleById(id);
                return new Dbmanager().GetCards();

            }
            catch (Exception ex)
            {

                Console.WriteLine("刪除員工姓名發生錯誤：" + ex.Message);
                return null;
            }


        }
        #endregion

        #region 取得所有資料內容
        [WebMethod(Description = "取得所有資料內容")]
        public List<Card> GetAllData()
        {
            try
            {              
                return new Dbmanager().GetCards();
            }
            catch (Exception ex)
            {
                Console.WriteLine("取得所有資料內容發生錯誤：" + ex.Message);
                return null; 
            }
        }
        #endregion

        #region 新增員工
        [WebMethod(Description = "新增員工")]
        public List<Card> AddNewData(int id, string nam, int age, string sta)
        {
            try
            {             
                Card newCard = new Card
                {
                    id = id,
                    nam = nam,
                    age = age,
                    sta = sta
                };
                new Dbmanager().Newchar(newCard);
                return new Dbmanager().GetCards();
            }
            catch (Exception ex)
            {
                Console.WriteLine("新增員工資料並回傳更新後的資料表發生錯誤：" + ex.Message);
                return null;
            }
        }
        #endregion

        #region 修改資料
        [WebMethod(Description = "修改資料")]
        public List<Card> UpdateData(int id, string nam, int age, string sta)
        {
            try
            {
                Card newCard = new Card
                {
                    id = id,
                    nam = nam,
                    age = age,
                    sta = sta
                };
                new Dbmanager().UpdateRole(newCard);
                return new Dbmanager().GetCards();
            }
            catch (Exception ex)
            {
                Console.WriteLine("修改員工資料並回傳更新後的資料表發生錯誤：" + ex.Message);
                return null;
            }
        }
        #endregion
    }
}
