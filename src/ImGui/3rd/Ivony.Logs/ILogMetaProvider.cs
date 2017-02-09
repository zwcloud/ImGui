using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义 LogMeta 数据提供程序
  /// </summary>
  public interface ILogMetaProvider
  {


    /// <summary>
    /// 获取 LogMeta 数据
    /// </summary>
    /// <param name="metaData">参考的日志元数据</param>
    /// <returns>LogMeta 数据</returns>
    LogMeta GetLogMeta( LogMeta metaData );
  }
}
