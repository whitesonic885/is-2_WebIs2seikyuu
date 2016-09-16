using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Services;
using Oracle.DataAccess.Client;

namespace is2seikyuu
{
	/// <summary>
	/// [is2seikyuu]
	/// </summary>
	//--------------------------------------------------------------------------
	// 修正履歴
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 東都）高木 オブジェクトの破棄
	//	disposeReader(reader);
	//	reader = null;
	//--------------------------------------------------------------------------
	// DEL 2007.05.10 東都）高木 未使用関数のコメント化
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//--------------------------------------------------------------------------
	// MOD 2007.07.26 東都）高木 依頼主に登録がないにもかかわらず請求先が削除できない
	//--------------------------------------------------------------------------
	// MOD 2008.11.26 東都）高木 部課コードが空白でもエラーがでなくする 
	//--------------------------------------------------------------------------
	// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 
	//--------------------------------------------------------------------------

	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2seikyuu")]

	public class Service1 : is2common.CommService
	{
		public Service1()
		{
			//CODEGEN: この呼び出しは、ASP.NET Web サービス デザイナで必要です。
			InitializeComponent();

			connectService();
		}

		#region コンポーネント デザイナで生成されたコード 
		
		//Web サービス デザイナで必要です。
		private IContainer components = null;
				
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion


		/*********************************************************************
		 * 請求先マスタ一覧取得
		 * 引数：会員ＣＤ、部門ＣＤ
		 * 戻値：ステータス、一覧（郵便番号、得意先ＣＤ）...
		 *
		 *********************************************************************/
// ADD 2005.05.11 東都）高木 ORA-03113対策？ START
		private static string GET_CLAIM_SELECT_2
			= "SELECT 得意先ＣＤ, 得意先部課ＣＤ, 得意先部課名, 更新日時 \n"
			+ " FROM ＳＭ０４請求先 \n";
		private static string GET_CLAIM_SELECT_2_ORDER
			=   " AND 削除ＦＧ = '0' \n"
			+ " ORDER BY 得意先ＣＤ, 得意先部課ＣＤ \n";
// ADD 2005.05.11 東都）高木 ORA-03113対策？ END
		[WebMethod]
		public string[] Get_Claim(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先マスタ一覧取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[2];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			string s郵便番号 = "";
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT 郵便番号 \n"
					+  " FROM ＣＭ０２部門 \n"
					+ " WHERE 会員ＣＤ = '" + sKcode + "' \n"
					+ "   AND 部門ＣＤ = '" + sBcode + "' \n"
					+    "AND 削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				int iCnt = 1;
				while (reader.Read())
				{
					s郵便番号 = reader.GetString(0).Trim();
					iCnt++;
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// ADD 2005.05.11 東都）高木 請求先が０件時の対応 START
				sRet[1] = s郵便番号;
// ADD 2005.05.11 東都）高木 請求先が０件時の対応 END
				if(iCnt == 1) 
					sRet[0] = "該当データがありません";
				else
					sRet[0] = "正常終了";
				logWriter(sUser, INF, sRet[0]);

				cmdQuery
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//					= "SELECT '|' || TRIM(得意先ＣＤ) || '|' "
//					+     "|| TRIM(得意先部課ＣＤ) || '|' "
//					+     "|| TRIM(得意先部課名)   || '|' "
//					+     "|| TO_CHAR(更新日時) || '|' \n"
//					+  " FROM ＳＭ０４請求先 "
//					+ " WHERE 郵便番号 = '" + s郵便番号 + "' \n"
//					+   " AND 会員ＣＤ = '" + sKcode + "' \n"
//					+   " AND 削除ＦＧ = '0' \n"
//					+ " ORDER BY 得意先ＣＤ, 得意先部課ＣＤ \n";
					= GET_CLAIM_SELECT_2
					+ " WHERE 郵便番号 = '" + s郵便番号 + "' \n"
					+   " AND 会員ＣＤ = '" + sKcode + "' \n"
					+ GET_CLAIM_SELECT_2_ORDER;
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END

				reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
// MOD 2005.05.11 東都）高木 ORA-03113対策？ START
//					sList.Add(reader.GetString(0));
					sList.Add("|" + reader.GetString(0).Trim()
							+ "|" + reader.GetString(1).Trim()
							+ "|" + reader.GetString(2).Trim()
							+ "|" + reader.GetDecimal(3).ToString().Trim()
							+ "|");
// MOD 2005.05.11 東都）高木 ORA-03113対策？ END
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				sRet = new string[sList.Count + 2];
// ADD 2005.05.11 東都）高木 請求先が０件時の対応 START
				sRet[1] = s郵便番号;
// ADD 2005.05.11 東都）高木 請求先が０件時の対応 END
				if(sList.Count == 0) 
					sRet[0] = "該当データがありません";
				else
				{
					sRet[0] = "正常終了";
					sRet[1] = s郵便番号;
					iCnt = 2;
					IEnumerator enumList = sList.GetEnumerator();
					while(enumList.MoveNext())
					{
						sRet[iCnt] = enumList.Current.ToString();
						iCnt++;
					}
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * 請求先マスタ一覧取得
		 * 引数：会員ＣＤ、部門ＣＤ,得意先ＣＤ,部課ＣＤ
		 * 戻値：ステータス、一覧（郵便番号、得意先ＣＤ）...
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Sel_Claim(string[] sUser, string sKcode, string sBcode, string sTcode, string sTBcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先マスタ情報取得開始");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[2];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT SM04.得意先部課名 \n"
					+  " FROM ＣＭ０２部門 CM02, ＳＭ０４請求先 SM04 \n"
					+ " WHERE CM02.会員ＣＤ = '" + sKcode + "' \n"
					+    "AND CM02.部門ＣＤ = '" + sBcode + "' \n"
					+    "AND CM02.郵便番号 = SM04.郵便番号 \n"
					+    "AND CM02.会員ＣＤ = SM04.会員ＣＤ \n"
					+    "AND SM04.得意先ＣＤ = '" + sTcode + "' \n"
					+    "AND SM04.得意先部課ＣＤ = '" + sTBcode + "' \n"
					+    "AND CM02.削除ＦＧ = '0' \n"
					+    "AND SM04.削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				int iCnt = 1;
				while (reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();
					iCnt++;
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
				if(iCnt == 1) 
					sRet[0] = "該当データがありません";
				else
					sRet[0] = "正常終了";
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * 請求先マスタ存在チェック
		 * 引数：郵便番号、得意先ＣＤ、得意先部課ＣＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_seikyuucnt(string[] sUser, string sYubin, string sTcode, string sTbcode)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先マスタ存在チェック開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END
// MOD 2008.11.26 東都）高木 部課コードが空白でもエラーがでなくする START
			if(sTbcode.Length == 0) sTbcode = " ";
// MOD 2008.11.26 東都）高木 部課コードが空白でもエラーがでなくする END

			string cmdQuery = "";
			try
			{
				cmdQuery
//					= "SELECT TO_CHAR(COUNT(*)) "
//					= "SELECT NVL(COUNT(*),0) "
//					= "SELECT COUNT(*) "
					= "SELECT COUNT(ROWID) "
					+   "FROM ＳＭ０４請求先 "
					+  "WHERE 郵便番号       = '" + sYubin + "' "
					+    "AND 得意先ＣＤ     = '" + sTcode + "' "
					+    "AND 得意先部課ＣＤ = '" + sTbcode + "' "
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
					+    "AND 会員ＣＤ = '" + sUser[0] + "' "
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
					+    "AND 削除ＦＧ = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				int iCnt = 1;
				while (reader.Read())
				{
//					sRet[0] = reader.GetString(0);
					sRet[0] = reader.GetDecimal(0).ToString().Trim();
					iCnt++;
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
//				if(iCnt == 1) 
//					sRet[0] = "0";
//				else
//					sRet[0] = "1";
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * 請求先マスタ追加
		 * 引数：郵便番号、得意先ＣＤ、得意先部課ＣＤ...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Ins_Claim(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先マスタ追加開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = "";
			try
			{
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
//				cmdQuery 
//					= "DELETE FROM ＳＭ０４請求先 \n"
//					+ " WHERE 郵便番号           = '" + sKey[0] +"' \n"
//					+ "   AND 得意先ＣＤ         = '" + sKey[1] +"' \n"
//					+ "   AND 得意先部課ＣＤ     = '" + sKey[2] +"' \n"
//// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
//					+ "   AND 会員ＣＤ           = '" + sUser[0] + "' "
//// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
//					+ "   AND 削除ＦＧ           = '1'";
//
//				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
				// 他の会員で同じ、郵便番号、請求先ＣＤで登録していた場合
				// 他の会員で同じ、発店ＣＤ、請求先ＣＤで登録していた場合
				string s削除ＦＧ = "";
				cmdQuery 
					= "SELECT 削除ＦＧ \n"
					+ " FROM ＳＭ０４請求先 \n"
					+ " WHERE 郵便番号 = '" + sKey[0] +"' \n"
					+ " AND 得意先ＣＤ = '" + sKey[1] +"' \n"
					+ " AND 得意先部課ＣＤ = '" + sKey[2] +"' \n"
					+ " AND 会員ＣＤ = '" + sKey[4] + "' "
					;
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read()){
					s削除ＦＧ = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;
				if(s削除ＦＧ == "1"){
					cmdQuery
						= "UPDATE ＳＭ０４請求先 SET \n"
						+ " 得意先部課名 = '" + sKey[3] + "' \n"
//						+ ",会員ＣＤ = '" + sKey[4] + "' \n"
						+ ",削除ＦＧ = '0' \n"
						+ ",登録日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ ",登録ＰＧ = '" + sKey[5] + "' \n"
						+ ",登録者 = '" + sKey[6] + "' \n"
						+ ",更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ ",更新ＰＧ = '" + sKey[5] + "' \n"
						+ ",更新者 = '" + sKey[6] + "' \n"
						+ " WHERE 郵便番号 = '" + sKey[0] +"' \n"
						+ " AND 得意先ＣＤ = '" + sKey[1] +"' \n"
						+ " AND 得意先部課ＣＤ = '" + sKey[2] +"' \n"
						+ " AND 会員ＣＤ = '" + sKey[4] + "' "
						;
				}else{
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
					//追加
					cmdQuery
						= "INSERT INTO ＳＭ０４請求先 \n"
						+ " VALUES ('" + sKey[0] + "' " 
						+         ",'" + sKey[1] + "' "
						+         ",'" + sKey[2] + "' "
						+         ",'" + sKey[3] + "' "
						+         ",'" + sKey[4] + "' "
						+         ",'0' "
						+         ",TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
						+         ",'" + sKey[5] + "' "
						+         ",'" + sKey[6] + "' "
						+         ",TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
						+         ",'" + sKey[5] + "' "
						+         ",'" + sKey[6] + "' "
						+ " ) \n";
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
				}
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
				CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "正常終了";
				
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
// DEL 2005.05.31 東都）高木 不要な為削除 START
//				string sErr = ex.Message.Substring(0,9);
//				if(sErr == "ORA-00001")
//					sRet[0] = "同一のコードが既に他の端末より登録されています。\r\n再度、最新データを呼び出して更新してください。";
//				else
// DEL 2005.05.31 東都）高木 不要な為削除 END
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * 請求先マスタ更新
		 * 引数：郵便番号、得意先ＣＤ、得意先部課ＣＤ...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Upd_Claim(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先マスタ更新開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = "";
			try
			{
				cmdQuery
					= "UPDATE ＳＭ０４請求先 \n"
					+   " SET 郵便番号 = '" + sKey[0] + "' "
					+       ",得意先部課名 = '" + sKey[3] + "' " 
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
//					+       ",会員ＣＤ = '" + sKey[4] + "' "
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
					+       ",更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",更新ＰＧ = '" + sKey[5] + "' "
					+       ",更新者 = '" + sKey[6] + "' \n"
					+ " WHERE 郵便番号 = '" + sKey[0] + "' \n"
					+   " AND 得意先ＣＤ = '" + sKey[1] + "' \n"
					+   " AND 得意先部課ＣＤ = '" + sKey[2] + "' \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
					+   " AND 会員ＣＤ = '" + sKey[4] + "' \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
// MOD 2007.02.09 東都）高木 誤りのチェック START
//					+   " AND 更新日時 = '" + sKey[7] + "' \n"
					+   " AND 更新日時 = " + sKey[7] + " \n"
// MOD 2007.02.09 東都）高木 誤りのチェック END
					+   " AND 削除ＦＧ = '0' \n";

				if (CmdUpdate(sUser, conn2, cmdQuery) != 0)
				{
					tran.Commit();
					sRet[0] = "正常終了";
				}
				else
				{
					tran.Rollback();
					sRet[0] = "データ編集中に他の端末より更新されています。\r\n再度、最新データを呼び出して更新してください。";
				}
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * 請求先マスタ削除
		 * 引数：郵便番号、得意先ＣＤ、得意先部課ＣＤ...
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Del_Claim(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先マスタ削除開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 START
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 東都）伊賀 会員チェック追加 END
// MOD 2008.11.26 東都）高木 部課コードが空白でもエラーがでなくする START
			if(sKey[2].Length == 0) sKey[2] = " ";
// MOD 2008.11.26 東都）高木 部課コードが空白でもエラーがでなくする END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = "";
			try
			{
				cmdQuery
					= "UPDATE ＳＭ０４請求先 \n"
					+   " SET 削除ＦＧ = '1' " 
					+       ",更新日時 = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",更新ＰＧ = '" + sKey[3] + "' "
					+       ",更新者 = '" + sKey[4] + "' \n"
					+ " WHERE 郵便番号 = '" + sKey[0] + "' \n"
					+   " AND 得意先ＣＤ = '" + sKey[1] + "' \n"
					+   " AND 得意先部課ＣＤ = '" + sKey[2] + "' \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 START
					+   " AND 会員ＣＤ = '" + sUser[0] + "' \n"
// MOD 2011.03.09 東都）高木 請求先マスタの主キーに[会員ＣＤ]を追加 END
					;

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "正常終了";
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				tran.Rollback();
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				tran.Rollback();
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			return sRet;
		}

// ADD 2005.11.07 東都）伊賀 依頼主チェック追加 START
		/*********************************************************************
		 * 請求先使用中チェック
		 * 引数：会員ＣＤ、得意先ＣＤ、得意先部課ＣＤ
		 * 戻値：ステータス
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Sel_IrainusiSeikyuu(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//			logFileOpen(sUser);
			logWriter(sUser, INF, "請求先使用中チェック開始");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// ＤＢ接続
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
				sRet[0] = "ＤＢ接続エラー";
				return sRet;
			}
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//
//			// 会員チェック
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			try
			{
// MOD 2007.07.26 東都）高木 依頼主に登録がないにもかかわらず請求先が削除できない START
//				string cmdQuery 
//					= "SELECT COUNT(*) \n" 
//					+   "FROM ＳＭ０１荷送人 \n"
//					+ " WHERE 会員ＣＤ           = '" + sData[0] +"' \n"
//					+ "   AND 得意先ＣＤ         = '" + sData[1] +"' \n"
//					+ "   AND 得意先部課ＣＤ     = '" + sData[2] +"'"
//					+ "   AND 削除ＦＧ           = '0' \n";

				string cmdQuery = "SELECT COUNT(*) \n";

				if(sData.Length < 4)
				{
					cmdQuery 
						= "SELECT /*+ INDEX(SM01 SM01PKEY) */ \n"
						+ " COUNT(*) \n"
						+ " FROM ＳＭ０１荷送人 SM01 \n"
						+ " WHERE SM01.会員ＣＤ = '" + sData[0] +"' \n"
						+ " AND SM01.得意先ＣＤ = '" + sData[1] +"' \n"
						+ " AND SM01.得意先部課ＣＤ = '" + sData[2] +"' \n"
						+ " AND SM01.削除ＦＧ = '0' \n"
						;
				}
				else
				{
					cmdQuery 
						= "SELECT /*+ INDEX(SM01 SM01PKEY) INDEX(CM02 CM02PKEY) */ \n"
						+ " COUNT(*) \n"
						+ " FROM ＳＭ０１荷送人 SM01 \n"
						+ " , ＣＭ０２部門 CM02 \n"
						+ " WHERE SM01.会員ＣＤ = '" + sData[0] +"' \n"
						+ " AND SM01.得意先ＣＤ = '" + sData[1] +"' \n"
						+ " AND SM01.得意先部課ＣＤ = '" + sData[2] +"' \n"
						+ " AND SM01.削除ＦＧ = '0' \n"
						+ " AND SM01.会員ＣＤ = CM02.会員ＣＤ \n"
						+ " AND SM01.部門ＣＤ = CM02.部門ＣＤ \n"
						+ " AND CM02.削除ＦＧ = '0' \n"
						+ " AND CM02.郵便番号 = '" + sData[3] +"' \n"
						;
				}
// MOD 2007.07.26 東都）高木 依頼主に登録がないにもかかわらず請求先が削除できない END

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				reader.Read();
				if (reader.GetDecimal(0) == 0)
				{
					sRet[0] = "正常終了";
					sRet[1] = "0";
				}
				else
				{
					sRet[0] = "依頼主データが存在するため削除できません";
					sRet[1] = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "サーバエラー：" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 START
				conn2 = null;
// ADD 2007.04.28 東都）高木 オブジェクトの破棄 END
// DEL 2007.05.10 東都）高木 未使用関数のコメント化
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.11.07 東都）伊賀 依頼主チェック追加 END
	}
}
