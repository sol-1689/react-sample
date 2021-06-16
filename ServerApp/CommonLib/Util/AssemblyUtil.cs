using SAIN;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SAIN.Service
{
    public class AssemblyUtil
    {
        /// <summary>
        /// Assemblyのバインドができなかった際のイベントを設定する。
        /// PowerShellなど.configのbindingRedirect効かない環境で使うことを想定。
        /// </summary>
        /// <param name="assemblyList"></param>
        public static void SetAssemblyListResolve(params Assembly[] assemblyList)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                LogUtil.Info("リダイレクト対象DLL " + args.Name
                    + "  ロードしたDLL" + (args.RequestingAssembly != null ? args.RequestingAssembly.FullName : ""));

                var requestedAssembly = new AssemblyName(args.Name);
                var redirectAssembly = assemblyList.Where(m => new AssemblyName(m.FullName).Name == requestedAssembly.Name).FirstOrDefault();
                if (redirectAssembly == null)
                {
                    LogUtil.Info("リダイレクト対象なし 。");
                    return null;
                }
                else
                {
                    LogUtil.Info("リダイレクトDLL  " + redirectAssembly.FullName);
                    return redirectAssembly;
                }
            };
        }

        /// <summary>
        /// Assemblyのバインドができなかった際のイベントを設定する。
        /// PowerShellなど.configのbindingRedirect効かない環境で使うことを想定。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="token"></param>
        public static void SetAssemblyResolve(string name, string version, string token)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
            //本体が参照しているdllのバージョンを指定する
            var targetVersion = new Version(version);
                var publicKeyToken = token;

                LogUtil.Info("リダイレクト対象DLL " + args.Name
                              + " ロードしたDL" + (args.RequestingAssembly != null ? args.RequestingAssembly.FullName : ""));

                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != name)
                    return null;

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                return Assembly.Load(requestedAssembly);
            };
        }
    }
}

