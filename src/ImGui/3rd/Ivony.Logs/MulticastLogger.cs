using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 多播日志记录器
  /// </summary>
  public class MulticastLogger : Logger
  {


    /// <summary>
    /// 创建一个多播日志记录器
    /// </summary>
    /// <param name="loggers">需要记录日志的日志记录器</param>
    public MulticastLogger( params Logger[] loggers ) : this( true, loggers ) { }

    /// <summary>
    /// 创建一个多播日志记录器
    /// </summary>
    /// <param name="throwExceptions">当任何一个日志记录器抛出异常时，是否应当中断日志记录并抛出异常</param>
    /// <param name="loggers">需要记录日志的日志记录器</param>
    public MulticastLogger( bool throwExceptions, params Logger[] loggers )
    {
      ThrowExceptions = throwExceptions;
      Loggers = new ReadOnlyCollection<Logger>( loggers.SelectMany( ExpandMulticast ).ToArray() );
    }

    private static IEnumerable<Logger> ExpandMulticast( Logger logger )
    {
      var multi = logger as MulticastLogger;
      if ( multi != null )
        return multi.Loggers;

      else if ( logger == null )//如果有空的日志记录器混进来则忽略之。
        return new Logger[0];

      else
        return new[] { logger };
    }


    /// <summary>
    /// 需要被转发的日志记录器
    /// </summary>
    public ICollection<Logger> Loggers { get; private set; }

    /// <summary>
    /// 当记录日志出现异常时是否中断日志记录并抛出异常
    /// </summary>
    public bool ThrowExceptions { get; private set; }




    /// <summary>
    /// 对所有的日志记录器同时写入日志条目
    /// </summary>
    /// <param name="entry">要记录的日志条目</param>
    public override void LogEntry( LogEntry entry )
    {

      List<Exception> exceptions = new List<Exception>();

      foreach ( var logger in Loggers )
      {
        try
        {
          logger.LogEntry( entry );
        }
        catch ( Exception e )
        {

          if ( ThrowExceptions )
            exceptions.Add( e );
        }
      }

      if ( exceptions.Any() )
        throw new AggregateException( exceptions.ToArray() );
    }


    /// <summary>
    /// 重写 Dispose 方法，释放所有日志记录器的资源
    /// </summary>
    public override void Dispose()
    {
      foreach ( var logger in Loggers )
      {
        logger.Dispose();
      }
    }
  }
}
