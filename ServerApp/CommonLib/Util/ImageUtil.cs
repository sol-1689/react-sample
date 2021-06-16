using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using ImageMagick;

namespace SAIN
{
    /// <summary>
    /// 画像の操作ユーティリティクラス
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// 指定された画像ファイルを圧縮したMemoryStreamデータを取得する
        /// </summary>
        /// <param name="ms">MemoryStream型の変数（参照渡し）</param>
        /// <param name="imageFullFilePath">圧縮対象の画像ファイルパス</param>
        /// <param name="qualityLevel">圧縮レベル0～100まで（数値が低いほど圧縮率高）</param>
        public static void Compression(MemoryStream ms, string imageFullFilePath, int? qualityLevel = null, Size? size = null)
        {
            if (!File.Exists(imageFullFilePath))
                throw new Exception("指定されたファイルが存在しない.");

            using (var image = new MagickImage(imageFullFilePath))
            {
                AdjustImage(image, qualityLevel, size);
                image.Write(ms);
            }
        }

        /// <summary>
        /// 指定された画像ファイルを圧縮・リサイズした画像ファイルをバイト配列として取得する。
        /// </summary>
        /// <param name="imageFullFilePath">圧縮対象の画像ファイルパス</param>
        /// <param name="qualityLevel">圧縮レベル0～100まで（数値が低いほど圧縮率高）</param>
        public static byte[] Compression(string imageFullFilePath, int? qualityLevel = null, Size? size = null)
        {
            if (!File.Exists(imageFullFilePath))
                return null;

            using (var image = new MagickImage(imageFullFilePath))
            {
                AdjustImage(image, qualityLevel, size);
                return image.ToByteArray();
            }
        }

        /// <summary>
        /// 指定された画像ファイルを圧縮・リサイズした画像ファイルをバイト配列として取得する。
        /// </summary>
        /// <param name="imageBytes">画像のバイト配列</param>
        /// <param name="qualityLevel">圧縮レベル0～100まで（数値が低いほど圧縮率高）</param>
        /// <param name="size">画像サイズ。</param>
        public static byte[] Compression(byte[] imageBytes, int? qualityLevel = null, Size? size = null)
        {
            using (var image = new MagickImage(imageBytes))
            {
                AdjustImage(image, qualityLevel, size);
                return image.ToByteArray();
            }
        }


        /// <summary>
        /// 指定された画像ファイルを圧縮・リサイズした画像ファイルをバイト配列として取得する。
        /// </summary>
        /// <param name="ms">MemoryStream型の変数（参照渡し）</param>
        /// <param name="originalImage">オリジナル画像のバイト配列</param>
        /// <param name="qualityLevel">圧縮レベル0～100まで（数値が低いほど圧縮率高）</param>
        /// <param name="size">画像サイズ。</param>
        public static void Compression(MemoryStream ms, byte[] originalImage, int? qualityLevel = null, Size? size = null)
        {
            using (var image = new MagickImage(originalImage))
            {
                AdjustImage(image, qualityLevel, size);
                image.Write(ms);
            }
        }

        /// <summary>
        /// 与えられたパラメータを元にImageを調整する。
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="qualityLevel">Quality level.</param>
        /// <param name="size">Size.</param>
        static void AdjustImage(MagickImage image, int? qualityLevel = null, Size? size = null)
        {
            if (qualityLevel != null)
                image.Quality = qualityLevel.Value;

            if (size != null)
                image.Resize(size.Value.Width, size.Value.Height);
        }
    }
}
