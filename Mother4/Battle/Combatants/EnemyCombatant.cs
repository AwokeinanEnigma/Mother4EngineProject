using System;
using Mother4.Battle.Actions;
using Mother4.Data;

namespace Mother4.Battle.Combatants
{
	internal class EnemyCombatant : Combatant
	{
		public EnemyType Enemy
		{
			get
			{
				return this.enemy;
			}
		}

		public EnemyCombatant(EnemyType enemy) : base(BattleFaction.EnemyTeam)
		{
			this.enemy = enemy;
			this.stats = EnemyStats.GetStats(enemy);
		}

		public override DecisionAction GetDecisionAction(BattleController controller, int priority, bool isFromUndo)
		{
			return new EnemyDecisionAction(new ActionParams
			{
				actionType = typeof(EnemyDecisionAction),
				controller = controller,
				sender = this,
				priority = priority
			});
		}

		private EnemyType enemy;
	}
}
