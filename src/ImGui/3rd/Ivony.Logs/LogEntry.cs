using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{


  /// <summary>
  /// 表示一条日志记录
  /// </summary>
  public class LogEntry
  {

    /// <summary>
    /// 创建一条日志记录
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="meta">日志元数据</param>
    /// <param name="raw">日志记录的原始对象</param>
    public LogEntry( string message, LogMeta meta = null, object raw = null ) : this( DateTime.UtcNow, message, meta ?? LogMeta.GetDefaultMeta(), raw ) { }


    private LogEntry( DateTime date, string message, LogMeta meta, object raw )
    {
      LogDate = date;
      Message = message;
      MetaData = meta;
      RawObject = raw;
    }



    /// <summary>
    /// 获取日志产生的时间，该时间以 UTC 时间表示
    /// </summary>
    public DateTime LogDate
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取日志元数据，其包含了日志来源、范畴等信息
    /// </summary>
    public LogMeta MetaData
    {
      get;
      private set;
    }

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message
    {
      get;
      private set;
    }

    /// <summary>
    /// 产生日志的原始对象
    /// </summary>
    public object RawObject
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建一个新的日志条目，使用新的日志元数据
    /// </summary>
    /// <param name="metaData">要设置的日志元数据</param>
    /// <returns></returns>
    public virtual LogEntry SetMetaData( params object[] metaData )
    {
      var meta = MetaData.SetMetaData( metaData );

      return new LogEntry( LogDate, Message, meta, RawObject );
    }
  }
}
