
using System;
using System.Linq;
using Domain.Util;

public static class DateTimeExtensions
{
    /// <summary>
    /// 年月日の時分秒を秒で返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int ToSeconds(this DateTime? dt)
    {
        if (dt == null)
            return 0;

        return ToSeconds(dt.Value);
    }

    /// <summary>
    /// 年月日の時分秒を秒で返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int ToSeconds(this DateTime dt)
    {
        var span = dt - new DateTime(dt.Year, dt.Month, dt.Day);
        return (int)span.TotalSeconds;
    }

    /// <summary>
    /// デフォルトの月日年の表示文字列を返す。(m/d/yyyy)
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultMDYYYY(this DateTime? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultMDYYYY(dt.Value);
    }


    /// <summary>
    /// デフォルトの月日年の表示文字列を返す。(m/d/yyyy)
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultMDYYYY(this DateTime dt)
    {
        return dt.ToString("M/d/yyyy");
    }

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultYYYYMMDD(this DateTime? dt, string format = "yyyy/MM/dd")
    {
        if (dt == null)
            return "";

        return ToDefaultYYYYMMDD(dt.Value, format);
    }


    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultYYYYMMDD(this DateTime dt, string format = "yyyy/MM/dd")
    {
        return dt.ToString(format);
    }

    /// <summary>
    /// デフォルトの年月日時間の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultYYYYMMDDHHMMSS(this DateTime? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultYYYYMMDDHHMMSS(dt.Value);
    }

    /// <summary>
    /// デフォルトの年月日時間の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultYYYYMMDDHHMMSS(this DateTime dt)
    {
        return dt.ToString("yyyy/MM/dd HH:mm:ss");
    }

    /// <summary>
    /// デフォルトの月日年の表示文字列を返す。(m/d/yyyy HH:mm:ss)
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultMDYYYYHHMMSS(this DateTime? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultMDYYYYHHMMSS(dt.Value);
    }


    /// <summary>
    /// デフォルトの月日年の表示文字列を返す。(m/d/yyyy HH:mm:ss)
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultMDYYYYHHMMSS(this DateTime dt)
    {
        return dt.ToString("M/d/yyyy HH:mm:ss");
    }


    /// <summary>
    /// デフォルトの年月日時間ミリ秒の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultYYYYMMDDHHMMSSFFF(this DateTime? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultYYYYMMDDHHMMSSFFF(dt.Value);
    }


    /// <summary>
    /// デフォルトの年月日時間の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultYYYYMMDDHHMMSSFFF(this DateTime dt)
    {
        return dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
    }

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSS(this DateTime? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultHHMMSS(dt.Value);
    }


    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSS(this DateTime dt)
    {
        return dt.ToString("HH:mm:ss");
    }

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSS(this TimeSpan? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultHHMMSS(dt.Value);
    }


    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSS(this TimeSpan dt)
    {
        //return dt.ToString("HH:mm:ss");
        const string format = "{0:00}:{1:00}:{2:00}";
        string result = string.Format(format, ((dt.Days * 24) + dt.Hours), dt.Minutes, dt.Seconds);
        return result;
    }

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSSorMMSS(this TimeSpan? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultHHMMSSorMMSS(dt.Value);
    }


    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSSorMMSS(this TimeSpan dt)
    {
        //1時間越えている場合だけ時間部分を表示。
        if (dt.Hours > 0)
            return dt.ToString(@"hh\:mm\:ss");
        else
            return dt.ToString(@"mm\:ss");
    }

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSSFForMMSSFF(this TimeSpan? dt)
    {
        if (dt == null)
            return "";

        return ToDefaultHHMMSSFForMMSSFF(dt.Value);
    }


    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string ToDefaultHHMMSSFForMMSSFF(this TimeSpan dt)
    {
        //1時間越えている場合だけ時間部分を表示。
        if(dt.Hours > 0)
            return dt.ToString(@"hh\:mm\:ss\.ff");
        else
            return dt.ToString(@"mm\:ss\.ff");
    }


    /// <summary>
    /// Timespanからmm:ss形式の文字列を作成する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToTimemmssString(this TimeSpan? value)
    {
        if (value == null)
            return "";

        return value.Value.ToString("mm\\:ss");
    }


    /// <summary>
    /// Timespanからmm:ss形式の文字列を作成する。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToTimemmssString(this TimeSpan value)
    {
        //int num = (int)Math.Truncate(value.Value);

        string result = value.Minutes + ":" + value.Seconds;
        return result;
    }


    /// <summary>
    /// 該当年月の日数を返す
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int DaysInMonth(this DateTime dt)
    {
        return DateTime.DaysInMonth(dt.Year, dt.Month);
    }

    // <summary>
    /// 該当年月の日数を返す
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int DaysInMonth(this DateTime? dt)
    {
        if (dt == null)
            return 0;

        return DaysInMonth(dt.Value);
    }


    /// <summary>
    /// 月初日を返す
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime BeginOfMonth(this DateTime dt)
    {
        //return dt.AddDays((dt.Day - 1) * -1);
        return new DateTime(dt.Year, dt.Month, 1);
    }

    /// <summary>
    /// 月初日を返す
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime? BeginOfMonth(this DateTime? dt)
    {
        if (dt == null)
            return null;

        //return dt.Value.AddDays((dt.Value.Day - 1) * -1);
        return BeginOfMonth(dt.Value);
    }


    /// <summary>
    /// 月末日を返す
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static DateTime EndOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, DaysInMonth(dt));
    }

    /// <summary>
    /// 月末日を返す
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static DateTime? EndOfMonth(this DateTime? dt)
    {
        if (dt == null)
            return null;

        return EndOfMonth(dt.Value);
    }


    /// <summary>
    /// 週の初日を返す（日曜開始 土曜終了）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime BeginOfWeek(this DateTime dt)
    {
        DateTime sunday = dt.AddDays(0 - (int)dt.DayOfWeek);
        return sunday;
    }

    /// <summary>
    /// 週の初日を返す(日曜開始 土曜終了）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime? BeginOfWeek(this DateTime? dt)
    {
        if (dt == null)
            return null;

        return BeginOfWeek(dt.Value);
    }

    /// <summary>
    /// 週の最終日を返す（日曜開始 土曜終了）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime EndOfWeek(this DateTime dt)
    {
        DateTime saturday = dt.AddDays(6 - (int)dt.DayOfWeek);
        return saturday;
    }

    /// <summary>
    /// 週の最終日を返す(日曜開始 土曜終了）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime? EndOfWeek(this DateTime? dt)
    {
        if (dt == null)
            return null;

        return EndOfWeek(dt.Value);
    }

    /// <summary>
    /// 1日の開始時間を返す（00:00:00）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime BeginOfDay(this DateTime dt)
    {
        DateTime begin = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        return begin;
    }

    /// <summary>
    /// 1日の開始時間を返す（00:00:00）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime? BeginOfDay(this DateTime? dt)
    {
        if (dt == null)
            return null;

        return BeginOfDay(dt.Value);
    }


    /// <summary>
    /// 1日の終了時間を返す（23:59:59）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime EndOfDay(this DateTime dt)
    {
        DateTime begin = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        return begin;
    }

    /// <summary>
    /// 1日の終了時間を返す（23:59:59）
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime? EndOfDay(this DateTime? dt)
    {
        if (dt == null)
            return null;

        return EndOfDay(dt.Value);
    }

    /// <summary>
    /// このインスタンスの値に、指定された日数を加算した新しい System.DateTime を返します。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Datetime</returns>
    public static DateTime? AddDays(this DateTime? dt, int dayCount)
    {
        if (dt == null)
            return null;

        return dt.Value.AddDays(dayCount);
    }


    /// <summary>
    /// 渡されたDateTimeの日にちから、その月の月末日までの日数を取得。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int RestDayCount(this DateTime? dt)
    {
        if (dt == null)
            return 0;

        return RestDayCount(dt.Value);
    }

    /// <summary>
    /// 渡されたDateTimeの日にちから、その月の月末日までの日数を取得。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int RestDayCount(this DateTime dt)
    {
        int startDay = dt.Day;
        int endDay = EndOfMonth(dt).Day;

        int result = (endDay + 1) - startDay;

        return result;
    }


    /// <summary>
    /// 渡されたDateTimeの月の初日から、渡されたDateTimeの日にちまでの日数を取得する。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int ProgressDayCount(this DateTime? dt)
    {
        if (dt == null)
            return 0;

        return ProgressDayCount(dt.Value);
    }

    /// <summary>
    /// 渡されたDateTimeの月の初日から、渡されたDateTimeの日にちまでの日数を取得する。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static int ProgressDayCount(this DateTime dt)
    {
        int startDay = 1;
        int endDay = dt.Day;

        int result = (endDay + 1) - startDay;

        return result;
    }

    public static string GetWeekNameJpn(this DateTime datetime)
    {
        return (WEEK_NAME_LIST).Substring(int.Parse(datetime.DayOfWeek.ToString("d")), 1);
    }

    public static string GetWeekNameJpn(this DateTime? datetime)
    {
        if (datetime == null)
            return "";

        return GetWeekNameJpn(datetime.Value);
    }

    /// <summary>曜日名一覧 </summary>
    public const string WEEK_NAME_LIST = "日月火水木金土";


    public static string GetJapaneseDateString(this DateTime datetime, string format = "ggyy年度")
    {
        return CommonUtil.GetJapaneseDateString(datetime, format);
    }

    public static string GetJapaneseDateString(this DateTime? datetime, string format = "ggyy年度")
    {
        return CommonUtil.GetJapaneseDateString(datetime, format);
    }

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>DateTime</returns>
    public static string TimeToString(this TimeSpan? dt)
    {
        if (dt == null)
            return "";

        return TimeToString(dt.Value);
    }

    public const string TimeToSpanHHmmddFormat = "{0:D2}:{1:D2}:{2:D2}";
    public const string TimeToSpanHHmmFormat = "{0:D2}:{1:D2}";

    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="format">{0:Hours}:{1:Minite}:{2:Second}</param>
    /// <returns></returns>
    public static string TimeToString(this TimeSpan dt, string format = TimeToSpanHHmmddFormat)
    {
        string result = string.Format(format, ((dt.Days * 24) + dt.Hours), dt.Minutes, dt.Seconds);
        return result;
    }


    /// <summary>
    /// デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="format">{0:Hours}:{1:Minite}:{2:Second}</param>
    /// <returns></returns>
    public static string TimeToString(this TimeSpan? dt, string format = TimeToSpanHHmmddFormat)
    {
        if (dt == null)
            return "";

        return TimeToString(dt.Value);
    }


    /// <summary>
    ///  デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="format">{0:Hours}:{1:Minite}:{2:Second}</param>
    /// <returns></returns>
    public static string TimeToString(this DateTime? dt, string format = TimeToSpanHHmmddFormat)
    {
        if (dt == null)
            return "";

        return TimeToString(dt.Value, format);
    }

    /// <summary>
    ///  デフォルトの年月日の表示文字列を返す。
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="format">{0:Hours}:{1:Minite}:{2:Second}</param>
    /// <returns></returns>
    public static string TimeToString(this DateTime dt, string format = TimeToSpanHHmmddFormat)
    {
        var time = dt - DateTime.MinValue;
        return TimeToString(time, format);
    }

}
