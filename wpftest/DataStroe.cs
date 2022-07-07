using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using wpftest.Properties;


//*******************************************************************************
//프로그램명    DataStore.cs
//메뉴ID        
//설명          데이터 처리 클래스
//작성일        2012.12.06
//개발자        남인호
//*******************************************************************************
// 변경일자     변경자      요청자      요구사항ID          요청 및 작업내용
//*******************************************************************************
//
//
//*******************************************************************************



namespace KR_POP
{
    /// <summary> 
    /// 데이터 처리 클래스
    /// </summary>
    public class DataStore
    {
        //private string CONN_STR = "Provider=SQLOLEDB.1;Persist Security Info=True;User ID=nanokem;Password=nanokem;Initial Catalog=Nanokem_POP;Data Source=192.168.46.21"; //MSSQL ConnectionString <- 진짜
        private SqlConnection p_Connection;
        private SqlCommand p_Command;
        private const string ConnectionStringDataSource = "Data Source=";
		//private const string ConnectionStringCatalogAndID = ";Initial Catalog=KRBPOP_C;UID=";
        private const string ConnectionStringPWD = ";PWD=";
        private const string ConnectionStringTimeout = "; Connection Timeout=0";
        private string ConnectionString = string.Empty;//"Data Source=192.168.46.20;Initial Catalog=KRBPOP_C;UID=POPAdmin;PWD=qhdlffj_22_; Connection Timeout=180"
        private static DataStore p_dataStore = new DataStore();
        public static DataStore Instance
        {
			get
			{

				if (p_dataStore == null)
				{
					p_dataStore = new DataStore();
				}

				if (p_dataStore.p_Connection == null)
				{
					p_dataStore.p_Connection = new SqlConnection(p_dataStore.ConnectionString);
					p_dataStore.p_Command = p_dataStore.p_Connection.CreateCommand();
				}

				if (p_dataStore.p_Command == null)
				{
					p_dataStore.p_Command = p_dataStore.p_Connection.CreateCommand();
				}

				return p_dataStore;
			}
		}

		public SqlCommand Command
		{
			get { return p_Command; }
		}

        public DataStore()
        {
            p_Connection = new SqlConnection(ConnectionString);
            p_Command = p_Connection.CreateCommand();
			p_Command.CommandTimeout = 60;
        }

        public void SetConnectionString(string ipAddress, string id, string password, string catalog)
        {
            StringBuilder sb = new StringBuilder(ConnectionStringDataSource);
            sb.Append(ipAddress);
			sb.Append(catalog);
            sb.Append(id);
            sb.Append(ConnectionStringPWD);
            sb.Append(password);
            sb.Append(ConnectionStringTimeout);

            ConnectionString = sb.ToString();

			if (string.IsNullOrEmpty(p_Connection.ConnectionString) == false)
			{
				p_Connection.Close();
			}

            p_Connection.ConnectionString = ConnectionString;

			if (p_Connection.State == ConnectionState.Closed)
			{
				p_Connection.Open();
			}
		}

		public void CloseConnection()
		{
			if (p_Connection.State != ConnectionState.Closed)
			{
				if (p_Command.Transaction != null)
				{
					p_Command.Transaction.Rollback();
				}

				p_Connection.Close();
			}
		}

        #region Base Query

        public DataSet QueryToDataSet(string queryString)
        {
            try
            {
                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }

                p_Command.CommandText = queryString;
                p_Command.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(p_Command);
                DataSet dataset = new DataSet();

                adapter.Fill(dataset);

                adapter.Dispose();

                return dataset;
            }
            catch (Exception e)
            {
                throw e;
            }
			//finally
			//{
			//    if (p_Connection.State != ConnectionState.Closed)
			//    {
			//        p_Connection.Close();
			//    }
			//}
        }

        public int QueryToInt32(string queryString)
        {
            try
            {
                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }

                p_Command.CommandText = queryString;
                int retVal = ((Int32?)p_Command.ExecuteScalar()) ?? 0;

                return retVal;
            }
            catch (Exception e)
            {
                throw e;
            }
			//finally
			//{
			//    if (p_Connection.State != ConnectionState.Closed)
			//    {
			//        p_Connection.Close();
			//    }
			//}
        }

        public object QueryToScalar(string queryString)
        {
            try
            {
                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }

                p_Command.CommandText = queryString;
                object retVal = p_Command.ExecuteScalar();

                return retVal;
            }
            catch (Exception e)
            {
                throw e;
            }
			//finally
			//{
			//    if (p_Connection.State != ConnectionState.Closed)
			//    {
			//        p_Connection.Close();
			//    }
			//}
        }

        #endregion



        #region 쿼리공통

        /// <summary>
        /// ProcedureToDataSet
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public DataSet ProcedureToDataSet(string procedureName, Dictionary<string, object> sqlParameter, bool logOn)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }

				if (logOn == true)
				{
					// DB Log를 남긴다.
					StringBuilder trxCommand = new StringBuilder(procedureName);

					if (p_Command.Parameters.Count > 0)
					{
						trxCommand.Append(" ");

						foreach (KeyValuePair<string, object> kvp in sqlParameter)
						{
							trxCommand.Append(kvp.Key + " = " + kvp.Value.ToString());
							trxCommand.Append(", ");
						}

						trxCommand.Remove(trxCommand.Length - 2, 2);
					}

					InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), trxCommand.ToString());
				}

                p_Command.CommandText = procedureName;
                p_Command.CommandType = CommandType.StoredProcedure;
                p_Command.Parameters.Clear();


                if (sqlParameter != null)
                {
                    foreach (KeyValuePair<string, object> kvp in sqlParameter)
                    {
                        p_Command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                    }
                }
               
                SqlDataAdapter adapter = new SqlDataAdapter(p_Command);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset);
                adapter.Dispose();
                

                return dataset;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.MSG_CAPTION_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                //if (p_Connection.State != ConnectionState.Closed)
                //{
                //    p_Connection.Close();
                //}
            }
        }

        /// <summary>
        /// 실행쿼리
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int ExecSQL(string sql, bool logOn)
        {
            if (logOn == true)
            {
                // DB Log를 남긴다.
                InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), sql);
            }

            int value = QueryToInt32(sql);

            return value;
        }

        public object ExecuteScalar(string sql, bool logOn)
        {
            if (logOn == true)
            {
                // DB Log를 남긴다.
                InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), sql);
            }

            object value = QueryToScalar(sql);

            return value;
        }

        public string[] ExecuteQuery(string queryString, bool logOn)
        {
            SqlTransaction transaction = null;

            try
            {

                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }



				if (logOn == true)
				{
					// DB Log를 남긴다.
					InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), queryString);
				}


                p_Command.CommandText = queryString;
                p_Command.CommandType = CommandType.Text;

                transaction = p_Connection.BeginTransaction();
                p_Command.Transaction = transaction;

                string value = p_Command.ExecuteScalar().ToString();
                transaction.Commit();

                return new string[] { Resources.success, value };
            }
            catch (NullReferenceException)
            {
                return new String[] { Resources.success, "NullReferenceException" };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new string[] { Resources.failure, ex.Message };
            }
            finally
            {
                //if (p_Connection.State != ConnectionState.Closed)
                //{
                //    p_Connection.Close();
                //}
            }
        }

		public string[] ExecuteProcedure(string procedureName, Dictionary<string, object> sqlParameter, bool logOn)
		{
            

			SqlTransaction transaction = null;

			try
			{
				if (p_Connection.State == ConnectionState.Closed)
				{
					p_Connection.Open();
				}



				if (logOn == true)
				{
					// DB Log를 남긴다.
                    //StringBuilder trxCommand = new StringBuilder(procedureName);

                    //if (p_Command.Parameters.Count > 0)
                    //{
                    //    trxCommand.Append(" ");

                    //    foreach (KeyValuePair<string, object> kvp in sqlParameter)
                    //    {
                    //        trxCommand.Append(kvp.Key + " = " + kvp.Value.ToString());
                    //        trxCommand.Append(", ");
                    //    }

                    //    trxCommand.Remove(trxCommand.Length - 2, 2);
                    //}

                    //InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), trxCommand.ToString());
				}



				transaction = p_Connection.BeginTransaction();
				p_Command.Transaction = transaction;

				p_Command.CommandText = procedureName;
				p_Command.CommandType = CommandType.StoredProcedure;
				p_Command.Parameters.Clear();


				if (sqlParameter != null)
				{
					foreach (KeyValuePair<string, object> kvp in sqlParameter)
					{
						p_Command.Parameters.AddWithValue(kvp.Key, kvp.Value);
					}
				}

				string value = Convert.ToString(p_Command.ExecuteScalar());

                #region 출하처리에 프로시저 예외처리 0일경우 commit 나머지 rollback
                string[] valueSplit = value.Split(',');
                if (valueSplit.Length > 1)
                {
                    if (valueSplit[0] == "0")
                    {
                        transaction.Commit();
                        return new string[] { "success", value };
                    }
                    else
                    {
                        throw new Exception(value);
                    }
                }
                #endregion

                transaction.Commit();

				return new String[] { Resources.success, value };   //성공! 쿼리에서 리턴값이 있을경우
			}
			catch (NullReferenceException)  //성공! 쿼리에서 리턴값이 없을경우
			{
				if (transaction != null)
				{
					transaction.Commit();
				}

				return new String[] { Resources.success, "NullReferenceException" };
			}
			catch (Exception ex)
			{
				try
				{
					if (transaction != null)
					{
						transaction.Rollback();
					}

					return new string[] { Resources.failure, ex.Message };
				}
				catch (Exception ex1)
				{
					if (transaction != null)
					{
						transaction.Rollback();
					}

					return new string[] { Resources.failure, ex.Message + "/" + ex1.Message };
				}
			}
            finally
            {
                //if (p_Connection.State != ConnectionState.Closed)
                //{
                //    p_Connection.Close();
                //}
            }
		}

		/// <summary>
		/// Procedure에 output Parameter가 있을 때 사용함, 트랜젝션 포함
		/// </summary>
		/// <param name="procedureName">호출할 Procedure 명</param>
		/// <param name="sqlParameter">Procedure로 전달할 변수들</param>
		/// <param name="outputParameters">Output으로 지정된 변수들</param>
		/// <param name="okValues">Output으로 넘어온 값들이 정상인지 판단하는 기준, 트랜젝션 commit/rollback의 기준이 됨</param>
		/// <returns>outputParameter별 값과 Result, Message Key가 추가됨</returns>
		public Dictionary<string, string> ExecuteProcedureOutput(string procedureName, Dictionary<string, object> sqlParameter, List<string> outputParameters, Dictionary<string, string> okValues, bool logOn)
		{
			// Output 결과 값을 넣을 Dictionary
			Dictionary<string, string> outputResult = new Dictionary<string, string>();
			SqlTransaction transaction = null;

			try
			{
				if (p_Connection.State == ConnectionState.Closed)
				{
					p_Connection.Open();
				}



				if (logOn == true)
				{
					// DB Log를 남긴다.
					StringBuilder trxCommand = new StringBuilder(procedureName);

					if (p_Command.Parameters.Count > 0)
					{
						trxCommand.Append(" ");

						foreach (KeyValuePair<string, object> kvp in sqlParameter)
						{
							trxCommand.Append(kvp.Key + " = " + kvp.Value.ToString());
							trxCommand.Append(", ");
						}

						trxCommand.Remove(trxCommand.Length - 2, 2);
					}

					InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), trxCommand.ToString());
				}



				transaction = p_Connection.BeginTransaction();
				p_Command.Transaction = transaction;

				p_Command.CommandText = procedureName;
				p_Command.CommandType = CommandType.StoredProcedure;
				p_Command.Parameters.Clear();


				if (sqlParameter != null)
				{
					foreach (KeyValuePair<string, object> kvp in sqlParameter)
					{
						p_Command.Parameters.AddWithValue(kvp.Key, kvp.Value);
					}

					// Output Parameter 지정 및 output 값 받을 Dictionary 준비
					foreach (string parameter in outputParameters)
					{
						p_Command.Parameters[parameter].Direction = ParameterDirection.Output;
						outputResult.Add(parameter, "");
					}
				}

				string value = Convert.ToString(p_Command.ExecuteScalar());

				//output 값 Dictionary에 저장
				foreach (string parameter in outputParameters)
				{
					outputResult[parameter] = p_Command.Parameters[parameter].Value.ToString();
				}

				// 기준값과 비교하여 트랜젝션을 Commit할 것인지 RollBack할 것인지 결정
				bool isOK = true;

				foreach (KeyValuePair<string, string> kvp in okValues)
				{
					if (outputResult[kvp.Key].Equals(kvp.Value) == false)
					{
						isOK = false;
					}
				}


				if (isOK == true)
				{
					outputResult.Add("Result", "success");
					outputResult.Add("Message", value);
					transaction.Commit();
				}
				else
				{
					outputResult.Add("Result", "failure");
					outputResult.Add("Message", value);
					transaction.Rollback();
				}


				return outputResult;
			}
			catch (NullReferenceException)  //성공! 쿼리에서 리턴값이 없을경우
			{
				//output 값 Dictionary에 저장
				foreach (string parameter in outputParameters)
				{
					outputResult[parameter] = p_Command.Parameters[parameter].Value.ToString();
				}

				// 기준값과 비교하여 트랜젝션을 Commit할 것인지 RollBack할 것인지 결정
				bool isOK = true;

				foreach (KeyValuePair<string, string> kvp in okValues)
				{
					if (outputResult[kvp.Key].Equals(kvp.Value) == false)
					{
						isOK = false;
					}
				}


				if (isOK == true)
				{
					outputResult.Add("Result", "success");
					outputResult.Add("Message", "NullReferenceException");
					transaction.Commit();
				}
				else
				{
					outputResult.Add("Result", "failure");
					outputResult.Add("Message", "NullReferenceException");
					transaction.Rollback();
				}


				return outputResult;
			}
			catch (Exception ex)
			{
				transaction.Rollback();

				try
				{
					outputResult.Clear();
					outputResult.Add("Result", "failure");
					outputResult.Add("Message", ex.Message);
					return outputResult;
				}
				catch (Exception ex1)
				{
					outputResult.Clear();
					outputResult.Add("Result", "failure");
					outputResult.Add("Message", ex1.Message);
					return outputResult;
				}
			}
			finally
			{
				//if (p_Connection.State != ConnectionState.Closed)
				//{
				//    p_Connection.Close();
				//}
			}
		}
		public Dictionary<string, string> ExecuteProcedureOutputNoTran(string procedureName, Dictionary<string, object> sqlParameter, Dictionary<string, int> outputParameters, bool logOn)
		{
			// Output 결과 값을 넣을 Dictionary
			Dictionary<string, string> outputResult = new Dictionary<string, string>();

			try
			{
				if (p_Connection.State == ConnectionState.Closed)
				{
					p_Connection.Open();
				}



				if (logOn == true)
				{
					// DB Log를 남긴다.
					StringBuilder trxCommand = new StringBuilder(procedureName);

					if (p_Command.Parameters.Count > 0)
					{
						trxCommand.Append(" ");

						foreach (KeyValuePair<string, object> kvp in sqlParameter)
						{
							trxCommand.Append(kvp.Key + " = " + kvp.Value.ToString());
							trxCommand.Append(", ");
						}

						trxCommand.Remove(trxCommand.Length - 2, 2);
					}

					InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), trxCommand.ToString());
				}



				p_Command.CommandText = procedureName;
				p_Command.CommandType = CommandType.StoredProcedure;
				p_Command.Parameters.Clear();


				if (sqlParameter != null)
				{
					foreach (KeyValuePair<string, object> kvp in sqlParameter)
					{
						p_Command.Parameters.AddWithValue(kvp.Key, kvp.Value);
					}

					// Output Parameter 지정 및 output 값 받을 Dictionary 준비
					foreach (KeyValuePair<string, int> kvp in outputParameters)
					{
						p_Command.Parameters[kvp.Key].Direction = ParameterDirection.Output;
						p_Command.Parameters[kvp.Key].Size = kvp.Value;
						outputResult.Add(kvp.Key, "");
					}
				}

				string value = Convert.ToString(p_Command.ExecuteScalar());

				//output 값 Dictionary에 저장
				foreach (KeyValuePair<string, int> kvp in outputParameters)
				{
					outputResult[kvp.Key] = p_Command.Parameters[kvp.Key].Value.ToString();
				}

				return outputResult;
			}
			catch (NullReferenceException)  //성공! 쿼리에서 리턴값이 없을경우
			{
				//output 값 Dictionary에 저장
				foreach (KeyValuePair<string, int> kvp in outputParameters)
				{
					outputResult[kvp.Key] = p_Command.Parameters[kvp.Key].Value.ToString();
				}


				return outputResult;
				//return null;
			}
			catch (Exception ex)
			{
				try
				{
					outputResult.Clear();
					List<string> result = new List<string>();
					result.Add("9999");
					result.Add(ex.Message);
					result.Add(ex.StackTrace);

					int i = 0;

					foreach (KeyValuePair<string, int> kvp in outputParameters)
					{
						outputResult[kvp.Key] = result.Count > i ? result[i++] : "";
					}

					return outputResult;
				}
				catch (Exception ex1)
				{
					outputResult.Clear();
					List<string> result = new List<string>();
					result.Add("9998");
					result.Add(ex1.Message);
					result.Add(ex1.StackTrace);

					int i = 0;

					foreach (KeyValuePair<string, int> kvp in outputParameters)
					{
						outputResult[kvp.Key] = result.Count > i ? result[i++] : "";
					}

					return outputResult;
				}
			}
			finally
			{
				//if (p_Connection.State != ConnectionState.Closed)
				//{
				//    p_Connection.Close();
				//}
			}
		}

		/// <summary>
		/// Transaction 없이 프로시져 실행
		/// Parent/Child 함께 적용되어야 할 때 사용
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="sqlParameter"></param>
		/// <param name="logOn"></param>
		/// <returns></returns>
		public string[] ExecuteProcedureWithoutTransaction(string procedureName, Dictionary<string, object> sqlParameter, bool logOn)
		{
            try
            {
                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }




                if (logOn == true)
                {
                    // DB Log를 남긴다.
                    StringBuilder trxCommand = new StringBuilder(procedureName);

                    if (p_Command.Parameters.Count > 0)
                    {
                        trxCommand.Append(" ");

                        foreach (KeyValuePair<string, object> kvp in sqlParameter)
                        {
                            trxCommand.Append(kvp.Key + " = " + kvp.Value.ToString());
                            trxCommand.Append(", ");
                        }

                        trxCommand.Remove(trxCommand.Length - 2, 2);
                    }

                    InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), trxCommand.ToString());
                }




                p_Command.CommandText = procedureName;
                p_Command.CommandType = CommandType.StoredProcedure;
                p_Command.Parameters.Clear();


                if (sqlParameter != null)
                {
                    foreach (KeyValuePair<string, object> kvp in sqlParameter)
                    {
                        p_Command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                    }
                }

                string value = Convert.ToString(p_Command.ExecuteScalar());

                return new String[] { Resources.success, value };   //성공! 쿼리에서 리턴값이 있을경우
            }
            catch (NullReferenceException)  //성공! 쿼리에서 리턴값이 없을경우
            {
                return new String[] { Resources.success, "NullReferenceException" };
            }
            catch (Exception ex)
            {
                try
                {
                    return new string[] { Resources.failure, ex.Message };
                }
                catch (Exception ex1)
                {
                    return new string[] { Resources.failure, ex.Message + "/" + ex1.Message };
                }
            }
		}

        /// <summary>
        /// 트랜잭션 단위로 프로시저 실행
        /// </summary>
        /// <param name="procedureName"> 파라메타 이름</param>
        /// <param name="sqlParameter"> 파라메타 변수</param>
        /// 실패하면 롤백이후 에러메세지
        /// 성공하면 성공 메세지
        public string[] ExecuteTranProcedure(string procedureName, Dictionary<string, object> sqlParameter, Boolean logOn)
        {
            try
            {
                if (p_Connection.State == ConnectionState.Closed)
                {
                    p_Connection.Open();
                }




				if (logOn == true)
				{
					// DB Log를 남긴다.
					StringBuilder trxCommand = new StringBuilder(procedureName);

					if (p_Command.Parameters.Count > 0)
					{
						trxCommand.Append(" ");

						foreach (KeyValuePair<string, object> kvp in sqlParameter)
						{
							trxCommand.Append(kvp.Key + " = " + kvp.Value.ToString());
							trxCommand.Append(", ");
						}

						trxCommand.Remove(trxCommand.Length - 2, 2);
					}

					InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod(), trxCommand.ToString());
				}




                //트랜잭션 단위로 실행
                p_Command.Transaction = p_Command.Connection.BeginTransaction();

                p_Command.CommandText = procedureName;
                p_Command.CommandType = CommandType.StoredProcedure;
                p_Command.Parameters.Clear();


                if (sqlParameter != null)
                {
                    foreach (KeyValuePair<string, object> kvp in sqlParameter)
                    {
                         p_Command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                    }
                }

                string value = p_Command.ExecuteNonQuery().ToString();
                //string value = p_Command.ExecuteScalar().ToString();

                //트랜잭션 commit
                p_Command.Transaction.Commit();

                return new string[] { Resources.success, value };
            }
            catch (NullReferenceException)
            {
				if (p_Command.Transaction != null)
				{
					p_Command.Transaction.Commit();
				}

				return new String[] { Resources.success, "NullReferenceException" };
            }
            catch (Exception ex)
            {
                if (p_Command.Transaction != null)
                {
                    //오류 발생시 Rollback
                    p_Command.Transaction.Rollback();
                }
                return new String[] { Resources.failure, ex.Message };
            }
            finally
            {
                //if (p_Connection.State != ConnectionState.Closed)
                //{
                //    p_Connection.Close();
                //}
            }
        }

        /// <summary>
        /// 스토어프로시져의 이름을 이용하여 파라메터 배열을 만든다
        /// </summary>
        /// <param name="spName">스토어프로시져이름</param>
        /// <param name="includeReturnValueParameter"> RETURN_VALUE 를 파라메터에 포함시킬것인가?</param>
        /// <returns>파라메테 배열</returns>
        public DbParameter[] DiscoverSpParameterSet(string spName, bool includeReturnValueParameter)
        {

            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException("프로시져이름이 없습니다.");
            }

            if (p_Connection.State == ConnectionState.Closed)
            {
                p_Connection.Open();
            }

            //OleDB관련 처리 필요할 수도 있음
            //if (p_Connection is System.Data.OleDb.OleDbConnection) 
            //{
            //    System.Data.OleDb.OleDbCommandBuilder.DeriveParameters((System.Data.OleDb.OleDbCommand)p_Command);
            //}

            // 파라메터에 RETURN_VALUE 를 포함시키지 않는다면 삭제를 한다.
            if (!includeReturnValueParameter)
            {
                p_Command.Parameters.RemoveAt(0);
            }

            DbParameter[] discoveredParameters = new DbParameter[this.p_Command.Parameters.Count];
            p_Command.Parameters.CopyTo(discoveredParameters, 0);

            // 파라메터값을 초기화 한다. DBNull value
            foreach (DbParameter discoveredParameter in discoveredParameters)
            {
                switch (discoveredParameter.DbType)
                {
                    case DbType.String:
                        {
                            discoveredParameter.Value = string.Empty;
                        }
                        break;
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.UInt16:
                    case DbType.UInt64:
                        {
                            discoveredParameter.Value = 0;
                        }
                        break;
                    default:
                        {
                            discoveredParameter.Value = DBNull.Value;
                        }
                        break;
                }

            }

            return discoveredParameters;
        }

        public string AssignParameterValues(DataRow dataRow)
        {
            if ((this.p_Command.Parameters == null) || (dataRow == null))
            {
                return "";
            }

            string rval = "";

            int i = 0;

            foreach (DbParameter commandParameter in this.p_Command.Parameters)
            {
                if (commandParameter.ParameterName == null || commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format(
                            "Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
                            i, commandParameter.ParameterName));

                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName) != -1)
                {
                    //MessageBox.Show(commandParameter.ParameterName + " = " + dataRow[commandParameter.ParameterName].ToString());
                    commandParameter.Value = dataRow[commandParameter.ParameterName];
                    rval = rval + commandParameter.ParameterName + " = " + dataRow[commandParameter.ParameterName].ToString() + ", ";
                }
                i++;
            }
            return rval;
        }

        public int ExecuteNonQuery(string spName, DataRow row, bool logOn)
        {
            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException("spName");
            }


            if (p_Connection.State == ConnectionState.Closed)
            {
                p_Connection.Open();
            }

            this.p_Command.CommandText = spName;
            this.p_Command.CommandType = CommandType.StoredProcedure;


            if (row != null)
            {
                DiscoverSpParameterSet(spName, true);
                AssignParameterValues(row);
                
                int result = this.p_Command.ExecuteNonQuery();

                if (logOn == true)
                {
                    // DB Log를 남긴다.
                    InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod());
                }

                return result;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 스토어프로시져를 실행 시킨 결과를 DataSet으로 돌려 준다.
        /// </summary>
        /// <param name="spName">스토어프로시져이름</param>
        /// <param name="param">파라메터배열</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string procedureName, DbParameter[] param, bool logOn)
        {
            if (procedureName == null || procedureName.Length == 0) throw new ArgumentNullException("Stored Procedure Name이 필요합니다.");


			if (p_Connection.State == ConnectionState.Closed)
			{
				p_Connection.Open();
			}

            p_Command.Parameters.Clear();

            p_Command.CommandText = String.Format("{0}", procedureName);
            p_Command.CommandType = CommandType.StoredProcedure;

            if (param != null)
            {
                Array.ForEach(param, commandParameter => p_Command.Parameters.Add(commandParameter));
            }
            //foreach (DbParameter commandParameter in param)
            //{
            //    _mCmd.Parameters.Add(commandParameter);
            //}

            DataSet ds = null;

            try
            {
                IDbDataAdapter adapter = new SqlDataAdapter((SqlCommand)p_Command);
                
                ds = new DataSet();

                adapter.Fill(ds);

                if (logOn == true)
                {
                    // DB Log를 남긴다.
                    InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod());
                }
            }
            catch (SystemException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw e;
            }
			//finally
			//{
			//    p_Connection.Close();
			//}

            //ds.Tables.Add(result);
            return ds;

        }

        /// <summary>
        /// 테이블과 파라메터 동기화 후 output 파라메터를 테이블에 넣어 준다.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="row"></param>
        /// <param name="flag">다른것과 비교 할려고 만든것 의미 없음</param>
        /// <returns></returns>
        public int ExecuteAsInOk(string spName, DataRow row, string inDate, bool logOn)
        {
            string workDate = string.Empty;

            if (spName == null || spName.Length == 0)
            {
                throw new ArgumentNullException("spName");
            }

            if (p_Connection.State == ConnectionState.Closed)
            {
                p_Connection.Open();
            }

            this.p_Command.CommandText = spName;
            this.p_Command.CommandType = CommandType.StoredProcedure;


            int rval = 1;

            if (row != null)
            {
                try
                {
                    if (logOn == true)
                    {
                        // DB Log를 남긴다.
                        InsertTrxLog(new System.Diagnostics.StackTrace(1, false).GetFrame(0).GetMethod());
                    }


                    DiscoverSpParameterSet(spName, true);
                    AssignParameterValues(row);

                    rval = this.p_Command.ExecuteNonQuery();

                    foreach (DbParameter commandParameter in this.p_Command.Parameters)
                    {
                        if (commandParameter.Direction == ParameterDirection.InputOutput)
                        {
                            //MessageBox.Show(commandParameter.ParameterName + " : " + commandParameter.Direction.ToString());
                            if (row.Table.Columns.IndexOf(commandParameter.ParameterName) != -1)
                            {
                                row[commandParameter.ParameterName] = commandParameter.Value;
                            }
                        }
                    }

                    return rval;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Resources.MSG_CAPTION_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        
        #endregion



		#region Transcation 별도 처리

		public void TransactionBegin()
		{
			if (p_Connection.State == ConnectionState.Closed)
			{
				p_Connection.Open();
			}

			SqlTransaction transaction = p_Connection.BeginTransaction();
			p_Command.Transaction = transaction;
		}

		public void TransactionCommit()
		{
			if (p_Command.Transaction != null)
			{
				p_Command.Transaction.Commit();
			}
#if  TransationCheck
			else
			{
				MessageBox.Show("트랜젝션이 없습니다. (Commit)");
			}
#endif
		}

		public void TransactionRollBack()
		{
			if (p_Command.Transaction != null)
			{
				p_Command.Transaction.Rollback();
			}
#if  TransationCheck
			else
			{
				MessageBox.Show("트랜젝션이 없습니다. (RollBack)");
			}
#endif
		}

		#endregion



        #region TxnLog
        private string[] InsertTrxLog(System.Reflection.MethodBase baseInfo)
        {
            //p_Connection Opne/Close를 하지 않는다.
            try
            {
#if UseTxnLog
                string formName = baseInfo.ReflectedType.Name;
                string functionName = baseInfo.Name;
                StringBuilder trxCommand = new StringBuilder(p_Command.CommandText);

                if (p_Command.Parameters.Count > 0)
                {
                    trxCommand.Append(" ");

                    foreach (SqlParameter param in p_Command.Parameters)
                    {
                        trxCommand.Append(param.ParameterName + " = " + param.Value.ToString());
                        trxCommand.Append(", ");
                    }

                    trxCommand.Remove(trxCommand.Length - 2, 2);
                }


                p_Command.CommandText = "xp_com_TxnLog_i";
                p_Command.CommandType = CommandType.StoredProcedure;
                p_Command.Parameters.Clear();

                p_Command.Parameters.AddWithValue("@TxnYear", DateTime.Today.ToString("yyyy"));
                p_Command.Parameters.AddWithValue("@TxnModule", functionName);
                p_Command.Parameters.AddWithValue("@TxnSource", trxCommand.ToString());
                //p_Command.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                p_Command.Parameters.AddWithValue("@CreatePersonID", Globals.Settings.GetString(Resources.PersonID));
                p_Command.Parameters.AddWithValue("@CreateForm", formName);
                p_Command.Parameters.AddWithValue("@CreateIP", Globals.Settings.GetString(Resources.UserIP));


                string value = p_Command.ExecuteScalar().ToString();
                return new String[] { "success", value };
#else
                return new string[] { string.Empty, string.Empty };
#endif
            }
            catch (NullReferenceException)
            {
                return new String[] { Resources.success, "NullReferenceException" };
            }
            catch (Exception ex)
            {
                return new String[] { Resources.failure, ex.Message };
            }
            //finally
            //{
            //    if (p_Connection.State != ConnectionState.Closed)
            //    {
            //        p_Connection.Close();
            //    }
            //}

        }

        private string[] InsertTrxLog(System.Reflection.MethodBase baseInfo, string sql)
        {
            //p_Connection Opne/Close를 하지 않는다.
            try
            {
#if UseTxnLog
                string formName = baseInfo.ReflectedType.Name;
                string functionName = baseInfo.Name;

                p_Command.CommandText = "xp_com_TxnLog_i";
                p_Command.CommandType = CommandType.StoredProcedure;
                p_Command.Parameters.Clear();

                p_Command.Parameters.AddWithValue("@TxnYear", DateTime.Today.ToString("yyyy"));
                p_Command.Parameters.AddWithValue("@TxnModule", functionName);
                p_Command.Parameters.AddWithValue("@TxnSource", sql);
                p_Command.Parameters.AddWithValue("@CreatePersonID", Globals.Settings.GetString(Resources.PersonID));
                p_Command.Parameters.AddWithValue("@CreateForm", formName);
                p_Command.Parameters.AddWithValue("@CreateIP", Globals.Settings.GetString(Resources.UserIP));

                string value = p_Command.ExecuteScalar().ToString();
                return new String[] { "success", value };
#else
                return new string[] { string.Empty, string.Empty };
#endif
            }
            catch (NullReferenceException)
            {
                return new String[] { Resources.success, "NullReferenceException" };
            }
            catch (Exception ex)
            {
                return new String[] { Resources.failure, ex.Message };
            }
            //finally
            //{
            //    if (p_Connection.State != ConnectionState.Closed)
            //    {
            //        p_Connection.Close();
            //    }
            //}

        }

        #endregion
    }
}
