using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{


  /// <summary>
  /// 定义按照日志记录源名称进行限制的日志筛选器
  /// </summary>
  [EditorBrowsable( EditorBrowsableState.Advanced )]
  public sealed class LogSourceNameRestrictFilter : LogFilter
  {


    /// <summary>
    /// 创建 LogSourceNameRestrictFilter 对象
    /// </summary>
    /// <param name="sourceName">日志源名称</param>
    public LogSourceNameRestrictFilter( string sourceName )
    {

      if ( sourceName == null )
        throw new ArgumentNullException( "logSource" );

      SourceName = sourceName;
    }


    /// <summary>
    /// 日志源名称
    /// </summary>
    public string SourceName
    {
      get;
      private set;
    }


    /// <summary>
    /// 重写 Writable 方法，确认日志条目是否由指定的日志源产生
    /// </summary>
    /// <param name="entry">要检查的日志条目</param>
    /// <returns>日志条目是否由指定的日志源产生</returns>
    public override bool Writable( LogEntry entry )
    {
      var source = entry.MetaData.LogSource();

      if ( source == null )
        return false;

      return string.Equals( SourceName, source.Name, StringComparison.OrdinalIgnoreCase );
    }
  }
}
