using System;
using System.Collections.Generic;
using System.Linq;
#if !NETSTANDARD1_6
using System.Runtime.Remoting.Messaging;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义日志范畴对象
  /// </summary>
  public class LogScope : IDisposable
  {

    /// <summary>
    /// 创建 LogScope 实例
    /// </summary>
    /// <param name="name">范畴名称</param>
    protected LogScope( string name )
    {
      if ( name == null )
        throw new ArgumentNullException( "name" );

      if ( name == "" )
        throw new ArgumentException( "范畴名称不能为空", "name" );

      if ( name.Contains( '/' ) )
        throw new ArgumentException( "范畴名称不能包含 \"/\" 字符", "name" );

      Name = name;
    }

    private LogScope()
    {

    }


    /// <summary>
    /// 范畴名称
    /// </summary>
    public string Name
    {
      get;
      private set;
    }


#if NETSTANDARD1_6
    private static readonly AsyncLocal<LogScope> slot = new AsyncLocal<LogScope>() { Value = _root };

    /// <summary>
    /// 获取当前范畴对象
    /// </summary>
    public static LogScope CurrentScope
    {
      get { return slot.Value = slot.Value ?? RootScope; }
      private set { slot.Value = value ?? RootScope; }
    }
#else
    private static readonly string logScopeContextName = "log-scope";
    /// <summary>
    /// 获取当前范畴对象
    /// </summary>
    public static LogScope CurrentScope
    {
        get { return CallContext.LogicalGetData(logScopeContextName) as LogScope ?? RootScope; }
        private set
        {
            if (value == null || value == RootScope)
                CallContext.FreeNamedDataSlot(logScopeContextName);
            else
                CallContext.LogicalSetData(logScopeContextName, value);
        }
    }
#endif

    private static LogScope _root = new LogScope();


    /// <summary>
    /// 根范畴
    /// </summary>
    public static LogScope RootScope
    {
      get { return _root; }
    }


    /// <summary>
    /// 父级范畴
    /// </summary>
    public LogScope Parent
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建并进入一个日志范畴
    /// </summary>
    /// <param name="name">范畴名称</param>
    /// <returns></returns>
    public static LogScope EnterScope( string name )
    {
      return EnterScope( new LogScope( name ) );
    }



    /// <summary>
    /// 进入一个日志范畴
    /// </summary>
    /// <param name="scope">要进入的日志范畴</param>
    /// <returns></returns>
    public static LogScope EnterScope( LogScope scope )
    {
      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      if ( scope.Parent != null )
        throw new InvalidOperationException( "无法进入这个范畴，因为这个范畴已经被使用" );

      scope.Parent = CurrentScope;
      return CurrentScope = scope;
    }



    /// <summary>
    /// 离开指定的范畴，若指定范畴在当前上下文不存在，则不进行任何操作，并返回 null 。
    /// </summary>
    /// <param name="scope">要离开的范畴</param>
    /// <returns></returns>
    public static LogScope LeaveScope( LogScope scope )
    {

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      var current = CurrentScope;

      while ( current != scope )
      {
        current._leaved = true;
        current = current.Parent;

        if ( current == null )
          return null;
      }

      return CurrentScope = current.Parent;
    }


    private readonly object _sync = new object();

    private bool _leaved = false;

    /// <summary>
    /// 离开当前范畴
    /// </summary>
    public void Leave()
    {
      lock ( _sync )
      {
        if ( _leaved == false )
        {
          LeaveScope( this );
        }
      }
    }


    void IDisposable.Dispose()
    {
      Leave();
    }
  }
}
