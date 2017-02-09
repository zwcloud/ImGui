using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{


  /// <summary>
  /// 代表一个不将日志记录到任何地方的日志记录器
  /// </summary>
  public sealed class BlankLogger : Logger
  {

    /// <summary>
    /// 重写 LogEntry 方法，什么事都不干。
    /// </summary>
    /// <param name="entry">要记录的日志条目</param>
    public override void LogEntry( LogEntry entry )
    {
      return;
    }


    /// <summary>
    /// 获取一个 BlankLogger 的实例
    /// </summary>
    public static Logger Instance = new BlankLogger();
  }
}
