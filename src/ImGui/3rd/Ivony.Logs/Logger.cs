using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{


  /// <summary>
  /// 定义所有日志记录器的基类型
  /// </summary>
  public abstract class Logger : IDisposable
  {


    /// <summary>
    /// 创建 Logger 对象
    /// </summary>
    protected Logger() { }




    /// <summary>
    /// 由派生类实现，编写一条日志
    /// </summary>
    /// <param name="entry">要编写的日志条目</param>
    public abstract void LogEntry( LogEntry entry );





    /// <summary>
    /// 重载 + 运算符，合并两个日志记录器，创建一个多播日志记录器
    /// </summary>
    /// <param name="logger1">要合并的第一个日志记录器</param>
    /// <param name="logger2">要合并的第二个日志记录器</param>
    /// <returns>合并后的多播日志记录器</returns>
    public static Logger operator +( Logger logger1, Logger logger2 )
    {

      if ( logger1 == null && logger2 == null )
        return null;

      if ( logger1 == null )
        return logger2;

      if ( logger2 == null )
        return logger1;


      return new MulticastLogger( logger1, logger2 );
    }



    private object _sync = new object();

    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get { return _sync; }
    }




    /// <summary>
    /// 派生类重写此方法以释放资源
    /// </summary>
    public virtual void Dispose()
    {
    }
  }
}
