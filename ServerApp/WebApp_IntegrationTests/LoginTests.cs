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
using System.Linq;
using System.Threading.Tasks;
using Domain.Util;

namespace WebApp.IntegrationTests
{
    [TestClass()]
    public class LoginTests
    {
        string connectionString;
        CustomProfiledDbConnection Connection;
        LoginService service;
       

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

            var repository = new LoginRepository(this.Connection);
            this.service = new LoginService();
            service.repository = repository;
        }

        [TestCleanup]
        public void Clean()
        {
            //各テストの実行後に呼ばれる処理
            Connection?.Dispose(); //一応テスト毎にCloseしておく。
        }




        [TestMethod()]
        public async Task ログイン_成功()
        {
            ClearDatabase();
            InsertTestData();

            var user = await service.GetUserByLoginInfo("user1", "password");

            Assert.AreEqual(1, user.UserNo);
            Assert.AreEqual("ユーザー1", user.UserName);
            Assert.AreEqual(3, user.Role);

            //削除された課以外を取得
            var divisionList = user.DivisionNoList;
            Assert.AreEqual(2, divisionList.Count);
            Assert.AreEqual(true, divisionList.Any(d => d == 1));
            Assert.AreEqual(true, divisionList.Any(d => d == 2));
        }


        [TestMethod()]
        public async Task ログイン_失敗()
        {
            ClearDatabase();
            InsertTestData();

            //ユーザーID違い
            var user = await service.GetUserByLoginInfo("user1a", "password");
            Assert.AreEqual(null, user);

            //パスワード違い
            user = await service.GetUserByLoginInfo("user1", "passworda");
            Assert.AreEqual(null, user);

            //削除済みユーザー
            user = await service.GetUserByLoginInfo("user3", "password");
            Assert.AreEqual(null, user);
        }





        private void ClearDatabase()
        {
            int result = Connection.Execute(@"
TRUNCATE TABLE M_Division;
TRUNCATE TABLE M_User;
TRUNCATE TABLE M_UserDivision;
");
        }

        private void InsertTestData()
        {
            int result = Connection.Execute(@"
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (1, N'課1', N'課1', 1, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (2, N'課2', N'課2', 3, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (3, N'課3', N'課3', 2, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (4, N'課4', N'課4', 4, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 1)
INSERT [M_Division] ([DivisionNo], [DivisionName], [Remarks], [OrderNo], [CreateDateTime], [UpdateDateTime], [DeleteFlag]) VALUES (5, N'課5', N'課5', 5, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), 0)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (1, N'ユーザー1', N'user1', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 3, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (2, N'ユーザー2', N'user2', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 2, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (3, N'ユーザー3', N'user3', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 1, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 1)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (4, N'ユーザー4', N'user4', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 0, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 1)
INSERT [M_User] ([UserNo], [UserName], [LoginId], [Password], [Role], [CreateBy], [CreateDateTime], [UpdateBy], [UpdateDateTime], [DeleteFlag]) VALUES (5, N'ユーザー5', N'user5', 0x168C8164330CA4E0C7FFD622E41A3FD5882B23E9135B6A49F9F95F15D0EF0854, 1, 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0, CAST(N'2020-01-15T14:59:15.6530000' AS DateTime2), 0)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (1, 1)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (1, 2)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (1, 4)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (2, 2)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (3, 1)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (4, 0)
INSERT [M_UserDivision] ([UserNo], [DivisionNo]) VALUES (5, 1)

");
        }
    }
}