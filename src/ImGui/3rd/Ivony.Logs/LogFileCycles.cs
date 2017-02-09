using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义日志文件周期性枚举
  /// </summary>
  public static class LogFileCycles
  {


    private class DailyCycle : LogFilenameStrategy
    {
      public override string GetName( LogEntry entry )
      {
        return entry.LogDate.ToString( "yyyyMMdd" );
      }
    }

    private class HourlyCycle : LogFilenameStrategy
    {
      public override string GetName( LogEntry entry )
      {
        return entry.LogDate.ToString( "yyyyMMdd-HH" );
      }
    }

    private class MinutelyCycle : LogFilenameStrategy
    {
      public override string GetName( LogEntry entry )
      {
        return entry.LogDate.ToString( "yyyyMMdd.HHmm" );
      }
    }

    private class MonthlyCycle : LogFilenameStrategy
    {
      public override string GetName( LogEntry entry )
      {
        return entry.LogDate.ToString( "yyyy-MM" );
      }
    }


    private static LogFilenameStrategy _daily = new DailyCycle();
    private static LogFilenameStrategy _hourly = new HourlyCycle();
    private static LogFilenameStrategy _minutely = new MinutelyCycle();
    private static LogFilenameStrategy _monthly = new MonthlyCycle();


    /// <summary>
    /// 获取每日产生一个新的日志文件的日志文件名策略
    /// </summary>
    public static LogFilenameStrategy Daily { get { return _daily; } }
    /// <summary>
    /// 获取每小时产生一个新的日志文件的日志文件名策略
    /// </summary>
    public static LogFilenameStrategy Hourly { get { return _hourly; } }
    /// <summary>
    /// 获取每分钟产生一个新的日志文件的日志文件名策略
    /// </summary>
    public static LogFilenameStrategy Minutely { get { return _minutely; } }
    /// <summary>
    /// 获取每月产生一个新的日志文件的日志文件名策略
    /// </summary>
    public static LogFilenameStrategy Monthly { get { return _monthly; } }


  }
}
