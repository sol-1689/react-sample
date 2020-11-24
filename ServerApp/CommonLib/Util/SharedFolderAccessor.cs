using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SAIN.Service.Common
{
    /// <summary>
    /// 共有フォルダに接続する。
    /// </summary>
    public class SharedFolderAccessor
    {

        //private static readonly log4net.ILog logger
        //  = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// WNetAddConnection2で認証する(共有フォルダに接続)
        /// </summary>
        /// <param name="sharedPath">共有フォルダパス</param>
        /// <param name="userId">ユーザID</param>
        /// <param name="password">接続パスワード</param>userId
        /// <returns></returns>
        public static bool Connect(string sharedPath, string userId, string password)
        {
            //logger.Info(string.Format("共有フォルダに接続するための処理を開始します。リモート名【{0}】ユーザID【{1}】パスワード【{2}】",sharedPath, userId, password));

            try
            {
                //設定
                NETRESOURCE netResource = new NETRESOURCE();
                netResource.dwScope = 0;
                netResource.dwType = 1;
                netResource.dwDisplayType = 0;
                netResource.dwUsage = 0;
                netResource.lpLocalName = ""; // ネットワークドライブにする場合は"z:"などドライブレター設定
                netResource.lpProvider = "";

                //共有フォルダのパス
                netResource.lpRemoteName = sharedPath;

                int successCount = 0;
                //既に接続されている場合があれば切断する。
                successCount = WNetCancelConnection2(sharedPath, 0, true);
                LogUtil.Info("切断結果 コード：" + successCount);

                //接続実行
                successCount = WNetAddConnection2(ref netResource, password, userId, 0);
                LogUtil.Info("接続結果 コード：" + successCount);
                if (successCount != 0)
                {
                    LogUtil.Warn("共有フォルダ接続に失敗しました。");
                    return false;
                }
                //fortest
                //bool files = Directory.Exists(sharedPath);
            }
            catch (Exception ex)
            {
                LogUtil.Error("共有フォルダ接続に失敗しました。", ex);
                return false;
            }
            finally
            {
                LogUtil.Info("共有フォルダに接続するための処理を終了します。");
            }
            return true;
        }


        //認証情報を使って接続するWin32 API宣言
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int WNetAddConnection2(
        ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, Int32 dwFlags);

        //接続切断するWin32 API を宣言
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int WNetCancelConnection2(string lpName, Int32 dwFlags, bool fForce);


        //WNetAddConnection2に渡す接続の詳細情報の構造体
        [StructLayout(LayoutKind.Sequential)]
        internal struct NETRESOURCE
        {
            /// <summary> 列挙の範囲 </summary> 
            public int dwScope;
            /// <summary>リソースタイプ </summary> 
            public int dwType;
            /// <summary> 表示オブジェクト </summary>
            public int dwDisplayType;
            /// <summary>リソースの使用方法</summary>
            public int dwUsage;
            /// <summary> ローカルデバイス名。使わないならNULL </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpLocalName;
            /// <summary>リモートネットワーク名。使わないならNULL</summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRemoteName;
            /// <summary>ネットワーク内の提供者に提供された文字列 </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpComment;
            /// <summary>リソースを所有しているプロバイダ名 </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpProvider;

            //パラメータ説明
            /**
              dwScope(列挙の範囲)
              RESOURCE_CONNECTED(0x1)  現在接続されたリソースを列挙する。dwUsageメンバーを指定できない。
              RESOURCE_GLOBALNET(0x2)  ネットワークに関するすべてのリソースを列挙する。
              RESOURCE_REMEMBERED(0x3) 接続を列挙する。dwUsageメンバーを指定できない。


              dwType(リソースタイプ)
              RESOURCETYPE_ANY(0x0)   すべてのリソース
              RESOURCETYPE_DISK(0x1) ディスクリソース
              RESOURCETYPE_PRINT(0x2) プリンタリソース

              dwDisplayType(表示オブジェクト)
              RESOURCEDISPLAYTYPE_DOMAIN(0x1)  ドメインオブジェクトを表示する
              RESOURCEDISPLAYTYPE_SERVER(0x2)  サーバオブジェクトを表示する
              RESOURCEDISPLAYTYPE_SHARE(0x3)   シェアオブジェクトを表示する
              RESOURCEDISPLAYTYPE_GENERIC(0x0) オブジェクトの表示は重要ではないことを示す

              dwUsage(リソースの使用方法)
              RESOURCEUSAGE_CONNECTABLE(0x1) 接続可能なリソースであることを示す。lpRemoteNameメンバーによって示された名前はWNetAddConnection機能に通過できる。
              RESOURCEUSAGE_CONTAINER(0x2)   リソースはコンテナリソースです。lpRemoteNameメンバーによって示された名前はWNetOpenEnum機能に通過できる。
           
             **/
        }
    }
}