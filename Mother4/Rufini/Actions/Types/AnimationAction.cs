using System;
using System.Collections.Generic;
using Carbine.Graphics;
using Carbine.Utility;
using Mother4.Data;
using Mother4.Scripts;
using Mother4.Scripts.Actions;
using SFML.System;

namespace Rufini.Actions.Types
{
	internal class AnimationAction : RufiniAction
	{
		public AnimationAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "spr",
					Type = typeof(string)
				},
				new ActionParam
				{
					Name = "sub",
					Type = typeof(string)
				},
				new ActionParam
				{
					Name = "x",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "y",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "depth",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "blk",
					Type = typeof(bool)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			ActionReturnContext result = default(ActionReturnContext);
			this.context = context;
			string value = base.GetValue<string>("spr");
			string value2 = base.GetValue<string>("sub");
			int value3 = base.GetValue<int>("x");
			int value4 = base.GetValue<int>("y");
			int value5 = base.GetValue<int>("depth");
			this.blocking = base.GetValue<bool>("blk");
			this.graphic = new IndexedColorGraphic(Paths.GRAPHICS + value + ".dat", value2, new Vector2f((float)value3, (float)value4), (value5 < 0) ? value4 : value5);
			this.graphic.OnAnimationComplete += this.OnAnimationComplete;
			this.context.Pipeline.Add(this.graphic);
			if (this.blocking)
			{
				result.Wait = ScriptExecutor.WaitType.Event;
			}
			return result;
		}

		private void OnAnimationComplete(Graphic graphic)
		{
			this.context.Pipeline.Remove(graphic);
			this.graphic.OnAnimationComplete -= this.OnAnimationComplete;
			this.timerId = TimerManager.Instance.StartTimer(1);
			TimerManager.Instance.OnTimerEnd += this.OnTimerEnd;
			if (this.blocking)
			{
				this.context.Executor.Continue();
			}
		}

		private void OnTimerEnd(int timerIndex)
		{
			if (this.timerId == timerIndex)
			{
				this.graphic.Dispose();
				TimerManager.Instance.OnTimerEnd -= this.OnTimerEnd;
			}
		}

		private Graphic graphic;

		private ExecutionContext context;

		private bool blocking;

		private int timerId;
	}
}
