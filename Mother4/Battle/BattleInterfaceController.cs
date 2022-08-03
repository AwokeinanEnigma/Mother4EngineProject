using System;
using System.Collections.Generic;
using System.Linq;
using Carbine.Actors;
using Carbine.Audio;
using Carbine.Graphics;
using Carbine.Input;
using Mother4.Battle.Combatants;
using Mother4.Battle.PsiAnimation;
using Mother4.Battle.UI;
using Mother4.Data;
using Mother4.GUI;
using Mother4.GUI.Modifiers;
using Mother4.Psi;
using Mother4.Scripts.Text;
using Mother4.Utility;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Battle
{
	internal class BattleInterfaceController : IDisposable
	{
		public bool AllowUndo
		{
			get
			{
				return this.isUndoAllowed;
			}
			set
			{
				this.isUndoAllowed = value;
			}
		}

		public int ActiveCharacter
		{
			get
			{
				return this.activeCharacter;
			}
			set
			{
				this.activeCharacter = value;
			}
		}

		public bool RunAttempted { get; set; }

		public CarbineSound PrePlayerAttack
		{
			get
			{
				return this.prePlayerAttack;
			}
		}

		public CarbineSound PreEnemyAttack
		{
			get
			{
				return this.preEnemyAttack;
			}
		}

		public CarbineSound PrePsiSound
		{
			get
			{
				return this.prePsiSound;
			}
		}

		public CarbineSound TalkSound
		{
			get
			{
				return this.talkSound;
			}
		}

		public CarbineSound EnemyDeathSound
		{
			get
			{
				return this.enemyDeathSound;
			}
		}

		public CarbineSound GroovySound
		{
			get
			{
				return this.groovySound;
			}
		}

		public CarbineSound ReflectSound
		{
			get
			{
				return this.reflectSound;
			}
		}

		public event BattleInterfaceController.InteractionCompletionHandler OnInteractionComplete;

		public event BattleInterfaceController.TextboxCompletionHandler OnTextboxComplete;

		public BattleInterfaceController(RenderPipeline pipeline, ActorManager actorManager, CombatantController combatantController, bool letterboxing)
		{
			this.pipeline = pipeline;
			this.actorManager = actorManager;
			this.combatantController = combatantController;
			this.psiMenu = new SectionedPsiBox(this.pipeline, 1, 14f);
			this.topLetterbox = new RectangleShape(new Vector2f(320f, 14f));
			this.topLetterbox.FillColor = Color.Black;
			this.topLetterbox.Position = new Vector2f(0f, -14f);
			this.topLetterboxY = this.topLetterbox.Position.Y;
			this.topLetterboxTargetY = (float)(letterboxing ? 0 : -14);
			this.bottomLetterbox = new RectangleShape(new Vector2f(320f, 35f));
			this.bottomLetterbox.FillColor = Color.Black;
			this.bottomLetterbox.Position = new Vector2f(0f, 180f);
			this.bottomLetterboxY = this.bottomLetterbox.Position.Y;
			this.bottomLetterboxTargetY = (float)(180L + (letterboxing ? -35L : 0L));
			this.buttonBar = new ButtonBar(pipeline);
			actorManager.Add(this.buttonBar);
			Combatant[] factionCombatants = combatantController.GetFactionCombatants(BattleFaction.PlayerTeam);
			CharacterType[] array = new CharacterType[factionCombatants.Length];
			for (int i = 0; i < factionCombatants.Length; i++)
			{
				array[i] = ((PlayerCombatant)factionCombatants[i]).Character;
			}
			this.cardBar = new CardBar(pipeline, array);
			actorManager.Add(this.cardBar);
			this.enemyGraphics = new Dictionary<int, IndexedColorGraphic>();
			this.enemyIDs = new List<int>();
			foreach (Combatant combatant in combatantController.CombatantList)
			{
				switch (combatant.Faction)
				{
				case BattleFaction.PlayerTeam:
				{
					PlayerCombatant playerCombatant = (PlayerCombatant)combatant;
					playerCombatant.OnStatChange += this.OnPlayerStatChange;
					playerCombatant.OnStatusEffectChange += this.OnPlayerStatusEffectChange;
					break;
				}
				case BattleFaction.EnemyTeam:
				{
					EnemyCombatant enemyCombatant = (EnemyCombatant)combatant;
					enemyCombatant.OnStatusEffectChange += this.OnEnemyStatusEffectChange;
					IndexedColorGraphic indexedColorGraphic = new IndexedColorGraphic(EnemyGraphics.GetFilename(enemyCombatant.Enemy), "front", default(Vector2f), 0);
					indexedColorGraphic.CurrentPalette = uint.MaxValue;
					indexedColorGraphic.CurrentPalette = 0U;
					this.enemyGraphics.Add(enemyCombatant.ID, indexedColorGraphic);
					pipeline.Add(indexedColorGraphic);
					this.enemyIDs.Add(enemyCombatant.ID);
					break;
				}
				}
			}
			this.AlignEnemyGraphics();
			this.textbox = new BattleTextbox(pipeline, 0);
			this.textbox.OnTextboxComplete += this.TextboxComplete;
			this.textbox.OnTextTrigger += this.TextTrigger;
			actorManager.Add(this.textbox);
			this.dimmer = new ScreenDimmer(pipeline, Color.Transparent, 0, 15);
			this.state = BattleInterfaceController.State.Waiting;
			this.selectionState = default(SelectionState);
			this.selectedEnemyID = -1;
			this.comboCircle = new ComboAnimator(pipeline, 0);
			this.moveBeepX = AudioManager.Instance.Use(Paths.AUDIO + "cursorx.wav", AudioType.Sound);
			this.moveBeepY = AudioManager.Instance.Use(Paths.AUDIO + "cursory.wav", AudioType.Sound);
			this.selectBeep = AudioManager.Instance.Use(Paths.AUDIO + "confirm.wav", AudioType.Sound);
			this.cancelBeep = AudioManager.Instance.Use(Paths.AUDIO + "cancel.wav", AudioType.Sound);
			this.prePlayerAttack = AudioManager.Instance.Use(Paths.AUDIO + "prePlayerAttack.wav", AudioType.Sound);
			this.preEnemyAttack = AudioManager.Instance.Use(Paths.AUDIO + "preEnemyAttack.wav", AudioType.Sound);
			this.prePsiSound = AudioManager.Instance.Use(Paths.AUDIO + "prePsi.wav", AudioType.Sound);
			this.talkSound = AudioManager.Instance.Use(Paths.AUDIO + "floydTalk.wav", AudioType.Sound);
			this.enemyDeathSound = AudioManager.Instance.Use(Paths.AUDIO + "enemyDeath.wav", AudioType.Sound);
			this.smashSound = AudioManager.Instance.Use(Paths.AUDIO + "smaaash.wav", AudioType.Sound);
			this.comboHitA = AudioManager.Instance.Use(Paths.AUDIO + "hitA.wav", AudioType.Sound);
			this.comboHitB = AudioManager.Instance.Use(Paths.AUDIO + "hitB.wav", AudioType.Sound);
			this.comboSuccess = AudioManager.Instance.Use(Paths.AUDIO + "Combo16.wav", AudioType.Sound);
			this.comboSoundMap = new Dictionary<CharacterType, List<CarbineSound>>();
			for (int j = 0; j < array.Length; j++)
			{
				List<CarbineSound> list;
				if (this.comboSoundMap.ContainsKey(array[j]))
				{
					list = this.comboSoundMap[array[j]];
				}
				else
				{
					list = new List<CarbineSound>();
					this.comboSoundMap.Add(array[j], list);
				}
				for (int k = 0; k < 3; k++)
				{
					string str = CharacterComboSounds.Get(array[j], 0, k, 120);
					CarbineSound item = AudioManager.Instance.Use(Paths.AUDIO + str, AudioType.Sound);
					list.Add(item);
				}
			}
			this.winSounds = new Dictionary<int, CarbineSound>();
			this.winSounds.Add(0, AudioManager.Instance.Use(Paths.AUDIO + "win1.wav", AudioType.Stream));
			this.winSounds.Add(1, AudioManager.Instance.Use(Paths.AUDIO + "win2.wav", AudioType.Stream));
			this.winSounds.Add(2, AudioManager.Instance.Use(Paths.AUDIO + "win3.wav", AudioType.Stream));
			this.winSounds.Add(3, AudioManager.Instance.Use(Paths.AUDIO + "win4.wav", AudioType.Stream));
			this.groovySound = AudioManager.Instance.Use(Paths.AUDIO + "Groovy.wav", AudioType.Sound);
			this.reflectSound = AudioManager.Instance.Use(Paths.AUDIO + "homerun.wav", AudioType.Sound);
			this.jingler = new LevelUpJingler(array, true);
			this.graphicModifiers = new List<IGraphicModifier>();
			this.damageNumbers = new List<DamageNumber>();
			this.psiAnimators = new List<PsiAnimator>();
			InputManager.Instance.AxisPressed += this.AxisPressed;
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
		}

		~BattleInterfaceController()
		{
			this.Dispose(false);
		}

		private void TextTrigger(TextTrigger trigger)
		{
			switch (trigger.Type)
			{
			case 0:
				this.youWon = new YouWon(this.pipeline);
				return;
			case 1:
			{
				CharacterType character;
				bool flag = Enum.TryParse<CharacterType>(trigger.Data[0], true, out character);
				if (flag)
				{
					this.jingler.Play(character);
					return;
				}
				break;
			}
			case 2:
			{
				int i = 0;
				int hp = 0;
				int.TryParse(trigger.Data[0], out i);
				int.TryParse(trigger.Data[1], out hp);
				StatSet statChange = new StatSet
				{
					HP = hp
				};
				this.combatantController[i].AlterStats(statChange);
				return;
			}
			case 3:
			{
				int i2 = 0;
				int pp = 0;
				int.TryParse(trigger.Data[0], out i2);
				int.TryParse(trigger.Data[1], out pp);
				StatSet statChange2 = new StatSet
				{
					PP = pp
				};
				this.combatantController[i2].AlterStats(statChange2);
				break;
			}
			default:
				return;
			}
		}

		public void PlayWinBGM(int type)
		{
			if (this.winSounds.ContainsKey(type))
			{
				this.winSounds[type].Play();
			}
		}

		public void StopWinBGM()
		{
			foreach (CarbineSound carbineSound in this.winSounds.Values)
			{
				carbineSound.Stop();
			}
		}

		public void PlayLevelUpBGM()
		{
			this.jingler.Play();
		}

		public void EndLevelUpBGM()
		{
			this.jingler.End();
		}

		public void StopLevelUpBGM()
		{
			this.jingler.Stop();
		}

		private CarbineSound GetComboSound(CharacterType character, int index)
		{
			CarbineSound result = null;
			if (this.comboSoundMap.ContainsKey(character))
			{
				result = this.comboSoundMap[character][index % this.comboSoundMap[character].Count];
			}
			return result;
		}

		private void OnPlayerStatChange(Combatant sender, StatSet change)
		{
			PlayerCombatant playerCombatant = (PlayerCombatant)sender;


            this.UpdatePlayerCard(playerCombatant.ID, playerCombatant.Stats.HP, playerCombatant.Stats.PP, playerCombatant.Stats.Meter);
		}

		private void OnPlayerStatusEffectChange(Combatant sender, StatusEffect statusEffect, bool added)
		{
			if (added)
			{
				if (statusEffect != StatusEffect.Talking)
				{
					return;
				}
				this.TalkifyPlayer(sender as PlayerCombatant);
				this.SetCardSpring(sender.ID, BattleCard.SpringMode.BounceUp, new Vector2f(0f, 8f), new Vector2f(0f, 0.1f), new Vector2f(0f, 1f));
				return;
			}
			else
			{
				if (statusEffect != StatusEffect.Talking)
				{
					return;
				}
				this.RemoveTalker(this.cardBar.GetCardGraphic(sender.ID));
				this.SetCardSpring(sender.ID, BattleCard.SpringMode.Normal, new Vector2f(0f, 0f), new Vector2f(0f, 0f), new Vector2f(0f, 0f));
				return;
			}
		}

		private void OnEnemyStatusEffectChange(Combatant sender, StatusEffect statusEffect, bool added)
		{
			if (added)
			{
				if (statusEffect != StatusEffect.Talking)
				{
					return;
				}
				this.TalkifyEnemy(sender as EnemyCombatant);
				return;
			}
			else
			{
				if (statusEffect != StatusEffect.Talking)
				{
					return;
				}
				this.RemoveTalker(this.enemyGraphics[sender.ID]);
				return;
			}
		}

		public PsiAnimator AddPsiAnimation(PsiElementList animation, Combatant sender, Combatant[] targets)
		{
			Graphic senderGraphic = null;
			if (sender.Faction == BattleFaction.EnemyTeam)
			{
				senderGraphic = this.enemyGraphics[sender.ID];
			}
			else if (sender.Faction == BattleFaction.PlayerTeam)
			{
				senderGraphic = this.cardBar.GetCardGraphic(sender.ID);
			}
			int[] array = new int[targets.Length];
			Graphic[] array2 = new Graphic[targets.Length];
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i].Faction == BattleFaction.EnemyTeam)
				{
					array2[i] = this.enemyGraphics[targets[i].ID];
					array[i] = -1;
				}
				else if (targets[i].Faction == BattleFaction.PlayerTeam)
				{
					array2[i] = this.cardBar.GetCardGraphic(targets[i].ID);
					array[i] = targets[i].ID;
				}
			}
			PsiAnimator psiAnimator = new PsiAnimator(this.pipeline, this.graphicModifiers, animation, senderGraphic, array2, this.cardBar, array);
			this.psiAnimators.Add(psiAnimator);
			return psiAnimator;
		}

		public DamageNumber AddDamageNumber(Combatant combatant, int number)
		{
			Vector2f offset = default(Vector2f);
			Vector2f position;
			if (combatant.Faction == BattleFaction.PlayerTeam)
			{
				Graphic cardGraphic = this.cardBar.GetCardGraphic(combatant.ID);
				position = new Vector2f((float)((int)cardGraphic.Position.X), (float)((int)cardGraphic.Position.Y)) + new Vector2f((float)((int)(cardGraphic.Size.X / 2f)), 2f);
				offset.Y = -10f;
			}
			else if (combatant.Faction == BattleFaction.EnemyTeam)
			{
				Graphic graphic = this.enemyGraphics[combatant.ID];
				position = new Vector2f((float)((int)graphic.Position.X), (float)((int)graphic.Position.Y));
				offset.Y = (float)((int)(-graphic.Size.Y / 3f));
			}
			else
			{
				position = new Vector2f(-320f, -180f);
			}
			DamageNumber damageNumber = new DamageNumber(this.pipeline, position, offset, 30, number);
			damageNumber.SetVisibility(true);
			this.damageNumbers.Add(damageNumber);
			damageNumber.Start();
			return damageNumber;
		}

		public void StartComboCircle(EnemyCombatant enemy, PlayerCombatant player)
		{
			Graphic graphic = this.enemyGraphics[enemy.ID];
			this.comboCircle.Setup(graphic, player);
		}

		public void StopComboCircle(bool explode)
		{
			this.comboCircle.Stop(explode);
			if (explode)
			{
				this.comboSuccess.Stop();
				this.comboSuccess.Play();
			}
		}

		public void AddComboHit(int damage, int comboCount, CharacterType character, Combatant target, bool smash)
		{
			this.comboCircle.AddHit(damage, smash);
			if ((comboCount + 1) % 4 != 0)
			{
				this.comboHitA.Play();
			}
			else
			{
				this.comboHitB.Play();
			}
			if (this.hitSound != null)
			{
				this.hitSound.Stop();
			}
			this.hitSound = this.GetComboSound(character, comboCount);
			if (this.hitSound != null)
			{
				this.hitSound.Play();
			}
			if (smash)
			{
				this.smashSound.Stop();
				this.smashSound.Play();
				new BattleSmash(this.pipeline, this.enemyGraphics[target.ID].Position);
			}
		}

		public bool IsComboCircleDone()
		{
			return this.comboCircle.Stopped;
		}

		public void FlashEnemy(EnemyCombatant combatant, Color color, int duration, int count)
		{
			this.FlashEnemy(combatant, color, ColorBlendMode.Multiply, duration, count);
		}

		public void FlashEnemy(EnemyCombatant combatant, Color color, ColorBlendMode blendMode, int duration, int count)
		{
			this.graphicModifiers.Add(new GraphicFader(this.enemyGraphics[combatant.ID], color, blendMode, duration, count));
		}

		public void BlinkEnemy(EnemyCombatant combatant, int duration, int count)
		{
			this.graphicModifiers.Add(new GraphicBlinker(this.enemyGraphics[combatant.ID], duration, count));
		}

		public void TalkifyPlayer(PlayerCombatant combatant)
		{
			this.graphicModifiers.Add(new GraphicTalker(this.pipeline, this.cardBar.GetCardGraphic(combatant.ID)));
		}

		public void TalkifyEnemy(EnemyCombatant combatant)
		{
			this.graphicModifiers.Add(new GraphicTalker(this.pipeline, this.enemyGraphics[combatant.ID]));
			this.graphicModifiers.Add(new GraphicBouncer(this.enemyGraphics[combatant.ID], GraphicBouncer.SpringMode.BounceUp, new Vector2f(0f, 4f), new Vector2f(0f, 0.1f), new Vector2f(0f, 1f)));
		}

		private void RemoveTalker(Graphic graphic)
		{
			foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
			{
				if (graphicModifier is GraphicTalker && graphicModifier.Graphic == graphic)
				{
					(graphicModifier as GraphicTalker).Dispose();
				}
			}
			this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicTalker && x.Graphic == graphic);
			this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicBouncer && x.Graphic == graphic);
		}

		public void RemoveTalkers()
		{
			foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
			{
				if (graphicModifier is GraphicTalker)
				{
					(graphicModifier as GraphicTalker).Dispose();
				}
			}
			this.graphicModifiers.RemoveAll((IGraphicModifier x) => x is GraphicTalker);
		}

		private void ResetEnemySelectDim()
		{
			Color color = new Color((byte)128, (byte)128, (byte)128);
			foreach (KeyValuePair<int, IndexedColorGraphic> enemyGraphic in this.enemyGraphics)
			{
				KeyValuePair<int, IndexedColorGraphic> kvp = enemyGraphic;
				this.graphicModifiers.RemoveAll((Predicate<IGraphicModifier>)(x => x.Graphic == kvp.Value && x is GraphicFader));
				if (this.selectedEnemyID >= 0)
				{
					if (kvp.Key == this.selectedEnemyID)
					{
						kvp.Value.Color = Color.White;
						this.graphicModifiers.Add((IGraphicModifier)new GraphicFader(kvp.Value, new Color((byte)64, (byte)64, (byte)64), ColorBlendMode.Screen, 30, -1));
					}
					else
					{
						kvp.Value.ColorBlendMode = ColorBlendMode.Multiply;
						kvp.Value.Color = color;
					}
				}
				else
				{
					kvp.Value.ColorBlendMode = ColorBlendMode.Multiply;
					kvp.Value.Color = Color.White;
				}
			}
		}
		private void AlignEnemyGraphics()
		{
			int num = 0;
			int num2 = 320 / (this.enemyGraphics.Count + 1);
			for (int i = 0; i < this.enemyIDs.Count; i++)
			{
				int id = this.enemyIDs[i];
				num += num2;
				Vector2f vector2f = new Vector2f((float)num, (float)(78 + ((i % 2 == 0) ? 0 : 12)));
				this.enemyGraphics[id].Depth = (int)vector2f.Y - 78;
				if (this.graphicModifiers != null)
				{
					Console.WriteLine("old:({0},{1}) new:({2},{3})", new object[]
					{
						this.enemyGraphics[id].Position.X,
						this.enemyGraphics[id].Position.Y,
						vector2f.X,
						vector2f.Y
					});
					for (int j = 0; j < this.graphicModifiers.Count; j++)
					{
						if (this.graphicModifiers[j].Graphic == this.enemyGraphics[id])
						{
							this.graphicModifiers.Remove(this.graphicModifiers[j]);
							Console.WriteLine("removed");
						}
					}
					this.graphicModifiers.RemoveAll((IGraphicModifier x) => x.Graphic == this.enemyGraphics[id] && x is GraphicTranslator);
					this.graphicModifiers.Add(new GraphicTranslator(this.enemyGraphics[id], vector2f, 10));
				}
				else
				{
					this.enemyGraphics[id].Position = vector2f;
				}
			}
		}

		public void DoGroovy(int id)
		{
			if (this.groovy != null)
			{
				this.groovy.Dispose();
			}
			Vector2f cardTopMiddle = this.cardBar.GetCardTopMiddle(id);
			this.groovy = new Groovy(this.pipeline, cardTopMiddle);
			this.groovySound.Play();
		}

		private void TextboxComplete()
		{
			this.textboxHideFlag = true;
			if (this.youWon != null)
			{
				this.youWon.Remove();
				this.youWon.Dispose();
				this.youWon = null;
			}
			if (this.OnTextboxComplete != null)
			{
				this.OnTextboxComplete();
			}
		}

		private void AxisPressed(InputManager sender, Vector2f axis)
		{
			bool flag = axis.X < 0f;
			bool flag2 = axis.X > 0f;
			bool flag3 = axis.Y < 0f;
			bool flag4 = axis.Y > 0f;
			if (this.state != BattleInterfaceController.State.Waiting)
			{
				if (flag || flag2)
				{
					this.moveBeepX.Play();
				}
				if (flag3 || flag4)
				{
					this.moveBeepY.Play();
				}
			}
			switch (this.state)
			{
			case BattleInterfaceController.State.Waiting:
			case BattleInterfaceController.State.PsiAttackSelection:
			case BattleInterfaceController.State.SpecialSelection:
			case BattleInterfaceController.State.ItemSelection:
				break;
			case BattleInterfaceController.State.TopLevelSelection:
				if (flag)
				{
					this.buttonBar.SelectLeft();
					return;
				}
				if (flag2)
				{
					this.buttonBar.SelectRight();
					return;
				}
				break;
			case BattleInterfaceController.State.PsiTypeSelection:
				if (flag3)
				{
					this.psiMenu.SelectUp();
					return;
				}
				if (flag4)
				{
					this.psiMenu.SelectDown();
					return;
				}
				if (flag)
				{
					this.psiMenu.SelectLeft();
					return;
				}
				if (flag2)
				{
					this.psiMenu.SelectRight();
					return;
				}
				break;
			case BattleInterfaceController.State.EnemySelection:
				if (flag)
				{
					this.enemySelectIndex--;
					if (this.enemySelectIndex < 0)
					{
						this.enemySelectIndex = this.enemyIDs.Count - 1;
					}
					this.selectedEnemyID = this.enemyIDs[this.enemySelectIndex];
					this.ResetEnemySelectDim();
					return;
				}
				if (flag2)
				{
					this.enemySelectIndex++;
					if (this.enemySelectIndex >= this.enemyIDs.Count)
					{
						this.enemySelectIndex = 0;
					}
					this.selectedEnemyID = this.enemyIDs[this.enemySelectIndex];
					this.ResetEnemySelectDim();
				}
				break;
			default:
				return;
			}
		}

		private void ShowPsiTypeSelector(Func<PlayerCombatant> CurrentPlayerCombatant)
		{
			throw new NotImplementedException();
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
			if (this.state != BattleInterfaceController.State.Waiting)
			{
				if (b == Button.A)
				{
					this.selectBeep.Play();
				}
				if (b == Button.B)
				{
					this.cancelBeep.Play();
				}
			}
			switch (this.state)
			{
			case BattleInterfaceController.State.Waiting:
				break;
			case BattleInterfaceController.State.TopLevelSelection:
				this.TopLevelSelection(b);
				return;
			case BattleInterfaceController.State.PsiTypeSelection:
				this.PsiTypeSelection(b);
				return;
			case BattleInterfaceController.State.PsiAttackSelection:
				this.PsiAttackSelection(b);
				return;
			case BattleInterfaceController.State.SpecialSelection:
				this.SpecialSelection(b);
				return;
			case BattleInterfaceController.State.ItemSelection:
				this.ItemSelection(b);
				return;
			case BattleInterfaceController.State.EnemySelection:
				this.EnemySelection(b);
				break;
			default:
				return;
			}
		}

		private PlayerCombatant CurrentPlayerCombatant()
		{
			return (PlayerCombatant)this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[this.cardBar.SelectedIndex];
		}

		private void StartEnemySelection()
		{
			this.state = BattleInterfaceController.State.EnemySelection;
			this.buttonBar.Hide();
			this.selectedEnemyID = this.enemyIDs[this.enemySelectIndex % this.enemyIDs.Count];
			this.ResetEnemySelectDim();
		}

		public void HidePSI() { 
		//	this.psiMenu
		}

		private void TopLevelSelection(Button b)
		{
			switch (b)
			{
			case Button.A:
				switch (this.buttonBar.SelectedAction)
				{
				case ButtonBar.Action.Bash:
					this.StartEnemySelection();
					return;
				case ButtonBar.Action.Psi:
							{
								Console.Write("psi selected");
					PlayerCombatant playerCombatant = this.CurrentPlayerCombatant();
					//if (playerCombatant.GetStatusEffects().ToList().Contains())
					this.psiMenu.Reset();
					this.psiMenu.MaxLevel = this.CurrentPlayerCombatant().Stats.Level;
								Console.WriteLine($"Current Character: { playerCombatant.Character}");
					this.psiMenu.OffensePsiItems = PsiManager.Instance.GetCharacterOffensePsi(playerCombatant.Character);
								foreach (var psi in psiMenu.OffensePsiItems) {
									Console.WriteLine(psi.Name);
								}
					this.psiMenu.DefensePsiItems = PsiManager.Instance.GetCharacterDefensePsi(playerCombatant.Character);
					this.psiMenu.AssistPsiItems = PsiManager.Instance.GetCharacterAssistPsi(playerCombatant.Character);
					this.psiMenu.OtherPsiItems = PsiManager.Instance.GetCharacterOtherPsi(playerCombatant.Character);
					this.state = BattleInterfaceController.State.PsiTypeSelection;
					this.buttonBar.Hide();
					this.psiMenu.Show();
					return;
				}
				case ButtonBar.Action.Items:
					this.state = BattleInterfaceController.State.ItemSelection;
					this.buttonBar.Hide();
					return;
				case ButtonBar.Action.Talk:
					this.state = BattleInterfaceController.State.EnemySelection;
					this.buttonBar.Hide();
					this.selectedEnemyID = this.enemyIDs[this.enemySelectIndex % this.enemyIDs.Count];
					this.ResetEnemySelectDim();
					return;
				case ButtonBar.Action.Guard:
					this.CompleteMenuGuard();
					return;
				case ButtonBar.Action.Run:
					this.buttonBar.Hide();
					this.RunAttempted = true;
					this.CompleteMenuRun();
					this.state = BattleInterfaceController.State.Waiting;
					return;
				default:
					throw new NotImplementedException("Tried to use unimplemented button action.");
				}
				break;
			case Button.B:
				if (this.isUndoAllowed)
				{
					this.CompleteMenuUndo();
				}
				return;
			default:
				return;
			}
		}

		private void PsiTypeSelection(Button b)
		{
			switch (b)
			{
			case Button.A:
			{
				if (this.psiMenu.InTypeSelection())
				{
					this.psiMenu.SelectRight();
					return;
				}
				PsiType psiType = this.psiMenu.SelectedPsiType();
						Console.WriteLine(psiType);
				int num = this.psiMenu.SelectedLevel();
				IPsi psi;
				switch (psiType)
				{
				case PsiType.Offense:
					psi = this.psiMenu.SelectOffensePsi();
					break;
				case PsiType.Defense:
					psi = this.psiMenu.SelectDefensePsi();
					break;
				case PsiType.Assist:
					psi = this.psiMenu.SelectAssistPsi();
					break;	
				case PsiType.Other:
					psi = this.psiMenu.SelectOtherPsi();
					break;
				default:
					throw new InvalidOperationException();
				}
				if (psi.PP[num] > this.CurrentPlayerCombatant().Stats.PP)
				{
					this.ShowMessage("Not enough PP!!", false);
					return;
				}
				this.psiMenu.Hide();
				this.StartEnemySelection();
				this.selectionState.Psi = psi;
				this.selectionState.PsiLevel = num;
				return;
			}
			case Button.B:
				this.psiMenu.Hide();
				this.state = BattleInterfaceController.State.TopLevelSelection;
				this.buttonBar.Show();
				return;
			default:
				return;
			}
		}

		private void PsiAttackSelection(Button b)
		{
			switch (b)
			{
			case Button.A:
				break;
			case Button.B:
				this.psiMenu.Show();
				this.state = BattleInterfaceController.State.TopLevelSelection;
				break;
			default:
				return;
			}
		}

		private void SpecialSelection(Button b)
		{
			switch (b)
			{
			case Button.A:
				break;
			case Button.B:
				this.state = BattleInterfaceController.State.TopLevelSelection;
				this.buttonBar.Show();
				break;
			default:
				return;
			}
		}

		private void ItemSelection(Button b)
		{
			switch (b)
			{
			case Button.A:
				break;
			case Button.B:
				this.state = BattleInterfaceController.State.TopLevelSelection;
				this.buttonBar.Show();
				break;
			default:
				return;
			}
		}

		private void EnemySelection(Button b)
		{
			switch (b)
			{
			case Button.A:
				this.CompleteEnemySelection(this.buttonBar.SelectedAction);
				return;
			case Button.B:
				this.selectedEnemyID = -1;
				this.ResetEnemySelectDim();
				this.state = BattleInterfaceController.State.TopLevelSelection;
				this.buttonBar.Show();
				return;
			default:
				return;
			}
		}

		private void CompleteMenuUndo()
		{
			if (this.OnInteractionComplete != null)
			{
				this.selectionState.Type = SelectionState.SelectionType.Undo;
				this.OnInteractionComplete(this.selectionState);
			}
		}

		private void CompleteEnemySelection(ButtonBar.Action buttonAction)
		{
			if (this.OnInteractionComplete != null)
			{
				switch (buttonAction)
				{
				case ButtonBar.Action.Bash:
					this.selectionState.Type = SelectionState.SelectionType.Bash;
					break;
				case ButtonBar.Action.Psi:
					this.selectionState.Type = SelectionState.SelectionType.PSI;
					break;
				case ButtonBar.Action.Talk:
					this.selectionState.Type = SelectionState.SelectionType.Talk;
					break;
				}
				this.selectionState.Targets = new Combatant[]
				{
					this.combatantController[this.selectedEnemyID]
				};
				this.selectionState.AttackIndex = 0;
				this.selectionState.ItemIndex = -1;
				this.state = BattleInterfaceController.State.Waiting;
				this.OnInteractionComplete(this.selectionState);
			}
			this.selectedEnemyID = -1;
			this.ResetEnemySelectDim();
		}

		private void CompleteMenuGuard()
		{
			if (this.OnInteractionComplete != null)
			{
				this.selectionState.Type = SelectionState.SelectionType.Guard;
				this.selectionState.Targets = null;
				this.selectionState.AttackIndex = -1;
				this.selectionState.ItemIndex = -1;
				this.state = BattleInterfaceController.State.Waiting;
				this.OnInteractionComplete(this.selectionState);
			}
		}

		private void CompleteMenuRun()
		{
			if (this.OnInteractionComplete != null)
			{
				this.selectionState.Type = SelectionState.SelectionType.Run;
				this.selectionState.Targets = null;
				this.selectionState.AttackIndex = -1;
				this.selectionState.ItemIndex = -1;
				this.state = BattleInterfaceController.State.Waiting;
				this.OnInteractionComplete(this.selectionState);
			}
		}

		public void BeginPlayerInteraction(CharacterType character)
		{
			int num = 0;
			PlayerCombatant playerCombatant = null;
			foreach (Combatant combatant in this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam))
			{
				playerCombatant = (PlayerCombatant)combatant;
				if (playerCombatant.Character == character)
				{
					break;
				}
				num++;
			}
			Combatant firstLiveCombatant = this.combatantController.GetFirstLiveCombatant(BattleFaction.PlayerTeam);
			bool showRun = firstLiveCombatant != null && firstLiveCombatant.ID == playerCombatant.ID;
			this.state = BattleInterfaceController.State.TopLevelSelection;
			bool lockPSI = false;
			foreach (StatusEffectInstance statusEffectInstance in playerCombatant.GetStatusEffects())
			{
				if (statusEffectInstance.Type == StatusEffect.DisablePSI)
				{
					lockPSI = true;
				}
			}
			this.buttonBar.SetActions(BattleButtonBars.GetActions(character, showRun, lockPSI));
			this.buttonBar.Show(0);
			this.cardBar.SelectedIndex = num;
		}

		public void EndPlayerInteraction()
		{
			this.cardBar.SelectedIndex = -1;
		}

		public void SetActiveCard(int index)
		{
			this.cardBar.SelectedIndex = index;
		}

		public void PopCard(int index, int height)
		{
			this.cardBar.PopCard(index, height);
		}

		public void SetCardSpring(int index, BattleCard.SpringMode mode, Vector2f amplitude, Vector2f speed, Vector2f decay)
		{
			this.cardBar.SetSpring(index, mode, amplitude, speed, decay);
		}

		public void SetCardGroovy(int index, bool groovy)
		{
			this.cardBar.SetGroovy(index, groovy);
		}

		public void AddCardSpring(int index, Vector2f amplitude, Vector2f speed, Vector2f decay)
		{
			this.cardBar.AddSpring(index, amplitude, speed, decay);
		}

		public void HideButtonBar()
		{
			this.buttonBar.Hide();
		}

		public void ShowButtonBar()
		{
			this.buttonBar.Show();
		}

		public void ShowMessage(string message, bool useButton)
		{
			this.textbox.Reset(message, useButton);
			this.textbox.Show();
		}

		public void SetLetterboxing(float letterboxing)
		{
			this.topLetterboxTargetY = (float)(-(float)((int)(14f * (1f - letterboxing))));
			this.bottomLetterboxTargetY = (float)(180L - (long)((int)(35f * letterboxing)));
		}

		public void AddEnemy(int id)
		{
			EnemyCombatant enemyCombatant = (EnemyCombatant)this.combatantController[id];
			this.enemyIDs.Add(id);
			new IndexedColorGraphic(EnemyGraphics.GetFilename(enemyCombatant.Enemy), "front", default(Vector2f), 0);
			this.AlignEnemyGraphics();
		}

		public void DoEnemyDeathAnimation(int id)
		{
			this.enemyDeathSound.Play();
			this.graphicModifiers.Add(new GraphicDeathFader(this.enemyGraphics[id], 40));
		}

		public void RemoveAllModifiers()
		{
			this.graphicModifiers.Clear();
		}

		public void RemoveEnemy(int id)
		{
			this.RemoveTalker(this.enemyGraphics[id]);
			this.graphicModifiers.RemoveAll((IGraphicModifier x) => x.Graphic == this.enemyGraphics[id]);
			this.pipeline.Remove(this.enemyGraphics[id]);
			this.enemyGraphics[id].Dispose();
			this.enemyGraphics.Remove(id);
			this.enemyIDs.Remove(id);
			this.AlignEnemyGraphics();
		}

		public void UpdatePlayerCard(int id, int hp, int pp, float meter)
		{
			PlayerCombatant playerCombatant = (PlayerCombatant)this.combatantController[id];
			this.cardBar.SetHP(playerCombatant.PartyIndex, hp);
			this.cardBar.SetPP(playerCombatant.PartyIndex, pp);
			this.cardBar.SetMeter(playerCombatant.PartyIndex, meter);
		}

		public void Update()
		{
			foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
			{
				graphicModifier.Update();
			}
			this.graphicModifiers.RemoveAll((IGraphicModifier x) => x.Done);
			foreach (PsiAnimator psiAnimator in this.psiAnimators)
			{
				psiAnimator.Update();
			}
			this.psiAnimators.RemoveAll((PsiAnimator x) => x.Complete);
			foreach (DamageNumber damageNumber in this.damageNumbers)
			{
				damageNumber.Update();
			}
			if (this.youWon != null)
			{
				this.youWon.Update();
			}
			if (this.groovy != null)
			{
				this.groovy.Update();
			}
			this.comboCircle.Update();
			this.dimmer.Update();
			if (this.topLetterboxY < this.topLetterboxTargetY - 0.5f || this.topLetterboxY > this.topLetterboxTargetY + 0.5f)
			{
				this.topLetterboxY += (this.topLetterboxTargetY - this.topLetterboxY) / 10f;
				this.topLetterbox.Position = new Vector2f(this.topLetterbox.Position.X, (float)((int)this.topLetterboxY));
			}
			else if ((int)this.topLetterboxY != (int)this.topLetterboxTargetY)
			{
				this.topLetterboxY = this.topLetterboxTargetY;
				this.topLetterbox.Position = new Vector2f(this.topLetterbox.Position.X, (float)((int)this.topLetterboxY));
			}
			if (this.bottomLetterboxY > this.bottomLetterboxTargetY + 0.5f || this.bottomLetterboxY < this.bottomLetterboxTargetY - 0.5f)
			{
				this.bottomLetterboxY += (this.bottomLetterboxTargetY - this.bottomLetterboxY) / 10f;
				this.bottomLetterbox.Position = new Vector2f(this.bottomLetterbox.Position.X, (float)((int)this.bottomLetterboxY));
			}
			else if ((int)this.bottomLetterboxY != (int)this.bottomLetterboxTargetY)
			{
				this.bottomLetterboxY = this.bottomLetterboxTargetY;
				this.bottomLetterbox.Position = new Vector2f(this.bottomLetterbox.Position.X, (float)((int)this.bottomLetterboxY));
			}
			if (this.textboxHideFlag)
			{
				this.textbox.Hide();
				this.textboxHideFlag = false;
			}
		}

		public void Draw(RenderTarget target)
		{
			target.Draw(this.topLetterbox);
			target.Draw(this.bottomLetterbox);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					foreach (Graphic graphic in this.enemyGraphics.Values)
					{
						graphic.Dispose();
					}
					this.jingler.Stop();
					this.jingler.Dispose();
				}
				AudioManager.Instance.Unuse(this.moveBeepX);
				AudioManager.Instance.Unuse(this.moveBeepY);
				AudioManager.Instance.Unuse(this.selectBeep);
				AudioManager.Instance.Unuse(this.cancelBeep);
				AudioManager.Instance.Unuse(this.prePlayerAttack);
				AudioManager.Instance.Unuse(this.preEnemyAttack);
				AudioManager.Instance.Unuse(this.prePsiSound);
				AudioManager.Instance.Unuse(this.talkSound);
				AudioManager.Instance.Unuse(this.enemyDeathSound);
				foreach (KeyValuePair<CharacterType, List<CarbineSound>> keyValuePair in this.comboSoundMap)
				{
					List<CarbineSound> value = keyValuePair.Value;
					foreach (CarbineSound sound in value)
					{
						AudioManager.Instance.Unuse(sound);
					}
				}
				AudioManager.Instance.Unuse(this.smashSound);
				AudioManager.Instance.Unuse(this.comboHitA);
				AudioManager.Instance.Unuse(this.comboHitB);
				AudioManager.Instance.Unuse(this.comboSuccess);
				AudioManager.Instance.Unuse(this.groovySound);
				AudioManager.Instance.Unuse(this.reflectSound);
				foreach (CarbineSound sound2 in this.winSounds.Values)
				{
					AudioManager.Instance.Unuse(sound2);
				}
				this.textbox.OnTextboxComplete -= this.TextboxComplete;
				this.textbox.OnTextTrigger -= this.TextTrigger;
				InputManager.Instance.AxisPressed -= this.AxisPressed;
				InputManager.Instance.ButtonPressed -= this.ButtonPressed;
			}
			this.disposed = true;
		}

		private const int TOP_LETTERBOX_HEIGHT = 14;

		private const int BOTTOM_LETTERBOX_HEIGHT = 35;

		private const float LETTERBOX_SPEED_FACTOR = 10f;

		private const int ENEMY_SPACING = 10;

		private const int ENEMY_DEPTH = 0;

		private const int ENEMY_TRANSLATE_FRAMES = 10;

		private const int ENEMY_DEATH_FRAMES = 40;

		public const int ENEMY_MIDLINE = 78;

		public const int ENEMY_OFFSET = 12;

		private bool disposed;

		private RenderPipeline pipeline;

		private ActorManager actorManager;

		private CombatantController combatantController;

		private Shape topLetterbox;

		private Shape bottomLetterbox;

		private float topLetterboxY;

		private float bottomLetterboxY;

		private float topLetterboxTargetY;

		private float bottomLetterboxTargetY;

		private ButtonBar buttonBar;

		private SectionedPsiBox psiMenu;

		private CardBar cardBar;

		private Dictionary<int, IndexedColorGraphic> enemyGraphics;

		private BattleTextbox textbox;

		private ScreenDimmer dimmer;

		private ComboAnimator comboCircle;

		private int selectedEnemyID;

		private int enemySelectIndex;

		private List<int> enemyIDs;

		private List<IGraphicModifier> graphicModifiers;

		private List<PsiAnimator> psiAnimators;

		private BattleInterfaceController.State state;

		private SelectionState selectionState;

		private Groovy groovy;

		private CarbineSound moveBeepX;

		private CarbineSound moveBeepY;

		private CarbineSound selectBeep;

		private CarbineSound cancelBeep;

		private CarbineSound prePlayerAttack;

		private CarbineSound preEnemyAttack;

		private CarbineSound prePsiSound;

		private CarbineSound talkSound;

		private CarbineSound enemyDeathSound;

		private CarbineSound smashSound;

		private Dictionary<CharacterType, List<CarbineSound>> comboSoundMap;

		private CarbineSound comboHitA;

		private CarbineSound comboHitB;

		private CarbineSound hitSound;

		private CarbineSound comboSuccess;

		private CarbineSound groovySound;

		private CarbineSound reflectSound;

		private Dictionary<int, CarbineSound> winSounds;

		private YouWon youWon;

		private LevelUpJingler jingler;

		private List<DamageNumber> damageNumbers;

		private bool textboxHideFlag;

		private bool isUndoAllowed;

		private int activeCharacter;

		private enum State
		{
			Waiting,
			TopLevelSelection,
			PsiTypeSelection,
			PsiAttackSelection,
			SpecialSelection,
			ItemSelection,
			EnemySelection,
			AllySelection
		}

		public delegate void InteractionCompletionHandler(SelectionState state);

		public delegate void TextboxCompletionHandler();
	}
}
