using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义日志类别的抽象
  /// </summary>
  public abstract class LogType
  {

    /// <summary>
    /// 显示名称
    /// </summary>
    public abstract string DisplayName { get; }

    /// <summary>
    /// 严重程度
    /// </summary>
    public abstract int Serverity { get; }


    /// <summary>
    /// 用于唯一标识该日志类别的 GUID。
    /// </summary>
    public abstract Guid Guid { get; }


    /// <summary>
    /// 重写 Equals 方法，比较两个 LogType
    /// </summary>
    /// <param name="obj">要比较的对象</param>
    /// <returns>两个对象是否相等</returns>
    public override bool Equals( object obj )
    {
      var type = obj as LogType;
      if ( type == null )
        return false;

      else
        return type.Guid.Equals( Guid );
    }


    /// <summary>
    /// 重写 GetHashCode 方法，确保两个相同的 LogType 对象返回一样的结果
    /// </summary>
    /// <returns>对象的哈希值</returns>
    public override int GetHashCode()
    {
      return Guid.GetHashCode();
    }





    /// <summary>
    /// 重载 == 运算符，判断两个日志类型是否相等
    /// </summary>
    /// <param name="type1">第一个日志类型</param>
    /// <param name="type2">第二个日志类型</param>
    /// <returns>两个日志类型是否相等</returns>
    public static bool operator ==( LogType type1, LogType type2 )
    {
      if ( object.ReferenceEquals( type1, null ) && object.ReferenceEquals( type2, null ) )
        return true;

      if ( object.ReferenceEquals( type1, null ) || object.ReferenceEquals( type2, null ) )
        return false;

      return type1.Guid.Equals( type2.Guid );
    }


    /// <summary>
    /// 重载 == 运算符，判断两个日志类型是否不相等
    /// </summary>
    /// <param name="type1">第一个日志类型</param>
    /// <param name="type2">第二个日志类型</param>
    /// <returns>两个日志类型是否不相等</returns>
    public static bool operator !=( LogType type1, LogType type2 )
    {
      if ( object.ReferenceEquals( type1, null ) && object.ReferenceEquals( type2, null ) )
        return false;

      if ( object.ReferenceEquals( type1, null ) || object.ReferenceEquals( type2, null ) )
        return true;

      return !type1.Guid.Equals( type2.Guid );
    }



    private class BuiltInLogType : LogType
    {
      public BuiltInLogType( string name, int servertity, Guid guid )
      {
        _name = name;
        _serverity = servertity;
        _guid = guid;
      }



      private string _name;

      public override string DisplayName
      {
        get { return _name; }
      }

      private int _serverity;
      public override int Serverity
      {
        get { return _serverity; }
      }


      private Guid _guid;
      public override Guid Guid
      {
        get { return _guid; }
      }

    }


    /// <summary>信息类别，代表一般日志信息</summary>
    public static readonly LogType Info = new BuiltInLogType( "Infomation", 1000, new Guid( "{185D55AC-9E39-4331-9988-1EE8D3804E83}" ) );
    /// <summary>重要信息类别，代表重要的日志信息</summary>
    public static readonly LogType ImportantInfo = new BuiltInLogType( "Important", 2000, new Guid( "{B51EBECC-0639-4E56-A14C-CAE491DE28D4}" ) );
    /// <summary>警告信息类别，代表系统可能存在问题的信息</summary>
    public static readonly LogType Warning = new BuiltInLogType( "Warnning", 3000, new Guid( "{3E35B201-01FD-481E-9BB8-62286137AF76}" ) );
    /// <summary>错误信息类别，代表系统出现了错误的日志信息</summary>
    public static readonly LogType Error = new BuiltInLogType( "Error", 4000, new Guid( "{93C31802-5A3E-4E6F-A2CB-0C8DDAEA2D65}" ) );
    /// <summary>异常信息类别，代表系统出现了未能处理的异常信息</summary>
    public static readonly LogType Exception = new BuiltInLogType( "Exception", 4500, new Guid( "{83C1A0D8-C0ED-4A9C-848E-26BD7E8600A4}" ) );
    /// <summary>致命错误类别，代表系统出现了严重的无法挽回的错误信息</summary>
    public static readonly LogType FatalError = new BuiltInLogType( "Fatal", 5000, new Guid( "{1D7001EB-50B9-4788-BFE5-86A641DB439F}" ) );
    /// <summary>系统崩溃错误类别，代表系统已经或者而即将崩溃的错误信息</summary>
    public static readonly LogType CrashError = new BuiltInLogType( "Crash", 10000, new Guid( "{45FFE3F2-E96A-460A-ACC2-3047D98C3DEF}" ) );
  }
}
