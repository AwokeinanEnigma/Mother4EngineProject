using System;
using System.Collections.Generic;
using fNbt;

namespace Carbine.Flags
{
	public class ValueManager
	{
		public static ValueManager Instance
		{
			get
			{
				if (ValueManager.instance == null)
				{
					ValueManager.instance = new ValueManager();
				}
				return ValueManager.instance;
			}
		}

		public int this[int index]
		{
			get
			{
				if (!this.values.ContainsKey(index))
				{
					return 0;
				}
				return this.values[index];
			}
			set
			{
				if (this.values.ContainsKey(index))
				{
					this.values[index] = value;
					return;
				}
				this.values.Add(index, value);
			}
		}

		private ValueManager()
		{
			this.values = new Dictionary<int, int>();
		}

		public void Reset()
		{
			this.values.Clear();
		}

		public void LoadFromNBT(NbtIntArray valueTag)
		{
			this.values.Clear();
			if (valueTag != null)
			{
				int[] intArrayValue = valueTag.IntArrayValue;
				for (int i = 0; i < intArrayValue.Length; i += 2)
				{
					this[intArrayValue[i]] = intArrayValue[i + 1];
				}
			}
		}

		public NbtTag ToNBT()
		{
			int[] array = new int[this.values.Count * 2];
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.values)
			{
				array[num++] = keyValuePair.Key;
				array[num++] = keyValuePair.Value;
			}
			return new NbtIntArray("vals", array);
		}

		public const string NBT_TAG_NAME = "vals";

		public const int VALUE_ACTION_RETURN = 0;

		public const int VALUE_MONEY = 1;

		private static ValueManager instance;

		private Dictionary<int, int> values;
	}
}
