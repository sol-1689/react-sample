
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Transactions;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Domain.Util
{
    /// <summary>
    /// 汎用的な処理を保持したクラス。
    /// 他プロジェクトに移植しても使えるような汎用的な処理をまとめる。
    /// </summary>
    public class CommonUtil
    {

        

        /// <summary>
        /// フォーマットされた日時を返す
        /// </summary>
        /// <param name="dt">日付文字列 YYYYMMDD</param>
        /// <param name="tm">時間文字列 hhmmss</param>
        /// <returns>YYYY/MM/DD HH:mm:ss(日本)</returns>
        public static string FormatDateTimeString(string dt, string tm, string format = null)
        {
            if (dt == null || tm == null) return null;
            if ((dt.Trim().Length < 8) || (tm.Trim().Length < 6)) return "";

            if (format == null)
                format = "G";
            return DateTime.ParseExact(dt + tm, "yyyyMMddHHmmss", null).ToString(format);
        }

        /// <summary>
        /// フォーマットされた日付を返す
        /// </summary>
        /// <param name="dt">日付文字列 YYYYMMDD</param>
        /// <returns>YYYY/MM/DD</returns>
        public static string FormatDateString(string dt, string format = "yyyy/MM/dd")
        {
            if (dt == null) return null;
            if ((dt.Trim().Length < 8)) return "";

            return DateTime.ParseExact(dt, "yyyyMMdd", null).ToString(format);
        }

        /// <summary>
        /// フォーマットされた日付を返す。
        /// フォーマットに失敗した場合は、そのままの値を返す。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string TryFormatDateTimeString(string dt, string format = "yyyy/MM/dd HH:mm:ss")
        {
            return CommonUtil.TryFormatDateTimeString(dt, format);
        }

        /// <summary>
        /// フォーマットされた日付を返す。
        /// フォーマットに失敗した場合は、そのままの値を返す。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string TryFormatDateString(string dt, string format = "yyyy/MM/dd")
        {
            if (dt == null || (dt.Trim().Length != 8))
                return dt;

            DateTime result;
            bool isSuccess = DateTime.TryParseExact(dt, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out result);

            if (isSuccess)
                return result.ToString(format);
            else
                return dt;
        }

        /// <summary>
        /// スラッシュなしの日付形式の文字列にスラッシュを追加にして返す。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string AddSlashToDateString(string dt)
        {
            if (string.IsNullOrWhiteSpace(dt))
                return "";

            dt = CommonUtil.RemoveSlash(dt);

            string result = "";

            if(dt.Length >= 4)
                result = dt.Substring(0, 4);

            if (dt.Length >= 6)
                result = result + "/" + dt.Substring(4, 2);

            if (dt.Length >= 8)
                result = result + "/" + dt.Substring(6, 2);

            return result;
        }

        /// <summary>
        /// 現在の日付を文字列として取得する。
        /// </summary>
        /// <param name="format">日付フォーマット</param>
        /// <returns>Formatされた日付文字列</returns>
        public static string GetNow(string format = "yyyy/MM/dd HH:mm:ss")
        {
            return DateTime.Now.ToString(format);
        }


        /// <summary>
        /// 現在の日付に指定された日数を加算した文字列を取得する。
        /// </summary>
        /// <param name="format">日付フォーマット</param>
        /// <returns>Formatされた日付文字列</returns>
        public static string GetAddedDate(int addDate, string format = "yyyyMMdd")
        {
            return DateTime.Now.AddDays(addDate).ToString(format);
        }

        /// <summary>
        /// 現在の日付に指定された月数を加算した文字列を取得する。
        /// </summary>
        /// <param name="addMonth"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetAddedMonth(int addMonth, string format = "yyyyMMdd")
        {
            return DateTime.Now.AddMonths(addMonth).ToString(format);
        }

        /// <summary>
        /// 指定された日数を加算した日付文字列を取得する。
        /// </summary>
        /// <param name="format">時刻フォーマット</param>
        /// <returns>Formatされた日付文字列</returns>
        public static string GetAddedDateString(string dt, int addDate, string dtFormat, string returnFormat = "yyyyMMdd")
        {
            return DateTime.ParseExact(dt, dtFormat, null).AddDays(addDate).ToString(returnFormat);
        }

        /// <summary>
        /// 現在の日付を文字列として取得する。
        /// </summary>
        /// <param name="format">日付フォーマット</param>
        /// <returns>Formatされた日付文字列</returns>
        public static string GetNowDate(string format = "yyyyMMdd")
        {
            return DateTime.Now.ToString(format);
        }

        /// <summary>
        /// 現在の日付時刻を文字列として取得する。
        /// </summary>
        /// <param name="format">日付フォーマット</param>
        /// <returns>Formatされた日付文字列</returns>
        public static string GetNowDateTime(string format = "yyyyMMddHHmmss")
        {
            return DateTime.Now.ToString(format);
        }


        /// <summary>
        /// 現在の時刻を文字列として取得する。
        /// </summary>
        /// <param name="format">時刻フォーマット</param>
        /// <returns>Formatされた日付文字列</returns>
        public static string GetNowTime(string format = "HHmmss")
        {
            return DateTime.Now.ToString(format);
        }

        /// <summary>
        /// 1つ目の日付が2つ目の日付より先の日付か。
        /// </summary>
        /// <param name="date"></param>
        /// <param name="baseDate"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool IsForwardDate(string date, string baseDate = null, string format = "yyyyMMdd")
        {
            if (string.IsNullOrEmpty(date))
                return false;

            baseDate = baseDate ?? DateTime.Now.ToString(format);

            return date.CompareTo(baseDate) > 0;
        }


        /// <summary>
        /// スラッシュを取り除く
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSlash(string str)
        {
            if (!String.IsNullOrEmpty(str))
                str = str.Replace("/", "");
            return str;
        }

        /// <summary>
        /// ハイフンを取り除く
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveDash(string str)
        {
            if (!String.IsNullOrEmpty(str))
                str = str.Replace("-", "").Replace("ー", "");
            return str;
        }

        /// <summary>
        /// 末尾の空白を取り除く
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimEndBlank(string str)
        {
            if (!String.IsNullOrEmpty(str))
                str = str.TrimEnd();
            return str;
        }

        /// <summary>
        /// 文字列の空白を除去し、ブランクであればNULLに変換する
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAndConvertToNull(string str)
        {

            if (str == "")
            {
                str = null;
            }

            if (str != null)
            {
                str = str.Trim();
            }

            return str;
        }

        /// <summary>
        /// ストリングをSHA1ハッシュ化する
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] MakeSHA1Hash(string s)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);
            System.Security.Cryptography.SHA1Managed shaM = new System.Security.Cryptography.SHA1Managed();
            return shaM.ComputeHash(data);
        }


        /// <summary>
        /// 日付の形式を変換する。
        /// </summary>
        /// <param name="baseDateString"></param>
        /// <param name="fromFormat"></param>
        /// <param name="toFormat"></param>
        /// <returns></returns>
        public static string ConvertDateFormat(string baseDateString, string fromFormat, string toFormat)
        {
            DateTime outDate;
            DateTime.TryParseExact(baseDateString, fromFormat, null, System.Globalization.DateTimeStyles.AssumeLocal, out outDate);

            string date = outDate.ToString(toFormat);
            return date;
        }


        /// <summary>
        /// 日付形式の文字列をチェックする。
        /// </summary>
        /// <param name="fromStr"></param>
        /// <returns></returns>
        public static bool IsDateFormat(string dateStr, string format = "yyyy/MM/dd")
        {
            if (string.IsNullOrEmpty(dateStr))
                return false;

            return DateTime.TryParseExact(dateStr, format, null, DateTimeStyles.None, out var d);
        }


        /// <summary>
        /// 全ての空白系の文字を取り除く
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveBlank(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                str = str.Replace(" ", "");
                str = str.Replace("　", "");
                str = str.Replace("\t", "");
                str = str.Replace("\n", "");
            }
            return str;
        }

        /// <summary>
        /// 全ての改行の文字を取り除く
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveNewLine(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                foreach (string each in NewLineStrs)
                {
                    str = str.Replace(each, "");
                }
                //str = str.Replace("\n", "");
                //str = str.Replace("\r", "");
                //str = str.Replace("\r\n", "");
                //str = str.Replace(Environment.NewLine, "");
            }
            return str;
        }

        /// <summary>
        /// 渡された文字コードを文字列に変換する。
        /// </summary>
        /// <param name="charCode16">UTF-8の文字コードを表す16進数。</param>
        /// <returns></returns>
        public static string GetStringFromUtfCharCode(int charCode16)
        {
            char c = Convert.ToChar(charCode16);  // 数値(文字コード) -> 文字
            string newChar = c.ToString();    // "が" という「文字列」
            return newChar;
        }

        /// <summary>
        /// 渡された文字コードを文字列に変換する。
        /// </summary>
        /// <param name="charCode16">UTF-8の文字コードを表す16進数を表す文字列。</param>
        /// <returns></returns>
        public static string GetStringFromUtfCharCode(string charCode)
        {
            int charCode16 = Convert.ToInt32(charCode, 16);  // 16進数文字列 -> 数値
            char c = Convert.ToChar(charCode16);  // 数値(文字コード) -> 文字
            string newChar = c.ToString();    // "が" という「文字列」
            return newChar;
        }


        ///// <summary>
        ///// サムネイル情報を取得・作成する。
        ///// </summary>
        ///// <param name="fileStream"></param>
        ///// <returns></returns>
        //public static MemoryStream CreateThumbnail(MemoryStream fileStream,int ThumbWidth, int ThumbHeight)
        //{
        //    if (fileStream == null)
        //        return null;

        //    //サムネイルの作成
        //    Image originalImage = Image.FromStream(fileStream, true, false);
        //    int[] propertyIds = originalImage.PropertyIdList;

        //    // 画像サイズ、方向取得
        //    int indexWidth = Array.IndexOf(propertyIds, 0xA002);
        //    int indexHeight = Array.IndexOf(propertyIds, 0xA003);
        //    int indexOrientaion = Array.IndexOf(propertyIds, 0x0112);

        //    // スマホ写真などで画像サイズなどのindexが取得できない場合(-1)は、画像情報を取得(画像方向は1固定)
        //    int width = 0;
        //    int height = 0;
        //    int orientaion = 1;

        //    if (indexWidth != -1)
        //    {
        //        PropertyItem prop = originalImage.PropertyItems[indexWidth];
        //        width = BitConverter.ToUInt16(prop.Value, 0);
        //    }
        //    else
        //    {
        //        width = originalImage.Width;
        //    }

        //    if (indexHeight != -1)
        //    {
        //        PropertyItem prop = originalImage.PropertyItems[indexHeight];
        //        height = BitConverter.ToUInt16(prop.Value, 0);
        //    }
        //    else
        //    {
        //        height = originalImage.Height;
        //    }

        //    ThumbWidth = ThumbWidth > width ? width : ThumbWidth;
        //    ThumbHeight = ThumbHeight > height ? height : ThumbHeight;

        //    if (indexOrientaion != -1)
        //    {
        //        PropertyItem prop = originalImage.PropertyItems[indexOrientaion];
        //        orientaion = prop.Value[0];

        //        // 画像方向未定(0)の場合は、回転処理なしにする。
        //        if (orientaion == 0)
        //            orientaion = 1;
        //    }
        //    int dataIndex = Array.IndexOf(propertyIds, 0x501b); // サムネイル・データindex

        //    //回転なし
        //    if (orientaion == 1)
        //    {
        //        Image thumbnail = null;
        //        if (dataIndex == -1)
        //        {
        //            //オリジナル画像にサムネイルが含まれない場合、サムネイルを作成する。
        //            thumbnail = originalImage.GetThumbnailImage(ThumbWidth, ThumbHeight, () => false, IntPtr.Zero);
        //        }
        //        else
        //        {
        //            //オリジナル画像にサムネイルが含まれる場合、サムネイルを取得する。
        //            PropertyItem prop = originalImage.PropertyItems[dataIndex];
        //            byte[] thmbnailBytes = prop.Value;

        //            ImageConverter imgconv = new ImageConverter();
        //            thumbnail = (Image)imgconv.ConvertFrom(thmbnailBytes);
        //        }
        //        MemoryStream result = new MemoryStream();
        //        thumbnail.Save(result, ImageFormat.Jpeg);
        //        thumbnail.Dispose();

        //        originalImage.Dispose();
        //        result.Seek(0, SeekOrigin.Begin);
        //        return result;
        //    }
        //    else
        //    {
        //        // 回転有
        //        LogUtil.Info("サムネ回転あり");
        //        // 回転有の場合は、現状横回転のみのため、縦横を入れ替える。
        //        Image rotatedImage = RotateImage(originalImage, orientaion);
        //        int[] rotatedIds = rotatedImage.PropertyIdList;

        //        //回転画像でサムネイルを生成する。
        //        Image thumbnail = rotatedImage.GetThumbnailImage(ThumbHeight, ThumbWidth, () => false, IntPtr.Zero);

        //        MemoryStream thumbnailStream = new MemoryStream();
        //        thumbnail.Save(thumbnailStream, ImageFormat.Jpeg);
        //        thumbnail.Dispose();

        //        return thumbnailStream;

        //        //// サムネイルを元のイメージファイルにセット
        //        //if (dataIndex > 0)
        //        //{
        //        //    PropertyItem dataProp = rotatedImage.PropertyItems[dataIndex];
        //        //    dataProp.Value = thumbnailStream.ToArray();
        //        //    rotatedImage.SetPropertyItem(dataProp);
        //        //}

        //        //// 画像方向も再セット
        //        //if (indexOrientaion > 0)
        //        //{
        //        //    PropertyItem orientaionProp = originalImage.PropertyItems[indexOrientaion];
        //        //    orientaionProp.Value[0] = 1; //そのまま
        //        //    rotatedImage.SetPropertyItem(orientaionProp);
        //        //}

        //        //ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders().Where(e => e.MimeType == "image/jpeg").FirstOrDefault();

        //        //// エンコーダに渡すパラメータの作成
        //        //System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
        //        //EncoderParameter encParam = new EncoderParameter(encoder, 99L);

        //        //// パラメータを配列に格納
        //        //EncoderParameters encParams = new EncoderParameters(1);
        //        //encParams.Param[0] = encParam;

        //        ////画像 バイナリーに変換
        //        //MemoryStream reloatedStream = new MemoryStream();
        //        //rotatedImage.Save(reloatedStream, jpgEncoder, encParams);
        //        //rotatedImage.Dispose();

        //        //originalImage.Dispose();
        //        //reloatedStream.Seek(0, SeekOrigin.Begin);
        //        //return reloatedStream;
        //        //return new C0020_ThumbnailInfo(orientaion, width, height, thumbnailStream, reloatedStream);
        //    }
        //}

        ///// <summary>
        ///// 画像を回転させる。
        ///// 画像回転 6(90度), 8(270度)のみ対応
        ///// </summary>
        ///// <param name="orgImage"></param>
        ///// <param name="orientation"></param>
        ///// <returns></returns>
        //public static Image RotateImage(Image orgImage, int orientation)
        //{
        //    Image rotateImage = (Image)orgImage.Clone();

        //    var flip = RotateFlipType.RotateNoneFlipNone;
        //    switch (orientation)
        //    {
        //        case 6:
        //            flip = RotateFlipType.Rotate90FlipNone;
        //            break;
        //        case 8:
        //            flip = RotateFlipType.Rotate270FlipNone;
        //            break;
        //    }
        //    // 画像回転
        //    rotateImage.RotateFlip(flip);

        //    return rotateImage;
        //}

        ///// <summary>
        ///// 圧縮した画像のbyte配列を取得する。
        ///// </summary>
        ///// <param name="imgBytes"></param>
        ///// <returns></returns>
        //public static byte[] GetThumbnailByte(byte[] imgBytes, long compressionRatio = 20L)
        //{
        //    var imageConverter = new ImageConverter();

        //    using (Image originalImage = (Image)imageConverter.ConvertFrom(imgBytes))
        //    using (Bitmap bitmap = new Bitmap(originalImage))
        //    {
        //        ImageCodecInfo codecinfo = GetEncoderInfo("image/jpeg");
        //        var encoder = System.Drawing.Imaging.Encoder.Quality;

        //        EncoderParameters encoderParameters = new EncoderParameters(1);

        //        EncoderParameter encoderParameter = new EncoderParameter(encoder, compressionRatio);
        //        encoderParameters.Param[0] = encoderParameter;

        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            bitmap.Save(memoryStream, codecinfo, encoderParameters);
        //            memoryStream.Seek(0, SeekOrigin.Begin);
        //            return memoryStream.ToArray();
        //        }
        //    }
        //}

        ///// <summary>
        ///// 指定されたMimeTypeの画像エンコード情報を取得する。
        ///// </summary>
        ///// <param name="mimeType"></param>
        ///// <returns></returns>
        //public static ImageCodecInfo GetEncoderInfo(string mimeType)
        //{
        //    ImageCodecInfo[] encoders;
        //    encoders = ImageCodecInfo.GetImageEncoders();
        //    for (int i = 0; i < encoders.Length; ++i)
        //    {
        //        if (encoders[i].MimeType == mimeType)
        //            return encoders[i];
        //    }
        //    return null;
        //}


        /// <summary> 
        /// 指定した精度の数値に切り捨てします。
        /// </summary>
        /// <param name="dValue">丸め対象の倍精度浮動小数点数。</param>
        /// <param name="iDigits">戻り値の有効桁数の精度。</param>
        /// <returns>iDigits に等しい精度の数値に切り捨てられた数値。</returns>
        public static double ToRoundDown(double dValue, int iDigits)
        {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef :
                                Math.Ceiling(dValue * dCoef) / dCoef;
        }


        /// <summary>
        /// 指定した精度の数値に切り捨てします。
        /// </summary>
        /// <param name="dValue">丸め対象の倍精度浮動小数点数。</param>
        /// <param name="iDigits">戻り値の有効桁数の精度。</param>
        /// <returns>iDigits に等しい精度の数値に切り捨てられた数値。</returns>
        public static decimal ToRoundDown(decimal dValue, int iDigits)
        {
            string doubleStr = Math.Pow(10, iDigits).ToString();
            decimal dCoef = decimal.Parse(doubleStr);

            return dValue > 0 ? Math.Floor(dValue * dCoef) / dCoef :
                                Math.Ceiling(dValue * dCoef) / dCoef;
        }

        /// <summary>
        /// オブジェクトをInt型に変換する。変換対象がNULLまたは変換エラーは0で返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static int ConvertToInt(object obj)
        {
            if (obj == null)
                return 0;

            string str = obj.ToString();
            str = str.Replace(",", "");

            int result;
            if (!int.TryParse(str, out result))
                return 0;

            return result;
        }

        /// <summary>
        /// オブジェクトをInt型に変換する。変換対象がNULL、または変換エラーはNullで返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static int? ConvertToNullableInt(object obj)
        {
            if (obj == null)
                return null;

            string str = obj.ToString().Replace(",", "");

            int result;
            if (!int.TryParse(str, out result))
                return null;

            return result;
        }

        /// <summary>
        /// オブジェクトをDecimal型に変換する。NULLまたは変換エラーは0で返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(object obj)
        {
            if (obj == null)
                return 0m;

            string str = obj.ToString();
            str = str.Replace(",", "");

            decimal result;
            if (!decimal.TryParse(str, out result))
                return 0m;

            return result;
        }

        /// <summary>
        /// オブジェクトをDecimal型に変換する。NULLまたは変換エラーはNullで返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static decimal? ConvertToNullableDecimal(object obj)
        {
            if (obj == null)
                return null;

            string str = obj.ToString().Replace(",", "");

            decimal result;
            if (!decimal.TryParse(str, out result))
                return null;

            return result;
        }

        /// <summary>
        /// オブジェクトをDecimal型に変換する。NULLまたは変換エラーは0で返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static double ConvertToDouble(object obj)
        {
            if (obj == null)
                return 0d;

            string str = obj.ToString();
            str = str.Replace(",", "");

            double result;
            if (!double.TryParse(str, out result))
                return 0d;

            return result;
        }

        /// <summary>
        /// オブジェクトをDouble型に変換する。NULLまたは変換エラーはNullで返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static double? ConvertToNullableDouble(object obj)
        {
            if (obj == null)
                return null;

            string str = obj.ToString().Replace(",", "");

            double result;
            if (!double.TryParse(str, out result))
                return null;

            return result;
        }


        /// <summary>
        /// オブジェクトをLong型に変換する。NULLまたは変換エラーは0で返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static double ConvertToLong(object obj)
        {
            if (obj == null)
                return 0d;

            string str = obj.ToString();
            str = str.Replace(",", "");

            long result;
            if (!long.TryParse(str, out result))
                return 0d;

            return result;
        }

        /// <summary>
        /// オブジェクトをLong型に変換する。NULLまたは変換エラーはNullで返す。
        /// </summary>
        /// <param name="obj">変換対象</param>
        /// <param name="needRemoveCommma">変換対象にカンマが含まれる場合はtrue</param>
        /// <returns></returns>
        public static long? ConvertToNullableLong(object obj)
        {
            if (obj == null)
                return null;

            string str = obj.ToString().Replace(",", "");

            long result;
            if (!long.TryParse(str, out result))
                return null;

            return result;
        }

        /// <summary>
        /// Int型の数値をフォーマットしたStringで返す。数値がNullの場合、文字列もNullで返す。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToIntString(int? value, string format = "#,0")
        {
            string result = null;
            if (value != null)
            {
                result = value.Value.ToString(format);
            }
            return result;
        }

        /// <summary>
        /// Deciml型の数値をフォーマットしたStringで返す。数値がNullの場合、文字列もNullで返す。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDecimalString(decimal? value, string format = "#,0")
        {
            string result = "";
            if (value != null)
            {
                result = value.Value.ToString(format);
            }
            return result;
        }

        ///// <summary>
        ///// 全角文字に変換する。
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static string ConvertToZENKAKU(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        return str;

        //    return Strings.StrConv(str, VbStrConv.Wide);
        //}

        ///// <summary>
        ///// 半角文字に変換する。
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static string ConvertToHANKAKU(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        return str;

        //    return Strings.StrConv(str, VbStrConv.Narrow);
        //}

        ///// <summary>
        ///// カナ文字に変換する。
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static string ConvertToKANA(string str)
        //{
        //    if (string.IsNullOrEmpty(str))
        //        return str;

        //    return Strings.StrConv(str, VbStrConv.Katakana);
        //}



        /// <summary>
        /// 数値で表された年、月を0埋めをした6桁の文字列で返す。
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static string FormatYearMonthNumber(int? year, int? month)
        {
            if (year == null)
            {
                year = 0;
            }

            if (month == null)
            {
                month = 1;
            }

            if (IsBetween(0, 9999, (int)year) && IsBetween(1, 12, (int)month))
            {
                string y = string.Format("{0:D4}", year);
                string m = string.Format("{0:D2}", month);
                return y + m;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 指定の書式で表される文字列をDateTime形式に変換する。変換に失敗した場合はNullを返す。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? ConvertToDateTime(string dt, string format)
        {
            if (string.IsNullOrEmpty(dt) || string.IsNullOrEmpty(format))
            {
                return null;
            }

            DateTime result;
            bool isSuccess = DateTime.TryParseExact(dt, format, null, System.Globalization.DateTimeStyles.None, out result);

            if (isSuccess)
            {
                return result;
            }
            else
            {
                return null;
            }

        }

        //public const string[] JAPANESE_YEAR_LIST = { "明治", "大正", "昭和", "平成" };
        /// <summary>
        /// 和暦を取得する
        /// </summary>
        public static string GetJapaneseDateString(DateTime targetDate, string format = "ggyy年度")
        {
            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();

            return targetDate.ToString(format, culture);
        }

        /// <summary>
        /// 和暦を取得する
        /// </summary>
        public static string GetJapaneseDateString(DateTime? targetDate, string format = "ggyy年度")
        {
            if (targetDate == null)
                return "";

            return GetJapaneseDateString(targetDate.Value, format);
        }


        /// <summary>
        /// 対象日付の月初めを取得する
        /// </summary>
        /// <param name="targetMonth"></param>
        /// <returns></returns>
        public static DateTime GetTargetMonthStartDate(DateTime targetMonth)
        {
            return new DateTime(targetMonth.Year, targetMonth.Month, 1);
        }

        /// <summary>
        /// 対象日付の月終わりを取得する
        /// </summary>
        /// <param name="targetMonth"></param>
        /// <returns></returns>
        public static DateTime GetTargetMonthEndDate(DateTime targetMonth)
        {
            return new DateTime(targetMonth.Year, targetMonth.Month,
                DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month));
        }

        /// <summary>曜日名一覧 </summary>
        public const string WEEK_NAME_LIST = "日月火水木金土";
        /// <summary>
        /// 現在日付から曜日を取得する
        /// </summary>
        /// <param name="targetDateTime"></param>
        /// <returns></returns>
        public static string GetWeekNameJpn(DateTime targetDateTime)
        {
            return (WEEK_NAME_LIST).Substring(int.Parse(targetDateTime.DayOfWeek.ToString("d")), 1);
        }

        /// <summary>
        /// 生年月日から現在の年齢を計算する
        /// </summary>
        /// <param name="birthDay">生年月日</param>
        /// <param name="baseDate">基準日（Nullの場合、現在日とする）</param>
        /// <returns></returns>
        public static int? CalculateNowAge(DateTime? birthDay, DateTime? baseDate = null)
        {
            if (birthDay == null)
                return null;

            int age;

            if (baseDate == null)
            {
                baseDate = DateTime.Today;
            }

            age = baseDate.Value.Year - birthDay.Value.Year;

            if (baseDate.Value.Date < birthDay.Value.AddYears(age).Date)
            {
                // 誕生日が来てない場合は1歳引く
                age--;
            }

            return age;
        }



        ///// <summary>
        ///// 生年月日から年齢を計算する
        ///// </summary>
        ///// <param name="birthDate">生年月日</param>
        ///// <param name="today">現在の日付</param>
        ///// <returns>年齢</returns>
        //public static int? GetAge(DateTime? birthDate, DateTime? targetDate)
        //{

        //    if (birthDate == null || targetDate == null)
        //        return null;

        //    int age = targetDate.Value.Year - birthDate.Value.Year;

        //    //誕生日がまだ来ていなければ、1引く
        //    if (targetDate.Value.Month < birthDate.Value.Month ||
        //        (targetDate.Value.Month == birthDate.Value.Month &&
        //        targetDate.Value.Day < birthDate.Value.Day))
        //    {
        //        age--;
        //    }

        //    return age;
        //}


        static readonly string[] kl = new string[] { "", "十", "百", "千" };
        static readonly string[] tl = new string[] { "", "万", "億", "兆", "京" };
        static readonly string[] nl = new string[] { "", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

        /// <summary>
        /// 算用数字から漢数字へ変換
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertToKansuji(long number)
        {
            if (number == 0)
            {
                return "〇";
            }

            string str = "";
            int keta = 0;
            while (number > 0)
            {
                int k = keta % 4;
                int n = (int)(number % 10);

                if (k == 0 && number % 10000 > 0)
                {
                    str = tl[keta / 4] + str;
                }

                if (k != 0 && n == 1)
                {
                    str = kl[k] + str;
                }
                else if (n != 0)
                {
                    str = nl[n] + kl[k] + str;
                }

                keta++;
                number /= 10;
            }
            return str;
        }

        ///// <summary>既存の星座一覧 </summary>
        //static readonly List<SeizaDateModel> seizaList = new List<SeizaDateModel>
        //    {
        //        new SeizaDateModel{ SEIZA = "山羊座", DT_FROM_MMdd =  101,DT_TO_MMdd =  119},
        //        new SeizaDateModel{ SEIZA = "水瓶座", DT_FROM_MMdd =  120,DT_TO_MMdd =  218},
        //        new SeizaDateModel{ SEIZA = "魚座",   DT_FROM_MMdd =  219,DT_TO_MMdd =  320},
        //        new SeizaDateModel{ SEIZA = "牡羊座", DT_FROM_MMdd =  321,DT_TO_MMdd =  419},
        //        new SeizaDateModel{ SEIZA = "牡牛座", DT_FROM_MMdd =  420,DT_TO_MMdd =  520},
        //        new SeizaDateModel{ SEIZA = "双子座", DT_FROM_MMdd =  521,DT_TO_MMdd =  621},
        //        new SeizaDateModel{ SEIZA = "かに座", DT_FROM_MMdd =  622,DT_TO_MMdd =  722},
        //        new SeizaDateModel{ SEIZA = "獅子座", DT_FROM_MMdd =  723,DT_TO_MMdd =  822},
        //        new SeizaDateModel{ SEIZA = "乙女座", DT_FROM_MMdd =  823,DT_TO_MMdd =  922},
        //        new SeizaDateModel{ SEIZA = "天秤座", DT_FROM_MMdd =  923,DT_TO_MMdd = 1023},
        //        new SeizaDateModel{ SEIZA = "蠍座",   DT_FROM_MMdd = 1024,DT_TO_MMdd = 1122},
        //        new SeizaDateModel{ SEIZA = "射手座", DT_FROM_MMdd = 1123,DT_TO_MMdd = 1221},
        //        new SeizaDateModel{ SEIZA = "山羊座", DT_FROM_MMdd = 1222,DT_TO_MMdd = 1231}
        //    };

        ///// <summary>
        ///// 生年月日から星座を取得する
        ///// </summary>
        ///// <param name="birthDate">生年月日</param>
        ///// <returns>星座</returns>
        //public static string GetSeiza(DateTime? birthDate)
        //{
        //    if (birthDate == null)
        //        return null;


        //    int birthDateMMdd = birthDate.Value.Month * 100 + birthDate.Value.Day;
        //    foreach (var seiza in seizaList)
        //    {
        //        if (birthDateMMdd >= seiza.DT_FROM_MMdd && birthDateMMdd <= seiza.DT_TO_MMdd)
        //            return seiza.SEIZA;
        //    }

        //    return null;
        //}

        /// <summary>
        /// 指定された文字で埋められた文字列を取得する。
        /// </summary>
        /// <param name="maxLength">文字数</param>
        /// <param name="c">文字列を埋める文字</param>
        /// <returns></returns>
        public static string GetFilledString(int? maxLength, char c = 'あ')
        {
            string result = new string(c, maxLength.Value);
            return result;
        }



        /// <summary>
        /// 渡されたオブジェクトをコピーした新しいオブジェクトを作成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public static T DeepCopyFrom<T>(T target)
        {
            object clone = null;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, target);
                stream.Position = 0;
                clone = formatter.Deserialize(stream);
            }
            return (T)clone;
        }

        /// <summary>
        /// 渡されたオブジェクトをコピーした新しいオブジェクトを作成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObject"></param>
        /// <returns></returns>
        public static List<T> DeepCopyListFrom<T>(IEnumerable<T> targetList)
        {
            if (targetList == null)
                return new List<T>();

            List<T> result = targetList.Select(m => DeepCopyFrom(m)).ToList();
            return result;
        }

        /// <summary>
        /// 渡されたオブジェクトから、渡されたプロパティ名に合致するプロパティの値を取得する。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T model, string propName)
        {
            object value = model.CallGetPropertyByName(propName);
            return value;
        }

        /// <summary>
        /// 渡されたオブジェクト全体をValidateし結果を返す。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static List<ValidationResult> GetValidateResult<T>(T model)
        {
            var validationResult = new List<ValidationResult>();

            var context = new ValidationContext(model, null, null);

            Validator.TryValidateObject(model, context, validationResult);
            return validationResult;
        }

        
        /// <summary>
        /// 渡されたオブジェクトの指定されたプロパティをValidateし結果を返す。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static List<ValidationResult> GetValidateResult(object model, string propertyName)
        {
            object value = model.CallGetPropertyByName(propertyName);
            var context = new ValidationContext(model) { MemberName = propertyName };
            var validationResult = new List<ValidationResult>();

            Validator.TryValidateProperty(value, context, validationResult);

            return validationResult;
        }

        /// <summary>
        /// 渡されたオブジェクトの指定されたプロパティをValidateし結果を返す。
        /// </summary>
        /// <param name="model">IValidatableObjectを実装したmodel</param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static List<ValidationResult> GetValidateResult(IValidatableObject model, string propertyName)
        {
            object value = model.CallGetPropertyByName(propertyName);
            var context = new ValidationContext(model) { MemberName = propertyName };
            var validationResult = new List<ValidationResult>();

            Validator.TryValidateProperty(value, context, validationResult);

            //IValidatableObject実装してた場合の処理追加
            var result = model.Validate(context);
           // var errors = result.Where(m => m.MemberNames.Contains(propertyName));
           // validationResult.AddRange(errors);

           return validationResult;
        }

        /// <summary>
        /// 渡されたオブジェクトのプロパティのValidate属性から、エラーメッセージを作成する。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetValidateErrorMessage(string propertyName, object model)
        {
            //validate属性チェック用の汎用ロジック。
            var validationResult = CommonUtil.GetValidateResult(model, propertyName);
            string message = string.Join(Environment.NewLine, validationResult.Select(m => m.ErrorMessage)); 
            return message;
        }


        /// <summary>
        /// 渡されたオブジェクトのプロパティのValidate属性から、エラーメッセージを作成する。
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="model">IValidatableObjectを実装したmodel</param>
        /// <returns></returns>
        public static string GetValidateErrorMessage(string propertyName, IValidatableObject model)
        {
            //validate属性チェック用の汎用ロジック。
            var validationResult = CommonUtil.GetValidateResult(model, propertyName);
            string message = string.Join(Environment.NewLine, validationResult.Select(m => m.ErrorMessage)); 
            return message;
        }

        ///// <summary>
        ///// 渡されたIDataErrorInfoをValidateし結果を返す。
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //public static IEnumerable<string> GetValidateErrorMessages(IDataErrorInfo model)
        //{
        //    var propertyNames = GetPropertyNames(model.GetType());

        //    foreach (var propertyName in propertyNames)
        //    {
        //        string errorMessage = model[propertyName];
        //        if (!string.IsNullOrEmpty(errorMessage))
        //            yield return errorMessage;
        //    }
        //}


        /// <summary>
        /// 渡されたIDataErrorInfoをValidateし結果を返す。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool ValidateDataErrorInfo(IDataErrorInfo model)
        {
            var propertyNames = GetPropertyNames(model.GetType());

            foreach (var propertyName in propertyNames)
            {
                string errorMessage = model[propertyName];
                if (!string.IsNullOrEmpty(errorMessage))
                    return false;
            }
            return true;
        }

        

        /// <summary>
        /// Typeとプロパティ名のキャッシュ。
        /// </summary>
        readonly static ConcurrentDictionary<Type, List<string>> TypePropCache = new ConcurrentDictionary<Type, List<string>>();

        /// <summary>
        /// Typeのもつプロパティの名前のリストを取得する。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetPropertyNames(Type type)
        {
            List<string> propNames = TypePropCache.GetOrAdd(type, (l) => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                                        .Where(p => !(p.GetIndexParameters().Length > 0))//インデクサは対象外
                                                                                        .Select(m => m.Name).ToList());
            return propNames;
        }

        /// <summary>
        /// 2つのリストの持つプロパティの値が等しいかをチェックする。
        /// 型のもつ同名のプロパティをチェックしているだけなので、違う型の比較もできる。
        /// </summary>
        /// <typeparam name="SelfType"></typeparam>
        /// <typeparam name="ToType"></typeparam>
        /// <param name="self"></param>
        /// <param name="to"></param>
        /// <param name="emptyStrEqualsNull">空文字をNullと同一と扱うかどうか</param>
        /// <param name="ignorePropertyName">比較対象外の列名</param>
        /// <returns>同じならtrue、違ったらfalseを返す。</returns>
        public static bool CheckListPropertiesEqual<SelfType, ToType>(IEnumerable<SelfType> _self, IEnumerable<ToType> _to,
            bool emptyStrEqualsNull = true, params string[] ignorePropertyName)
        {
            var self = _self == null ? null : _self.ToArray();
            var to = _to == null ? null : _to.ToArray();

            int selfCount = (self == null ? 0 : self.Length);
            int toCount = (to == null ? 0 : to.Length);

            if (selfCount != toCount)
                return false;

            if (selfCount == 0 && toCount == 0)
                return true;

            for (int i = 0; i < selfCount; i++)
            {
                bool isEqual = CheckPropertiesEqual(self[i], to[i], emptyStrEqualsNull, ignorePropertyName);
                if (!isEqual)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        ///  2つのオブジェクトの持つプロパティの値が等しいかをチェックする。
        ///  型のもつ同名のプロパティをチェックしているだけなので、違う型の比較もできる。
        /// </summary>
        /// <typeparam name="SelfType"></typeparam>
        /// <typeparam name="ToType"></typeparam>
        /// <param name="self"></param>
        /// <param name="to"></param>
        /// <param name="emptyStrEqualsNull">空文字をNullと同一と扱うかどうか</param>
        /// <param name="ignorePropertyName">比較対象外の列名</param>
        /// <returns>同じならtrue、違ったらfalseを返す。</returns>
        public static bool CheckPropertiesEqual(Object self, Object to, bool emptyStrEqualsNull = true, 
            params string[] ignorePropertyName)
        {
            if (self == null || to == null)
                return false;

            //プロパティ名を取得。
            Type fromType = self.GetType();
            List<string> formPropNames = GetPropertyNames(fromType);

            Type toType = to.GetType();
            List<string> toPropNames = GetPropertyNames(toType);

            //比較
            foreach (string propertyName in TypePropCache[fromType])
            {
                if (!ignorePropertyName.Contains(propertyName) && 
                    toPropNames.Contains(propertyName)) //両方にないプロパティは無視
                {
                    object selfValue = self.CallGetPropertyByName(propertyName);
                    object toValue = to.CallGetPropertyByName(propertyName);

                    if (!IsEqualsObject(selfValue, toValue, emptyStrEqualsNull))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 2つのオブジェクトが同一かをチェックする。
        /// </summary>
        /// <param name="selfValue"></param>
        /// <param name="toValue"></param>
        /// <param name="emptyStrEqualsNull"></param>
        /// <returns>同じならtrue、違ったらfalseを返す。</returns>
        public static bool IsEqualsObject(object selfValue, object toValue, bool emptyStrEqualsNull = true)
        {
            //空文字をNullと同等に扱う場合
            if (emptyStrEqualsNull)
            {
                string selfStr = selfValue as string;
                string toStr = toValue as string;

                if (selfStr == "" && toStr == null)
                    return true; //片方空文字でもう片方がnullの場合trueにする。

                if (selfStr == null && toStr == "")
                    return true; //片方空文字でもう片方がnullの場合trueにする。

                //これ以外のケースは下の処理で判別。(string以外の型なら両方nullになり必ず下の処理に行く。)
            }

            if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                return false;

            return true;
        }
       
        /// <summary>
        /// 規定の文字数以上であれば取得したデータから削除する
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="length">規定最大文字数</param>
        /// <returns>削除修正後文字列</returns>
        public static string RemoveString(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            StringInfo si = new StringInfo(str);

            if (si.LengthInTextElements > length)
                return str.Remove(length);

            return str;
        }


        

        /// <summary>
        /// 指定した日付の終了日を取得する
        /// </summary>
        /// <param name="targetDate"></param>
        /// <param name="addDateCount"></param>
        /// <returns></returns>
        public static DateTime GetEndDate(DateTime? targetDate, int? addDateCount)
        {
            //if (addDateCount == null)
            //    addDateCount = 0;
            return targetDate.Value.AddDays(addDateCount != null ? (addDateCount.Value - 1) : 0);
        }


        /// <summary>
        /// 数値範囲ﾁｪｯｸ
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="targetDate"></param>
        /// <returns></returns>
        public static bool IsBetween(int? fromNum, int? toNum, int inputNum)
        {
            return fromNum <= inputNum && toNum >= inputNum;
        }

        /// <summary>
        /// 数値範囲ﾁｪｯｸ
        /// </summary>
        /// <param name="fromNum"></param>
        /// <param name="toNum"></param>
        /// <param name="inputNum"></param>
        /// <returns></returns>
        public static bool IsBetween(decimal? fromNum, decimal? toNum, decimal inputNum)
        {
            return fromNum <= inputNum && toNum >= inputNum;
        }


        /// <summary>
        /// 指定した日付が期間内に入っているか
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="targetDate"></param>
        /// <returns></returns>
        public static bool IsBetween(DateTime fromDate, DateTime toDate, DateTime targetDate)
        {
            return fromDate <= targetDate && toDate >= targetDate;
        }


        /// <summary>
        /// 除算をする。
        /// </summary>
        /// <param name="top">分子</param>
        /// <param name="bottom">分母</param>
        /// <param name="iDigits">切り捨てをする桁数</param>
        /// <returns>指定の桁数で切り捨てた結果を返す。分母が0またはNullの場合はNullを返す。</returns>
        public static decimal? Divide(decimal? top, decimal? bottom, int iDigits = 0)
        {
            if (bottom == null || bottom == 0)
            {
                return null;
            }

            if (top == null || top == 0)
            {
                return 0;
            }

            decimal result = CommonUtil.ToRoundDown(((decimal)top) / ((decimal)bottom), iDigits);
            return result;
        }

        
        /// <summary>タグ文字列を表す正規表現。 </summary>
        static readonly Regex tagRegex = new Regex(@"<.*?>");

        /// <summary>
        /// 渡された文字列からタグ(Htmlやxmlのタグ)を除去した文字列を返す。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveTag(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = tagRegex.Replace(text, string.Empty);
            return result;
        }

        /// <summary>
        /// 郵便番号をを整形する。（例： 0001111 → 〒000-1111）
        /// </summary>
        /// <param name="zipCd"></param>
        /// <returns></returns>
        public static string FormatZipCode(string zipCd)
        {
            if(string.IsNullOrEmpty(zipCd))
                return null;

            if(!zipCd.Contains("-"))
                zipCd = zipCd.Insert(3, "-");

            if(!zipCd.Contains("〒"))
                zipCd = "〒" + zipCd;

            return zipCd;
        }

        /// <summary>
        /// リストの日付の範囲が重複していないかチェックする
        /// ※FROMまたはTOがNull、またはFROM>TOだとデータエラーとしてFalseを返します
        /// </summary>
        /// <typeparam name="T">リストの型</typeparam>
        /// <param name="list">データリスト</param>
        /// <param name="fromSelector">日付FROMのキー</param>
        /// <param name="toSelector">日付TOのキー</param>
        /// <returns>True: 重複なし　False：データエラーまたは重複あり</returns>
        public static bool CheckDateOverlap<T>(List<T> list, Func<T, DateTime?> fromSelector, Func<T, DateTime?> toSelector)
        {

            //件数チェック
            if (list == null || list.Count < 1)
            {
                return true;
            }

            //FROMとTOがNullではないか、また、FROM<TOになっているかチェックする
            foreach (var m in list)
            {
                if (fromSelector(m) == null || toSelector(m) == null)
                {
                    return false;
                }

                if (fromSelector(m) > toSelector(m))
                {
                    return false;
                }
            }

            //FROMとTOで並べ替え
            var orderedList = list.OrderBy(m => fromSelector(m)).OrderBy(m => toSelector(m)).ToList();

            //前行のTOの値を保存する
            DateTime? tempTo = null;

            foreach (var m in orderedList)
            {
                //1行目は無視
                if (tempTo != null)
                {
                    //現在行のFROMが前行のTO以下だとエラー
                    if (fromSelector(m) <= tempTo)
                    {
                        return false;
                    }
                }

                //前行のTOの値を更新する
                tempTo = toSelector(m);
            }

            return true;
        }

        ///// <summary>
        ///// HHMMのint時間を時間：分に分解する
        ///// </summary>
        ///// <param name="hhmmTime"></param>
        ///// <returns></returns>
        //public static int[] PartIntHHMMTime(int? hhmmTime)
        //{

        //    if (hhmmTime == null)
        //        return null;

        //    int[] intArr = new int[2];

        //    intArr[0] = hhmmTime.Value / 100;

        //    intArr[1] = hhmmTime.Value % 100;

        //    return intArr;
        //}


        /// <summary>
        /// HHMMのint時間(10:21なら1021とか)を時間：分に分解する
        /// </summary>
        /// <param name="hhmmTime"></param>
        /// <returns></returns>
        public static TimeSpan? PartIntHHMMTime(int? hhmmTime)
        {
            if (hhmmTime == null)
                return null;

            int hh = hhmmTime.Value / 100;
            int mm = hhmmTime.Value % 100;

            var result = new TimeSpan(hh, mm, 0);
            return result;
        }


        /// <summary>
        /// 渡されたModelを.jsonファイルとしてローカルディスクに書き出す。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="filePath"></param>
        /// <param name="JsonEncoding">デフォルトはUTF8</param>
        public static void DeleteFilesInDirectory(string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            if (!dirInfo.Exists)
                return;

            foreach (var file in dirInfo.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// json文字列を.jsonファイルとしてローカルディスクに書き出す。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="filePath"></param>
        /// <param name="JsonEncoding">デフォルトはUTF8</param>
        public static void WriteJsonString(string jsonString, string filePath, Encoding JsonEncoding = null)
        {
            JsonEncoding = JsonEncoding ?? Encoding.UTF8;
            FileInfo fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (FileStream fs = fi.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            using (StreamWriter sw = new StreamWriter(fs, JsonEncoding))
            {
                sw.Write(jsonString);
            }
        }

        /// <summary>
        /// ローカルディスクの.jsonファイルを読み込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directoryPath"></param>
        /// <param name="searchPattern"></param>
        /// <param name="JsonEncoding">デフォルトはUTF8</param>
        /// <returns></returns>
        public static string ReadCurrentFileText(string directoryPath, string searchPattern = "*", Encoding JsonEncoding = null)
        {
            JsonEncoding = JsonEncoding ?? Encoding.UTF8;
            DirectoryInfo di = new DirectoryInfo(directoryPath);

            var currentFile = di.GetFiles(searchPattern).OrderByDescending(m => m.LastWriteTime).FirstOrDefault();

            if (currentFile == null)
                return null;

            string text = File.ReadAllText(currentFile.FullName, JsonEncoding);
            return text;
        }





        /// <summary>改行文字のリスト。 </summary>
        public static readonly string[] NewLineStrs = new string[] { Environment.NewLine ,"\r" , "\n" ,"\r\n" };

        /// <summary>
        /// 改行コードで分割する。
        /// </summary>
        /// <param name="targetString"></param>
        /// <returns></returns>
        public static string[] SplitNewLine(string targetString)
        {
            if (targetString == null)
            {
                return null;
            }

            return targetString.Split(NewLineStrs, StringSplitOptions.None);
        }

        /// <summary>
        /// 改行文字かどうか
        /// </summary>
        /// <returns></returns>
        public static bool IsNewLine(string text)
        {
            bool result = NewLineStrs.Contains(text);
            return result;
        }

        /// <summary>
        /// 渡されたリストの順列を作成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetPermutation<T>(IEnumerable<T> targets, int? maxDepth = null)
        {
            return GetPermutationCore(targets, 0, maxDepth);
        }


        /// <summary>
        /// 渡されたリストの順列を作成する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<T>> GetPermutationCore<T>(IEnumerable<T> targets, int currentDepth, int? maxDepth = null)
        {
            if (targets.Any())
            {
                foreach (var header in targets) //headerは木構造のノード。1個ずづずらしていく。
                {
                    bool isHeaderExcepted = false;

                    //ノードの下に紐付くノードのリスト。
                    IEnumerable<T> headerExceptedList =
                        (maxDepth != null && currentDepth >= maxDepth.Value)
                        ? Enumerable.Empty<T>()
                        : targets.Where(m =>
                            {
                                if (!isHeaderExcepted && m.Equals(header))
                                {//headerに使っている要素以外のもの。
                                    isHeaderExcepted = true;
                                    return false;   //同じものあった場合1つだけ除く
                                }
                                else
                                    return true;
                            }).ToArray();

                    //headerノードの下に紐付くノードで再帰処理。下に行くほどリストの要素数が減っていく(下に紐付くノード数が減っていく)。
                    foreach (var each in GetPermutationCore(headerExceptedList, currentDepth + 1, maxDepth))
                        yield return (new T[] { header }).Concat(each);
                }
            }
            else
            {
                yield return targets; //要素が1つもない場合はそのまま返す。
            }
        }

        /// <summary>
        /// 渡されたリストの組み合わせ結果を取得する。
        /// </summary>
        /// <example>
        /// {1,2} {3,4} {5,6}という３つのリストを引数で受け取ると、
        /// 結果は {1,3,5} {1,3,6} {1,4,5} {1,4,6} {2,3,5} {2,3,6} {2,4,5} {2,4,6}の8通りになる。
        /// IgnoreDublicationがtrueの場合、重複した値は無視される。
        /// {1,2} {2,3}が引数の場合、{2,2}は結果に含まれない。
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="targets">可変長引数。組み合わせに使用したいリスト。</param>
        /// <param name="IgnoreDublication">重複した値を無視するか。</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> GetKumiawase<T>(bool IgnoreDublication, params IEnumerable<T>[] targets)
        {
            if (targets.Count() > 0)
            {
                var firstList = targets.First();
                var restList = GetKumiawase<T>(IgnoreDublication, targets.Skip(1).ToArray()).ToArray();

                foreach (var eachValue in firstList)
                {
                    if (restList.Length < 1)
                    {
                        yield return new List<T>() { eachValue };
                    }
                    else
                    {
                        foreach (var eachRestList in restList)
                        {
                            var list = eachRestList.ToList();

                            if (IgnoreDublication)
                            {
                                if (list.Contains(eachValue))
                                    continue;
                            }

                            list.Insert(0, eachValue);
                            yield return list;
                        }
                    }
                }

            }
        }

        ///// <summary>
        ///// 緯度経度からGeo情報を作成する。
        ///// </summary>
        ///// <param name="longitude"></param>
        ///// <param name="latitude"></param>
        ///// <param name="srid"></param>
        ///// <returns></returns>
        //public static SqlGeography CreateGeography(string longitude, string latitude, int srid = CommonStrings.GeoSRID)
        //{
        //    return CreateGeography(ConvertToDouble(longitude), ConvertToDouble(latitude), srid);
        //}

        ///// <summary>
        ///// 緯度経度からGeo情報を作成する。
        ///// </summary>
        ///// <param name="longitude"></param>
        ///// <param name="latitude"></param>
        ///// <param name="srid"></param>
        ///// <returns></returns>
        //public static SqlGeography CreateGeography(double? longitude, double? latitude, int srid = CommonStrings.GeoSRID)
        //{
        //    if (longitude == null || latitude == null)
        //        return null;

        //    var result = SqlGeography.Point(latitude.Value, longitude.Value, srid);
        //    return result;
        //}


        /// <summary>
        /// 渡されたリストの組み合わせ結果を取得する。
        /// </summary>
        /// <example>
        /// {1,2,3,4}というリストを引数で受け取り、argIterationが2の場合、
        /// 結果は {1,2} {1,3} {1,4} {2,3} {2,4} {3,4} を返す。
        /// 3の場合は [1, 2, 3], [1, 2, 4], [1, 3, 4], [2, 3, 4]。
        /// 重複した値は無視される。
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="argList"></param>
        /// <param name="argStart"></param>
        /// <param name="argIteration"></param>
        /// <param name="argIndicies"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> GetPermutationSubset<T>(IList<T> argList, int argStart, int argIteration, List<int> argIndicies = null)
        {
            argIndicies = argIndicies ?? new List<int>();
            for (int i = argStart; i < argList.Count; i++)
            {
                argIndicies.Add(i);
                if (argIteration > 0)
                {
                    foreach (var array in GetPermutationSubset(argList, i + 1, argIteration - 1, argIndicies))
                    {
                        yield return array;
                    }
                }
                else
                {
                    var array = new T[argIndicies.Count];
                    for (int j = 0; j < argIndicies.Count; j++)
                    {
                        array[j] = argList[argIndicies[j]];
                    }

                    yield return array;
                }
                argIndicies.RemoveAt(argIndicies.Count - 1);
            }
        }

        ///// <summary>
        ///// ランダムなパスワードを作成する。
        ///// </summary>
        ///// <param name="length"></param>
        ///// <param name="numberOfNonAlphanumericCharacters"></param>
        ///// <returns></returns>
        //public static string CreateRandomPassword(int length, int numberOfNonAlphanumericCharacters = 0)
        //{
        //    string pass = System.Web.Security.Membership.GeneratePassword(length, numberOfNonAlphanumericCharacters);
        //    return pass;
        //}


        /// <summary>
        /// 引数で渡されたオブジェクトがnull出ない場合に、Disposeを行う。
        /// </summary>
        /// <param name="resource"></param>
        public static void Dispose(IDisposable resource)
        {
            if (resource != null)
                resource.Dispose();
        }


        /// <summary>
        /// 引数で渡されたオブジェクトリストがnull出ない場合に、Disposeを行う。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resources"></param>
        public static void DisposeAll<T>(IEnumerable<T> resources) where T : IDisposable
        {
            if (resources == null)
                return;

            foreach (IDisposable resource in resources)
            {
                if (resource != null)
                    resource.Dispose();
            }
        }


        ///// <summary>
        ///// Expression からメンバー名を取得する。
        ///// </summary>
        ///// <typeparam name="ObjectType"></typeparam>
        ///// <typeparam name="MemberType"></typeparam>
        ///// <param name="type"></param>
        ///// <param name="expression"></param>
        ///// <returns></returns>
        //public static string GetMemberName<ObjectType, MemberType>(this ObjectType type, Expression<Func<ObjectType, MemberType>> expression)
        //{
        //    return ((MemberExpression)expression.Body).Member.Name;
        //}

        /// <summary>
        /// Expression からメンバー名を取得する。
        /// </summary>
        /// <typeparam name="MemberType"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetMemberName<ObjectType, MemberType>(Expression<Func<ObjectType, MemberType>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        ///// <summary>
        ///// 渡されたリストの順列を作成する。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="targets"></param>
        ///// <returns></returns>
        //public static IEnumerable<List<T>> GetKumiawaseAll<T>(params IEnumerable<T>[] targets)
        //{
        //    if (targets.Count() > 0)
        //    {
        //        foreach (var eachList in targets)
        //        {
        //            var restList = GetKumiawaseAll<T>(targets.Skip(1).ToArray()).ToArray();

        //            foreach (T eachValue in eachList)
        //            {
        //                if (restList.Length < 1)
        //                {
        //                    yield return new List<T>() { eachValue };
        //                }
        //                else
        //                {
        //                    foreach (var eachRestList in restList)
        //                    {
        //                        var list = eachRestList.ToList();
        //                        if (list.Contains(eachValue))
        //                            continue;

        //                        list.Insert(0, eachValue);
        //                        yield return list;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 検索の文字列を分割する。
        /// </summary>
        /// <param name="SearchText"></param>
        /// <returns></returns>
        public static string[] SplitSearchText(string SearchText, params string[] splitChar)
        {
            if (string.IsNullOrEmpty(SearchText))
                return new string[] { };

            splitChar = splitChar ?? new string[] { " ", "　" };

            string[] splited = SearchText.Split(splitChar, StringSplitOptions.None);
            return splited;
        }


        public static string SubString(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            if (length > text.Length)
                length = text.Length;

            return text.Substring(0, length);
        }

        /// <summary>
        /// ランダムなカラーコードを作成する。#FFFFFF形式。
        /// </summary>
        /// <returns></returns>
        public static string CreateRandomColorHex(int? seed = null)
        {
            Random random = null;
            if (seed == null)
                random = new Random();
            else
                random = new Random(seed.Value);

            var color = String.Format("#{0:X6}", random.Next(0x1000000));
            return color;
        }

        /// <summary>
        /// SHA256形式のハッシュバイトを取得する。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] CreateSHA256Hash(string input)
        {
            using(SHA256 hasher = SHA256.Create())
            {
                byte[] data = hasher.ComputeHash(Encoding.Unicode.GetBytes(input));
                return data;
            }
        }

        /// <summary>
        /// SHA256形式のハッシュバイトを取得する。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateSHA256HashStr(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// リトライ処理を行う。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="retryCount"></param>
        /// <param name="deleyMilli"></param>
        /// <param name="needRetry"></param>
        /// <returns></returns>
        public static async Task<T> Retry<T>(Func<Task<T>> action, int retryCount, Func<T, bool> needRetry, int deleyMilli = 0)
        {
            T result = default(T);
            for(int i = 0; i < retryCount; i++)
            {
                result = await action();

                if (needRetry(result))
                {
                    if(deleyMilli > 0)
                        await Task.Delay(deleyMilli);

                    continue;
                }
                break;
            }
            return result;
        }


        /// <summary>
        /// 半角数字を全角数字に変換する。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <returns>変換後の文字列</returns>
        public static string HanToZenNum(string s)
        {
            return Regex.Replace(s, "[0-9]", p => ((char)(p.Value[0] - '0' + '０')).ToString());
        }


        /// <summary>
        /// 文字列を指定した文字数で区切ったリストを取得する。
        /// </summary>
        /// <param name="s"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        public static List<string> SplitTextByCount(string s, int takeCount)
        {
            if (string.IsNullOrEmpty(s))
                return new List<string>();

            if (takeCount == 0)
            {
                return new List<string>() { s };
            }

            int currentPosition = 0;
            List<string> result = new List<string>();

            while(s.Length > currentPosition)
            {
                int remainCount = s.Length - currentPosition;
                int count = remainCount < takeCount ? remainCount : takeCount;

                result.Add(s.Substring(currentPosition, count));
                currentPosition += takeCount;
            }

            return result;
        }

    }
}
