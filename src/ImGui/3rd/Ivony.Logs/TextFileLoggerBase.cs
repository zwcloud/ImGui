using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 文本文件日志记录器基类
  /// </summary>
  public abstract class TextFileLoggerBase : TextLogger
  {


    /// <summary>
    /// 初始化 TextFileLoggerBase 对象
    /// </summary>
    /// <param name="filter">可选的日志筛选器</param>
    /// <param name="encoding">写入文件所用的编码</param>
    protected TextFileLoggerBase( LogFilter filter = null, Encoding encoding = null )
      : base( filter )
    {
      _encoding = encoding ?? TextLogFileManager.DefaultEncoding;
    }


    /// <summary>
    /// 派生类重写此方法获取要写入的日志文件路径
    /// </summary>
    /// <param name="entry">要写入的日志条目</param>
    /// <returns>要将该日志文件写入的路径</returns>
    protected abstract string GetFilepath( LogEntry entry );


    private Encoding _encoding;

    /// <summary>
    /// 获取写入文件所用的编码
    /// </summary>
    protected virtual Encoding Encoding
    {
      get { return _encoding; }
    }




    /// <summary>
    /// 重写 WriteLogMessage 方法将日志写入文本文件
    /// </summary>
    /// <param name="entry">日志条目对象</param>
    /// <param name="lines">要写入的文本行</param>
    protected override void WriteLogMessage( LogEntry entry, string[] lines )
    {
      var path = GetFilepath( entry );

      var build = new StringBuilder();
      foreach ( var l in lines )
      {
        build.Append( l );
        build.Append( Environment.NewLine );
      }

      TextLogFileManager.WriteText( path, build.ToString(), Encoding, entry.LogType().Serverity >= LogType.Error.Serverity );
    }


  }
}
