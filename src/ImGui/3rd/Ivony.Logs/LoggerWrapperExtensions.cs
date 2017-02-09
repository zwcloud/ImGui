using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义 Logger 的各种包装辅助方法
  /// </summary>
  public static class LoggerWrapperExtensions
  {

    private class LoggerWithFilter : Logger
    {


      public LoggerWithFilter( Logger logger, LogFilter filter )
      {
        Contract.Assert( logger != null );
        Contract.Assert( filter != null );

        Filter = filter;

        InnerLogger = logger;
      }


      public Logger InnerLogger { get; private set; }

      public LogFilter Filter { get; private set; }



      public override void LogEntry( LogEntry entry )
      {
        if ( Filter.Writable( entry ) )
          InnerLogger.LogEntry( entry );
      }

    }


    /// <summary>
    /// 创建一个新的日志记录器，只记录符合条件的日志
    /// </summary>
    /// <param name="logger">内部日志记录器，符合条件的日志将使用这个日志记录器记录</param>
    /// <param name="filter">日志记录条件</param>
    /// <returns>只记录符合条件的日志的日志记录器</returns>
    public static Logger WithFilter( this Logger logger, LogFilter filter )
    {
      if ( logger == null )
        return null;

      if ( filter == null )
        throw new ArgumentNullException( "filter" );


      var loggerWithFilter = logger as LoggerWithFilter;
      if ( loggerWithFilter != null )
        return new LoggerWithFilter( loggerWithFilter.InnerLogger, loggerWithFilter.Filter & filter );

      else
        return new LoggerWithFilter( logger, filter );
    }






    private class LoggerWithSource : Logger
    {


      public LoggerWithSource( Logger logger, LogSource source )
      {
        Contract.Assert( logger != null );
        Contract.Assert( source != null );
        Source = source;
      }


      public LogSource Source { get; private set; }

      public Logger InnerLogger { get; private set; }




      public override void LogEntry( LogEntry entry )
      {
        entry = entry.SetMetaData( Source );
        InnerLogger.LogEntry( entry );
      }
    }


    /// <summary>
    /// 创建一个日志记录器，对于所有的日志添加日志源信息
    /// </summary>
    /// <param name="logger">内部日志记录器，将使用这个日志记录器进行实际的日志记录</param>
    /// <param name="source">要添加的日志源信息</param>
    /// <returns>会自动添加日志源信息的日志记录器</returns>
    public static Logger WithSource( this Logger logger, LogSource source )
    {
      if ( logger == null )
        return null;

      if ( source == null )
        throw new ArgumentNullException( "source" );

      return new LoggerWithSource( logger, source );

    }


    private sealed class IgnoreErrorLogger : Logger
    {

      private Logger _logger;

      public IgnoreErrorLogger( Logger logger )
      {
        _logger = logger;
      }

      public override void LogEntry( LogEntry entry )
      {
        try
        {
          _logger.LogEntry( entry );
        }
        catch { }
      }
    }


    /// <summary>
    /// 创建一个新的日志记录器，忽略记录日志中出现的任何异常
    /// </summary>
    /// <param name="logger">用于记录异常的日志记录器</param>
    /// <returns></returns>
    public static Logger IgnoreError( this Logger logger )
    {
      if ( logger == null )
        throw new ArgumentNullException( "logger" );

      if ( logger is IgnoreErrorLogger )
        return logger;

      return new IgnoreErrorLogger( logger );
    }


  }
}
