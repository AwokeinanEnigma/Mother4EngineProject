using System;
using System.Collections.Generic;
using Carbine.Graphics;
using Mother4.Battle;
using Mother4.Data;
using Mother4.Data.Character;
using Mother4.Data.Psi;
using Rufini.Strings;
using SFML.Graphics;
using SFML.System;

namespace Mother4.GUI
{
	// Token: 0x02000037 RID: 55
	internal class PsiList : Renderable
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00006E89 File Offset: 0x00005089
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00006E91 File Offset: 0x00005091
		public override Vector2f Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.SetPosition(value);
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00006E9A File Offset: 0x0000509A
		public PsiLevel? SelectedPsiLevel
		{
			get
			{
				return this.GetPsiLevel();
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00006EA2 File Offset: 0x000050A2
		public PsiList.PanelType SelectedPanelType
		{
			get
			{
				if (this.selectedList != this.psiGroupList)
				{
					return PsiList.PanelType.PsiTypePanel;
				}
				return PsiList.PanelType.PsiGroupPanel;
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006EB8 File Offset: 0x000050B8
		public PsiList(Vector2f position, CharacterType character, int width, int rows, int depth)
		{
			this.position = position;
			this.width = width;
			this.rows = rows;
			this.size = new Vector2f((float)this.width, (float)(this.rows * Fonts.Main.LineHeight));
			this.depth = depth;
			this.SetupLists(character);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00006F54 File Offset: 0x00005154
		private Dictionary<Type, List<PsiList.PsiListItem>> CategorizePsi(List<PsiLevel> knownPsi)
		{
			Dictionary<PsiType, List<int>> dictionary = new Dictionary<PsiType, List<int>>();
			foreach (PsiLevel psiLevel in knownPsi)
			{
				List<int> list;
				if (!dictionary.TryGetValue(psiLevel.PsiType, out list))
				{
					list = new List<int>();
					dictionary.Add(psiLevel.PsiType, list);
				}
				list.Add(psiLevel.Level);
			}
			foreach (List<int> list2 in dictionary.Values)
			{
				list2.Sort();
			}
			Dictionary<Type, List<PsiList.PsiListItem>> dictionary2 = new Dictionary<Type, List<PsiList.PsiListItem>>();
			foreach (KeyValuePair<PsiType, List<int>> keyValuePair in dictionary)
			{
				PsiType key = keyValuePair.Key;
				List<int> value = keyValuePair.Value;
				PsiData data = PsiFile.Instance.GetData(key);
				List<PsiList.PsiListItem> list3;
				if (!dictionary2.TryGetValue(data.GetType(), out list3))
				{
					list3 = new List<PsiList.PsiListItem>();
					dictionary2.Add(data.GetType(), list3);
				}
				int[] array = new int[value.Count];
				string[] array2 = new string[value.Count];
				for (int i = 0; i < value.Count; i++)
				{
					array[i] = value[i];
					array2[i] = PsiLetters.Get((int)data.Symbols[value[i]]);
				}
				PsiList.PsiListItem item = new PsiList.PsiListItem
				{
					Label = (StringFile.Instance.Get(data.Key).Value ?? string.Empty),
					Symbols = array2,
					Levels = array,
					PsiData = data
				};
				list3.Add(item);
			}
			foreach (List<PsiList.PsiListItem> list4 in dictionary2.Values)
			{
				list4.Sort(delegate (PsiList.PsiListItem x, PsiList.PsiListItem y)
				{
					int num2 = x.PsiData.Order - y.PsiData.Order;
					if (num2 == 0)
					{
						x.Label.CompareTo(y.Label);
					}
					return num2;
				});
			}
			this.psiLevels = new PsiLevel[PsiList.PSI_GROUP_TYPES.Length][][];
			foreach (KeyValuePair<Type, List<PsiList.PsiListItem>> keyValuePair2 in dictionary2)
			{
				int num = 0;
				for (int j = 0; j < PsiList.PSI_GROUP_TYPES.Length; j++)
				{
					if (keyValuePair2.Key == PsiList.PSI_GROUP_TYPES[j])
					{
						num = j;
						break;
					}
				}
				this.psiLevels[num] = new PsiLevel[keyValuePair2.Value.Count][];
				for (int k = 0; k < keyValuePair2.Value.Count; k++)
				{
					PsiList.PsiListItem psiListItem = keyValuePair2.Value[k];
					this.psiLevels[num][k] = new PsiLevel[psiListItem.Symbols.Length];
					for (int l = 0; l < psiListItem.Symbols.Length; l++)
					{
						this.psiLevels[num][k][l] = new PsiLevel(PsiFile.Instance.GetPsiType(psiListItem.PsiData.QualifiedName), psiListItem.Levels[l]);
					}
				}
			}
			return dictionary2;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00007338 File Offset: 0x00005538
		private void CreatePsiPageLists(int index, Dictionary<Type, List<PsiList.PsiListItem>> psiBuckets)
		{
			List<PsiList.PsiListItem> list;
			string[] array;
			if (psiBuckets.TryGetValue(PsiList.PSI_GROUP_TYPES[index], out list))
			{
				array = new string[list.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (StringFile.Instance.Get(list[i].PsiData.Key).Value ?? string.Empty);
				}
			}
			else
			{
				array = new string[0];
			}
			if (array.Length > 0)
			{
				this.psiLabelList[index] = new ScrollingList(this.psiGroupList.Position + new Vector2f(this.psiGroupList.Size.X + 16f, 0f), this.depth, array, this.rows, (float)Fonts.Main.LineHeight, (float)this.width - this.psiGroupList.Size.X - 16f, Paths.GRAPHICS + "cursor.dat");
				this.psiLabelList[index].ShowSelectionRectangle = false;
				this.psiLabelList[index].UseHighlightTextColor = false;
				this.psiLabelList[index].ShowCursor = false;
				this.psiLabelList[index].Focused = false;
				int num = 0;
				for (int j = 0; j < array.Length; j++)
				{
					num = Math.Max(num, list[j].Symbols.Length);
				}
				this.psiLevelList[index] = new ScrollingList[num];
				using (Text text = new Text())
				{
					text.Font = Fonts.Main.Font;
					text.CharacterSize = Fonts.Main.Size;
					int num2 = 0;
					for (int k = num - 1; k >= 0; k--)
					{
						int num3 = 0;
						string[] array2 = new string[array.Length];
						for (int l = 0; l < array.Length; l++)
						{
							array2[l] = ((list[l].Symbols.Length > k) ? list[l].Symbols[k] : string.Empty);
							text.DisplayedString = array2[l];
							num3 = Math.Max(num3, (int)text.GetLocalBounds().Width + 1);
						}
						this.psiLevelList[index][k] = new ScrollingList(this.psiLabelList[index].Position + new Vector2f(this.psiLabelList[index].Size.X - (float)num2 - (float)num3, 0f), this.depth, array2, this.rows, (float)Fonts.Main.LineHeight, 16f, Paths.GRAPHICS + "cursor.dat");
						this.psiLevelList[index][k].ShowSelectionRectangle = false;
						this.psiLevelList[index][k].UseHighlightTextColor = false;
						this.psiLevelList[index][k].ShowCursor = (k == 0);
						this.psiLevelList[index][k].ShowArrows = false;
						this.psiLevelList[index][k].Focused = false;
						num2 += num3 + 10;
					}
					int num4 = Math.Max(0, (int)this.psiLevelList[index][0].Position.X - ((int)this.psiLabelList[index].Position.X + (int)this.psiLabelList[index].Size.X - 55));
					if (num4 > 0)
					{
						for (int m = 0; m < num; m++)
						{
							this.psiLevelList[index][m].Position -= new Vector2f((float)num4, 0f);
						}
					}
					return;
				}
			}
			this.psiLabelList[index] = null;
			this.psiLevelList[index] = null;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000076F4 File Offset: 0x000058F4
		private void SetupLists(CharacterType character)
		{
			string[] array = new string[PsiList.PSI_GROUP_STRINGS.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StringFile.Instance.Get(PsiList.PSI_GROUP_STRINGS[i]).Value;
			}
			this.psiGroupList = new ScrollingList(this.position, this.depth, array, this.rows, (float)Fonts.Main.LineHeight, 50f, Paths.GRAPHICS + "cursor.dat");
			this.selectedList = this.psiGroupList;
			StatSet stats = CharacterStats.GetStats(character);
			CharacterData data = CharacterFile.Instance.GetData(character);
			List<PsiLevel> knownPsi = data.GetKnownPsi(stats.Level);
			Dictionary<Type, List<PsiList.PsiListItem>> psiBuckets = this.CategorizePsi(knownPsi);
			this.psiLabelList = new ScrollingList[PsiList.PSI_GROUP_STRINGS.Length];
			this.psiLevelList = new ScrollingList[PsiList.PSI_GROUP_STRINGS.Length][];
			for (int j = 0; j < PsiList.PSI_GROUP_TYPES.Length; j++)
			{
				this.CreatePsiPageLists(j, psiBuckets);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000077F4 File Offset: 0x000059F4
		private void SetPosition(Vector2f position)
		{
			Vector2f position2 = this.position;
			this.position = position;
			Vector2f v = this.position - position2;
			this.psiGroupList.Position += v;
			for (int i = 0; i < this.psiLabelList.Length; i++)
			{
				if (this.psiLabelList[i] != null)
				{
					this.psiLabelList[i].Position += v;
				}
				if (this.psiLevelList[i] != null)
				{
					for (int j = 0; j < this.psiLevelList[i].Length; j++)
					{
						if (this.psiLevelList[i][j] != null)
						{
							this.psiLevelList[i][j].Position += v;
						}
					}
				}
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000078AF File Offset: 0x00005AAF
		public void Show()
		{
			this.visible = true;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000078B8 File Offset: 0x00005AB8
		public void Hide()
		{
			this.visible = false;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000078C4 File Offset: 0x00005AC4
		private PsiLevel? GetPsiLevel()
		{
			PsiLevel? result = null;
			int selectedIndex = this.psiGroupList.SelectedIndex;
			if (this.selectedList == this.psiLabelList[selectedIndex])
			{
				int selectedIndex2 = this.psiLabelList[selectedIndex].SelectedIndex;
				result = new PsiLevel?(this.psiLevels[selectedIndex][selectedIndex2][this.selectedLevel]);
			}
			return result;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007925 File Offset: 0x00005B25
		private void ChangeSelectedLevel(int groupIndex, int newLevel)
		{
			this.psiLevelList[groupIndex][this.selectedLevel].ShowCursor = false;
			this.selectedLevel = newLevel;
			this.psiLevelList[groupIndex][this.selectedLevel].ShowCursor = true;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007958 File Offset: 0x00005B58
		public void Reset()
		{
			this.SelectPsiGroupList(this.psiGroupList.SelectedIndex);
			this.psiGroupList.SelectedIndex = 0;
			for (int i = 0; i < this.psiLabelList.Length; i++)
			{
				if (this.psiLabelList[i] != null)
				{
					this.psiLabelList[i].SelectedIndex = 0;
				}
				if (this.psiLevelList[i] != null)
				{
					for (int j = 0; j < this.psiLevelList[i].Length; j++)
					{
						if (this.psiLevelList[i][j] != null)
						{
							this.psiLevelList[i][j].SelectedIndex = 0;
						}
					}
				}
			}
			this.ChangeSelectedLevel(this.psiGroupList.SelectedIndex, 0);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000079FC File Offset: 0x00005BFC
		public void SelectUp()
		{
			if (this.selectedList == this.psiGroupList)
			{
				this.selectedList.SelectPrevious();
				return;
			}
			if (this.selectedList == this.psiLabelList[this.psiGroupList.SelectedIndex])
			{
				int selectedIndex = this.selectedList.SelectedIndex;
				this.selectedList.SelectPrevious();
				if (selectedIndex != this.selectedList.SelectedIndex)
				{
					for (int i = 0; i < this.psiLevelList[this.psiGroupList.SelectedIndex].Length; i++)
					{
						this.psiLevelList[this.psiGroupList.SelectedIndex][i].SelectPrevious();
					}
					while (this.psiLevelList[this.psiGroupList.SelectedIndex][this.selectedLevel].SelectedItem.Length == 0)
					{
						this.ChangeSelectedLevel(this.psiGroupList.SelectedIndex, this.selectedLevel - 1);
					}
				}
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00007AE0 File Offset: 0x00005CE0
		public void SelectDown()
		{
			if (this.selectedList == this.psiGroupList)
			{
				this.selectedList.SelectNext();
				return;
			}
			if (this.selectedList == this.psiLabelList[this.psiGroupList.SelectedIndex])
			{
				int selectedIndex = this.selectedList.SelectedIndex;
				this.selectedList.SelectNext();
				if (selectedIndex != this.selectedList.SelectedIndex)
				{
					for (int i = 0; i < this.psiLevelList[this.psiGroupList.SelectedIndex].Length; i++)
					{
						this.psiLevelList[this.psiGroupList.SelectedIndex][i].SelectNext();
					}
					while (this.psiLevelList[this.psiGroupList.SelectedIndex][this.selectedLevel].SelectedItem.Length == 0)
					{
						this.ChangeSelectedLevel(this.psiGroupList.SelectedIndex, this.selectedLevel - 1);
					}
				}
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00007BC4 File Offset: 0x00005DC4
		private void SelectPsiGroupList(int groupIndex)
		{
			if (this.psiLabelList[groupIndex] != null)
			{
				this.psiLabelList[groupIndex].SelectedIndex = 0;
				this.psiLabelList[groupIndex].ShowSelectionRectangle = false;
				this.psiLabelList[groupIndex].UseHighlightTextColor = false;
				this.psiLabelList[groupIndex].Focused = false;
				for (int i = 0; i < this.psiLevelList[groupIndex].Length; i++)
				{
					this.psiLevelList[groupIndex][i].SelectedIndex = 0;
					this.psiLevelList[groupIndex][i].ShowSelectionRectangle = false;
					this.psiLevelList[groupIndex][i].UseHighlightTextColor = false;
					this.psiLevelList[groupIndex][i].Focused = false;
				}
			}
			this.selectedList = this.psiGroupList;
			this.selectedList.ShowSelectionRectangle = true;
			this.selectedList.UseHighlightTextColor = true;
			this.selectedList.ShowCursor = true;
			this.selectedList.Focused = true;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00007CA8 File Offset: 0x00005EA8
		public void SelectLeft()
		{
			if (this.selectedList == this.psiLabelList[this.psiGroupList.SelectedIndex])
			{
				if (this.selectedLevel <= 0)
				{
					this.SelectPsiGroupList(this.psiGroupList.SelectedIndex);
					return;
				}
				this.ChangeSelectedLevel(this.psiGroupList.SelectedIndex, this.selectedLevel - 1);
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00007D04 File Offset: 0x00005F04
		private void SelectPsiLabelList(int groupIndex)
		{
			if (this.psiLabelList[groupIndex] != null)
			{
				this.psiGroupList.ShowCursor = false;
				this.psiGroupList.Focused = false;
				this.selectedList = this.psiLabelList[groupIndex];
				this.selectedList.ShowSelectionRectangle = true;
				this.selectedList.UseHighlightTextColor = true;
				this.selectedList.Focused = true;
				this.ChangeSelectedLevel(groupIndex, 0);
				for (int i = 0; i < this.psiLevelList[groupIndex].Length; i++)
				{
					this.psiLevelList[groupIndex][i].ShowSelectionRectangle = false;
					this.psiLevelList[groupIndex][i].UseHighlightTextColor = true;
					this.psiLevelList[groupIndex][i].Focused = true;
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00007DB8 File Offset: 0x00005FB8
		public void SelectRight()
		{
			if (this.selectedList == this.psiGroupList)
			{
				this.SelectPsiLabelList(this.psiGroupList.SelectedIndex);
				return;
			}
			if (this.selectedList == this.psiLabelList[this.psiGroupList.SelectedIndex])
			{
				int num = Math.Min(this.psiLevelList[this.psiGroupList.SelectedIndex].Length - 1, this.selectedLevel + 1);
				if (this.psiLevelList[this.psiGroupList.SelectedIndex][num].SelectedItem.Length > 0)
				{
					this.ChangeSelectedLevel(this.psiGroupList.SelectedIndex, num);
				}
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007E56 File Offset: 0x00006056
		public void Accept()
		{
			if (this.selectedList == this.psiGroupList)
			{
				this.SelectPsiLabelList(this.psiGroupList.SelectedIndex);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00007E77 File Offset: 0x00006077
		public void Cancel()
		{
			if (this.selectedList != this.psiGroupList)
			{
				this.SelectPsiGroupList(this.psiGroupList.SelectedIndex);
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00007E98 File Offset: 0x00006098
		public override void Draw(RenderTarget target)
		{
			if (this.psiGroupList.Visible)
			{
				this.psiGroupList.Draw(target);
			}
			if (this.psiLabelList[this.psiGroupList.SelectedIndex] != null)
			{
				if (this.psiLabelList[this.psiGroupList.SelectedIndex].Visible)
				{
					this.psiLabelList[this.psiGroupList.SelectedIndex].Draw(target);
				}
				for (int i = 0; i < this.psiLevelList[this.psiGroupList.SelectedIndex].Length; i++)
				{
					if (this.psiLevelList[this.psiGroupList.SelectedIndex][i].Visible)
					{
						this.psiLevelList[this.psiGroupList.SelectedIndex][i].Draw(target);
					}
				}
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007F5C File Offset: 0x0000615C
		protected override void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.psiGroupList.Dispose();
				for (int i = 0; i < this.psiLabelList.Length; i++)
				{
					if (this.psiLabelList[i] != null)
					{
						this.psiLabelList[i].Dispose();
					}
					if (this.psiLevelList[i] != null)
					{
						for (int j = 0; j < this.psiLevelList[i].Length; j++)
						{
							this.psiLevelList[i][j].Dispose();
						}
					}
				}
			}
			base.Dispose(disposing);
		}

		// Token: 0x040001FB RID: 507
		private const string CURSOR_FILE = "cursor.dat";

		// Token: 0x040001FC RID: 508
		private const int PSI_GROUP_LIST_WIDTH = 50;

		// Token: 0x040001FD RID: 509
		private const int PSI_LABEL_LIST_LEFT_MARGIN = 16;

		// Token: 0x040001FE RID: 510
		private const int PSI_SYMBOL_CURSOR_MARGIN = 10;

		// Token: 0x040001FF RID: 511
		private const int PSI_SYMBOL_LISTS_DEFAULT_WIDTH = 55;

		// Token: 0x04000200 RID: 512
		private static readonly string[] PSI_GROUP_STRINGS = new string[]
		{
			"psi.offense",
			"psi.recovery",
			"psi.support",
			"psi.other"
		};

		// Token: 0x04000201 RID: 513
		private static readonly Type[] PSI_GROUP_TYPES = new Type[]
		{
			typeof(OffensePsiData),
			typeof(AssistPsiData),
			typeof(DefensePsiData),
			typeof(OtherPsiData)
		};

		// Token: 0x04000202 RID: 514
		private int width;

		// Token: 0x04000203 RID: 515
		private int rows;

		// Token: 0x04000204 RID: 516
		private ScrollingList psiGroupList;

		// Token: 0x04000205 RID: 517
		private ScrollingList[] psiLabelList;

		// Token: 0x04000206 RID: 518
		private ScrollingList[][] psiLevelList;

		// Token: 0x04000207 RID: 519
		private PsiLevel[][][] psiLevels;

		// Token: 0x04000208 RID: 520
		private ScrollingList selectedList;

		// Token: 0x04000209 RID: 521
		private int selectedLevel;

		// Token: 0x02000038 RID: 56
		public enum PanelType
		{
			// Token: 0x0400020C RID: 524
			PsiGroupPanel,
			// Token: 0x0400020D RID: 525
			PsiTypePanel
		}

		// Token: 0x02000039 RID: 57
		private struct PsiListItem
		{
			// Token: 0x0400020E RID: 526
			public string Label;

			// Token: 0x0400020F RID: 527
			public string[] Symbols;

			// Token: 0x04000210 RID: 528
			public int[] Levels;

			// Token: 0x04000211 RID: 529
			public PsiData PsiData;
		}
	}
}
