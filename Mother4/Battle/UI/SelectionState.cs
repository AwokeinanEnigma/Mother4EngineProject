using System;
using Mother4.Battle.Combatants;
using Mother4.Psi;

namespace Mother4.Battle.UI
{
	internal struct SelectionState
	{
		public SelectionState.SelectionType Type;

		public Combatant[] Targets;

		public int AttackIndex;

		public int ItemIndex;

		public IPsi Psi;

		public int PsiLevel;

		public enum SelectionType
		{
			Bash,
			PSI,
			Talk,
			Items,
			Guard,
			Run,
			Undo
		}
	}
}
