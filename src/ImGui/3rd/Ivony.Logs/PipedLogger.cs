using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{


  /// <summary>
  /// 定义一个管道日志记录器
  /// </summary>
  public abstract class PipedLogger : Logger
  {


    /// <summary>
    /// 内联日志记录器，所有通过此日志记录器的日志先交由内联记录器处理
    /// </summary>
    protected Logger InnerLogger { get; private set; }


    /// <summary>
    /// 异常日志记录器，若内联日志记录器出现异常，则交由此日志记录器处理
    /// </summary>
    protected Logger ExceptionLogger { get; private set; }



    /// <summary>
    /// 创建 PipedLogger 实例
    /// </summary>
    /// <param name="innerLogger">内联日志记录器</param>
    protected PipedLogger( Logger innerLogger ) : this( innerLogger, null ) { }

    /// <summary>
    /// 创建 PipedLogger 实例
    /// </summary>
    /// <param name="innerLogger">内联日志记录器</param>
    /// <param name="exceptionLogger">异常日志记录器</param>
    protected PipedLogger( Logger innerLogger, Logger exceptionLogger )
    {

      if ( innerLogger == null )
        throw new ArgumentNullException( "innerLogger" );

      InnerLogger = innerLogger;
      ExceptionLogger = exceptionLogger;
    }



    /// <summary>
    /// 重写 LogEntry 方法，交由内联日志记录器记录
    /// </summary>
    /// <param name="entry">要记录的日志条目</param>
    public override void LogEntry( LogEntry entry )
    {
      if ( InnerLogger == null )
        throw new InvalidOperationException();

      try
      {
        InnerLogger.LogEntry( entry );
      }
      catch ( Exception e )
      {
        try
        {
          if ( ExceptionLogger != null )
            ExceptionLogger.LogException( e );
        }
        catch { }

        throw;
      }
    }




    /// <summary>
    /// 重写 Dispose 方法，释放内联日志记录器的资源
    /// </summary>
    public override void Dispose()
    {
      InnerLogger.Dispose();
    }

  }
}
