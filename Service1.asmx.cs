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
	// �C������
	//--------------------------------------------------------------------------
	// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j��
	//	disposeReader(reader);
	//	reader = null;
	//--------------------------------------------------------------------------
	// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
	//	logFileOpen(sUser);
	//	userCheck2(conn2, sUser);
	//	logFileClose();
	//--------------------------------------------------------------------------
	// MOD 2007.07.26 ���s�j���� �˗���ɓo�^���Ȃ��ɂ�������炸�����悪�폜�ł��Ȃ�
	//--------------------------------------------------------------------------
	// MOD 2008.11.26 ���s�j���� ���ۃR�[�h���󔒂ł��G���[���łȂ����� 
	//--------------------------------------------------------------------------
	// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� 
	//--------------------------------------------------------------------------

	[System.Web.Services.WebService(
		 Namespace="http://Walkthrough/XmlWebServices/",
		 Description="is2seikyuu")]

	public class Service1 : is2common.CommService
	{
		public Service1()
		{
			//CODEGEN: ���̌Ăяo���́AASP.NET Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
			InitializeComponent();

			connectService();
		}

		#region �R���|�[�l���g �f�U�C�i�Ő������ꂽ�R�[�h 
		
		//Web �T�[�r�X �f�U�C�i�ŕK�v�ł��B
		private IContainer components = null;
				
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
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
		 * ������}�X�^�ꗗ�擾
		 * �����F����b�c�A����b�c
		 * �ߒl�F�X�e�[�^�X�A�ꗗ�i�X�֔ԍ��A���Ӑ�b�c�j...
		 *
		 *********************************************************************/
// ADD 2005.05.11 ���s�j���� ORA-03113�΍�H START
		private static string GET_CLAIM_SELECT_2
			= "SELECT ���Ӑ�b�c, ���Ӑ敔�ۂb�c, ���Ӑ敔�ۖ�, �X�V���� \n"
			+ " FROM �r�l�O�S������ \n";
		private static string GET_CLAIM_SELECT_2_ORDER
			=   " AND �폜�e�f = '0' \n"
			+ " ORDER BY ���Ӑ�b�c, ���Ӑ敔�ۂb�c \n";
// ADD 2005.05.11 ���s�j���� ORA-03113�΍�H END
		[WebMethod]
		public string[] Get_Claim(string[] sUser, string sKcode, string sBcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������}�X�^�ꗗ�擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[2];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			string s�X�֔ԍ� = "";
			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT �X�֔ԍ� \n"
					+  " FROM �b�l�O�Q���� \n"
					+ " WHERE ����b�c = '" + sKcode + "' \n"
					+ "   AND ����b�c = '" + sBcode + "' \n"
					+    "AND �폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				int iCnt = 1;
				while (reader.Read())
				{
					s�X�֔ԍ� = reader.GetString(0).Trim();
					iCnt++;
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// ADD 2005.05.11 ���s�j���� �����悪�O�����̑Ή� START
				sRet[1] = s�X�֔ԍ�;
// ADD 2005.05.11 ���s�j���� �����悪�O�����̑Ή� END
				if(iCnt == 1) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
					sRet[0] = "����I��";
				logWriter(sUser, INF, sRet[0]);

				cmdQuery
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//					= "SELECT '|' || TRIM(���Ӑ�b�c) || '|' "
//					+     "|| TRIM(���Ӑ敔�ۂb�c) || '|' "
//					+     "|| TRIM(���Ӑ敔�ۖ�)   || '|' "
//					+     "|| TO_CHAR(�X�V����) || '|' \n"
//					+  " FROM �r�l�O�S������ "
//					+ " WHERE �X�֔ԍ� = '" + s�X�֔ԍ� + "' \n"
//					+   " AND ����b�c = '" + sKcode + "' \n"
//					+   " AND �폜�e�f = '0' \n"
//					+ " ORDER BY ���Ӑ�b�c, ���Ӑ敔�ۂb�c \n";
					= GET_CLAIM_SELECT_2
					+ " WHERE �X�֔ԍ� = '" + s�X�֔ԍ� + "' \n"
					+   " AND ����b�c = '" + sKcode + "' \n"
					+ GET_CLAIM_SELECT_2_ORDER;
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END

				reader = CmdSelect(sUser, conn2, cmdQuery);
				while (reader.Read())
				{
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H START
//					sList.Add(reader.GetString(0));
					sList.Add("|" + reader.GetString(0).Trim()
							+ "|" + reader.GetString(1).Trim()
							+ "|" + reader.GetString(2).Trim()
							+ "|" + reader.GetDecimal(3).ToString().Trim()
							+ "|");
// MOD 2005.05.11 ���s�j���� ORA-03113�΍�H END
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				sRet = new string[sList.Count + 2];
// ADD 2005.05.11 ���s�j���� �����悪�O�����̑Ή� START
				sRet[1] = s�X�֔ԍ�;
// ADD 2005.05.11 ���s�j���� �����悪�O�����̑Ή� END
				if(sList.Count == 0) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
				{
					sRet[0] = "����I��";
					sRet[1] = s�X�֔ԍ�;
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * ������}�X�^�ꗗ�擾
		 * �����F����b�c�A����b�c,���Ӑ�b�c,���ۂb�c
		 * �ߒl�F�X�e�[�^�X�A�ꗗ�i�X�֔ԍ��A���Ӑ�b�c�j...
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Sel_Claim(string[] sUser, string sKcode, string sBcode, string sTcode, string sTBcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������}�X�^���擾�J�n");

			OracleConnection conn2 = null;
			ArrayList sList = new ArrayList();
			string[] sRet = new string[2];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			string cmdQuery = "";
			try
			{
				cmdQuery
					= "SELECT SM04.���Ӑ敔�ۖ� \n"
					+  " FROM �b�l�O�Q���� CM02, �r�l�O�S������ SM04 \n"
					+ " WHERE CM02.����b�c = '" + sKcode + "' \n"
					+    "AND CM02.����b�c = '" + sBcode + "' \n"
					+    "AND CM02.�X�֔ԍ� = SM04.�X�֔ԍ� \n"
					+    "AND CM02.����b�c = SM04.����b�c \n"
					+    "AND SM04.���Ӑ�b�c = '" + sTcode + "' \n"
					+    "AND SM04.���Ӑ敔�ۂb�c = '" + sTBcode + "' \n"
					+    "AND CM02.�폜�e�f = '0' \n"
					+    "AND SM04.�폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				int iCnt = 1;
				while (reader.Read())
				{
					sRet[1] = reader.GetString(0).Trim();
					iCnt++;
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
				if(iCnt == 1) 
					sRet[0] = "�Y���f�[�^������܂���";
				else
					sRet[0] = "����I��";
				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * ������}�X�^���݃`�F�b�N
		 * �����F�X�֔ԍ��A���Ӑ�b�c�A���Ӑ敔�ۂb�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Get_seikyuucnt(string[] sUser, string sYubin, string sTcode, string sTbcode)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������}�X�^���݃`�F�b�N�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END
// MOD 2008.11.26 ���s�j���� ���ۃR�[�h���󔒂ł��G���[���łȂ����� START
			if(sTbcode.Length == 0) sTbcode = " ";
// MOD 2008.11.26 ���s�j���� ���ۃR�[�h���󔒂ł��G���[���łȂ����� END

			string cmdQuery = "";
			try
			{
				cmdQuery
//					= "SELECT TO_CHAR(COUNT(*)) "
//					= "SELECT NVL(COUNT(*),0) "
//					= "SELECT COUNT(*) "
					= "SELECT COUNT(ROWID) "
					+   "FROM �r�l�O�S������ "
					+  "WHERE �X�֔ԍ�       = '" + sYubin + "' "
					+    "AND ���Ӑ�b�c     = '" + sTcode + "' "
					+    "AND ���Ӑ敔�ۂb�c = '" + sTbcode + "' "
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
					+    "AND ����b�c = '" + sUser[0] + "' "
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
					+    "AND �폜�e�f = '0' \n";

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				int iCnt = 1;
				while (reader.Read())
				{
//					sRet[0] = reader.GetString(0);
					sRet[0] = reader.GetDecimal(0).ToString().Trim();
					iCnt++;
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * ������}�X�^�ǉ�
		 * �����F�X�֔ԍ��A���Ӑ�b�c�A���Ӑ敔�ۂb�c...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Ins_Claim(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������}�X�^�ǉ��J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = "";
			try
			{
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
//				cmdQuery 
//					= "DELETE FROM �r�l�O�S������ \n"
//					+ " WHERE �X�֔ԍ�           = '" + sKey[0] +"' \n"
//					+ "   AND ���Ӑ�b�c         = '" + sKey[1] +"' \n"
//					+ "   AND ���Ӑ敔�ۂb�c     = '" + sKey[2] +"' \n"
//// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
//					+ "   AND ����b�c           = '" + sUser[0] + "' "
//// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
//					+ "   AND �폜�e�f           = '1'";
//
//				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
				// ���̉���œ����A�X�֔ԍ��A������b�c�œo�^���Ă����ꍇ
				// ���̉���œ����A���X�b�c�A������b�c�œo�^���Ă����ꍇ
				string s�폜�e�f = "";
				cmdQuery 
					= "SELECT �폜�e�f \n"
					+ " FROM �r�l�O�S������ \n"
					+ " WHERE �X�֔ԍ� = '" + sKey[0] +"' \n"
					+ " AND ���Ӑ�b�c = '" + sKey[1] +"' \n"
					+ " AND ���Ӑ敔�ۂb�c = '" + sKey[2] +"' \n"
					+ " AND ����b�c = '" + sKey[4] + "' "
					;
				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);
				if(reader.Read()){
					s�폜�e�f = reader.GetString(0).TrimEnd();
				}
				disposeReader(reader);
				reader = null;
				if(s�폜�e�f == "1"){
					cmdQuery
						= "UPDATE �r�l�O�S������ SET \n"
						+ " ���Ӑ敔�ۖ� = '" + sKey[3] + "' \n"
//						+ ",����b�c = '" + sKey[4] + "' \n"
						+ ",�폜�e�f = '0' \n"
						+ ",�o�^���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ ",�o�^�o�f = '" + sKey[5] + "' \n"
						+ ",�o�^�� = '" + sKey[6] + "' \n"
						+ ",�X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') \n"
						+ ",�X�V�o�f = '" + sKey[5] + "' \n"
						+ ",�X�V�� = '" + sKey[6] + "' \n"
						+ " WHERE �X�֔ԍ� = '" + sKey[0] +"' \n"
						+ " AND ���Ӑ�b�c = '" + sKey[1] +"' \n"
						+ " AND ���Ӑ敔�ۂb�c = '" + sKey[2] +"' \n"
						+ " AND ����b�c = '" + sKey[4] + "' "
						;
				}else{
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
					//�ǉ�
					cmdQuery
						= "INSERT INTO �r�l�O�S������ \n"
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
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
				}
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
				CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "����I��";
				
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
// DEL 2005.05.31 ���s�j���� �s�v�Ȉ׍폜 START
//				string sErr = ex.Message.Substring(0,9);
//				if(sErr == "ORA-00001")
//					sRet[0] = "����̃R�[�h�����ɑ��̒[�����o�^����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
//				else
// DEL 2005.05.31 ���s�j���� �s�v�Ȉ׍폜 END
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * ������}�X�^�X�V
		 * �����F�X�֔ԍ��A���Ӑ�b�c�A���Ӑ敔�ۂb�c...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Upd_Claim(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������}�X�^�X�V�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = "";
			try
			{
				cmdQuery
					= "UPDATE �r�l�O�S������ \n"
					+   " SET �X�֔ԍ� = '" + sKey[0] + "' "
					+       ",���Ӑ敔�ۖ� = '" + sKey[3] + "' " 
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
//					+       ",����b�c = '" + sKey[4] + "' "
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
					+       ",�X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",�X�V�o�f = '" + sKey[5] + "' "
					+       ",�X�V�� = '" + sKey[6] + "' \n"
					+ " WHERE �X�֔ԍ� = '" + sKey[0] + "' \n"
					+   " AND ���Ӑ�b�c = '" + sKey[1] + "' \n"
					+   " AND ���Ӑ敔�ۂb�c = '" + sKey[2] + "' \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
					+   " AND ����b�c = '" + sKey[4] + "' \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
// MOD 2007.02.09 ���s�j���� ���̃`�F�b�N START
//					+   " AND �X�V���� = '" + sKey[7] + "' \n"
					+   " AND �X�V���� = " + sKey[7] + " \n"
// MOD 2007.02.09 ���s�j���� ���̃`�F�b�N END
					+   " AND �폜�e�f = '0' \n";

				if (CmdUpdate(sUser, conn2, cmdQuery) != 0)
				{
					tran.Commit();
					sRet[0] = "����I��";
				}
				else
				{
					tran.Rollback();
					sRet[0] = "�f�[�^�ҏW���ɑ��̒[�����X�V����Ă��܂��B\r\n�ēx�A�ŐV�f�[�^���Ăяo���čX�V���Ă��������B";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

		/*********************************************************************
		 * ������}�X�^�폜
		 * �����F�X�֔ԍ��A���Ӑ�b�c�A���Ӑ敔�ۂb�c...
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public string[] Del_Claim(string[] sUser, string[] sKey)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������}�X�^�폜�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[1];

			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� START
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}
//// ADD 2005.05.23 ���s�j�ɉ� ����`�F�b�N�ǉ� END
// MOD 2008.11.26 ���s�j���� ���ۃR�[�h���󔒂ł��G���[���łȂ����� START
			if(sKey[2].Length == 0) sKey[2] = " ";
// MOD 2008.11.26 ���s�j���� ���ۃR�[�h���󔒂ł��G���[���łȂ����� END

			OracleTransaction tran;
			tran = conn2.BeginTransaction();

			string cmdQuery = "";
			try
			{
				cmdQuery
					= "UPDATE �r�l�O�S������ \n"
					+   " SET �폜�e�f = '1' " 
					+       ",�X�V���� = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') "
					+       ",�X�V�o�f = '" + sKey[3] + "' "
					+       ",�X�V�� = '" + sKey[4] + "' \n"
					+ " WHERE �X�֔ԍ� = '" + sKey[0] + "' \n"
					+   " AND ���Ӑ�b�c = '" + sKey[1] + "' \n"
					+   " AND ���Ӑ敔�ۂb�c = '" + sKey[2] + "' \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� START
					+   " AND ����b�c = '" + sUser[0] + "' \n"
// MOD 2011.03.09 ���s�j���� ������}�X�^�̎�L�[��[����b�c]��ǉ� END
					;

				int iUpdRow = CmdUpdate(sUser, conn2, cmdQuery);
				tran.Commit();
				sRet[0] = "����I��";
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
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			return sRet;
		}

// ADD 2005.11.07 ���s�j�ɉ� �˗���`�F�b�N�ǉ� START
		/*********************************************************************
		 * ������g�p���`�F�b�N
		 * �����F����b�c�A���Ӑ�b�c�A���Ӑ敔�ۂb�c
		 * �ߒl�F�X�e�[�^�X
		 *
		 *********************************************************************/
		[WebMethod]
		public String[] Sel_IrainusiSeikyuu(string[] sUser, string[] sData)
		{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//			logFileOpen(sUser);
			logWriter(sUser, INF, "������g�p���`�F�b�N�J�n");

			OracleConnection conn2 = null;
			string[] sRet = new string[2];
			// �c�a�ڑ�
			conn2 = connect2(sUser);
			if(conn2 == null)
			{
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
				sRet[0] = "�c�a�ڑ��G���[";
				return sRet;
			}
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//
//			// ����`�F�b�N
//			sRet[0] = userCheck2(conn2, sUser);
//			if(sRet[0].Length > 0)
//			{
//				disconnect2(sUser, conn2);
//				logFileClose();
//				return sRet;
//			}

			try
			{
// MOD 2007.07.26 ���s�j���� �˗���ɓo�^���Ȃ��ɂ�������炸�����悪�폜�ł��Ȃ� START
//				string cmdQuery 
//					= "SELECT COUNT(*) \n" 
//					+   "FROM �r�l�O�P�ב��l \n"
//					+ " WHERE ����b�c           = '" + sData[0] +"' \n"
//					+ "   AND ���Ӑ�b�c         = '" + sData[1] +"' \n"
//					+ "   AND ���Ӑ敔�ۂb�c     = '" + sData[2] +"'"
//					+ "   AND �폜�e�f           = '0' \n";

				string cmdQuery = "SELECT COUNT(*) \n";

				if(sData.Length < 4)
				{
					cmdQuery 
						= "SELECT /*+ INDEX(SM01 SM01PKEY) */ \n"
						+ " COUNT(*) \n"
						+ " FROM �r�l�O�P�ב��l SM01 \n"
						+ " WHERE SM01.����b�c = '" + sData[0] +"' \n"
						+ " AND SM01.���Ӑ�b�c = '" + sData[1] +"' \n"
						+ " AND SM01.���Ӑ敔�ۂb�c = '" + sData[2] +"' \n"
						+ " AND SM01.�폜�e�f = '0' \n"
						;
				}
				else
				{
					cmdQuery 
						= "SELECT /*+ INDEX(SM01 SM01PKEY) INDEX(CM02 CM02PKEY) */ \n"
						+ " COUNT(*) \n"
						+ " FROM �r�l�O�P�ב��l SM01 \n"
						+ " , �b�l�O�Q���� CM02 \n"
						+ " WHERE SM01.����b�c = '" + sData[0] +"' \n"
						+ " AND SM01.���Ӑ�b�c = '" + sData[1] +"' \n"
						+ " AND SM01.���Ӑ敔�ۂb�c = '" + sData[2] +"' \n"
						+ " AND SM01.�폜�e�f = '0' \n"
						+ " AND SM01.����b�c = CM02.����b�c \n"
						+ " AND SM01.����b�c = CM02.����b�c \n"
						+ " AND CM02.�폜�e�f = '0' \n"
						+ " AND CM02.�X�֔ԍ� = '" + sData[3] +"' \n"
						;
				}
// MOD 2007.07.26 ���s�j���� �˗���ɓo�^���Ȃ��ɂ�������炸�����悪�폜�ł��Ȃ� END

				OracleDataReader reader = CmdSelect(sUser, conn2, cmdQuery);

				reader.Read();
				if (reader.GetDecimal(0) == 0)
				{
					sRet[0] = "����I��";
					sRet[1] = "0";
				}
				else
				{
					sRet[0] = "�˗���f�[�^�����݂��邽�ߍ폜�ł��܂���";
					sRet[1] = reader.GetDecimal(0).ToString().Trim();
				}
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				disposeReader(reader);
				reader = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END

				logWriter(sUser, INF, sRet[0]);
			}
			catch (OracleException ex)
			{
				sRet[0] = chgDBErrMsg(sUser, ex);
			}
			catch (Exception ex)
			{
				sRet[0] = "�T�[�o�G���[�F" + ex.Message;
				logWriter(sUser, ERR, sRet[0]);
			}
			finally
			{
				disconnect2(sUser, conn2);
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� START
				conn2 = null;
// ADD 2007.04.28 ���s�j���� �I�u�W�F�N�g�̔j�� END
// DEL 2007.05.10 ���s�j���� ���g�p�֐��̃R�����g��
//				logFileClose();
			}
			
			return sRet;
		}
// ADD 2005.11.07 ���s�j�ɉ� �˗���`�F�b�N�ǉ� END
	}
}
