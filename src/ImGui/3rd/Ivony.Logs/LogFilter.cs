using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义日志筛选器的抽象基类
  /// </summary>
  public abstract class LogFilter
  {


    /// <summary>
    /// 确定指定的日志条目是否需要记录
    /// </summary>
    /// <param name="entry">日志条目</param>
    /// <returns>是否需要记录</returns>
    public abstract bool Writable( LogEntry entry );




    /// <summary>
    /// 从两个日志筛选器中创建一个新的日志筛选器，只要满足其中任何一个日志筛选器的条件即记录日志
    /// </summary>
    /// <param name="filter1">第一个日志筛选器</param>
    /// <param name="filter2">第二个日志筛选器</param>
    /// <returns>创建的新的日志筛选器</returns>
    public static LogFilter operator |( LogFilter filter1, LogFilter filter2 )
    {
      var anyFilter1 = filter1 as AnyFilter;
      var anyFilter2 = filter2 as AnyFilter;

      var filters = new List<LogFilter>();

      if ( anyFilter1 != null )
        filters.AddRange( anyFilter1.Filters );
      else
        filters.Add( filter1 );

      if ( anyFilter2 != null )
        filters.AddRange( anyFilter2.Filters );
      else
        filters.Add( filter2 );

      return new AnyFilter( filters );
    }


    /// <summary>
    /// 从两个日志筛选器中创建一个新的日志筛选器，需要满足所有日志筛选器的条件才记录日志
    /// </summary>
    /// <param name="filter1">第一个日志筛选器</param>
    /// <param name="filter2">第二个日志筛选器</param>
    /// <returns>创建的新的日志筛选器</returns>
    public static LogFilter operator &( LogFilter filter1, LogFilter filter2 )
    {
      var allFilter1 = filter1 as AllFilter;
      var allFilter2 = filter2 as AllFilter;

      var filters = new List<LogFilter>();

      if ( allFilter1 != null )
        filters.AddRange( allFilter1.Filters );
      else
        filters.Add( filter1 );

      if ( allFilter2 != null )
        filters.AddRange( allFilter2.Filters );
      else
        filters.Add( filter2 );

      return new AllFilter( filters );
    }


    private class AnyFilter : LogFilter
    {


      public AnyFilter( IEnumerable<LogFilter> filters )
      {
        Filters = filters.ToArray();
      }


      public LogFilter[] Filters
      {
        get;
        private set;
      }


      public override bool Writable( LogEntry entry )
      {
        return Filters.Any( filter => filter.Writable( entry ) );
      }
    }




    private class AllFilter : LogFilter
    {


      public AllFilter( IEnumerable<LogFilter> filters )
      {
        Filters = filters.ToArray();
      }


      public LogFilter[] Filters
      {
        get;
        private set;
      }



      public override bool Writable( LogEntry entry )
      {
        return Filters.All( filter => filter.Writable( entry ) );
      }
    }






    static LogFilter()
    {
      Info = new ServerityBasedFilter( LogType.Info.Serverity, int.MaxValue );
      ImportantInfo = new ServerityBasedFilter( LogType.ImportantInfo.Serverity, int.MaxValue );
      Warning = new ServerityBasedFilter( LogType.Warning.Serverity, int.MaxValue );
      Error = new ServerityBasedFilter( LogType.Error.Serverity, int.MaxValue );
      Exception = new ServerityBasedFilter( LogType.Exception.Serverity, int.MaxValue );
      FatalError = new ServerityBasedFilter( LogType.FatalError.Serverity, int.MaxValue );

      InfoOnly = new LogTypeBasedFilter( LogType.Info ) | new LogTypeBasedFilter( LogType.ImportantInfo );
      WarningOnly = new LogTypeBasedFilter( LogType.Warning );
      ErrorOnly = new LogTypeBasedFilter( LogType.Error );
      ExceptionOnly = new LogTypeBasedFilter( LogType.Exception );
    }


    /// <summary>
    /// 指定记录一般性消息以及更严重的消息的日志筛选器
    /// </summary>
    public static LogFilter Info
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定只记录一般性消息的日志筛选器
    /// </summary>
    public static LogFilter InfoOnly
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定记录一般性消息以及更严重的消息的日志筛选器
    /// </summary>
    public static LogFilter ImportantInfo
    {
      get;
      private set;
    }



    /// <summary>
    /// 指定记录警告消息以及更严重的消息的日志筛选器
    /// </summary>
    public static LogFilter Warning
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定只记录警告消息的日志筛选器
    /// </summary>
    public static LogFilter WarningOnly
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定记录错误消息以及更严重的消息的日志筛选器
    /// </summary>
    public static LogFilter Error
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定只记录错误消息的日志筛选器
    /// </summary>
    public static LogFilter ErrorOnly
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定记录异常消息以及更严重的消息的日志筛选器
    /// </summary>
    public static LogFilter Exception
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定只记录异常消息的日志筛选器
    /// </summary>
    public static LogFilter ExceptionOnly
    {
      get;
      private set;
    }

    /// <summary>
    /// 指定记录致命错误消息以及更严重的消息的日志筛选器
    /// </summary>
    public static LogFilter FatalError
    {
      get;
      private set;
    }


    private class ServerityBasedFilter : LogFilter
    {

      public ServerityBasedFilter( int minServerity, int maxServerity )
      {
        MinServerity = minServerity;
        MaxServerity = maxServerity;
      }

      public int MinServerity
      {
        get;
        private set;
      }

      public int MaxServerity
      {
        get;
        private set;
      }


      public override bool Writable( LogEntry entry )
      {
        var serverity = entry.LogType().Serverity;

        return serverity >= MinServerity && serverity < MaxServerity;

      }
    }


    private class LogTypeBasedFilter : LogFilter
    {
      public LogTypeBasedFilter( LogType type )
      {
        LogType = type;
      }


      public LogType LogType
      {
        get;
        private set;
      }


      public override bool Writable( LogEntry entry )
      {
        return LogType.Equals( entry.LogType() );
      }
    }




    /// <summary>
    /// 获取按照日志记录源的名称筛选日志
    /// </summary>
    /// <param name="logSource">日志记录源名称</param>
    /// <returns>筛选只指定日志记录源的日志筛选器</returns>
    public static LogFilter BySource( string logSource )
    {
      return new LogSourceNameRestrictFilter( logSource );
    }
  }
}
