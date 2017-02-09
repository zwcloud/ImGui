using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Ivony.Logs
{

  /// <summary>
  /// 提供文本日志文件创建、编写等服务的类
  /// </summary>
  public static class TextLogFileManager
  {


    private static ConcurrentDictionary<string, TextFileWriter> _collection = new ConcurrentDictionary<string, TextFileWriter>( StringComparer.OrdinalIgnoreCase );


    private static bool _autoFlush = true;

    /// <summary>
    /// 获取或设置一个值，该值指示是否在每次写入文本日志后，将立即刷新日志流。
    /// </summary>
    public static bool AutoFlush
    {
      get { return _autoFlush; }
      set
      {
        _autoFlush = value;

        if ( value )
          Flush();
      }
    }



    /// <summary>
    /// 对指定的文本日志文件写入一段文本
    /// </summary>
    /// <param name="filepath">日志文件路径</param>
    /// <param name="content">文本内容</param>
    /// <param name="encoding">文本编码</param>
    /// <param name="flush">在写入后是否立即刷新日志流</param>
    public static void WriteText( string filepath, string content, Encoding encoding, bool flush = false )
    {
      encoding = encoding ?? DefaultEncoding;

      var writer = GetWriter( filepath, encoding );
      writer.WriteText( content, AutoFlush || flush );
    }




    private static TextFileWriter GetWriter( string filepath, Encoding encoding )
    {
      var writer = _collection.GetOrAdd( filepath, new TextFileWriter( filepath, encoding ) );


      if ( writer.Encoding.Equals( encoding ) == false )
        throw new InvalidOperationException( "file encoding is not compatible" );

      return writer;
    }



    /// <summary>
    /// 刷新所有日志文本流，确保所有修改已经写入。
    /// </summary>
    public static void Flush()
    {

      foreach ( var writer in _collection.Values )
      {
        writer.Flush();
      }

    }

    /// <summary>
    /// 刷新指定日志文本流，确保所有修改已经写入。
    /// </summary>
    public static void Flush( string filepath )
    {

      TextFileWriter writer;
      if ( _collection.TryGetValue( filepath, out writer ) )
        writer.Flush();
    }


    /// <summary>
    /// 获取编写日志时默认所需要采用的编码
    /// </summary>
    public static Encoding DefaultEncoding
    {
      get { return Encoding.UTF8; }
    }





  }
}
