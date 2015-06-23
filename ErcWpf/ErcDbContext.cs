// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from erc_db on 2015-06-23 09:27:06Z.
// Please visit http://code.google.com/p/dblinq2007/ for more information.
//
namespace RedAlliance.Erc.Sql
{
	using System;
	using System.ComponentModel;
	using System.Data;
#if MONO_STRICT
	using System.Data.Linq;
#else   // MONO_STRICT
	using DbLinq.Data.Linq;
	using DbLinq.Vendor;
#endif  // MONO_STRICT
	using System.Data.Linq.Mapping;
	using System.Diagnostics;
	
	
	public partial class ErcDb : DataContext
	{
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		#endregion
		
		
		public ErcDb(string connectionString) : 
				base(connectionString)
		{
			this.OnCreated();
		}
		
		public ErcDb(string connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public ErcDb(IDbConnection connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public Table<Code> Code
		{
			get
			{
				return this.GetTable<Code>();
			}
		}
	}
	
	#region Start MONO_STRICT
#if MONO_STRICT

	public partial class ErcDb
	{
		
		public ErcDb(IDbConnection connection) : 
				base(connection)
		{
			this.OnCreated();
		}
	}
	#region End MONO_STRICT
	#endregion
#else     // MONO_STRICT
	
	public partial class ErcDb
	{
		
		public ErcDb(IDbConnection connection) : 
				base(connection, new DbLinq.SqlCe.SqlCeVendor())
		{
			this.OnCreated();
		}
		
		public ErcDb(IDbConnection connection, IVendor sqlDialect) : 
				base(connection, sqlDialect)
		{
			this.OnCreated();
		}
		
		public ErcDb(IDbConnection connection, MappingSource mappingSource, IVendor sqlDialect) : 
				base(connection, mappingSource, sqlDialect)
		{
			this.OnCreated();
		}
	}
	#region End Not MONO_STRICT
	#endregion
#endif     // MONO_STRICT
	#endregion
	
	[Table(Name="code")]
	public partial class Code : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _inputSymbol;
		
		private string _outputSymbol;
		
		private int _position;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnInputSymbolChanged();
		
		partial void OnInputSymbolChanging(string value);
		
		partial void OnOutputSymbolChanged();
		
		partial void OnOutputSymbolChanging(string value);
		
		partial void OnPositionChanged();
		
		partial void OnPositionChanging(int value);
		#endregion
		
		
		public Code()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_inputSymbol", Name="input_symbol", DbType="nvarchar", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string InputSymbol
		{
			get
			{
				return this._inputSymbol;
			}
			set
			{
				if (((_inputSymbol == value) 
							== false))
				{
					this.OnInputSymbolChanging(value);
					this.SendPropertyChanging();
					this._inputSymbol = value;
					this.SendPropertyChanged("InputSymbol");
					this.OnInputSymbolChanged();
				}
			}
		}
		
		[Column(Storage="_outputSymbol", Name="output_symbol", DbType="nvarchar", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string OutputSymbol
		{
			get
			{
				return this._outputSymbol;
			}
			set
			{
				if (((_outputSymbol == value) 
							== false))
				{
					this.OnOutputSymbolChanging(value);
					this.SendPropertyChanging();
					this._outputSymbol = value;
					this.SendPropertyChanged("OutputSymbol");
					this.OnOutputSymbolChanged();
				}
			}
		}
		
		[Column(Storage="_position", Name="position", DbType="int", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if ((_position != value))
				{
					this.OnPositionChanging(value);
					this.SendPropertyChanging();
					this._position = value;
					this.SendPropertyChanged("Position");
					this.OnPositionChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
