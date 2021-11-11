using System;
using System.Collections;
using Microsoft.Build.Shared;

namespace Microsoft.Build.BuildEngine
{
	// Token: 0x0200000F RID: 15
	internal sealed class CopyOnWriteHashtable : IDictionary, ICollection, IEnumerable, ICloneable
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00003C38 File Offset: 0x00002C38
		internal CopyOnWriteHashtable(StringComparer stringComparer) : this(0, stringComparer)
		{
			this.sharedLock = new object();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003C50 File Offset: 0x00002C50
		internal CopyOnWriteHashtable(int capacity, StringComparer stringComparer)
		{
			ErrorUtilities.VerifyThrowArgumentNull(stringComparer, "stringComparer");
			this.sharedLock = new object();
			if (capacity == 0)
			{
				this.writeableData = new Hashtable(stringComparer);
			}
			else
			{
				this.writeableData = new Hashtable(capacity, stringComparer);
			}
			this.readonlyData = null;
			this.stringComparer = stringComparer;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003CA8 File Offset: 0x00002CA8
		internal CopyOnWriteHashtable(IDictionary dictionary, StringComparer stringComparer)
		{
			ErrorUtilities.VerifyThrowArgumentNull(dictionary, "dictionary");
			ErrorUtilities.VerifyThrowArgumentNull(stringComparer, "stringComparer");
			this.sharedLock = new object();
			CopyOnWriteHashtable copyOnWriteHashtable = dictionary as CopyOnWriteHashtable;
			if (copyOnWriteHashtable == null)
			{
				this.writeableData = new Hashtable(dictionary, stringComparer);
				this.readonlyData = null;
				this.stringComparer = stringComparer;
				return;
			}
			if (copyOnWriteHashtable.stringComparer.GetHashCode() == stringComparer.GetHashCode())
			{
				this.ConstructFrom(copyOnWriteHashtable);
				return;
			}
			throw new InternalErrorException("Bug: Changing the case-sensitiveness of a copied hash-table.", false);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003D28 File Offset: 0x00002D28
		private CopyOnWriteHashtable(CopyOnWriteHashtable that)
		{
			this.sharedLock = new object();
			this.ConstructFrom(that);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003D44 File Offset: 0x00002D44
		private void ConstructFrom(CopyOnWriteHashtable that)
		{
			lock (that.sharedLock)
			{
				this.writeableData = null;
				if (that.writeableData != null)
				{
					that.readonlyData = that.writeableData;
					that.writeableData = null;
				}
				this.readonlyData = that.readonlyData;
				this.stringComparer = that.stringComparer;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003DB4 File Offset: 0x00002DB4
		internal bool IsShallowCopy
		{
			get
			{
				return this.readonlyData != null;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003DC2 File Offset: 0x00002DC2
		public bool Contains(object key)
		{
			return this.ReadOperation.Contains(key);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003DD0 File Offset: 0x00002DD0
		public void Add(object key, object value)
		{
			this.WriteOperation.Add(key, value);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003DE0 File Offset: 0x00002DE0
		public void Clear()
		{
			lock (this.sharedLock)
			{
				ErrorUtilities.VerifyThrow(this.stringComparer != null, "Should have a valid string comparer.");
				this.writeableData = new Hashtable(this.stringComparer);
				this.readonlyData = null;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003E44 File Offset: 0x00002E44
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.ReadOperation).GetEnumerator();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003E51 File Offset: 0x00002E51
		public IDictionaryEnumerator GetEnumerator()
		{
			return this.ReadOperation.GetEnumerator();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003E5E File Offset: 0x00002E5E
		public void Remove(object key)
		{
			this.WriteOperation.Remove(key);
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003E6C File Offset: 0x00002E6C
		public bool IsFixedSize
		{
			get
			{
				return this.ReadOperation.IsFixedSize;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003E79 File Offset: 0x00002E79
		public bool IsReadOnly
		{
			get
			{
				return this.ReadOperation.IsFixedSize;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00003E86 File Offset: 0x00002E86
		public ICollection Keys
		{
			get
			{
				return this.ReadOperation.Keys;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003E93 File Offset: 0x00002E93
		public ICollection Values
		{
			get
			{
				return this.ReadOperation.Values;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003EA0 File Offset: 0x00002EA0
		public void CopyTo(Array array, int arrayIndex)
		{
			this.ReadOperation.CopyTo(array, arrayIndex);
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003EAF File Offset: 0x00002EAF
		public int Count
		{
			get
			{
				return this.ReadOperation.Count;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00003EBC File Offset: 0x00002EBC
		public bool IsSynchronized
		{
			get
			{
				return this.ReadOperation.IsSynchronized;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003EC9 File Offset: 0x00002EC9
		public object SyncRoot
		{
			get
			{
				return this.ReadOperation.SyncRoot;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003ED6 File Offset: 0x00002ED6
		public bool ContainsKey(object key)
		{
			return this.ReadOperation.Contains(key);
		}

		// Token: 0x1700001E RID: 30
		public object this[object key]
		{
			get
			{
				return this.ReadOperation[key];
			}
			set
			{
				lock (this.sharedLock)
				{
					if (this.writeableData != null)
					{
						this.writeableData[key] = value;
					}
					else if (this.readonlyData[key] != value || !this.readonlyData.ContainsKey(key))
					{
						this.WriteOperation[key] = value;
					}
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003F68 File Offset: 0x00002F68
		public object Clone()
		{
			return new CopyOnWriteHashtable(this);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00003F70 File Offset: 0x00002F70
		private Hashtable ReadOperation
		{
			get
			{
				Hashtable result;
				lock (this.sharedLock)
				{
					if (this.readonlyData != null)
					{
						result = this.readonlyData;
					}
					else
					{
						result = this.writeableData;
					}
				}
				return result;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003FBC File Offset: 0x00002FBC
		private Hashtable WriteOperation
		{
			get
			{
				Hashtable result;
				lock (this.sharedLock)
				{
					if (this.writeableData == null)
					{
						this.writeableData = (Hashtable)this.readonlyData.Clone();
						this.readonlyData = null;
					}
					result = this.writeableData;
				}
				return result;
			}
		}

		// Token: 0x04000023 RID: 35
		private Hashtable writeableData;

		// Token: 0x04000024 RID: 36
		private Hashtable readonlyData;

		// Token: 0x04000025 RID: 37
		private object sharedLock;

		// Token: 0x04000026 RID: 38
		private StringComparer stringComparer;
	}
}
 
