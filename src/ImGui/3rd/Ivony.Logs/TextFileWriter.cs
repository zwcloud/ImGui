using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义文本日志编写器
  /// </summary>
  internal sealed class TextFileWriter : IDisposable
  {

    /// <summary>
    /// 创建 TextLogFileWriter 对象
    /// </summary>
    /// <param name="filepath">日志文件路径</param>
    /// <param name="encoding">文本编码格式</param>
    public TextFileWriter( string filepath, Encoding encoding )
    {

      if ( filepath == null )
        throw new ArgumentNullException( "filepath" );

      if ( encoding == null )
        throw new ArgumentNullException( "encoding" );

      Filepath = filepath;
      Encoding = encoding;

      if ( Path.IsPathRooted( filepath ) == false )
        throw new ArgumentException( "filepath must be absolute", "filepath" );


      Directory.CreateDirectory( Path.GetDirectoryName( filepath ) );
    }



    /// <summary>
    /// 文件路径
    /// </summary>
    public string Filepath { get; private set; }

    /// <summary>
    /// 文件编码
    /// </summary>
    public Encoding Encoding { get; private set; }



    private volatile ConcurrentBag<string> cache = new ConcurrentBag<string>();

    private StreamWriter writer;

    private object sync = new object();

    private Task flushTask;

    /// <summary>
    /// 写入文本
    /// </summary>
    /// <param name="text">要写入的文本</param>
    /// <param name="flush">写入后是否立即清理缓存</param>
    public void WriteText( string text, bool flush )
    {
      cache.Add( text );
      if ( flush )
        Flush();

      else
      {
        if ( flushTask == null )
          CreateFlushTask( TimeSpan.FromSeconds( 10 ) );
      }

    }

    private void CreateFlushTask( TimeSpan delay )
    {
      lock ( sync )
      {
        if ( flushTask != null )
          return;

        flushTask = DelayAndFlush( delay );
      }
    }


    private async Task DelayAndFlush( TimeSpan delay )
    {
      await Task.Delay( delay );
      Flush();

      lock ( sync )
      {
        flushTask = null;
      }
    }

    private void DisposeWriter()
    {
      writer.Dispose();
      writer = null;
    }



    public void Flush()
    {
      lock ( sync )
      {
        var list = cache;
        cache = new ConcurrentBag<string>();

        File.AppendAllLines( Filepath, list );
      }
    }


    public void Dispose()
    {
      DisposeWriter();
      Filepath = null;
    }
  }
}
