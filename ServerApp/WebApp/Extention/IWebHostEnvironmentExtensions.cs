using System;
using System.IO;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostingEnvironment"/>.
/// </summary>
// public static class MyIWebHostEnvironmentExtensions
// {
//     /// <summary>
//     /// Develop環境かをチェックする。デフォルトのものに条件を上書き。
//     /// </summary>
//     /// <param name="hostingEnvironment">An instance of <see cref="IHostingEnvironment"/>.</param>
//     /// <returns>True if the environment name is <see cref="EnvironmentName.Development"/>, otherwise false.</returns>
//     public static bool IsDevelopment(this IHostEnvironment hostingEnvironment)
//     {
//         if (hostingEnvironment == null)
//         {
//             throw new ArgumentNullException(nameof(hostingEnvironment));
//         }

//         return hostingEnvironment.IsEnvironment("Development")
//                     || hostingEnvironment.IsEnvironment("DockerDevelopment"); //追加
//     }
// }
