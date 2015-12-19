using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Permissions;

[assembly: SuppressMessage( "Microsoft.Performance", "CA1804:RemoveUnusedLocals", Scope = "member", Target = "Common.NodeTree`1+EnumeratorBase`1.Count", MessageId = "o" )]
[assembly: SuppressMessage( "Microsoft.Performance", "CA1804:RemoveUnusedLocals", Scope = "member", Target = "Common.NodeTree`1+BaseEnumerableCollectionPair+BaseNodesEnumerableCollection.Count", MessageId = "o" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Common.NodeTree`1.GetNodeTree(Common.INode`1<T>):Common.NodeTree`1<T>" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Common.NodeTree`1.GetNodeTree(Common.ITree`1<T>):Common.NodeTree`1<T>" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Common.NodeTree`1.NewTree():Common.ITree`1<T>" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Common.NodeTree`1.NewNode():Common.INode`1<T>" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Common.NodeTree`1.XmlAdapterTag" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Common.NodeTree`1.XmlDeserialize(System.IO.Stream):Common.ITree`1<T>" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Common.IEnumerableCollectionPair`1.Nodes" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Common.NodeTree`1.Nodes" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Common.NodeTree`1+AllEnumerator.Nodes" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Common.NodeTree`1+BaseEnumerableCollectionPair.Nodes" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Common.NodeTree`1+BaseEnumerableCollectionPair+BaseNodesEnumerableCollection.GetEnumerator():System.Collections.Generic.IEnumerator`1<Common.INode`1<T>>" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Common.NodeTree`1+AllEnumerator" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Common.NodeTree`1+BaseEnumerableCollectionPair" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Common.NodeTree`1+RootObject" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Common.NodeTree`1+NodeXmlSerializationAdapter" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Common.NodeTree`1+NodeXmlSerializationAdapter+IXmlCollection" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Scope = "type", Target = "Common.NodeTree`1+TreeXmlSerializationAdapter" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Common.NodeTree`1.Dispose():System.Void" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Common.NodeTree`1.Finalize():System.Void" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Common.NodeTree`1+BaseEnumerableCollectionPair+BaseNodesEnumerableCollection.Dispose():System.Void" )]
[assembly: SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member", Target = "Common.NodeTree`1+BaseEnumerableCollectionPair+BaseNodesEnumerableCollection.Finalize():System.Void" )]
[assembly: SuppressMessage( "Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Scope = "member", Target = "Common.NodeTree`1.XmlAdapterTag" )]
[assembly: SuppressMessage( "Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Scope = "member", Target = "Common.NodeTree`1+NodeXmlSerializationAdapter.Children" )]

namespace ImGui
{

	//-----------------------------------------------------------------------------
	// IDeepCopy

	public interface IDeepCopy
	{
		object CreateDeepCopy();
	}

	//-----------------------------------------------------------------------------
	// IEnumerableCollection

	public interface IEnumerableCollection<T> : IEnumerable<T>, ICollection
	{
		bool Contains( T item );
	}

	//-----------------------------------------------------------------------------
	// IEnumerableCollectionPair

	public interface IEnumerableCollectionPair<T>
	{
		IEnumerableCollection<INode<T>> Nodes { get; }
		IEnumerableCollection<T> Values { get; }
		INode<T> Find( T item );
		INode<T> Find( Predicate<T> predicate );
	}

	//-----------------------------------------------------------------------------
	// EventArgs

	public class NodeTreeDataEventArgs<T> : EventArgs
	{
		private T _Data = default( T );

		public T Data { get { return _Data; } }

		public NodeTreeDataEventArgs( T data )
		{
			_Data = data;
		}
	}

	public class NodeTreeNodeEventArgs<T> : EventArgs
	{
		private INode<T> _Node = null;

		public INode<T> Node { get { return _Node; } }

		public NodeTreeNodeEventArgs( INode<T> node )
		{
			_Node = node;
		}
	}

	public enum NodeTreeInsertOperation
	{
		Previous,
		Next,
		Child,
		Tree
	}

	public class NodeTreeInsertEventArgs<T> : EventArgs
	{
		private NodeTreeInsertOperation _Operation;
		public NodeTreeInsertOperation Operation { get { return _Operation; } }

		private INode<T> _Node = null;
		public INode<T> Node { get { return _Node; } }

		public NodeTreeInsertEventArgs( NodeTreeInsertOperation operation, INode<T> node )
		{
			_Operation = operation;
			_Node = node;
		}
	}

	//-----------------------------------------------------------------------------
	// INode<T>

	public interface INode<T> : IEnumerableCollectionPair<T>, IDisposable//, ICollection<T>
	{
		T Data { get; set; }

		string ToStringRecursive();

		int Depth { get; }
		int BranchIndex { get; }
		int BranchCount { get; }

		int Count { get; }
		int DirectChildCount { get; }

		INode<T> Parent { get; }
		INode<T> Previous { get; }
		INode<T> Next { get; }
		INode<T> Child { get; }

		ITree<T> Tree { get; }

		INode<T> Root { get; }
		INode<T> Top { get; }
		INode<T> First { get; }
		INode<T> Last { get; }

		INode<T> LastChild { get; }

		bool IsTree { get; }
		bool IsRoot { get; }
		bool IsTop { get; }
		bool HasParent { get; }
		bool HasPrevious { get; }
		bool HasNext { get; }
		bool HasChild { get; }
		bool IsFirst { get; }
		bool IsLast { get; }

		INode<T> this[ T item ] { get; }

		bool Contains( INode<T> item );
		bool Contains( T item );

		INode<T> InsertPrevious( T o );
		INode<T> InsertNext( T o );
		INode<T> InsertChild( T o );
		INode<T> Add( T o );
		INode<T> AddChild( T o );

		void InsertPrevious( ITree<T> tree );
		void InsertNext( ITree<T> tree );
		void InsertChild( ITree<T> tree );
		void Add( ITree<T> tree );
		void AddChild( ITree<T> tree );

		ITree<T> Cut( T o );
		ITree<T> Copy( T o );
		ITree<T> DeepCopy( T o );
		bool Remove( T o );

		ITree<T> Cut();
		ITree<T> Copy();
		ITree<T> DeepCopy();
		void Remove();

		bool CanMoveToParent { get; }
		bool CanMoveToPrevious { get; }
		bool CanMoveToNext { get; }
		bool CanMoveToChild { get; }
		bool CanMoveToFirst { get; }
		bool CanMoveToLast { get; }

		void MoveToParent();
		void MoveToPrevious();
		void MoveToNext();
		void MoveToChild();
		void MoveToFirst();
		void MoveToLast();

		IEnumerableCollectionPair<T> All { get; }
		IEnumerableCollectionPair<T> AllChildren { get; }
		IEnumerableCollectionPair<T> DirectChildren { get; }
		IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }

		void SortAllChildren();
		void SortAllChildren( Comparison<T> comparison );
		void SortAllChildren( IComparer<T> comparer );

		void SortDirectChildren();
		void SortDirectChildren( Comparison<T> comparison );
		void SortDirectChildren( IComparer<T> comparer );

		event EventHandler<NodeTreeDataEventArgs<T>> Validate;
		event EventHandler<NodeTreeDataEventArgs<T>> Setting;
		event EventHandler<NodeTreeDataEventArgs<T>> SetDone;
		event EventHandler<NodeTreeInsertEventArgs<T>> Inserting;
		event EventHandler<NodeTreeInsertEventArgs<T>> Inserted;
		event EventHandler Cutting;
		event EventHandler CutDone;
		event EventHandler<NodeTreeNodeEventArgs<T>> Copying;
		event EventHandler<NodeTreeNodeEventArgs<T>> Copied;
		event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopying;
		event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopied;
	}

	//-----------------------------------------------------------------------------
	// ITree<T>

	public interface ITree<T> : IEnumerableCollectionPair<T>, IDisposable//, ICollection<T>
	{
		Type DataType { get; }

		IEqualityComparer<T> DataComparer { get; set; }

		void XmlSerialize( Stream stream );

		void Clear();
		int Count { get; }
		int DirectChildCount { get; }

		INode<T> Root { get; }

		INode<T> this[ T o ] { get; }

		string ToStringRecursive();

		bool Contains( T item );
		bool Contains( INode<T> item );

		INode<T> InsertChild( T o );
		INode<T> AddChild( T o );

		void InsertChild( ITree<T> tree );
		void AddChild( ITree<T> tree );

		ITree<T> Cut( T o );
		ITree<T> Copy( T o );
		ITree<T> DeepCopy( T o );
		bool Remove( T o );

		ITree<T> Copy();
		ITree<T> DeepCopy();

		IEnumerableCollectionPair<T> All { get; }
		IEnumerableCollectionPair<T> AllChildren { get; }
		IEnumerableCollectionPair<T> DirectChildren { get; }
		IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }

		void SortAllChildren();
		void SortAllChildren( Comparison<T> comparison );
		void SortAllChildren( IComparer<T> comparer );

		event EventHandler<NodeTreeDataEventArgs<T>> Validate;
		event EventHandler Clearing;
		event EventHandler Cleared;
		event EventHandler<NodeTreeDataEventArgs<T>> Setting;
		event EventHandler<NodeTreeDataEventArgs<T>> SetDone;
		event EventHandler<NodeTreeInsertEventArgs<T>> Inserting;
		event EventHandler<NodeTreeInsertEventArgs<T>> Inserted;
		event EventHandler Cutting;
		event EventHandler CutDone;
		event EventHandler<NodeTreeNodeEventArgs<T>> Copying;
		event EventHandler<NodeTreeNodeEventArgs<T>> Copied;
		event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopying;
		event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopied;
	}

	//-----------------------------------------------------------------------------
	// NodeTree

	[Serializable]
	public class NodeTree<T> : INode<T>, ITree<T>, ISerializable
	{
		private T _Data = default( T );

		private NodeTree<T> _Parent = null;
		private NodeTree<T> _Previous = null;
		private NodeTree<T> _Next = null;
		private NodeTree<T> _Child = null;

		protected NodeTree() { }

		~NodeTree()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );

			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( disposing )
			{
				if ( _EventHandlerList != null ) _EventHandlerList.Dispose();
			}
		}

		//-----------------------------------------------------------------------------
		// Instantiation

		public static ITree<T> NewTree()
		{
			return new RootObject();
		}

		public static ITree<T> NewTree( IEqualityComparer<T> dataComparer )
		{
			return new RootObject( dataComparer );
		}

		protected static INode<T> NewNode()
		{
			return new NodeTree<T>();
		}

		protected virtual NodeTree<T> CreateTree()
		{
			return new RootObject();
		}

		protected virtual NodeTree<T> CreateNode()
		{
			return new NodeTree<T>();
		}

		//-----------------------------------------------------------------------------
		// ToString

		/// <summary>Obtains the <see cref="String"/> representation of this instance.</summary>
		/// <returns>The <see cref="String"/> representation of this instance.</returns>
		/// <remarks>
		/// <p>
		/// This method returns a <see cref="String"/> that represents this instance.
		/// </p>
		/// </remarks>
		public override string ToString()
		{
			T data = Data;
			if ( data == null ) return String.Empty;
			return data.ToString();
		}

		public virtual string ToStringRecursive()
		{
			string s = String.Empty;

			foreach ( NodeTree<T> node in All.Nodes )
				s += new String( '\t', node.Depth ) + node + Environment.NewLine;

			return s;
		}

		//-----------------------------------------------------------------------------
		// Counts

		public virtual int Depth
		{
			get
			{
				int i = -1;

				for ( INode<T> node = this ; !node.IsRoot ; node = node.Parent ) i++;

				return i;
			}
		}

		public virtual int BranchIndex
		{
			get
			{
				int i = -1;

				for ( INode<T> node = this ; node != null ; node = node.Previous ) i++;

				return i;
			}
		}

		public virtual int BranchCount
		{
			get
			{
				int i = 0;

				for ( INode<T> node = First ; node != null ; node = node.Next ) i++;

				return i;
			}
		}

		//-----------------------------------------------------------------------------
		// DeepCopyData

		[ReflectionPermission( SecurityAction.Demand, Unrestricted = true )]
		protected virtual T DeepCopyData( T data )
		{
			//if ( ! Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( data == null ) { Debug.Assert( true ); return default( T ); }

			// IDeepCopy
			IDeepCopy deepCopy = data as IDeepCopy;
			if ( deepCopy != null ) return ( T ) deepCopy.CreateDeepCopy();

			// ICloneable
			ICloneable cloneable = data as ICloneable;
			if ( cloneable != null ) return ( T ) cloneable.Clone();

			// Copy constructor
			ConstructorInfo ctor = data.GetType().GetConstructor(
				 BindingFlags.Instance | BindingFlags.Public,
				 null, new Type[] { typeof( T ) }, null );
			if ( ctor != null ) return ( T ) ctor.Invoke( new object[] { data } );
			//return ( T ) Activator.CreateInstance( typeof( T ), new object[] { data } );

			// give up
			return data;
		}

		//-----------------------------------------------------------------------------
		// RootObject

		[Serializable]
		protected class RootObject : NodeTree<T>
		{
			private int _Version = 0;
			protected override int Version
			{
				get { return _Version; }
				set { _Version = value; }
			}

			private IEqualityComparer<T> _DataComparer;
			public override IEqualityComparer<T> DataComparer
			{
				get
				{
					if ( _DataComparer == null ) _DataComparer = EqualityComparer<T>.Default;

					return _DataComparer;
				}

				set { _DataComparer = value; }
			}

			public RootObject()
			{
			}

			public RootObject( IEqualityComparer<T> dataComparer )
			{
				_DataComparer = dataComparer;
			}

			/// <summary>Obtains the <see cref="String"/> representation of this instance.</summary>
			/// <returns>The <see cref="String"/> representation of this instance.</returns>
			/// <remarks>
			/// <p>
			/// This method returns a <see cref="String"/> that represents this instance.
			/// </p>
			/// </remarks>
			public override string ToString() { return "ROOT: " + DataType.Name; }

			// Save
			/// <summary>Populates a SerializationInfo with the data needed to serialize the target T.</summary>
			/// <param name="info">The SerializationInfo to populate with data.</param>
			/// <param name="context">The destination for this serialization.</param>
			/// <remarks>
			/// <p>This method is called during serialization.</p>
			/// <p>Do not call this method directly.</p>
			/// </remarks>
			[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
			public override void GetObjectData( SerializationInfo info, StreamingContext context )
			{
				base.GetObjectData( info, context );

				info.AddValue( "RootObjectVersion", 1 );
				//info.AddValue( "DataType", _DataType );
			}

			// Load
			/// <summary>Initializes a new instance of the class during deserialization.</summary>
			/// <param name="info">The SerializationInfo populated with data.</param>
			/// <param name="context">The source for this serialization.</param>
			/// <remarks>
			/// <p>This method is called during deserialization.</p>
			/// <p>Do not call this method directly.</p>
			/// </remarks>
			protected RootObject( SerializationInfo info, StreamingContext context )
				: base( info, context )
			{
				int iVersion = info.GetInt32( "RootObjectVersion" );
				if ( iVersion != 1 ) throw new SerializationException( "Unknown version" );

				//_DataType = info.GetValue( "DataType", typeof( Type ) ) as Type;
			}
		}

		//-----------------------------------------------------------------------------
		// GetRootObject

		protected virtual RootObject GetRootObject
		{
			get
			{
				return ( RootObject ) Root;
			}
		}

		//-----------------------------------------------------------------------------
		// DataComparer

		public virtual IEqualityComparer<T> DataComparer
		{
			get
			{
				if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a Tree" );

				return GetRootObject.DataComparer;
			}

			set
			{
				if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a Tree" );

				GetRootObject.DataComparer = value;
			}
		}

		//-----------------------------------------------------------------------------
		// Version

		protected virtual int Version
		{
			get
			{
				INode<T> root = Root;

				if ( !root.IsTree ) throw new InvalidOperationException( "This is not a Tree" );

				return GetNodeTree( root ).Version;
			}

			set
			{
				INode<T> root = Root;

				if ( !root.IsTree ) throw new InvalidOperationException( "This is not a Tree" );

				GetNodeTree( root ).Version = value;
			}
		}

		protected bool HasChanged( int version ) { return ( Version != version ); }

		protected void IncrementVersion()
		{
			INode<T> root = Root;

			if ( !root.IsTree ) throw new InvalidOperationException( "This is not a Tree" );

			GetNodeTree( root ).Version++;
		}

		//-----------------------------------------------------------------------------
		// ISerializable

		// Save
		/// <summary>Populates a SerializationInfo with the data needed to serialize the target T.</summary>
		/// <param name="info">The SerializationInfo to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		/// <remarks>
		/// <p>This method is called during serialization.</p>
		/// <p>Do not call this method directly.</p>
		/// </remarks>
		[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "NodeTreeVersion", 1 );
			info.AddValue( "Data", _Data );
			info.AddValue( "Parent", _Parent );
			info.AddValue( "Previous", _Previous );
			info.AddValue( "Next", _Next );
			info.AddValue( "Child", _Child );
		}

		// Load
		/// <summary>Initializes a new instance of the class during deserialization.</summary>
		/// <param name="info">The SerializationInfo populated with data.</param>
		/// <param name="context">The source for this serialization.</param>
		/// <remarks>
		/// <p>This method is called during deserialization.</p>
		/// <p>Do not call this method directly.</p>
		/// </remarks>
		protected NodeTree( SerializationInfo info, StreamingContext context )
		{
			int iVersion = info.GetInt32( "NodeTreeVersion" );
			if ( iVersion != 1 ) throw new SerializationException( "Unknown version" );

			_Data = ( T ) info.GetValue( "Data", typeof( T ) );
			_Parent = ( NodeTree<T> ) info.GetValue( "Parent", typeof( NodeTree<T> ) );
			_Previous = ( NodeTree<T> ) info.GetValue( "Previous", typeof( NodeTree<T> ) );
			_Next = ( NodeTree<T> ) info.GetValue( "Next", typeof( NodeTree<T> ) );
			_Child = ( NodeTree<T> ) info.GetValue( "Child", typeof( NodeTree<T> ) );
		}

		//-----------------------------------------------------------------------------
		// XmlSerialization

		public virtual void XmlSerialize( Stream stream )
		{
			XmlSerializer xmlSerializer;

			try
			{
				xmlSerializer = new XmlSerializer( typeof( TreeXmlSerializationAdapter ) );
			}
			catch ( Exception x )
			{
				Debug.Assert( x == null ); // false
				throw;
			}

			try
			{
				xmlSerializer.Serialize( stream, new TreeXmlSerializationAdapter( XmlAdapterTag, this ) );
			}
			catch ( Exception x )
			{
				Debug.Assert( x == null ); // false
				throw;
			}

		}

		public static ITree<T> XmlDeserialize( Stream stream )
		{
			XmlSerializer xmlSerializer;

			try
			{
				xmlSerializer = new XmlSerializer( typeof( TreeXmlSerializationAdapter ) );
			}
			catch ( Exception x )
			{
				Debug.Assert( x == null ); // false
				throw;
			}

			object o;

			try
			{
				o = xmlSerializer.Deserialize( stream );
			}
			catch ( Exception x )
			{
				Debug.Assert( x == null ); // false
				throw;
			}

			TreeXmlSerializationAdapter adapter = ( TreeXmlSerializationAdapter ) o;

			ITree<T> tree = adapter.Tree;

			return tree;
		}

		//-----------------------------------------------------------------------------

		protected static readonly object XmlAdapterTag = new object();

		[XmlType( "Tree" )]
		public class TreeXmlSerializationAdapter
		{
			private int _Version = 0;

			[XmlAttribute]
			public int Version
			{
				get { return 1; }
				set { _Version = value; }
			}

			private ITree<T> _Tree;

			[XmlIgnore]
			public ITree<T> Tree
			{
				get { return _Tree; }
			}

			private TreeXmlSerializationAdapter()
			{
			}

			public TreeXmlSerializationAdapter( object tag, ITree<T> tree )
			{
				if ( !ReferenceEquals( XmlAdapterTag, tag ) ) throw new InvalidOperationException( "Don't use this class" );

				_Tree = tree;
			}

			public NodeXmlSerializationAdapter Root
			{
				get
				{
					return new NodeXmlSerializationAdapter( XmlAdapterTag, _Tree.Root );
				}

				set
				{
					_Tree = NodeTree<T>.NewTree();

					ReformTree( _Tree.Root, value );
				}
			}

			private void ReformTree( INode<T> parent, NodeXmlSerializationAdapter node )
			{
				foreach ( NodeXmlSerializationAdapter child in node.Children )
				{
					INode<T> n = parent.AddChild( child.Data );

					ReformTree( n, child );
				}
			}

		}

		[XmlType( "Node" )]
		public class NodeXmlSerializationAdapter
		{
			private int _Version = 0;

			[XmlAttribute]
			public int Version
			{
				get { return 1; }
				set { _Version = value; }
			}

			private INode<T> _Node;

			private IXmlCollection _Children = new ChildCollection();

			[XmlIgnore]
			public INode<T> Node
			{
				get { return _Node; }
			}

			// Load
			private NodeXmlSerializationAdapter()
			{
				_Node = NodeTree<T>.NewNode();
			}

			// Save
			public NodeXmlSerializationAdapter( object tag, INode<T> node )
			{
				if ( !ReferenceEquals( XmlAdapterTag, tag ) ) throw new InvalidOperationException( "Don't use this class" );

				_Node = node;

				foreach ( INode<T> child in node.DirectChildren.Nodes )
					_Children.Add( new NodeXmlSerializationAdapter( XmlAdapterTag, child ) );
			}

			public T Data
			{
				get { return _Node.Data; }

				set
				{
					GetNodeTree( _Node )._Data = value;
				}
			}

			public IXmlCollection Children
			{
				get { return _Children; }
				set { Debug.Assert( value == null ); }
			}

			public interface IXmlCollection : ICollection
			{
				NodeXmlSerializationAdapter this[ int index ] { get; }

				void Add( NodeXmlSerializationAdapter item );
			}

			private class ChildCollection : List<NodeXmlSerializationAdapter>, IXmlCollection { }
		}


		//-----------------------------------------------------------------------------
		// INode<T>

		public T Data
		{
			get
			{
				return _Data;
			}

			set
			{
				if ( IsRoot ) throw new InvalidOperationException( "This is a Root" );

				OnSetting( this, value );

				_Data = value;

				OnSetDone( this, value );
			}
		}

		public INode<T> Parent { get { return _Parent; } }
		public INode<T> Previous { get { return _Previous; } }
		public INode<T> Next { get { return _Next; } }
		public INode<T> Child { get { return _Child; } }

		//-----------------------------------------------------------------------------

		public ITree<T> Tree
		{
			get
			{
				return ( ITree<T> ) Root;
			}
		}

		public INode<T> Root
		{
			get
			{
				INode<T> node = this;

				while ( node.Parent != null )
					node = node.Parent;

				return node;
			}
		}

		public INode<T> Top
		{
			get
			{
				if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );
				//if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
				if ( this.IsRoot ) return null;

				INode<T> node = this;

				while ( node.Parent.Parent != null )
					node = node.Parent;

				return node;
			}
		}

		public INode<T> First
		{
			get
			{
				INode<T> node = this;

				while ( node.Previous != null )
					node = node.Previous;

				return node;
			}
		}

		public INode<T> Last
		{
			get
			{
				INode<T> node = this;

				while ( node.Next != null )
					node = node.Next;

				return node;
			}
		}

		public INode<T> LastChild
		{
			get
			{
				if ( Child == null ) return null;
				return Child.Last;
			}
		}

		//-----------------------------------------------------------------------------

		public bool HasPrevious { get { return Previous != null; } }
		public bool HasNext { get { return Next != null; } }
		public bool HasChild { get { return Child != null; } }
		public bool IsFirst { get { return Previous == null; } }
		public bool IsLast { get { return Next == null; } }

		public bool IsTree
		{
			get
			{
				if ( !IsRoot ) return false;
				return this is RootObject;
			}
		}

		public bool IsRoot
		{
			get
			{
				bool b = ( Parent == null );

				if ( b )
				{
					Debug.Assert( Previous == null );
					Debug.Assert( Next == null );
				}

				return b;
			}
		}

		public bool HasParent
		{
			get
			{
				//if ( IsRoot ) throw new InvalidOperationException( "This is a Root" );
				if ( IsRoot ) return false;
				return Parent.Parent != null;
			}
		}

		public bool IsTop
		{
			get
			{
				//if ( IsRoot ) throw new InvalidOperationException( "This is a Root" );
				if ( IsRoot ) return false;
				return Parent.Parent == null;
			}
		}

		//-----------------------------------------------------------------------------

		public virtual INode<T> this[ T item ]
		{
			get
			{
				if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

				IEqualityComparer<T> comparer = DataComparer;

				foreach ( INode<T> n in All.Nodes )
					if ( comparer.Equals( n.Data, item ) )
						return n;

				return null;
			}
		}

		public virtual bool Contains( INode<T> item )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			return All.Nodes.Contains( item );
		}

		public virtual bool Contains( T item )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			return All.Values.Contains( item );
		}

		public virtual INode<T> Find( T item )
		{
			return All.Find( item );
		}

		public virtual INode<T> Find( Predicate<T> predicate )
		{
			return All.Find( predicate );
		}

		//-----------------------------------------------------------------------------

		public INode<T> InsertPrevious( T o )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> newNode = CreateNode();
			newNode._Data = o;

			this.InsertPreviousCore( newNode );

			return newNode;
		}

		public INode<T> InsertNext( T o )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> newNode = CreateNode();
			newNode._Data = o;

			this.InsertNextCore( newNode );

			return newNode;
		}

		public INode<T> InsertChild( T o )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> newNode = CreateNode();
			newNode._Data = o;

			this.InsertChildCore( newNode );

			return newNode;
		}

		public INode<T> Add( T o )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			return this.Last.InsertNext( o );
		}

		public INode<T> AddChild( T o )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( Child == null )
				return InsertChild( o );
			else
				return Child.Add( o );
		}

		//-----------------------------------------------------------------------------

		public void InsertPrevious( ITree<T> tree )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> newTree = GetNodeTree( tree );

			if ( !newTree.IsRoot ) throw new ArgumentException( "Tree is not a Root" );
			if ( !newTree.IsTree ) throw new ArgumentException( "Tree is not a tree" );

			for ( INode<T> n = newTree.Child ; n != null ; n = n.Next )
			{
				NodeTree<T> node = GetNodeTree( n );
				NodeTree<T> copy = node.CopyCore();
				InsertPreviousCore( copy );
			}
		}

		public void InsertNext( ITree<T> tree )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> newTree = GetNodeTree( tree );

			if ( !newTree.IsRoot ) throw new ArgumentException( "Tree is not a Root" );
			if ( !newTree.IsTree ) throw new ArgumentException( "Tree is not a tree" );

			for ( INode<T> n = newTree.LastChild ; n != null ; n = n.Previous )
			{
				NodeTree<T> node = GetNodeTree( n );
				NodeTree<T> copy = node.CopyCore();
				InsertNextCore( copy );
			}
		}

		public void InsertChild( ITree<T> tree )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> newTree = GetNodeTree( tree );

			if ( !newTree.IsRoot ) throw new ArgumentException( "Tree is not a Root" );
			if ( !newTree.IsTree ) throw new ArgumentException( "Tree is not a tree" );

			for ( INode<T> n = newTree.LastChild ; n != null ; n = n.Previous )
			{
				NodeTree<T> node = GetNodeTree( n );
				NodeTree<T> copy = node.CopyCore();
				InsertChildCore( copy );
			}
		}

		public void Add( ITree<T> tree )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			this.Last.InsertNext( tree );
		}

		public void AddChild( ITree<T> tree )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( this.Child == null )
				this.InsertChild( tree );
			else
				this.Child.Add( tree );
		}

		//-----------------------------------------------------------------------------

		protected virtual void InsertPreviousCore( INode<T> newINode )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !newINode.IsRoot ) throw new ArgumentException( "Node is not a Root" );
			if ( newINode.IsTree ) throw new ArgumentException( "Node is a tree" );

			IncrementVersion();

			OnInserting( this, NodeTreeInsertOperation.Previous, newINode );

			NodeTree<T> newNode = GetNodeTree( newINode );

			newNode._Parent = this._Parent;
			newNode._Previous = this._Previous;
			newNode._Next = this;
			this._Previous = newNode;

			if ( newNode.Previous != null )
			{
				NodeTree<T> Previous = GetNodeTree( newNode.Previous );
				Previous._Next = newNode;
			}
			else // this is a first node
			{
				NodeTree<T> Parent = GetNodeTree( newNode.Parent );
				Parent._Child = newNode;
			}

			OnInserted( this, NodeTreeInsertOperation.Previous, newINode );
		}

		protected virtual void InsertNextCore( INode<T> newINode )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !newINode.IsRoot ) throw new ArgumentException( "Node is not a Root" );
			if ( newINode.IsTree ) throw new ArgumentException( "Node is a tree" );

			IncrementVersion();

			OnInserting( this, NodeTreeInsertOperation.Next, newINode );

			NodeTree<T> newNode = GetNodeTree( newINode );

			newNode._Parent = this._Parent;
			newNode._Previous = this;
			newNode._Next = this._Next;
			this._Next = newNode;

			if ( newNode.Next != null )
			{
				NodeTree<T> Next = GetNodeTree( newNode.Next );
				Next._Previous = newNode;
			}

			OnInserted( this, NodeTreeInsertOperation.Next, newINode );
		}

		protected virtual void InsertChildCore( INode<T> newINode )
		{
			if ( !newINode.IsRoot ) throw new ArgumentException( "Node is not a Root" );
			if ( newINode.IsTree ) throw new ArgumentException( "Node is a tree" );

			IncrementVersion();

			OnInserting( this, NodeTreeInsertOperation.Child, newINode );

			NodeTree<T> newNode = GetNodeTree( newINode );

			newNode._Parent = this;
			newNode._Next = this._Child;
			this._Child = newNode;

			if ( newNode.Next != null )
			{
				NodeTree<T> Next = GetNodeTree( newNode.Next );
				Next._Previous = newNode;
			}

			OnInserted( this, NodeTreeInsertOperation.Child, newINode );
		}

		protected virtual void AddCore( INode<T> newINode )
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );

			NodeTree<T> lastNode = GetNodeTree( Last );

			lastNode.InsertNextCore( newINode );
		}

		protected virtual void AddChildCore( INode<T> newINode )
		{
			if ( this.Child == null )
				this.InsertChildCore( newINode );
			else
			{
				NodeTree<T> childNode = GetNodeTree( Child );

				childNode.AddCore( newINode );
			}
		}

		//-----------------------------------------------------------------------------

		public ITree<T> Cut( T o )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			INode<T> n = this[ o ];
			if ( n == null ) return null;
			return n.Cut();
		}

		public ITree<T> Copy( T o )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			INode<T> n = this[ o ];
			if ( n == null ) return null;
			return n.Copy();
		}

		public ITree<T> DeepCopy( T o )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			INode<T> n = this[ o ];
			if ( n == null ) return null;
			return n.DeepCopy();
		}

		public bool Remove( T o )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			INode<T> n = this[ o ];
			if ( n == null ) return false;

			n.Remove();
			return true;
		}

		//-----------------------------------------------------------------------------

		private NodeTree<T> BoxInTree( NodeTree<T> node )
		{
			if ( !node.IsRoot ) throw new ArgumentException( "Node is not a Root" );
			if ( node.IsTree ) throw new ArgumentException( "Node is a tree" );

			NodeTree<T> tree = CreateTree();

			tree.AddChildCore( node );

			return tree;
		}

		public ITree<T> Cut()
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			NodeTree<T> node = CutCore();

			return BoxInTree( node );
		}

		public ITree<T> Copy()
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( IsTree )
			{
				NodeTree<T> NewTree = CopyCore();

				return NewTree;
			}
			else
			{
				NodeTree<T> NewNode = CopyCore();

				return BoxInTree( NewNode );
			}
		}

		public ITree<T> DeepCopy()
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( IsTree )
			{
				NodeTree<T> NewTree = DeepCopyCore();

				return NewTree;
			}
			else
			{
				NodeTree<T> NewNode = DeepCopyCore();

				return BoxInTree( NewNode );
			}
		}

		public void Remove()
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			RemoveCore();
		}

		//-----------------------------------------------------------------------------

		protected virtual NodeTree<T> CutCore()
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );

			IncrementVersion();

			OnCutting( this );

			INode<T> OldRoot = Root;

			if ( this._Next != null )
				this._Next._Previous = this._Previous;

			if ( this.Previous != null )
				this._Previous._Next = this._Next;
			else // this is a first node
			{
				Debug.Assert( Parent.Child == this );
				this._Parent._Child = this._Next;
				Debug.Assert( this.Next == null || this.Next.Previous == null );
			}

			this._Parent = null;
			this._Previous = null;
			this._Next = null;

			OnCutDone( OldRoot, this );

			return this;
		}

		protected virtual NodeTree<T> CopyCore()
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );
			if ( IsRoot && !IsTree ) throw new InvalidOperationException( "This is a Root" );

			if ( IsTree )
			{
				NodeTree<T> NewTree = CreateTree();

				OnCopying( this, NewTree );

				CopyChildNodes( this, NewTree, false );

				OnCopied( this, NewTree );

				return NewTree;
			}
			else
			{
				NodeTree<T> NewNode = CreateNode();

				NewNode._Data = Data;

				OnCopying( this, NewNode );

				CopyChildNodes( this, NewNode, false );

				OnCopied( this, NewNode );

				return NewNode;
			}
		}

		protected virtual NodeTree<T> DeepCopyCore()
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );
			if ( IsRoot && !IsTree ) throw new InvalidOperationException( "This is a Root" );

			if ( IsTree )
			{
				NodeTree<T> NewTree = CreateTree();

				OnCopying( this, NewTree );

				CopyChildNodes( this, NewTree, true );

				OnCopied( this, NewTree );

				return NewTree;
			}
			else
			{
				NodeTree<T> NewNode = CreateNode();

				NewNode._Data = DeepCopyData( Data );

				OnDeepCopying( this, NewNode );

				CopyChildNodes( this, NewNode, true );

				OnDeepCopied( this, NewNode );

				return NewNode;
			}
		}

		private void CopyChildNodes( INode<T> oldNode, NodeTree<T> newNode, bool bDeepCopy )
		{
			NodeTree<T> previousNewChildNode = null;

			for ( INode<T> oldChildNode = oldNode.Child ; oldChildNode != null ; oldChildNode = oldChildNode.Next )
			{
				NodeTree<T> newChildNode = CreateNode();

				if ( !bDeepCopy )
					newChildNode._Data = oldChildNode.Data;
				else
					newChildNode._Data = DeepCopyData( oldChildNode.Data );

				//				if ( ! bDeepCopy )
				//					OnCopying( oldChildNode, newChildNode );
				//				else
				//					OnDeepCopying( oldChildNode, newChildNode );

				if ( oldChildNode.Previous == null ) newNode._Child = newChildNode;

				newChildNode._Parent = newNode;
				newChildNode._Previous = previousNewChildNode;
				if ( previousNewChildNode != null ) previousNewChildNode._Next = newChildNode;

				//				if ( ! bDeepCopy )
				//					OnCopied( oldChildNode, newChildNode );
				//				else
				//					OnDeepCopied( oldChildNode, newChildNode );

				CopyChildNodes( oldChildNode, newChildNode, bDeepCopy );

				previousNewChildNode = newChildNode;
			}
		}


		protected virtual void RemoveCore()
		{
			if ( this.IsRoot ) throw new InvalidOperationException( "This is a Root" );

			CutCore();
		}

		//-----------------------------------------------------------------------------

		public bool CanMoveToParent
		{
			get
			{
				if ( this.IsRoot ) return false;
				if ( this.IsTop ) return false;

				return true;
			}
		}

		public bool CanMoveToPrevious
		{
			get
			{
				if ( this.IsRoot ) return false;
				if ( this.IsFirst ) return false;

				return true;
			}
		}

		public bool CanMoveToNext
		{
			get
			{
				if ( this.IsRoot ) return false;
				if ( this.IsLast ) return false;

				return true;
			}
		}

		public bool CanMoveToChild
		{
			get
			{
				if ( this.IsRoot ) return false;
				if ( this.IsFirst ) return false;

				return true;
			}
		}

		public bool CanMoveToFirst
		{
			get
			{
				if ( this.IsRoot ) return false;
				if ( this.IsFirst ) return false;

				return true;
			}
		}

		public bool CanMoveToLast
		{
			get
			{
				if ( this.IsRoot ) return false;
				if ( this.IsLast ) return false;

				return true;
			}
		}

		//-----------------------------------------------------------------------------

		public void MoveToParent()
		{
			if ( !CanMoveToParent ) throw new InvalidOperationException( "Cannot move to Parent" );

			NodeTree<T> parentNode = GetNodeTree( this.Parent );

			NodeTree<T> thisNode = this.CutCore();

			parentNode.InsertNextCore( thisNode );
		}

		public void MoveToPrevious()
		{
			if ( !CanMoveToPrevious ) throw new InvalidOperationException( "Cannot move to Previous" );

			NodeTree<T> previousNode = GetNodeTree( this.Previous );

			NodeTree<T> thisNode = this.CutCore();

			previousNode.InsertPreviousCore( thisNode );
		}

		public void MoveToNext()
		{
			if ( !CanMoveToNext ) throw new InvalidOperationException( "Cannot move to Next" );

			NodeTree<T> nextNode = GetNodeTree( this.Next );

			NodeTree<T> thisNode = this.CutCore();

			nextNode.InsertNextCore( thisNode );
		}

		public void MoveToChild()
		{
			if ( !CanMoveToChild ) throw new InvalidOperationException( "Cannot move to Child" );

			NodeTree<T> previousNode = GetNodeTree( this.Previous );

			NodeTree<T> thisNode = this.CutCore();

			previousNode.AddChildCore( thisNode );
		}

		public void MoveToFirst()
		{
			if ( !CanMoveToFirst ) throw new InvalidOperationException( "Cannot move to first" );

			NodeTree<T> firstNode = GetNodeTree( this.First );

			NodeTree<T> thisNode = this.CutCore();

			firstNode.InsertPreviousCore( thisNode );
		}


		public void MoveToLast()
		{
			if ( !CanMoveToLast ) throw new InvalidOperationException( "Cannot move to last" );

			NodeTree<T> lastNode = GetNodeTree( this.Last );

			NodeTree<T> thisNode = this.CutCore();

			lastNode.InsertNextCore( thisNode );
		}

		//-----------------------------------------------------------------------------
		// Enumerators

		public virtual IEnumerableCollection<INode<T>> Nodes
		{
			get
			{
				return All.Nodes;
			}
		}

		public virtual IEnumerableCollection<T> Values
		{
			get
			{
				return All.Values;
			}
		}

		//-----------------------------------------------------------------------------
		// BaseEnumerableCollectionPair

		protected abstract class BaseEnumerableCollectionPair : IEnumerableCollectionPair<T>
		{
			private NodeTree<T> _Root = null;

			protected NodeTree<T> Root
			{
				get { return _Root; }
				set { _Root = value; }
			}

			protected BaseEnumerableCollectionPair( NodeTree<T> root )
			{
				_Root = root;
			}

			// Nodes
			public abstract IEnumerableCollection<INode<T>> Nodes { get; }

			protected abstract class BaseNodesEnumerableCollection : IEnumerableCollection<INode<T>>, IEnumerator<INode<T>>
			{
				private int _Version = 0;
				private object _SyncRoot = new object();

				private NodeTree<T> _Root = null;
				protected NodeTree<T> Root
				{
					get { return _Root; }
					set { _Root = value; }
				}

				private INode<T> _CurrentNode = null;
				protected INode<T> CurrentNode
				{
					get { return _CurrentNode; }
					set { _CurrentNode = value; }
				}

				private bool _BeforeFirst = true;
				protected bool BeforeFirst
				{
					get { return _BeforeFirst; }
					set { _BeforeFirst = value; }
				}

				private bool _AfterLast = false;
				protected bool AfterLast
				{
					get { return _AfterLast; }
					set { _AfterLast = value; }
				}

				protected BaseNodesEnumerableCollection( NodeTree<T> root )
				{
					_Root = root;
					_CurrentNode = root;

					_Version = _Root.Version;
				}

				~BaseNodesEnumerableCollection()
				{
					Dispose( false );
				}

				protected abstract BaseNodesEnumerableCollection CreateCopy();

				protected virtual bool HasChanged { get { return _Root.HasChanged( _Version ); } }

				// IDisposable
				public void Dispose()
				{
					Dispose( true );

					GC.SuppressFinalize( this );
				}

				protected virtual void Dispose( bool disposing )
				{
				}

				// IEnumerable
				IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

				// IEnumerable<INode<T>>
				public virtual IEnumerator<INode<T>> GetEnumerator()
				{
					Reset();

					return this;
				}

				// ICollection
				public virtual int Count
				{
					get
					{
						BaseNodesEnumerableCollection e = CreateCopy();

						int i = 0;
						foreach ( INode<T> o in e ) i++;
						return i;
					}
				}

				public virtual bool IsSynchronized { get { return false; } }

				public virtual object SyncRoot { get { return _SyncRoot; } }

				void ICollection.CopyTo( Array array, int index )
				{
					if ( array == null ) throw new ArgumentNullException( "array" );
					if ( array.Rank > 1 ) throw new ArgumentException( "array is multidimensional", "array" );
					if ( index < 0 ) throw new ArgumentOutOfRangeException( "index" );

					int count = Count;

					if ( count > 0 )
						if ( index >= array.Length ) throw new ArgumentException( "index is out of bounds", "index" );

					if ( index + count > array.Length ) throw new ArgumentException( "Not enough space in array", "array" );

					BaseNodesEnumerableCollection e = CreateCopy();

					foreach ( INode<T> n in e )
						array.SetValue( n, index++ );
				}

				public virtual void CopyTo( T[] array, int index )
				{
					( ( ICollection ) this ).CopyTo( array, index );
				}

				// ICollectionEnumerable<INode<T>>
				public virtual bool Contains( INode<T> item )
				{
					BaseNodesEnumerableCollection e = CreateCopy();

					IEqualityComparer<INode<T>> comparer = EqualityComparer<INode<T>>.Default;

					foreach ( INode<T> n in e )
						if ( comparer.Equals( n, item ) )
							return true;

					return false;
				}

				// IEnumerator
				object IEnumerator.Current
				{
					get { return Current; }
				}

				// IEnumerator<INode<T>>
				public virtual void Reset()
				{
					if ( HasChanged ) throw new InvalidOperationException( "Tree has been modified." );

					_CurrentNode = _Root;

					_BeforeFirst = true;
					_AfterLast = false;
				}

				public virtual bool MoveNext()
				{
					if ( HasChanged ) throw new InvalidOperationException( "Tree has been modified." );

					_BeforeFirst = false;

					return true;
				}

				public virtual INode<T> Current
				{
					get
					{
						if ( _BeforeFirst ) throw new InvalidOperationException( "Enumeration has not started." );
						if ( _AfterLast ) throw new InvalidOperationException( "Enumeration has finished." );

						return _CurrentNode;
					}
				}

			}

			// Find
			public virtual INode<T> Find( T data )
			{
				IEqualityComparer<T> comparer = _Root.DataComparer;

				INode<T> retval = null;

				foreach ( INode<T> node in this.Nodes )
					if ( comparer.Equals( node.Data, data ) )
						if ( retval != null )
						{
							Debug.Assert( false, "Multiple matches" );
							return null;
						}
						else retval = node;

				return retval;
			}

			public virtual INode<T> Find( Predicate<T> predicate )
			{
				INode<T> retval = null;

				foreach ( INode<T> node in this.Nodes )
					if ( predicate( node.Data ) )
						if ( retval != null )
						{
							Debug.Assert( false, "Multiple matches" );
							return null;
						}
						else retval = node;

				return retval;
			}


			// Values
			public virtual IEnumerableCollection<T> Values
			{
				get
				{
					return new ValuesEnumerableCollection( _Root.DataComparer, this.Nodes );
				}
			}

			private class ValuesEnumerableCollection : IEnumerableCollection<T>, IEnumerator<T>
			{
				IEqualityComparer<T> _DataComparer;
				private IEnumerableCollection<INode<T>> _Nodes;
				private IEnumerator<INode<T>> _Enumerator;

				public ValuesEnumerableCollection( IEqualityComparer<T> dataComparer, IEnumerableCollection<INode<T>> nodes )
				{
					_DataComparer = dataComparer;
					_Nodes = nodes;
					_Enumerator = _Nodes.GetEnumerator();
				}

				protected ValuesEnumerableCollection( ValuesEnumerableCollection o )
				{
					_Nodes = o._Nodes;
					_Enumerator = _Nodes.GetEnumerator();
				}

				protected virtual ValuesEnumerableCollection CreateCopy()
				{
					return new ValuesEnumerableCollection( this );
				}

				// IDisposable
				~ValuesEnumerableCollection()
				{
					Dispose( false );
				}

				public void Dispose()
				{
					Dispose( true );

					GC.SuppressFinalize( this );
				}

				protected virtual void Dispose( bool disposing )
				{
				}

				// IEnumerable
				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}

				// IEnumerable<T>
				public virtual IEnumerator<T> GetEnumerator()
				{
					Reset();

					return this;
				}

				// ICollection
				public virtual int Count
				{
					get
					{
						return _Nodes.Count;
					}
				}

				public virtual bool IsSynchronized { get { return false; } }

				public virtual object SyncRoot { get { return _Nodes.SyncRoot; } }

				public virtual void CopyTo( Array array, int index )
				{
					if ( array == null ) throw new ArgumentNullException( "array" );
					if ( array.Rank > 1 ) throw new ArgumentException( "array is multidimensional", "array" );
					if ( index < 0 ) throw new ArgumentOutOfRangeException( "index" );

					int count = Count;

					if ( count > 0 )
						if ( index >= array.Length ) throw new ArgumentException( "index is out of bounds", "index" );

					if ( index + count > array.Length ) throw new ArgumentException( "Not enough space in array", "array" );

					ValuesEnumerableCollection e = CreateCopy();

					foreach ( T n in e )
						array.SetValue( n, index++ );
				}

				// IEnumerableCollection<T>
				public virtual bool Contains( T item )
				{
					ValuesEnumerableCollection e = CreateCopy();

					foreach ( T n in e )
						if ( _DataComparer.Equals( n, item ) )
							return true;

					return false;
				}

				// IEnumerator
				object IEnumerator.Current
				{
					get { return Current; }
				}

				// IEnumerator<T> Members
				public virtual void Reset()
				{
					_Enumerator.Reset();
				}

				public virtual bool MoveNext()
				{
					return _Enumerator.MoveNext();
				}

				public virtual T Current
				{
					get
					{
						if ( _Enumerator == null ) { Debug.Assert( false ); return default( T ); }
						if ( _Enumerator.Current == null ) { Debug.Assert( false ); return default( T ); }

						return _Enumerator.Current.Data;
					}
				}
			}
		}

		//-----------------------------------------------------------------------------
		// AllEnumerator

		public IEnumerableCollectionPair<T> All { get { return new AllEnumerator( this ); } }

		protected class AllEnumerator : BaseEnumerableCollectionPair
		{
			public AllEnumerator( NodeTree<T> root ) : base( root ) { }

			public override IEnumerableCollection<INode<T>> Nodes
			{
				get
				{
					return new NodesEnumerableCollection( Root );
				}
			}

			protected class NodesEnumerableCollection : BaseNodesEnumerableCollection
			{
				private bool _First = true;

				public NodesEnumerableCollection( NodeTree<T> root ) : base( root ) { }

				protected NodesEnumerableCollection( NodesEnumerableCollection o ) : base( o.Root ) { }

				protected override BaseNodesEnumerableCollection CreateCopy()
				{
					return new NodesEnumerableCollection( this );
				}

				public override void Reset()
				{
					base.Reset();

					_First = true;
				}

				public override bool MoveNext()
				{
					if ( !base.MoveNext() ) goto hell;

					if ( CurrentNode == null ) throw new InvalidOperationException( "Current is null" );

					if ( CurrentNode.IsRoot )
					{
						CurrentNode = CurrentNode.Child;
						if ( CurrentNode == null ) goto hell;
					}

					if ( _First ) { _First = false; return true; }

					if ( CurrentNode.Child != null ) { CurrentNode = CurrentNode.Child; return true; }

					for ( ; CurrentNode.Parent != null ; CurrentNode = CurrentNode.Parent )
					{
						if ( CurrentNode == Root ) goto hell;
						if ( CurrentNode.Next != null ) { CurrentNode = CurrentNode.Next; return true; }
					}

				hell:

					AfterLast = true;
					return false;
				}
			}

		}

		//-----------------------------------------------------------------------------
		// AllChildrenEnumerator

		public IEnumerableCollectionPair<T> AllChildren { get { return new AllChildrenEnumerator( this ); } }

		private class AllChildrenEnumerator : BaseEnumerableCollectionPair
		{
			public AllChildrenEnumerator( NodeTree<T> root ) : base( root ) { }

			public override IEnumerableCollection<INode<T>> Nodes
			{
				get
				{
					return new NodesEnumerableCollection( Root );
				}
			}

			protected class NodesEnumerableCollection : BaseNodesEnumerableCollection
			{
				public NodesEnumerableCollection( NodeTree<T> root ) : base( root ) { }

				protected NodesEnumerableCollection( NodesEnumerableCollection o ) : base( o.Root ) { }

				protected override BaseNodesEnumerableCollection CreateCopy()
				{
					return new NodesEnumerableCollection( this );
				}

				public override bool MoveNext()
				{
					if ( !base.MoveNext() ) goto hell;

					if ( CurrentNode == null ) throw new InvalidOperationException( "Current is null" );

					if ( CurrentNode.Child != null ) { CurrentNode = CurrentNode.Child; return true; }

					for ( ; CurrentNode.Parent != null ; CurrentNode = CurrentNode.Parent )
					{
						if ( CurrentNode == Root ) goto hell;
						if ( CurrentNode.Next != null ) { CurrentNode = CurrentNode.Next; return true; }
					}

				hell:

					AfterLast = true;
					return false;
				}
			}
		}

		//-----------------------------------------------------------------------------
		// DirectChildrenEnumerator

		public IEnumerableCollectionPair<T> DirectChildren { get { return new DirectChildrenEnumerator( this ); } }

		private class DirectChildrenEnumerator : BaseEnumerableCollectionPair
		{
			public DirectChildrenEnumerator( NodeTree<T> root ) : base( root ) { }

			public override IEnumerableCollection<INode<T>> Nodes
			{
				get
				{
					return new NodesEnumerableCollection( Root );
				}
			}

			protected class NodesEnumerableCollection : BaseNodesEnumerableCollection
			{
				public NodesEnumerableCollection( NodeTree<T> root ) : base( root ) { }

				protected NodesEnumerableCollection( NodesEnumerableCollection o ) : base( o.Root ) { }

				protected override BaseNodesEnumerableCollection CreateCopy()
				{
					return new NodesEnumerableCollection( this );
				}

				public override int Count
				{
					get
					{
						return Root.DirectChildCount;
					}
				}

				public override bool MoveNext()
				{
					if ( !base.MoveNext() ) goto hell;

					if ( CurrentNode == null ) throw new InvalidOperationException( "Current is null" );

					if ( CurrentNode == Root )
						CurrentNode = Root.Child;
					else
						CurrentNode = CurrentNode.Next;

					if ( CurrentNode != null ) return true;

				hell:

					AfterLast = true;
					return false;
				}
			}
		}

		//-----------------------------------------------------------------------------
		// DirectChildrenInReverseEnumerator

		public IEnumerableCollectionPair<T> DirectChildrenInReverse { get { return new DirectChildrenInReverseEnumerator( this ); } }

		private class DirectChildrenInReverseEnumerator : BaseEnumerableCollectionPair
		{
			public DirectChildrenInReverseEnumerator( NodeTree<T> root ) : base( root ) { }

			public override IEnumerableCollection<INode<T>> Nodes
			{
				get
				{
					return new NodesEnumerableCollection( Root );
				}
			}

			protected class NodesEnumerableCollection : BaseNodesEnumerableCollection
			{
				public NodesEnumerableCollection( NodeTree<T> root ) : base( root ) { }

				protected NodesEnumerableCollection( NodesEnumerableCollection o ) : base( o.Root ) { }

				protected override BaseNodesEnumerableCollection CreateCopy()
				{
					return new NodesEnumerableCollection( this );
				}

				public override int Count
				{
					get
					{
						return Root.DirectChildCount;
					}
				}

				public override bool MoveNext()
				{
					if ( !base.MoveNext() ) goto hell;

					if ( CurrentNode == null ) throw new InvalidOperationException( "Current is null" );

					if ( CurrentNode == Root )
						CurrentNode = Root.LastChild;
					else
						CurrentNode = CurrentNode.Previous;

					if ( CurrentNode != null ) return true;

				hell:

					AfterLast = true;
					return false;
				}
			}
		}

		//-----------------------------------------------------------------------------
		// DirectChildCount

		public int DirectChildCount
		{
			get
			{
				int i = 0;

				for ( INode<T> n = this.Child ; n != null ; n = n.Next ) i++;

				return i;
			}
		}

		//-----------------------------------------------------------------------------
		// ITree<T>

		public virtual Type DataType
		{
			get
			{
				return typeof( T );
			}
		}

		public void Clear()
		{
			if ( !this.IsRoot ) throw new InvalidOperationException( "This is not a Root" );
			if ( !this.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			OnClearing( this );

			_Child = null;

			OnCleared( this );
		}

		//-----------------------------------------------------------------------------
		// GetNodeTree

		protected static NodeTree<T> GetNodeTree( ITree<T> tree )
		{
			if ( tree == null ) throw new ArgumentNullException( "Tree is null" );

			return ( NodeTree<T> ) tree; // can throw an InvalidCastException.
		}

		protected static NodeTree<T> GetNodeTree( INode<T> node )
		{
			if ( node == null ) throw new ArgumentNullException( "Node is null" );

			return ( NodeTree<T> ) node; // can throw an InvalidCastException.
		}

		//-----------------------------------------------------------------------------
		// ICollection

		//		public virtual bool IsSynchronized { get { return false; } } // Not thread safe

		//		public virtual T SyncRoot { get { return this; } } // Not sure about this!

		public virtual int Count
		{
			get
			{
				int i = IsRoot ? 0 : 1;

				for ( INode<T> n = this.Child ; n != null ; n = n.Next )
					i += n.Count;

				return i;
			}
		}

		public virtual bool IsReadOnly { get { return false; } }

		//-----------------------------------------------------------------------------
		// Sorting

		public virtual void SortAllChildren()
		{
			foreach ( INode<T> node in All.Nodes ) node.SortDirectChildren();
		}

		public virtual void SortAllChildren( Comparison<T> comparison )
		{
			foreach ( INode<T> node in All.Nodes ) node.SortDirectChildren( comparison );
		}

		public virtual void SortAllChildren( IComparer<T> comparer )
		{
			foreach ( INode<T> node in All.Nodes ) node.SortDirectChildren( comparer );
		}

		public virtual void SortDirectChildren()
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( DirectChildCount < 2 ) return;

			List<INode<T>> children = new List<INode<T>>( DirectChildren.Nodes );

			children.Sort();

			SortDirectChildrenCore( children );
		}

		public virtual void SortDirectChildren( Comparison<T> comparison )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( DirectChildCount < 2 ) return;

			List<INode<T>> children = new List<INode<T>>( DirectChildren.Nodes );

			children.Sort( ( x, y ) => comparison( x.Data, y.Data ) );

			SortDirectChildrenCore( children );
		}

		public virtual void SortDirectChildren( IComparer<T> comparer )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );

			if ( DirectChildCount < 2 ) return;

			List<INode<T>> children = new List<INode<T>>( DirectChildren.Nodes );

			NodeComparer<T> nodeComparer = new NodeComparer<T>( comparer );

			children.Sort( nodeComparer );

			SortDirectChildrenCore( children );
		}

		class NodeComparer<T> : IComparer<INode<T>>
		{
			IComparer<T> _DataComparer = null;

			public NodeComparer( IComparer<T> dataComparer )
			{
				_DataComparer = dataComparer;
			}

			public int Compare( INode<T> x, INode<T> y )
			{
				return _DataComparer.Compare( x.Data, y.Data );
			}
		}

		void SortDirectChildrenCore( List<INode<T>> children )
		{
			_Child = null;

			NodeTree<T> previous = null;
			foreach ( INode<T> iChild in children )
			{
				NodeTree<T> child = GetNodeTree( iChild );

				child._Parent = this;

				child._Previous = null;
				child._Next = null;

				if ( _Child == null )
				{
					_Child = child;
				}
				else
				{
					previous._Next = child;
					child._Previous = previous;
				}

				previous = child;
			}
		}

		//-----------------------------------------------------------------------------
		// Events

		private EventHandlerList _EventHandlerList;

		protected EventHandlerList EventHandlerList
		{
			get { return _EventHandlerList; }
			//set { _EventHandlerList = value; }
		}

		protected EventHandlerList GetCreateEventHandlerList()
		{
			if ( _EventHandlerList == null ) _EventHandlerList = new EventHandlerList();

			return _EventHandlerList;
		}

		private static readonly object _ValidateEventKey = new object();
		private static readonly object _ClearingEventKey = new object();
		private static readonly object _ClearedEventKey = new object();
		private static readonly object _SettingEventKey = new object();
		private static readonly object _SetDoneEventKey = new object();
		private static readonly object _InsertingEventKey = new object();
		private static readonly object _InsertedEventKey = new object();
		private static readonly object _CuttingEventKey = new object();
		private static readonly object _CutDoneEventKey = new object();
		private static readonly object _CopyingEventKey = new object();
		private static readonly object _CopiedEventKey = new object();
		private static readonly object _DeepCopyingEventKey = new object();
		private static readonly object _DeepCopiedEventKey = new object();

		protected static object ValidateEventKey { get { return _ValidateEventKey; } }
		protected static object ClearingEventKey { get { return _ClearingEventKey; } }
		protected static object ClearedEventKey { get { return _ClearedEventKey; } }
		protected static object SettingEventKey { get { return _SettingEventKey; } }
		protected static object SetDoneEventKey { get { return _SetDoneEventKey; } }
		protected static object InsertingEventKey { get { return _InsertingEventKey; } }
		protected static object InsertedEventKey { get { return _InsertedEventKey; } }
		protected static object CuttingEventKey { get { return _CuttingEventKey; } }
		protected static object CutDoneEventKey { get { return _CutDoneEventKey; } }
		protected static object CopyingEventKey { get { return _CopyingEventKey; } }
		protected static object CopiedEventKey { get { return _CopiedEventKey; } }
		protected static object DeepCopyingEventKey { get { return _DeepCopyingEventKey; } }
		protected static object DeepCopiedEventKey { get { return _DeepCopiedEventKey; } }

		public event EventHandler<NodeTreeDataEventArgs<T>> Validate
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( ValidateEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( ValidateEventKey, value );
			}
		}

		public event EventHandler Clearing
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( ClearingEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( ClearingEventKey, value );
			}
		}

		public event EventHandler Cleared
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( ClearedEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( ClearedEventKey, value );
			}
		}

		public event EventHandler<NodeTreeDataEventArgs<T>> Setting
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( SettingEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( SettingEventKey, value );
			}
		}

		public event EventHandler<NodeTreeDataEventArgs<T>> SetDone
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( SetDoneEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( SetDoneEventKey, value );
			}
		}

		public event EventHandler<NodeTreeInsertEventArgs<T>> Inserting
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( InsertingEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( InsertingEventKey, value );
			}
		}

		public event EventHandler<NodeTreeInsertEventArgs<T>> Inserted
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( InsertedEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( InsertedEventKey, value );
			}
		}

		public event EventHandler Cutting
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( CuttingEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( CuttingEventKey, value );
			}
		}

		public event EventHandler CutDone
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( CutDoneEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( CutDoneEventKey, value );
			}
		}

		public event EventHandler<NodeTreeNodeEventArgs<T>> Copying
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( CopyingEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( CopyingEventKey, value );
			}
		}

		public event EventHandler<NodeTreeNodeEventArgs<T>> Copied
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( CopiedEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( CopiedEventKey, value );
			}
		}

		public event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopying
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( DeepCopyingEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( DeepCopyingEventKey, value );
			}
		}

		public event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopied
		{
			add
			{
				GetCreateEventHandlerList().AddHandler( DeepCopiedEventKey, value );
			}

			remove
			{
				GetCreateEventHandlerList().RemoveHandler( DeepCopiedEventKey, value );
			}
		}


		//-----------------------------------------------------------------------------
		// Validate

		protected virtual void OnValidate( INode<T> node, T data )
		{
			if ( !Root.IsTree ) throw new InvalidOperationException( "This is not a tree" );
			if ( data is INode<T> ) throw new ArgumentException( "Object is a node" );

			if ( ( !typeof( T ).IsClass ) || ( ( object ) data ) != null )
				if ( !DataType.IsInstanceOfType( data ) )
					throw new ArgumentException( "Object is not a " + DataType.Name );

			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeDataEventArgs<T>> e = ( EventHandler<NodeTreeDataEventArgs<T>> ) _EventHandlerList[ ValidateEventKey ];
				if ( e != null ) e( node, new NodeTreeDataEventArgs<T>( data ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnValidate( node, data );
		}

		//-----------------------------------------------------------------------------
		// Clear

		protected virtual void OnClearing( ITree<T> tree )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler e = ( EventHandler ) _EventHandlerList[ ClearingEventKey ];
				if ( e != null ) e( tree, EventArgs.Empty );
			}
		}

		protected virtual void OnCleared( ITree<T> tree )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler e = ( EventHandler ) _EventHandlerList[ ClearedEventKey ];
				if ( e != null ) e( tree, EventArgs.Empty );
			}
		}

		//-----------------------------------------------------------------------------
		// Set

		protected virtual void OnSetting( INode<T> node, T data )
		{
			OnSettingCore( node, data, true );
		}

		protected virtual void OnSettingCore( INode<T> node, T data, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeDataEventArgs<T>> e = ( EventHandler<NodeTreeDataEventArgs<T>> ) _EventHandlerList[ SettingEventKey ];
				if ( e != null ) e( node, new NodeTreeDataEventArgs<T>( data ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnSettingCore( node, data, false );

			if ( raiseValidate ) OnValidate( node, data );
		}

		protected virtual void OnSetDone( INode<T> node, T data )
		{
			OnSetDoneCore( node, data, true );
		}

		protected virtual void OnSetDoneCore( INode<T> node, T data, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeDataEventArgs<T>> e = ( EventHandler<NodeTreeDataEventArgs<T>> ) _EventHandlerList[ SetDoneEventKey ];
				if ( e != null ) e( node, new NodeTreeDataEventArgs<T>( data ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnSetDoneCore( node, data, false );

			//			if ( raiseValidate ) OnValidate( Node, Data );
		}

		//-----------------------------------------------------------------------------
		// Insert

		protected virtual void OnInserting( INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode )
		{
			OnInsertingCore( oldNode, operation, newNode, true );
		}

		protected virtual void OnInsertingCore( INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeInsertEventArgs<T>> e = ( EventHandler<NodeTreeInsertEventArgs<T>> ) _EventHandlerList[ InsertingEventKey ];
				if ( e != null ) e( oldNode, new NodeTreeInsertEventArgs<T>( operation, newNode ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnInsertingCore( oldNode, operation, newNode, false );

			if ( raiseValidate ) OnValidate( oldNode, newNode.Data );

			if ( raiseValidate ) OnInsertingTree( newNode );
		}

		protected virtual void OnInsertingTree( INode<T> newNode )
		{
			for ( INode<T> child = newNode.Child ; child != null ; child = child.Next )
			{
				OnInsertingTree( newNode, child );

				OnInsertingTree( child );
			}
		}

		protected virtual void OnInsertingTree( INode<T> newNode, INode<T> child )
		{
			OnInsertingTreeCore( newNode, child, true );
		}

		protected virtual void OnInsertingTreeCore( INode<T> newNode, INode<T> child, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeInsertEventArgs<T>> e = ( EventHandler<NodeTreeInsertEventArgs<T>> ) _EventHandlerList[ InsertingEventKey ];
				if ( e != null ) e( newNode, new NodeTreeInsertEventArgs<T>( NodeTreeInsertOperation.Tree, child ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnInsertingTreeCore( newNode, child, false );

			if ( raiseValidate ) OnValidate( newNode, child.Data );
		}

		protected virtual void OnInserted( INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode )
		{
			OnInsertedCore( oldNode, operation, newNode, true );
		}

		protected virtual void OnInsertedCore( INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeInsertEventArgs<T>> e = ( EventHandler<NodeTreeInsertEventArgs<T>> ) _EventHandlerList[ InsertedEventKey ];
				if ( e != null ) e( oldNode, new NodeTreeInsertEventArgs<T>( operation, newNode ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnInsertedCore( oldNode, operation, newNode, false );

			//			if ( raiseValidate ) OnValidate( OldNode, NewNode.Data );

			if ( raiseValidate ) OnInsertedTree( newNode );
		}

		protected virtual void OnInsertedTree( INode<T> newNode )
		{
			for ( INode<T> child = newNode.Child ; child != null ; child = child.Next )
			{
				OnInsertedTree( newNode, child );

				OnInsertedTree( child );
			}
		}

		protected virtual void OnInsertedTree( INode<T> newNode, INode<T> child )
		{
			OnInsertedTreeCore( newNode, child, true );
		}

		protected virtual void OnInsertedTreeCore( INode<T> newNode, INode<T> child, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeInsertEventArgs<T>> e = ( EventHandler<NodeTreeInsertEventArgs<T>> ) _EventHandlerList[ InsertedEventKey ];
				if ( e != null ) e( newNode, new NodeTreeInsertEventArgs<T>( NodeTreeInsertOperation.Tree, child ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnInsertedTreeCore( newNode, child, false );

			//			if ( raiseValidate ) OnValidate( newNode, child.Data );
		}

		//-----------------------------------------------------------------------------
		// Cut

		protected virtual void OnCutting( INode<T> oldNode )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler e = ( EventHandler ) _EventHandlerList[ CuttingEventKey ];
				if ( e != null ) e( oldNode, EventArgs.Empty );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnCutting( oldNode );
		}

		protected virtual void OnCutDone( INode<T> oldRoot, INode<T> oldNode )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler e = ( EventHandler ) _EventHandlerList[ CutDoneEventKey ];
				if ( e != null ) e( oldNode, EventArgs.Empty );
			}

			if ( !IsTree ) GetNodeTree( oldRoot ).OnCutDone( oldRoot, oldNode );
		}

		//-----------------------------------------------------------------------------
		// Copy

		protected virtual void OnCopying( INode<T> oldNode, INode<T> newNode )
		{
			OnCopyingCore( oldNode, newNode, true );
		}

		protected virtual void OnCopyingCore( INode<T> oldNode, INode<T> newNode, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeNodeEventArgs<T>> e = ( EventHandler<NodeTreeNodeEventArgs<T>> ) _EventHandlerList[ CopyingEventKey ];
				if ( e != null ) e( oldNode, new NodeTreeNodeEventArgs<T>( newNode ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnCopyingCore( oldNode, newNode, false );

			//			if ( raiseValidate ) OnValidate( oldNode, newNode.Data );
		}

		protected virtual void OnCopied( INode<T> oldNode, INode<T> newNode )
		{
			OnCopiedCore( oldNode, newNode, true );
		}

		protected virtual void OnCopiedCore( INode<T> oldNode, INode<T> newNode, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeNodeEventArgs<T>> e = ( EventHandler<NodeTreeNodeEventArgs<T>> ) _EventHandlerList[ CopiedEventKey ];
				if ( e != null ) e( oldNode, new NodeTreeNodeEventArgs<T>( newNode ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnCopiedCore( oldNode, newNode, false );

			//			if ( raiseValidate ) OnValidate( oldNode, newNode.Data );
		}

		//-----------------------------------------------------------------------------
		// DeepCopy

		protected virtual void OnDeepCopying( INode<T> oldNode, INode<T> newNode )
		{
			OnDeepCopyingCore( oldNode, newNode, true );
		}

		protected virtual void OnDeepCopyingCore( INode<T> oldNode, INode<T> newNode, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeNodeEventArgs<T>> e = ( EventHandler<NodeTreeNodeEventArgs<T>> ) _EventHandlerList[ DeepCopyingEventKey ];
				if ( e != null ) e( oldNode, new NodeTreeNodeEventArgs<T>( newNode ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnDeepCopyingCore( oldNode, newNode, false );

			//			if ( raiseValidate ) OnValidate( oldNode, newNode.Data );
		}

		protected virtual void OnDeepCopied( INode<T> oldNode, INode<T> newNode )
		{
			OnDeepCopiedCore( oldNode, newNode, true );
		}

		protected virtual void OnDeepCopiedCore( INode<T> oldNode, INode<T> newNode, bool raiseValidate )
		{
			if ( _EventHandlerList != null )
			{
				EventHandler<NodeTreeNodeEventArgs<T>> e = ( EventHandler<NodeTreeNodeEventArgs<T>> ) _EventHandlerList[ DeepCopiedEventKey ];
				if ( e != null ) e( oldNode, new NodeTreeNodeEventArgs<T>( newNode ) );
			}

			if ( !IsRoot ) GetNodeTree( Root ).OnDeepCopiedCore( oldNode, newNode, false );

			//			if ( raiseValidate ) OnValidate( oldNode, newNode.Data );
		}

		//-----------------------------------------------------------------------------

	} // class NodeTree

	//-----------------------------------------------------------------------------

}



