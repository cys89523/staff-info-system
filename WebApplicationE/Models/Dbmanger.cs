using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using WebApplicationE.ViewModels;


namespace Dbmanger.Models
{

    public class Dbmanager
    {
        #region 連線字串
        string OracleConn;
       
        public  Dbmanager()
        {
            conn conmdl = new conn();
            OracleConn = conmdl.OracleConn();
        }
        #endregion

        #region 取得所有資料
        public List<Card> GetCards()
        {
            List<Card> cards = new List<Card>();
            OracleConnection oracleConnection = new OracleConnection(OracleConn);

            try
            {
                string com = "SELECT id,nam,age,sta FROM STAFF ORDER BY id";
                OracleCommand sqlCommand = new OracleCommand(com);
                sqlCommand.Connection = oracleConnection;
                oracleConnection.Open();

                using (OracleDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Card card = new Card
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                nam = reader.GetString(reader.GetOrdinal("nam")),
                                age = reader.GetInt32(reader.GetOrdinal("age")),
                                sta = reader.GetString(reader.GetOrdinal("sta")).Replace("l", "尚在").Replace("d", "已故")
                            };
                            cards.Add(card);
                        }
                    }
                    else
                    {
                        Console.WriteLine("資料庫為空！");
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("資料庫錯誤：" + ex.Message);
                
            }
            finally
            {
                oracleConnection.Close();
            }

            return cards;
        }
        #endregion

        #region 插入新資料
        public void Newchar(Card card)
        {
            OracleConnection oracleConnection = new OracleConnection(OracleConn);

            try
            {
                string cmd = "INSERT INTO STAFF (id, nam, age, sta) VALUES (:id, :nam, :age, :sta)";
                OracleCommand oracleCommand = new OracleCommand(cmd);
                oracleCommand.Connection = oracleConnection;
                oracleCommand.Parameters.Add(new OracleParameter(":id", card.id));
                oracleCommand.Parameters.Add(new OracleParameter(":nam", card.nam));
                oracleCommand.Parameters.Add(new OracleParameter(":age", card.age));
                oracleCommand.Parameters.Add(new OracleParameter(":sta", card.sta));
                oracleConnection.Open();
                oracleCommand.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
               
                Console.WriteLine("An OracleException occurred: " + ex.Message);
            }
            finally
            {
             
                if (oracleConnection.State == ConnectionState.Open)
                    oracleConnection.Close();
            }

        }
        #endregion

        #region 透過id取資料
        public Card GetCardById(int id)
        {
            Card card = new Card();
            OracleConnection oracleConnection = new OracleConnection(OracleConn);
            string cmd = "SELECT id,nam,age,sta FROM STAFF WHERE id = :id";
            OracleCommand oracleCommand = new OracleCommand(cmd);
            oracleCommand.Connection = oracleConnection;
            oracleCommand.Parameters.Add(new OracleParameter(":id", id));
            oracleConnection.Open();

            OracleDataReader reader = oracleCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    card = new Card
                    {
                        id = reader.GetInt32(reader.GetOrdinal("id")),
                        nam = reader.GetString(reader.GetOrdinal("nam")),
                        age = reader.GetInt32(reader.GetOrdinal("age")),
                        sta = reader.GetString(reader.GetOrdinal("sta")).Replace("l", "尚在").Replace("d", "已故"),
                    };

                }
            }
            else
            {
                card.nam = "未找到該筆資料";
            }
            oracleConnection.Close();
            return card;
        }
        #endregion

        #region 更新資料
        public void UpdateRole(Card card)
        {
            OracleConnection oracleConnection = new OracleConnection(OracleConn);
            string cmd = "UPDATE STAFF SET nam=:nam,age=:age,sta=:sta WHERE id = :id ";
            OracleCommand oracleCommand = new OracleCommand(cmd);
            oracleCommand.Connection = oracleConnection;
            oracleCommand.Parameters.Add(new OracleParameter(":nam", card.nam));
            oracleCommand.Parameters.Add(new OracleParameter(":age", card.age));
            oracleCommand.Parameters.Add(new OracleParameter(":sta", card.sta));
            oracleCommand.Parameters.Add(new OracleParameter(":id", card.id));

            oracleConnection.Open();
            oracleCommand.ExecuteNonQuery();
            oracleConnection.Close();

        }
        #endregion

        #region 透過id刪資料
        public void DeleteRoleById(int id)
        {
            OracleConnection oracleConnection = new OracleConnection(OracleConn);
            string cmd = "DELETE FROM STAFF  WHERE STAFF.id = :id";
            OracleCommand oracleCommand = new OracleCommand(cmd);
            oracleCommand.Connection = oracleConnection;
            oracleCommand.Parameters.Add(new OracleParameter(":id", id));
            oracleConnection.Open();
            oracleCommand.ExecuteNonQuery();
            oracleConnection.Close();

        }
        #endregion

        #region 透過id搜名
        public string GetEmpName(string empno)
        {
            string result = "";

            try
            {
                string sql = @"
                                        select id,nam as empname from STAFF where id = :id
                                    ";
                using (OracleConnection con = new OracleConnection(OracleConn))
                using (OracleCommand cmd = new OracleCommand(sql, con) { BindByName = true })
                {
                    con.Open();
                    cmd.Parameters.Add(":id", empno);

                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string id = Convert.ToString(reader["id"]).Trim();
                        string empname = Convert.ToString(reader["empname"]).Trim();
                        result = $"ID: {id}, 姓名: {empname}";
                    }
                    else
                    {
                        result = "false";
                    }
                }
            }

            catch
            { }

            return result;
        }
        #endregion

        #region 透過id取資料
        public Card GetEmpData(string empno)
        {
            Card data = new Card();
            #region sql
            string sql = @"  
                                        select id,nam , age ,sta
                                        from STAFF
                                        where id = :id
                                    ";
            #endregion

            using (OracleConnection con = new OracleConnection(OracleConn))
            using (OracleCommand cmd = new OracleCommand(sql, con) { BindByName = true })
            {
                cmd.Parameters.Add(":id", empno);
                con.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    data.id = Convert.ToInt32(reader["id"]);
                    data.nam = Convert.ToString(reader["nam"]).Trim();
                    data.age = Convert.ToInt32(reader["age"]);
                    data.sta = Convert.ToString(reader["sta"]).Trim().Replace("l", "尚在").Replace("d", "已故");
                }
                con.Close();
            }
            return data;
        }
        #endregion

        #region 選不重複sta
        public List<SelectListItem> GetSectorSelectList()
        {
            IEnumerable<SectorViewModel> List = Enumerable.Empty<SectorViewModel>();

            List<SelectListItem> result = new List<SelectListItem>();

            try
            {
                string sql = @"
                                        select  distinct sta ,sta||'狀態'as name from STAFF
                                        order by sta
                                    ";

                using (DataTable dt = new DataTable())
                {
                    using (OracleConnection conn = new OracleConnection(OracleConn))
                    using (OracleCommand cmd = new OracleCommand(sql, conn) { BindByName = true })
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    List = dt.AsEnumerable().Select(t =>
                    new SectorViewModel
                    {
                        SecNo = Convert.ToString(t["sta"]).Trim(),
                        SecName = Convert.ToString(t["name"]).Trim(),
                    });
                }

                foreach (var item in List)
                {
                    result.Add(new SelectListItem()
                    {
                        Text = item.SecName,
                        Value = item.SecNo,
                        Selected = false
                    });
                }
            }
            catch
            { }

            return result;
        }
        #endregion

        #region 透過sta取資料
        public IEnumerable<Card> GetSectionEmpList(string sta)
        {
            IEnumerable<Card> data = Enumerable.Empty<Card>();
            #region sql
            string sql = @"  
                                        select id , nam ,age, sta 
                                        from STAFF
                                        where  sta= :sta
                                       
                                    ";
            #endregion

            using (OracleConnection con = new OracleConnection(OracleConn))
            using (OracleCommand cmd = new OracleCommand(sql, con) { BindByName = true })
            {
                cmd.Parameters.Add(":sta", sta);
                con.Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                data = dt.AsEnumerable().Select(tt => new Card
                {
                    id = Convert.ToInt32(tt["id"]),
                    nam = Convert.ToString(tt["nam"]),
                    sta = Convert.ToString(tt["sta"]),
                    age = Convert.ToInt32(tt["age"]),
                });
                con.Close();
            }
            return data;
        }
        #endregion

        #region   innerjoin
        public List<Card> JoinCards()
        {
            List<Card> cards = new List<Card>();
            OracleConnection oracleConnection = new OracleConnection(OracleConn);
            string com = "SELECT STAFF.id, STAFF.nam, STAFF.age, STAFF.sta, SIdentity.bor " +
                "FROM STAFF INNER JOIN SIdentity ON STAFF.id = SIdentity.id ORDER BY STAFF.id";
            OracleCommand sqlCommand = new OracleCommand(com);
            sqlCommand.Connection = oracleConnection;
            try
            {
                oracleConnection.Open();
                OracleDataReader reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Card card = new Card
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            nam = reader.GetString(reader.GetOrdinal("nam")),
                            age = reader.GetInt32(reader.GetOrdinal("age")),
                            sta = reader.GetString(reader.GetOrdinal("sta")).Replace("l", "尚在").Replace("d", "已故"),
                            bor = reader.GetString(reader.GetOrdinal("bor")),
                        };
                        cards.Add(card);
                    }
                }
                else
                {
                    Console.WriteLine("資料庫為空！");
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("發生資料庫錯誤：" + ex.Message);
            }
            catch (Exception ex)
            {
             
                Console.WriteLine("發生未知錯誤：" + ex.Message);
            }
            finally
            {
                oracleConnection.Close();
            }
            return cards;
        }
        #endregion

        #region  Leftjoin
        public List<Card> LJoinCards()
        {
            List<Card> cards = new List<Card>();
            OracleConnection oracleConnection = new OracleConnection(OracleConn);
            string com = "SELECT STAFF.id, STAFF.nam, STAFF.age, STAFF.sta,  SIdentity.bor " +
                "FROM STAFF  LEFT JOIN SIdentity ON STAFF.id=SIdentity.id ORDER BY STAFF.id";
            OracleCommand sqlCommand = new OracleCommand(com);
            sqlCommand.Connection = oracleConnection;

            try
            {
                oracleConnection.Open();
                OracleDataReader reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Card card = new Card
                        {
                            id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                            nam = reader.IsDBNull(reader.GetOrdinal("nam")) ? string.Empty : reader.GetString(reader.GetOrdinal("nam")),
                            age = reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32(reader.GetOrdinal("age")),
                            sta = reader.IsDBNull(reader.GetOrdinal("sta")) ? string.Empty : reader.GetString(reader.GetOrdinal("sta")).Replace("l", "尚在").Replace("d", "已故"),
                            bor = reader.IsDBNull(reader.GetOrdinal("bor")) ? string.Empty : reader.GetString(reader.GetOrdinal("bor")),
                        };
                        cards.Add(card);
                    }
                }
                else
                {
                    Console.WriteLine("資料庫為空！");
                }
            }
            catch (OracleException ex)
            {
               
                Console.WriteLine("發生資料庫錯誤：" + ex.Message);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("發生未知錯誤：" + ex.Message);
            }
            finally
            {
                oracleConnection.Close();
            }

            return cards;
        }
        #endregion

        #region Rightjoin
        public List<Card> RJoinCards()
        {
            List<Card> cards = new List<Card>();
            OracleConnection oracleConnection = new OracleConnection(OracleConn);
            string com = "SELECT STAFF.id, STAFF.nam, STAFF.age, STAFF.sta,  SIdentity.bor " +
                "FROM STAFF  RIGHT JOIN SIdentity ON STAFF.id=SIdentity.id ORDER BY STAFF.id";
            OracleCommand sqlCommand = new OracleCommand(com);
            sqlCommand.Connection = oracleConnection;
            oracleConnection.Open();

            OracleDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Card card = new Card
                    {
                        id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                        nam = reader.IsDBNull(reader.GetOrdinal("nam")) ? string.Empty : reader.GetString(reader.GetOrdinal("nam")),
                        age = reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32(reader.GetOrdinal("age")),
                        sta = reader.IsDBNull(reader.GetOrdinal("sta")) ? string.Empty : reader.GetString(reader.GetOrdinal("sta")).Replace("l", "尚在").Replace("d", "已故"),
                        bor = reader.IsDBNull(reader.GetOrdinal("bor")) ? string.Empty : reader.GetString(reader.GetOrdinal("bor")),
                    };
                    cards.Add(card);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            oracleConnection.Close();
            return cards;
        }
        #endregion

        #region 檢查帳密
        public bool CheckUserData(string account, string password)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(OracleConn))
                {
                    conn.Open();                  
                    string strSQL = "SELECT 1 FROM SAccount WHERE STID = :account AND PASS = :password";
                    OracleCommand cmd = new OracleCommand(strSQL, conn);
                    cmd.Parameters.Add(":account", OracleDbType.Varchar2).Value = account;
                    cmd.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LogLoginTime(account);
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }
        #endregion

        #region 取得SAccount所有資料
        public List< AccountViewModel> Getacc()
        {
            List<AccountViewModel> cards = new List<AccountViewModel>();
            OracleConnection oracleConnection = new OracleConnection(OracleConn);

            string com = "SELECT * FROM SAccount ";
            OracleCommand sqlCommand = new OracleCommand(com);
            sqlCommand.Connection = oracleConnection;
            oracleConnection.Open();

            OracleDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    AccountViewModel card = new AccountViewModel
                    {
                        SNo = reader.GetString(reader.GetOrdinal("STID")),
                        SPass = reader.GetString(reader.GetOrdinal("PASS")),
                        SName = reader.GetString(reader.GetOrdinal("NAM")),
                        SDate= reader.GetString(reader.GetOrdinal("DAT")),

                    };
                    cards.Add(card);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            oracleConnection.Close();
            return cards;
        }

        #endregion

        #region 記錄登入時間
        public void LogLoginTime(string account)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(OracleConn))
                {
                    conn.Open();

                   
                    string strSQL = "UPDATE SAccount SET DAT = SYSTIMESTAMP WHERE STID = :account";
                    OracleCommand cmd = new OracleCommand(strSQL, conn);
                    cmd.Parameters.Add(":account", OracleDbType.Varchar2).Value = account;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
              
                string error = ex.ToString();
            }
        }
        #endregion

        #region 取得登入者姓名
        public string GetUserName(string account)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(OracleConn))
                {
                    conn.Open();

                    string strSQL = "SELECT NAM FROM SAccount WHERE STID = :account";
                    OracleCommand cmd = new OracleCommand(strSQL, conn);
                    cmd.Parameters.Add(":account", OracleDbType.Varchar2).Value = account;

                    return cmd.ExecuteScalar()?.ToString(); 
                }
            }
            catch (Exception ex)
            {
             
                string error = ex.ToString();
                return null;
            }
        }
        #endregion
    }
}