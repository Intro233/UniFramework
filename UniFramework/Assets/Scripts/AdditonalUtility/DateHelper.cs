using System;
using UnityEngine;

public class DateHelper 
{
    
    #region 获取

    /// <summary>
    /// 获取系统当前时间戳/毫秒
    /// </summary>
    /// <returns></returns>
    public static long GetTimestamp()
    {
        return Date2TimeStamp(DateTime.UtcNow);
    }

    /// <summary>
    /// 获取系统当前时间戳/秒
    /// </summary>
    /// <returns></returns>
    public static long GetTimestampSeconds()
    {
        return Date2TimeStampSeconds(DateTime.UtcNow);
    }

    #endregion


    #region 转换

    /// <summary>
    /// 将分钟换算为D:H:M
    /// </summary>
    /// <param name="minutes">要转换的分钟数</param>
    /// <returns></returns>
    public static string Minutes2DHM(int minutes)
    {
        int days = minutes / (60 * 24);
        int hours = (minutes % (60 * 24)) / 60;
        int remainingMinutes = minutes % 60;

        return $"{days}天 {hours}小时 {remainingMinutes}分";
    }

    /// <summary>
    /// 将分钟换算为D:H:M
    /// </summary>
    /// <param name="minutes">要转换的分钟数</param>
    /// <returns></returns>
    public static string Minutes2DHM2(int minutes)
    {
        TimeSpan timeSpan = TimeSpan.FromMinutes(minutes);
        return $"{timeSpan.Days}天 {timeSpan.Hours}小时 {timeSpan.Minutes}分";
    }


    /// <summary>
    /// 日期转换为时间戳，毫秒级
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long Date2TimeStamp(DateTime dateTime)
    {
        TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0);
        return Convert.ToInt64(timeSpan.TotalMilliseconds);
    }

    /// <summary>
    /// 日期转换为时间戳，秒级
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long Date2TimeStampSeconds(DateTime dateTime)
    {
        TimeSpan timeSpan = dateTime - new DateTime(1970, 1, 1, 0, 0, 0);
        return Convert.ToInt64(timeSpan.TotalSeconds);
    }


    /// <summary>
    /// 时间戳转日期，单位毫秒
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime TimeStamp2Date(double timestamp)
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime date = startTime.AddMilliseconds(timestamp); //st为传入的时间戳
        return date;
    }

    /// <summary>
    /// 时间戳转日期 单位秒
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime TimeStampSeconds2Date(double timestamp)
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime date = startTime.AddSeconds(timestamp); //st为传入的时间戳
        return date;
    }


    /// <summary>
    /// 将秒数时间戳转换为多久之前。传入时间戳t(t= 当前时间戳() - 指定时间的时间戳 )
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public string GetTimeLongAgo(double t)
    {
        string str = "";
        double num;
        if (t < 60)
        {
            str = string.Format("{0}秒前", t);
        }
        else if (t >= 60 && t < 3600)
        {
            num = Math.Floor(t / 60);
            str = string.Format("{0}分钟前", num);
        }
        else if (t >= 3600 && t < 86400)
        {
            num = Math.Floor(t / 3600);
            str = string.Format("{0}小时前", num);
        }
        else if (t > 86400 && t < 2592000)
        {
            num = Math.Floor(t / 86400);
            str = string.Format("{0}天前", num);
        }
        else if (t > 2592000 && t < 31104000)
        {
            num = Math.Floor(t / 2592000);
            str = string.Format("{0}月前", num);
        }
        else
        {
            num = Math.Floor(t / 31104000);
            str = string.Format("{0}年前", num);
        }

        return str;
    }

    #endregion
}