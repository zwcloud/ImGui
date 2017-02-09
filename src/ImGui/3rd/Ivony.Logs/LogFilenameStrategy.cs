using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义日志文件名策略
  /// </summary>
  public abstract class LogFilenameStrategy
  {

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <param name="entry">日志记录</param>
    /// <returns>当前日志条目应当分配的名称</returns>
    public abstract string GetName( LogEntry entry );



    /// <summary>
    /// 提供从字符串对象到 LogFilenameStrategy 对象的隐式类型转换
    /// </summary>
    /// <param name="literal">要转换为日志文件名策略的文本字符串</param>
    /// <returns>一个静态的日志文件名策略</returns>
    public static implicit operator LogFilenameStrategy( string literal )
    {
      return new Literal( literal );
    }


    /// <summary>
    /// 合并两个日志文件名策略
    /// </summary>
    /// <param name="strategy1">第一个日志文件名策略</param>
    /// <param name="strategy2">另一个日志文件名策略</param>
    /// <returns>合并后的日志文件名策略</returns>
    public static LogFilenameStrategy operator +( LogFilenameStrategy strategy1, LogFilenameStrategy strategy2 )
    {
      return Series.Concat( strategy1, strategy2 );
    }


    /// <summary>
    /// 合并一个静态文本前缀到日志文件名策略
    /// </summary>
    /// <param name="literal">前缀文本</param>
    /// <param name="strategy">日志文件名策略</param>
    /// <returns>合并后的日志文件名策略</returns>
    public static LogFilenameStrategy operator +( string literal, LogFilenameStrategy strategy )
    {
      return Series.Concat( literal, strategy );
    }


    /// <summary>
    /// 合并一个静态文本后缀到日志文件名策略
    /// </summary>
    /// <param name="strategy">日志文件名策略</param>
    /// <param name="literal">后缀文本</param>
    /// <returns>合并后的日志文件名策略</returns>
    public static LogFilenameStrategy operator +( LogFilenameStrategy strategy, string literal )
    {
      return Series.Concat( strategy, literal );
    }


    private class Series : LogFilenameStrategy
    {

      public static LogFilenameStrategy Concat( LogFilenameStrategy strategy1, LogFilenameStrategy strategy2 )
      {
        var literal1 = strategy1 as Literal;
        if ( literal1 != null )
        {
          var literal2 = strategy2 as Literal;
          if ( literal2 != null )
            return new Literal( literal1.Text + literal2.Text );
        }

        return new Series( strategy1, strategy2 );
      }


      private readonly LogFilenameStrategy[] _strategies;


      public Series( LogFilenameStrategy strategy1, LogFilenameStrategy strategy2 )
      {


        var providers = new List<LogFilenameStrategy>();


        {
          var series = strategy1 as Series;
          if ( series != null )
            providers.AddRange( series._strategies );

          else
            providers.Add( strategy1 );
        }

        {
          var series = strategy2 as Series;
          if ( series != null )
            providers.AddRange( series._strategies );

          else
            providers.Add( strategy2 );
        }

        _strategies = providers.ToArray();

      }



      public override string GetName( LogEntry entry )
      {
        return string.Concat( _strategies.Select( s => s.GetName( entry ) ) );
      }

    }




    private class Literal : LogFilenameStrategy
    {

      private readonly string _text;


      public string Text { get { return _text; } }

      public Literal( string text )
      {
        _text = text;
      }


      public override string GetName( LogEntry entry )
      {
        return _text;
      }
    }


  }
}
