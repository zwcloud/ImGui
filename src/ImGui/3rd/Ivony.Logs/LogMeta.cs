using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ivony.Logs
{

  /// <summary>
  /// 定义日志元数据
  /// </summary>
  public sealed class LogMeta
  {

    private Dictionary<Type, object> data;



    /// <summary>
    /// 创建 LogMeta 对象
    /// </summary>
    private LogMeta()
    {
      data = new Dictionary<Type, object>();
    }


    /// <summary>
    /// 获取默认日志元数据
    /// </summary>
    /// <returns></returns>
    public static LogMeta GetDefaultMeta()
    {
      return new LogMeta().SetMetaData( LogType.Info, LogScope.CurrentScope );
    }




    /// <summary>
    /// 从既有的元数据创建 LogMeta 对象
    /// </summary>
    /// <param name="metaData">既有的元数据</param>
    public LogMeta( LogMeta metaData )
    {
      data = new Dictionary<Type, object>( metaData.data );
    }




    private LogMeta( Dictionary<Type, object> data )
    {
      this.data = data;
    }




    /// <summary>
    /// 获取指定类型的元数据
    /// </summary>
    /// <typeparam name="T">元数据类型</typeparam>
    /// <returns></returns>
    public T GetMetaData<T>() where T : class
    {
      var type = GetRootType( typeof( T ) );

      if ( data.ContainsKey( type ) )
        return data[type] as T;

      return null;
    }


    /// <summary>
    /// 设置一个元数据
    /// </summary>
    /// <param name="metaData">元数据对象</param>
    /// <returns>设置后的日志元数据</returns>
    public LogMeta SetMetaData( params object[] metaData )
    {
      var data = this.data;

      foreach ( var item in metaData )
        data[GetRootType( item.GetType() )] = item;

      return new LogMeta( data );
    }


    private Type GetRootType( Type type )
    {
#if NETSTANDARD1_6
      var typeInfo = type.GetTypeInfo();
      if (typeInfo.IsValueType )
        throw new InvalidOperationException( "值类型对象不能作为日志元数据储存" );
      if (typeInfo.BaseType != typeof( object ) )
        return GetRootType(typeInfo.BaseType );
      else
        return type;
#else//for xamarin
      if ( type.IsValueType )
        throw new InvalidOperationException( "值类型对象不能作为日志元数据储存" );

      if ( type.BaseType != typeof( object ) )
        return GetRootType( type.BaseType );

      else
        return type;
#endif
    }
  }
}
