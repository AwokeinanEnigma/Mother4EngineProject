using System;
using System.Collections.Generic;
using fNbt;

namespace Carbine.Flags
{
	public class FlagManager
	{
		public static FlagManager Instance
		{
			get
			{
				if (FlagManager.instance == null)
				{
					FlagManager.instance = new FlagManager();
				}
				return FlagManager.instance;
			}
		}

		public bool this[int flag]
		{
			get
			{
				return this.flags.ContainsKey(flag) && this.flags[flag];
			}
			set
			{
				if (flag > 0)
				{
					if (this.flags.ContainsKey(flag))
					{
						this.flags[flag] = value;
						return;
					}
					this.flags.Add(flag, value);
				}
			}
		}

		private FlagManager()
		{
			this.flags = new Dictionary<int, bool>();
			this.SetInitialState();
		}

		private void SetInitialState()
		{
			this.flags.Add(0, true);
		}

		public void Toggle(int flag)
		{
			if (this.flags.ContainsKey(flag))
			{
				this.flags[flag] = !this.flags[flag];
				return;
			}
			this.flags.Add(flag, true);
		}

		public void Reset()
		{
			this.flags.Clear();
			this.SetInitialState();
		}

		public void LoadFromNBT(NbtIntArray flagTag)
		{
			this.Reset();
			if (flagTag != null)
			{
				foreach (int num in flagTag.IntArrayValue)
				{
					int flag = num >> 1;
					bool value = (num & 1) == 1;
					this[flag] = value;
				}
			}
		}

		public NbtIntArray ToNBT()
		{
			int[] array = new int[this.flags.Count - 1];
			int num = 0;
			foreach (KeyValuePair<int, bool> keyValuePair in this.flags)
			{
				if (keyValuePair.Key > 0)
				{
					array[num++] = (keyValuePair.Key << 1 | (keyValuePair.Value ? 1 : 0));
				}
			}
			return new NbtIntArray("flags", array);
		}

		public const string NBT_TAG_NAME = "flags";

		public const int FLAG_TRUE = 0;

		public const int FLAG_DAY_NIGHT = 1;

		public const int FLAG_QUESTION_REGISTER = 2;

		public const int FLAG_TELEPATHY_REQUEST = 3;

		public const int FLAG_TELEPATHY_MODE = 4;

		private static FlagManager instance;

		private Dictionary<int, bool> flags;
	}
}
