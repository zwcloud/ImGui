using System;
using System.IO;

namespace Ivony.Logs
{


  /// <summary>
  /// 文本日志记录器基类，将日志信息以文本形式记录的所有记录器的基类
  /// </summary>
  public abstract class TextLogger : Logger
  {


    /// <summary>
    /// 创建 TextLogger 对象
    /// </summary>
    /// <param name="filter">日志筛选器</param>
    /// <param name="timezone">时区信息（默认为UTC-0）</param>
    protected TextLogger( LogFilter filter = null, TimeZoneInfo timezone = null )
    {
      _timezone = timezone ?? TimeZoneInfo.Utc;
    }




    /// <summary>
    /// 写入一条日志信息
    /// </summary>
    /// <param name="entry"></param>
    public override void LogEntry( LogEntry entry )
    {
      Write( entry, GetPadding( entry ), entry.Message );
    }



    /// <summary>
    /// 释放文本编写器
    /// </summary>
    /// <param name="writer"></param>
    protected virtual void ReleaseWriter( TextWriter writer )
    {
      writer.Flush();
    }




    /// <summary>
    /// 派生类实现此方法写入日志
    /// </summary>
    /// <param name="entry">要写入的日志条目</param>
    /// <param name="contents">要写入的日志内容</param>
    protected abstract void WriteLogMessage( LogEntry entry, string[] contents );






    /// <summary>
    /// 使用指定的前缀写入多行日志
    /// </summary>
    /// <param name="entry">要写入的日志条目</param>
    /// <param name="padding">填充字符串，将会添加在每一行日志的前面</param>
    /// <param name="message">要写入的日志消息</param>
    protected virtual void Write( LogEntry entry, string padding, string message )
    {


      var messageLines = SplitMultiLine( message );



      if ( messageLines.Length == 1 )
      {
        WriteLogMessage( entry, new[] { padding + " " + messageLines[0] } );
        return;
      }

      for ( int i = 0; i < messageLines.Length; i++ )
      {
        if ( i == 0 )
          messageLines[i] = padding + "/" + messageLines[i];
        else if ( i == messageLines.Length - 1 )
          messageLines[i] = padding + "\\" + messageLines[i];
        else
          messageLines[i] = padding + "|" + messageLines[i];
      }

      WriteLogMessage( entry, messageLines );
    }


    /// <summary>
    /// 将多行消息按照换行符拆分成多个字符串
    /// </summary>
    /// <param name="message">多行消息</param>
    /// <returns>拆分后的结果</returns>
    protected virtual string[] SplitMultiLine( string message )
    {
      if ( message == null )
        return null;

      return message.Split( new[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.None );
    }


    /// <summary>
    /// 获取日志消息的填充，将会自动添加在日志消息的每一行的开头
    /// </summary>
    /// <param name="entry">当前要记录的日志</param>
    /// <returns>当前日志消息的填充</returns>
    protected virtual string GetPadding( LogEntry entry )
    {
      return GetTypePrefix( entry.LogType() ) + " " + GetDateTime( entry ).ToString( DateTimeFormatString ) + " ";
    }

    /// <summary>
    /// 获取转换时区后的日志记录时间
    /// </summary>
    /// <param name="entry">日志条目</param>
    /// <returns>转换后的时间</returns>
    protected DateTime GetDateTime( LogEntry entry )
    {
            return TimeZoneInfo.ConvertTimeFromUtc( entry.LogDate, TimeZone );
    }



        /// <summary>
        /// 获取日志消息的类型前缀，将会自动添加在每一行消息以标识这个消息的类型
        /// </summary>
        /// <param name="type">当前要记录的日志</param>
        /// <returns>当前日志消息的前缀</returns>
        protected virtual string GetTypePrefix( LogType type )
    {

      if ( type == null || type.Serverity == 0 )
        return "??";

      else if ( type.Serverity <= 500 )
        return " #";

      else if ( type.Serverity <= LogType.Info.Serverity )
        return "##";

      else if ( type.Serverity <= LogType.ImportantInfo.Serverity )
        return "#@";

      else if ( type.Serverity <= LogType.Warning.Serverity )
        return "#!";

      else if ( type.Serverity <= LogType.Error.Serverity )
        return "@!";

      else if ( type.Serverity <= LogType.Exception.Serverity )
        return "E!";

      else if ( type.Serverity <= LogType.FatalError.Serverity )
        return "F!";

      else if ( type.Serverity <= LogType.CrashError.Serverity )
        return "!!";

      else
        return "?!";
    }


    /// <summary>
    /// 获取日期字符串格式
    /// </summary>
    protected virtual string DateTimeFormatString
    {
      get { return "yyyy-MM-dd HH:mm:ss"; }
    }


    private TimeZoneInfo _timezone;

    /// <summary>
    /// 获取应使用的时区信息（默认为UTC-0）
    /// </summary>
    public virtual TimeZoneInfo TimeZone
    {
      get { return _timezone; }
    }




    /// <summary>
    /// 格式化提供程序
    /// </summary>
    protected IFormatProvider FormatProvider
    {
      get;
      private set;
    }

  }
}
