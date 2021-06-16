using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Controllers;
using System;
using Dapper;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Domain.Repository;
using System.Data.SqlClient;
using Domain.Service;
using WebApp.Model;
using Domain.Model;
using System.Linq;
using System.Threading.Tasks;
using Domain.Util;

namespace WebApp.IntegrationTests
{
    [TestClass()]
    public class UserMainteTests
    {
        string connectionString;
        CustomProfiledDbConnection Connection;
        UserMainteService service;
        LoginService loginService;


        [TestInitialize]
        public void ReadConfig()
        {
            //各テストの実行前に呼ばれる処理
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.IntegrationTest.json")
             .Build();

            string envDB = Environment.GetEnvironmentVariable("ConnectionStrings__DB");

            this.connectionString = !string.IsNullOrEmpty(envDB) ? envDB : config.GetConnectionString("DB");
            this.Connection = new CustomProfiledDbConnection(new SqlConnection(connectionString), new TraceDbProfiler(new MockLogger<TraceDbProfiler>()));

            Console.WriteLine($"接続文字列【{connectionString}】");

            var repository = new UserMainteRepository(this.Connection);
            this.service = new UserMainteService();
            service.repository = repository;

            //登録後の確認用に利用
            loginService = new LoginService() { repository = new LoginRepository(Connection) };

            ClearDatabase();
            InsertMasterData();
            InsertTranData();
        }

        [TestCleanup]
        public void Clean()
        {
            //各テストの実行後に呼ばれる処理
            Connection?.Dispose(); //一応テスト毎にCloseしておく。
        }

        DateTime basedate = DateTime.Today;

        #region 登録テスト

        /// <summary>
        /// 登録テスト1（正常系）
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_新規登録_検索_登録結果確認()
        {
            // ユーザーを新規登録
            var registerModel = new UserRegisterModel();
            registerModel.UserNo = null;
            registerModel.UserName = "テスト太郎1";
            registerModel.LoginId = "tarou1";
            registerModel.Role = 10;
            registerModel.DivisionNoList = new List<int>() { 1, 2 };
            registerModel.PasswordStr = "password2";
            registerModel.IsDeleted = false;
            registerModel.CreateBy = registerModel.UpdateBy = 1;

            var insertResult = await service.Create(registerModel);

            var isSuccess = insertResult.Result;
            var message = insertResult.Message;

            // DB登録結果確認
            Assert.IsTrue(isSuccess);
            Assert.AreEqual("", message);

            // 登録内容で再取得し、取得できることを確認
            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "";
            searchModel.SearchLoginId = "tarou";
            searchModel.SearchRole = 10;
            searchModel.SearchDivisionNo = 2;
            searchModel.ShowDelete = false;
            searchModel.SortParams = new List<SortParam>() { new SortParam("", "", "asc") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 20 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            Assert.AreEqual(1, totalCount);
            Assert.AreEqual(1, list.Count);

            // データの中身確認
            var user = list[0];
            Assert.AreEqual(7, user.UserNo);
            Assert.AreEqual(registerModel.UserName, user.UserName);
            Assert.AreEqual(registerModel.LoginId, user.LoginId);
            Assert.AreEqual(registerModel.Role, user.Role);
            
            var divisionNoList = user.DivisionNoList;
            Assert.AreEqual(2, divisionNoList.Count);
            Assert.AreEqual(true, divisionNoList.Any(d => d == 1));
            Assert.AreEqual(true, divisionNoList.Any(d => d == 2));

            Assert.AreEqual(registerModel.IsDeleted.AsFlag(), user.DeleteFlag);
            Assert.AreEqual(registerModel.IsDeleted, user.IsDeleted);

            //ログイン出来るか確認
            var loginInfo = await loginService.GetUserByLoginInfo(registerModel.LoginId, registerModel.PasswordStr);
            Assert.IsNotNull(loginInfo);

        }

        /// <summary>
        /// 登録テスト2（異常系）
        /// 重複チェック
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_既登録()
        {
            var registerModel = new UserRegisterModel();
            registerModel.UserName = "テスト太郎1";
            registerModel.LoginId = "user1";
            registerModel.Role = 10;
            registerModel.DivisionNoList = new List<int>() { };
            registerModel.PasswordStr = "password";
            registerModel.IsDeleted = false;
            registerModel.CreateBy = registerModel.UpdateBy = 1;

            var insertResult = await service.Create(registerModel);

            var isSuccess = insertResult.Result;
            var message = insertResult.Message;

            Assert.AreEqual(false, isSuccess);
            Assert.AreEqual("既に登録されているログインIDです。", message);
        }

        #endregion


        #region 更新テスト

        /// <summary>
        /// 更新テスト1（正常系）
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_更新_検索_更新結果確認()
        {
            var registerModel = new UserRegisterModel();
            registerModel.UserNo = 3;
            registerModel.UserName = "テスト太郎3";
            registerModel.LoginId = "tarou3";
            registerModel.Role = 12;
            registerModel.DivisionNoList = new List<int>() { 1000, 1001};
            registerModel.PasswordStr = null; //パスワード変更なし
            registerModel.IsDeleted = false;
            registerModel.CreateBy = registerModel.UpdateBy = 1;

            var updateResult = await service.Update(registerModel);

            var isSuccess = updateResult.Result;
            var message = updateResult.Message;

            Assert.AreEqual(true, isSuccess);
            Assert.AreEqual("", message);


            // DB登録結果確認（更新後データの存在確認）
            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "テスト太郎";
            searchModel.SearchLoginId = "";
            searchModel.SearchRole = 12;
            searchModel.SearchDivisionNo = null;
            searchModel.ShowDelete = false;
            searchModel.SortParams = new List<SortParam>() { new SortParam("", "", "asc") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 20 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            Assert.AreEqual(1, totalCount);
            Assert.AreEqual(1, list.Count);

            // データの中身確認
            var user = list[0];
            Assert.AreEqual(registerModel.UserNo, user.UserNo);
            Assert.AreEqual(registerModel.UserName, user.UserName);
            Assert.AreEqual(registerModel.LoginId, user.LoginId);
            Assert.AreEqual(registerModel.Role, user.Role);

            var divisionNoList = user.DivisionNoList;
            Assert.AreEqual(2, divisionNoList.Count);
            Assert.AreEqual(true, divisionNoList.Any(d => d == 1000));
            Assert.AreEqual(true, divisionNoList.Any(d => d == 1001));

            Assert.AreEqual(registerModel.IsDeleted.AsFlag(), user.DeleteFlag);
            Assert.AreEqual(registerModel.IsDeleted, user.IsDeleted);
            //ログイン出来るか確認
            var loginInfo = await loginService.GetUserByLoginInfo(registerModel.LoginId, "password");
            Assert.IsNotNull(loginInfo);

            //パスワード変更あり
            registerModel.UserNo = 3;
            registerModel.PasswordStr = "password123"; 

            updateResult = await service.Update(registerModel);

            isSuccess = updateResult.Result;
            message = updateResult.Message;

            Assert.AreEqual(true, isSuccess);
            Assert.AreEqual("", message);

            //ログイン出来るか確認
            loginInfo = await loginService.GetUserByLoginInfo(registerModel.LoginId, registerModel.PasswordStr);
            Assert.IsNotNull(loginInfo);

        }

        /// <summary>
        /// 更新テスト2（異常系）
        /// 重複チェック
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_更新_既に登録されているユーザー名と同一()
        {

            var registerModel = new UserRegisterModel();
            registerModel.UserNo = 3;
            registerModel.UserName = "テスト太郎3";
            registerModel.LoginId = "user1";
            registerModel.Role = 12;
            registerModel.DivisionNoList = new List<int>() { 1000, 1001 };
            registerModel.PasswordStr = null; //パスワード変更なし
            registerModel.IsDeleted = false;
            registerModel.CreateBy = registerModel.UpdateBy = 1;


            var updateResult = await service.Update(registerModel);

            var isSuccess = updateResult.Result;
            var message = updateResult.Message;

            Assert.AreEqual(false, isSuccess);
            Assert.AreEqual("既に登録されているログインIDです。", message);
        }

        #endregion


        #region 検索テスト

        /// <summary>
        /// 検索条件未設定
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_検索_条件指定なし()
        {
            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "";
            searchModel.SearchLoginId = "";
            searchModel.SearchRole = null;
            searchModel.SearchDivisionNo = null;
            searchModel.ShowDelete = false;
            searchModel.SortParams = new List<SortParam>() { new SortParam("UserName", "", "desc") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 3 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            // 全4件の内、DeleteFlagが1のデータを除いて3件が検索される
            Assert.AreEqual(4, totalCount);
            Assert.AreEqual(3, list.Count);

            // データの中身確認
            //list = list.OrderBy(m => m.UserNo).ToList();
            var user = list[0];
            Assert.AreEqual(6, user.UserNo);
            Assert.AreEqual("ユーザー6", user.UserName);
            Assert.AreEqual("user6", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(11, user.Role);
            Assert.AreEqual("代理", user.RoleName);
            Assert.AreEqual(0, user.DeleteFlag);
            Assert.AreEqual(false, user.IsDeleted);

            //削除フラグが立った課を除いたものが取得される。
            var divisionList = user.DivisionNoList;
            Assert.AreEqual(2, divisionList.Count);
            Assert.AreEqual(true, divisionList.Any(d => d == 3));
            Assert.AreEqual(true, divisionList.Any(d => d == 5));

            var divisionNameList = user.DivisionNameList;
            Assert.AreEqual(2, divisionNameList.Count);
            Assert.AreEqual(true, divisionNameList.Any(d => d == "課3"));
            Assert.AreEqual(true, divisionNameList.Any(d => d == "課5"));

            // データの順番確認 (UserName DESC)
            Assert.AreEqual(6, list[0].UserNo);
            Assert.AreEqual(5, list[1].UserNo);
            Assert.AreEqual(2, list[2].UserNo);

            // 削除済みが含まれていない
            Assert.IsTrue(list.Where(p => p.IsDeleted).ToList().Count == 0);
        }

        /// <summary>
        /// 検索条件未：削除
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_検索_条件全指定()
        {

            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "ユーザー4";
            searchModel.SearchLoginId = "user4";
            searchModel.SearchRole = 13;
            searchModel.SearchDivisionNo = 5;
            searchModel.ShowDelete = true;
            searchModel.SortParams = new List<SortParam>() { new SortParam("", "", "asc") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 3 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            // 条件に合致する件が検索される
            Assert.AreEqual(1, totalCount);
            Assert.AreEqual(1, list.Count);

            var user = list[0];
            Assert.AreEqual(4, user.UserNo);
            Assert.AreEqual("ユーザー4", user.UserName);
            Assert.AreEqual("user4", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(13, user.Role);
            Assert.AreEqual("管理者", user.RoleName);
            Assert.AreEqual(1, user.DeleteFlag);
            Assert.AreEqual(true, user.IsDeleted);

            var divisionList = user.DivisionNoList;
            Assert.AreEqual(1, divisionList.Count);
            Assert.AreEqual(true, divisionList.Any(d => d == 5));

            var divisionNameList = user.DivisionNameList;
            Assert.AreEqual(1, divisionNameList.Count);
            Assert.AreEqual(true, divisionNameList.Any(d => d == "課5"));

        }


        /// <summary>
        /// 検索条件未設定
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_検索_詳細()
        {
            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "";
            searchModel.SearchUserNo = 3;

            var user = await service.GetDetail(searchModel);
            Assert.IsNotNull(user);

            // データの中身確認
            Assert.AreEqual(3, user.UserNo);
            Assert.AreEqual("ユーザー3", user.UserName);
            Assert.AreEqual("user3", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(10, user.Role);
            Assert.AreEqual("一般", user.RoleName);
            Assert.AreEqual(1, user.DeleteFlag);
            Assert.AreEqual(true, user.IsDeleted);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_検索_ページング()
        {
            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "";
            searchModel.SearchLoginId = "";
            searchModel.SearchRole = null;
            searchModel.SearchDivisionNo = null; ;
            searchModel.ShowDelete = false;
            searchModel.SortParams = new List<SortParam>() { new SortParam("UserName", "", "asc") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 2 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            // 全4件の内、DeleteFlagが1のデータを除いて3件が検索される
            Assert.AreEqual(4L, totalCount);
            Assert.AreEqual(2, list.Count);

            // データの中身確認
            var user = list[0];
            Assert.AreEqual(1, user.UserNo);
            Assert.AreEqual("ユーザー1", user.UserName);
            Assert.AreEqual("user1", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(13, user.Role);
            Assert.AreEqual("管理者", user.RoleName);
            Assert.AreEqual(0, user.DeleteFlag);
            Assert.AreEqual(false, user.IsDeleted);


            user = list[1];
            Assert.AreEqual(2, user.UserNo);
            Assert.AreEqual("ユーザー2", user.UserName);
            Assert.AreEqual("user2", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(11, user.Role);

            // 削除済みが含まれていない
            Assert.IsTrue(list.Where(p => p.IsDeleted).ToList().Count == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_検索_ソート()
        {
            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "";
            searchModel.SearchLoginId = "";
            searchModel.SearchRole = null;
            searchModel.SearchDivisionNo = null; ;
            searchModel.ShowDelete = false;
            searchModel.SortParams = new List<SortParam>() { new SortParam("UserName", "", "desc") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 2 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            // 全4件の内、DeleteFlagが1のデータを除いて3件が検索される
            Assert.AreEqual(4L, totalCount);
            Assert.AreEqual(2, list.Count);

            // データの中身確認
            var user = list[0];
            Assert.AreEqual(6, user.UserNo);
            Assert.AreEqual("ユーザー6", user.UserName);
            Assert.AreEqual("user6", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(11, user.Role);
            Assert.AreEqual("代理", user.RoleName);
            Assert.AreEqual(0, user.DeleteFlag);
            Assert.AreEqual(false, user.IsDeleted);

            user = list[1];
            Assert.AreEqual(5, user.UserNo);
            Assert.AreEqual("ユーザー5", user.UserName);
            Assert.AreEqual("user5", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(10, user.Role);

            // 削除済みが含まれていない
            Assert.IsTrue(list.Where(p => p.IsDeleted).ToList().Count == 0);
        }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod()]
        public async Task ユーザーマスタ_検索_条件指定_フリーテキスト()
        {

            var searchModel = new UserSearchParam();
            searchModel.SearchUserName = "ユーザー5 ザー2";
            searchModel.SearchLoginId = "";
            searchModel.SearchRole = null;
            searchModel.SearchDivisionNo = null; ;
            searchModel.ShowDelete = false;
            searchModel.SortParams = new List<SortParam>() { new SortParam("", "", "") };
            searchModel.PagingParam = new PagingParam() { CurrentPage = 0, RowCount = 3 };

            var result = await service.GetList(searchModel);
            var totalCount = result.TotalCount;
            var list = result.List;

            // 全4件の内、DeleteFlagが1のデータを除いて2件（ページング件数）が検索される
            Assert.AreEqual(2L, totalCount);
            Assert.AreEqual(2, list.Count);


            // データの中身確認
            var user = list[0];
            Assert.AreEqual(2, user.UserNo);
            Assert.AreEqual("ユーザー2", user.UserName);
            Assert.AreEqual("user2", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(11, user.Role);
            Assert.AreEqual("代理", user.RoleName);
            Assert.AreEqual(0, user.DeleteFlag);
            Assert.AreEqual(false, user.IsDeleted);

            user = list[1];
            Assert.AreEqual(5, user.UserNo);
            Assert.AreEqual("ユーザー5", user.UserName);
            Assert.AreEqual("user5", user.LoginId);
            Assert.AreEqual(null, user.Password);
            Assert.AreEqual(10, user.Role);

            // 削除済みが含まれていない
            Assert.IsTrue(list.Where(p => p.IsDeleted).ToList().Count == 0);

        }
        
        #endregion


        #region テストデータの用意

        private void ClearDatabase()
        {
            int result = Connection.Execute(@"
TRUNCATE TABLE M_Division;
TRUNCATE TABLE M_General;
TRUNCATE TABLE M_User;
TRUNCATE TABLE M_UserDivision;
");
        }

        private void InsertMasterData()
        {
            int result = Connection.Execute(
                @"
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (1, N'課1', N'課1', 1, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (2, N'課2', N'課2', 3, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (3, N'課3', N'課3', 2, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (4, N'課4', N'課4', 4, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (5, N'課5', N'課5', 5, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (1000, N'総務', N'総務', 100, CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (1001, N'人事企画', N'人事企画', 110, CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (1002, N'経理', N'経理', 120, CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (1003, N'支援', N'支援', 200, CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), CAST(N'2020-02-13T13:10:51.3200000' AS DateTime2), 0)
INSERT [M_General] ([GeneralNo], [GeneralName], [Remarks], [Description], [CategoryId], [CategoryName], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (N'10', N'一般', N'', N'', N'Role', N'役割', 0, CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_General] ([GeneralNo], [GeneralName], [Remarks], [Description], [CategoryId], [CategoryName], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (N'11', N'代理', N'', N'', N'Role', N'役割', 0, CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_General] ([GeneralNo], [GeneralName], [Remarks], [Description], [CategoryId], [CategoryName], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (N'12', N'課長', N'', N'', N'Role', N'役割', 0, CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_General] ([GeneralNo], [GeneralName], [Remarks], [Description], [CategoryId], [CategoryName], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (N'13', N'管理者', N'', N'', N'Role', N'役割', 0, CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T00:00:00.0000000' AS DateTime2), 0)

;");
        }

        private void InsertTranData()
        {
            int result = Connection.Execute(
                @"
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (1, N'ユーザー1', N'user1', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 13, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (2, N'ユーザー2', N'user2', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 11, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (3, N'ユーザー3', N'user3', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 10, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 1)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (4, N'ユーザー4', N'user4', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 13, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 1)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (5, N'ユーザー5', N'user5', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 10, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (6, N'ユーザー6', N'user6', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 11, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
ALTER SEQUENCE Seq_M_User RESTART WITH 7 INCREMENT BY 1 
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (1, 1)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (1, 2)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (2, 2)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (3, 1)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (4, 5)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (5, 1)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (6, 3)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (6, 4)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (6, 5)

");
        }

        #endregion

    }
}