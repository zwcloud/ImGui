using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 文件日志记录器
  /// </summary>
  public class TextFileLogger : TextFileLoggerBase
  {

    private LogFilenameStrategy _filenameProvider;


    private string basePath;


    private TextFileLogger( LogFilter filter, Encoding encoding )
      : base( filter, encoding )
    {
      basePath = System.IO.Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// 创建文本文件日志记录器
    /// </summary>
    /// <param name="filenameProvider">文件名提供程序</param>
    /// <param name="filter">日志筛选器</param>
    /// <param name="encoding">文件编码</param>
    public TextFileLogger( LogFilenameStrategy filenameProvider, LogFilter filter = null, Encoding encoding = null )
      : this( filter, encoding )
    {

      if ( filenameProvider == null )
        throw new ArgumentNullException( "filenameProvider" );

      _filenameProvider = filenameProvider;

    }

    /// <summary>
    /// 创建文本文件日志记录器
    /// </summary>
    /// <param name="logDirectory">存放日志文件的目录</param>
    /// <param name="filter">日志筛选器</param>
    /// <param name="cycle">日志文件记录周期</param>
    /// <param name="prefix">文件名前缀</param>
    /// <param name="postfix">文件名后缀</param>
    /// <param name="extension">文件扩展名</param>
    /// <param name="encoding">文件编码</param>
    public TextFileLogger( DirectoryInfo logDirectory, LogFilter filter = null, LogFilenameStrategy cycle = null, string prefix = "", string postfix = "", string extension = ".log", Encoding encoding = null )
      : this( filter, encoding )
    {

      if ( logDirectory == null )
        throw new ArgumentNullException( "logDirectory" );

      cycle = cycle ?? LogFileCycles.Daily;

      _filenameProvider = logDirectory.FullName + Path.DirectorySeparatorChar + prefix + cycle + postfix + extension;

    }




    /// <summary>
    /// 获取日志文件路径
    /// </summary>
    /// <param name="entry">要记录的日志条目</param>
    /// <returns>日志文件路径</returns>
    protected override string GetFilepath( LogEntry entry )
    {
      var path = _filenameProvider.GetName( entry );

      return Path.Combine( basePath, path );
    }

  }
}
