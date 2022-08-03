using System;
using System.Collections.Generic;
using Carbine.Actors;
using Carbine.Audio;
using Carbine.Graphics;
using Carbine.Input;
using Carbine.Utility;
using Mother4.Battle.Combatants;
using Mother4.Battle.PsiAnimation;
using Mother4.Battle.UI;
using Mother4.Battle.UI.Modifiers;
using Mother4.Data;
using Mother4.Data.Psi;
using Mother4.GUI.Modifiers;

using Mother4.Utility;
using SFML.Graphics;
using SFML.System;

namespace Mother4.Battle
{
	// Token: 0x020000C6 RID: 198
	internal class BattleInterfaceController : IDisposable
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x0001B203 File Offset: 0x00019403
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x0001B20B File Offset: 0x0001940B
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x0001B214 File Offset: 0x00019414
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x0001B21C File Offset: 0x0001941C
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0001B225 File Offset: 0x00019425
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x0001B22D File Offset: 0x0001942D
		public bool RunAttempted { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0001B236 File Offset: 0x00019436
		public CarbineSound PrePlayerAttack
		{
			get
			{
				return this.prePlayerAttack;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x0001B23E File Offset: 0x0001943E
		public CarbineSound PreEnemyAttack
		{
			get
			{
				return this.preEnemyAttack;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x0001B246 File Offset: 0x00019446
		public CarbineSound PrePsiSound
		{
			get
			{
				return this.prePsiSound;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x0001B24E File Offset: 0x0001944E
		public CarbineSound TalkSound
		{
			get
			{
				return this.talkSound;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000437 RID: 1079 RVA: 0x0001B256 File Offset: 0x00019456
		public CarbineSound EnemyDeathSound
		{
			get
			{
				return this.enemyDeathSound;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x0001B25E File Offset: 0x0001945E
		public CarbineSound GroovySound
		{
			get
			{
				return this.groovySound;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x0001B266 File Offset: 0x00019466
		public CarbineSound ReflectSound
		{
			get
			{
				return this.reflectSound;
			}
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600043A RID: 1082 RVA: 0x0001B270 File Offset: 0x00019470
		// (remove) Token: 0x0600043B RID: 1083 RVA: 0x0001B2A8 File Offset: 0x000194A8
		public event BattleInterfaceController.InteractionCompletionHandler OnInteractionComplete;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600043C RID: 1084 RVA: 0x0001B2E0 File Offset: 0x000194E0
		// (remove) Token: 0x0600043D RID: 1085 RVA: 0x0001B318 File Offset: 0x00019518
		public event BattleInterfaceController.TextboxCompletionHandler OnTextboxComplete;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600043E RID: 1086 RVA: 0x0001B350 File Offset: 0x00019550
		// (remove) Token: 0x0600043F RID: 1087 RVA: 0x0001B388 File Offset: 0x00019588
		public event BattleInterfaceController.TextTriggerHandler OnTextTrigger;

		// Token: 0x06000440 RID: 1088 RVA: 0x0001B3C0 File Offset: 0x000195C0
		public BattleInterfaceController(RenderPipeline pipeline, ActorManager actorManager, CombatantController combatantController, bool letterboxing)
		{
			this.pipeline = pipeline;
			this.actorManager = actorManager;
			this.combatantController = combatantController;
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
			this.psiMenu = new BattlePsiBox(array);
			this.pipeline.Add(this.psiMenu);
			this.selectionMarkers = new Dictionary<Graphic, Graphic>();
			for (int j = 0; j < array.Length; j++)
			{
				Graphic cardGraphic = this.cardBar.GetCardGraphic(j);
				Graphic graphic = new IndexedColorGraphic(Paths.GRAPHICS + "cursor.dat", "down", VectorMath.Truncate(cardGraphic.Position - cardGraphic.Origin + new Vector2f(cardGraphic.Size.X / 2f, 4f)), cardGraphic.Depth + 10);
				graphic.Visible = false;
				this.pipeline.Add(graphic);
				this.selectionMarkers.Add(cardGraphic, graphic);
			}
			this.enemyGraphics = new Dictionary<int, IndexedColorGraphic>();
			this.enemyIDs = new List<int>();
			this.partyIDs = new List<int>();
			foreach (Combatant combatant in combatantController.CombatantList)
			{
				switch (combatant.Faction)
				{
					case BattleFaction.PlayerTeam:
						{
							PlayerCombatant playerCombatant = (PlayerCombatant)combatant;
							playerCombatant.OnStatChange += this.OnPlayerStatChange;
							playerCombatant.OnStatusEffectChange += this.OnPlayerStatusEffectChange;
							this.partyIDs.Add(playerCombatant.ID);
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
							Graphic graphic2 = new IndexedColorGraphic(Paths.GRAPHICS + "cursor.dat", "down", VectorMath.Truncate(indexedColorGraphic.Position - indexedColorGraphic.Origin + new Vector2f(indexedColorGraphic.Size.X / 2f, 4f)), indexedColorGraphic.Depth + 10);
							graphic2.Visible = false;
							this.pipeline.Add(graphic2);
							this.selectionMarkers.Add(indexedColorGraphic, graphic2);
							break;
						}
				}
			}
			this.AlignEnemyGraphics();
			this.textbox = new BattleTextBox();
			this.textbox.OnTextboxComplete += this.TextboxComplete;
			this.textbox.OnTextTrigger += this.TextTrigger;
			pipeline.Add(this.textbox);
			this.dimmer = new ScreenDimmer(pipeline, Color.Transparent, 0, 15);
			this.state = BattleInterfaceController.State.Waiting;
			this.selectionState = default(SelectionState);
			this.selectedTargetId = -1;
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
			for (int k = 0; k < array.Length; k++)
			{
				List<CarbineSound> list;
				if (this.comboSoundMap.ContainsKey(array[k]))
				{
					list = this.comboSoundMap[array[k]];
				}
				else
				{
					list = new List<CarbineSound>();
					this.comboSoundMap.Add(array[k], list);
				}
				for (int l = 0; l < 3; l++)
				{
					string str = CharacterComboSounds.Get(array[k], 0, l, 120);
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

		// Token: 0x06000441 RID: 1089 RVA: 0x0001BBF8 File Offset: 0x00019DF8
		~BattleInterfaceController()
		{
			this.Dispose(false);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x0001BC28 File Offset: 0x00019E28
		private void TextTrigger(int type, string[] args)
		{
			switch (type)
			{
				case 0:
					this.youWon = new YouWon(this.pipeline);
					return;
				case 1:
					{
						CharacterType character;
						bool flag = Enum.TryParse<CharacterType>(args[0], true, out character);
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
						int.TryParse(args[0], out i);
						int.TryParse(args[1], out hp);
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
						int.TryParse(args[0], out i2);
						int.TryParse(args[1], out pp);
						StatSet statChange2 = new StatSet
						{
							PP = pp
						};
						this.combatantController[i2].AlterStats(statChange2);
						return;
					}
				default:
					if (this.OnTextTrigger != null)
					{
						this.OnTextTrigger(type, args);
					}
					break;
			}
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x0001BD22 File Offset: 0x00019F22
		public void PlayWinBGM(int type)
		{
			if (this.winSounds.ContainsKey(type))
			{
				this.winSounds[type].Play();
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0001BD44 File Offset: 0x00019F44
		public void StopWinBGM()
		{
			foreach (CarbineSound carbineSound in this.winSounds.Values)
			{
				carbineSound.Stop();
			}
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x0001BD9C File Offset: 0x00019F9C
		public void PlayLevelUpBGM()
		{
			this.jingler.Play();
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x0001BDA9 File Offset: 0x00019FA9
		public void EndLevelUpBGM()
		{
			this.jingler.End();
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x0001BDB6 File Offset: 0x00019FB6
		public void StopLevelUpBGM()
		{
			this.jingler.Stop();
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x0001BDC4 File Offset: 0x00019FC4
		private CarbineSound GetComboSound(CharacterType character, int index)
		{
			CarbineSound result = null;
			if (this.comboSoundMap.ContainsKey(character))
			{
				result = this.comboSoundMap[character][index % this.comboSoundMap[character].Count];
			}
			return result;
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0001BE08 File Offset: 0x0001A008
		private void OnPlayerStatChange(Combatant sender, StatSet change)
		{
			PlayerCombatant playerCombatant = (PlayerCombatant)sender;
			this.UpdatePlayerCard(playerCombatant.ID, playerCombatant.Stats.HP, playerCombatant.Stats.PP, playerCombatant.Stats.Meter);
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x0001BE4C File Offset: 0x0001A04C
		private void OnPlayerStatusEffectChange(Combatant sender, StatusEffect statusEffect, bool added)
		{
			if (added)
			{
				if (statusEffect == StatusEffect.Talking)
				{
					this.TalkifyPlayer(sender as PlayerCombatant);
					this.SetCardSpring(sender.ID, BattleCard.SpringMode.BounceUp, new Vector2f(0f, 8f), new Vector2f(0f, 0.1f), new Vector2f(0f, 1f));
					return;
				}
				switch (statusEffect)
				{
					case StatusEffect.Shield:
						this.SetCardGlow(sender.ID, BattleCard.GlowType.Shield);
						return;
					case StatusEffect.PsiShield:
						this.SetCardGlow(sender.ID, BattleCard.GlowType.PsiSheild);
						return;
					case StatusEffect.Counter:
						this.SetCardGlow(sender.ID, BattleCard.GlowType.Counter);
						return;
					case StatusEffect.PsiCounter:
						this.SetCardGlow(sender.ID, BattleCard.GlowType.PsiCounter);
						return;
					case StatusEffect.Eraser:
						this.SetCardGlow(sender.ID, BattleCard.GlowType.Eraser);
						return;
					default:
						return;
				}
			}
			else
			{
				if (statusEffect == StatusEffect.Talking)
				{
					this.RemoveTalker(this.cardBar.GetCardGraphic(sender.ID));
					this.SetCardSpring(sender.ID, BattleCard.SpringMode.Normal, new Vector2f(0f, 0f), new Vector2f(0f, 0f), new Vector2f(0f, 0f));
					return;
				}
				switch (statusEffect)
				{
					case StatusEffect.Shield:
					case StatusEffect.PsiShield:
					case StatusEffect.Counter:
					case StatusEffect.PsiCounter:
					case StatusEffect.Eraser:
						this.SetCardGlow(sender.ID, BattleCard.GlowType.None);
						return;
					default:
						return;
				}
			}
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0001BF94 File Offset: 0x0001A194
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

		// Token: 0x0600044C RID: 1100 RVA: 0x0001BFD8 File Offset: 0x0001A1D8
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

		// Token: 0x0600044D RID: 1101 RVA: 0x0001C0BC File Offset: 0x0001A2BC
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

		// Token: 0x0600044E RID: 1102 RVA: 0x0001C1D8 File Offset: 0x0001A3D8
		public void StartComboCircle(EnemyCombatant enemy)
		{
			Graphic graphic = this.enemyGraphics[enemy.ID];
			this.comboCircle.Setup(graphic);
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0001C203 File Offset: 0x0001A403
		public void StopComboCircle(bool explode)
		{
			this.comboCircle.Stop(explode);
			if (explode)
			{
				this.comboSuccess.Stop();
				this.comboSuccess.Play();
			}
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0001C22C File Offset: 0x0001A42C
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

		// Token: 0x06000451 RID: 1105 RVA: 0x0001C2D7 File Offset: 0x0001A4D7
		public bool IsComboCircleDone()
		{
			return this.comboCircle.Stopped;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0001C2E4 File Offset: 0x0001A4E4
		public void FlashEnemy(EnemyCombatant combatant, Color color, int duration, int count)
		{
			this.FlashEnemy(combatant, color, ColorBlendMode.Multiply, duration, count);
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001C2F2 File Offset: 0x0001A4F2
		public void FlashEnemy(EnemyCombatant combatant, Color color, ColorBlendMode blendMode, int duration, int count)
		{
			this.graphicModifiers.Add(new GraphicFader(this.enemyGraphics[combatant.ID], color, blendMode, duration, count));
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001C31B File Offset: 0x0001A51B
		public void BlinkEnemy(EnemyCombatant combatant, int duration, int count)
		{
			this.graphicModifiers.Add(new GraphicBlinker(this.enemyGraphics[combatant.ID], duration, count));
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0001C340 File Offset: 0x0001A540
		public void TalkifyPlayer(PlayerCombatant combatant)
		{
			this.graphicModifiers.Add(new GraphicTalker(this.pipeline, this.cardBar.GetCardGraphic(combatant.ID)));
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001C36C File Offset: 0x0001A56C
		public void TalkifyEnemy(EnemyCombatant combatant)
		{
			this.graphicModifiers.Add(new GraphicTalker(this.pipeline, this.enemyGraphics[combatant.ID]));
			this.graphicModifiers.Add(new GraphicBouncer(this.enemyGraphics[combatant.ID], GraphicBouncer.SpringMode.BounceUp, new Vector2f(0f, 4f), new Vector2f(0f, 0.1f), new Vector2f(0f, 1f)));
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001C42C File Offset: 0x0001A62C
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

		// Token: 0x06000458 RID: 1112 RVA: 0x0001C4E4 File Offset: 0x0001A6E4
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

		// Token: 0x06000459 RID: 1113 RVA: 0x0001C56C File Offset: 0x0001A76C
		public void AddShieldAnimation(Combatant combatant)
		{
			Graphic graphic = null;
			if (combatant is PlayerCombatant)
			{
				graphic = this.cardBar.GetCardGraphic(combatant.ID);
			}
			else if (combatant is EnemyCombatant)
			{
				graphic = this.enemyGraphics[combatant.ID];
			}
			if (graphic != null)
			{
				this.graphicModifiers.Add(new GraphicShielder(this.pipeline, graphic));
			}
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001C5CC File Offset: 0x0001A7CC
		private void SetSelectionMarkerVisibility(Graphic graphic, bool visible)
		{
			Graphic graphic2 = this.selectionMarkers[graphic];
			if (visible)
			{
				graphic2.Position = VectorMath.Truncate(graphic.Position - graphic.Origin + new Vector2f(graphic.Size.X / 2f, 4f));
				graphic2.Depth = 32767;
				this.pipeline.Update(graphic2);
			}
			graphic2.Visible = visible;
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001C67C File Offset: 0x0001A87C
		private void ResetTargetingSelection()
		{
			using (Dictionary<int, IndexedColorGraphic>.Enumerator enumerator = this.enemyGraphics.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, IndexedColorGraphic> kvp = enumerator.Current;
					this.graphicModifiers.RemoveAll(delegate (IGraphicModifier x)
					{
						IndexedColorGraphic graphic = x.Graphic;
						KeyValuePair<int, IndexedColorGraphic> kvp17 = kvp;
						return graphic == kvp17.Value && x is GraphicFader;
					});
					if (this.selectionState.TargetingMode == TargetingMode.Enemy)
					{
						KeyValuePair<int, IndexedColorGraphic> kvp18 = kvp;
						if (kvp18.Key == this.selectedTargetId)
						{
							KeyValuePair<int, IndexedColorGraphic> kvp2 = kvp;
							kvp2.Value.Color = Color.White;
							List<IGraphicModifier> list = this.graphicModifiers;
							KeyValuePair<int, IndexedColorGraphic> kvp3 = kvp;
							list.Add(new GraphicFader(kvp3.Value, new Color(64, 64, 64), ColorBlendMode.Screen, 30, -1));
							KeyValuePair<int, IndexedColorGraphic> kvp4 = kvp;
							this.SetSelectionMarkerVisibility(kvp4.Value, true);
						}
						else
						{
							KeyValuePair<int, IndexedColorGraphic> kvp5 = kvp;
							kvp5.Value.ColorBlendMode = ColorBlendMode.Multiply;
							KeyValuePair<int, IndexedColorGraphic> kvp6 = kvp;
							kvp6.Value.Color = new Color(128, 128, 128);
							KeyValuePair<int, IndexedColorGraphic> kvp7 = kvp;
							this.SetSelectionMarkerVisibility(kvp7.Value, false);
						}
					}
					else if (this.selectionState.TargetingMode == TargetingMode.AllEnemies)
					{
						KeyValuePair<int, IndexedColorGraphic> kvp8 = kvp;
						kvp8.Value.Color = Color.White;
						List<IGraphicModifier> list2 = this.graphicModifiers;
						KeyValuePair<int, IndexedColorGraphic> kvp9 = kvp;
						list2.Add(new GraphicFader(kvp9.Value, new Color(64, 64, 64), ColorBlendMode.Screen, 30, -1));
						KeyValuePair<int, IndexedColorGraphic> kvp10 = kvp;
						this.SetSelectionMarkerVisibility(kvp10.Value, true);
					}
					else if (this.selectionState.TargetingMode == TargetingMode.PartyMember || this.selectionState.TargetingMode == TargetingMode.AllPartyMembers)
					{
						KeyValuePair<int, IndexedColorGraphic> kvp11 = kvp;
						kvp11.Value.ColorBlendMode = ColorBlendMode.Multiply;
						KeyValuePair<int, IndexedColorGraphic> kvp12 = kvp;
						kvp12.Value.Color = new Color(128, 128, 128);
						KeyValuePair<int, IndexedColorGraphic> kvp13 = kvp;
						this.SetSelectionMarkerVisibility(kvp13.Value, false);
					}
					else
					{
						KeyValuePair<int, IndexedColorGraphic> kvp14 = kvp;
						kvp14.Value.ColorBlendMode = ColorBlendMode.Multiply;
						KeyValuePair<int, IndexedColorGraphic> kvp15 = kvp;
						kvp15.Value.Color = Color.White;
						KeyValuePair<int, IndexedColorGraphic> kvp16 = kvp;
						this.SetSelectionMarkerVisibility(kvp16.Value, false);
					}
				}
			}
			for (int i = 0; i < this.partyIDs.Count; i++)
			{
				Graphic cardGraphic = this.cardBar.GetCardGraphic(i);
				if (this.selectionState.TargetingMode == TargetingMode.PartyMember)
				{
					this.SetSelectionMarkerVisibility(cardGraphic, this.partySelectIndex == i);
				}
				else if (this.selectionState.TargetingMode == TargetingMode.AllPartyMembers)
				{
					this.SetSelectionMarkerVisibility(cardGraphic, true);
				}
				else
				{
					this.SetSelectionMarkerVisibility(cardGraphic, false);
				}
			}
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0001C9A4 File Offset: 0x0001ABA4
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

		// Token: 0x0600045D RID: 1117 RVA: 0x0001CB94 File Offset: 0x0001AD94
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

		// Token: 0x0600045E RID: 1118 RVA: 0x0001CBDE File Offset: 0x0001ADDE
		private void TextboxComplete()
		{
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

		// Token: 0x0600045F RID: 1119 RVA: 0x0001CC18 File Offset: 0x0001AE18
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
						this.selectedTargetId = this.enemyIDs[this.enemySelectIndex];
						this.ResetTargetingSelection();
						return;
					}
					if (flag2)
					{
						this.enemySelectIndex++;
						if (this.enemySelectIndex >= this.enemyIDs.Count)
						{
							this.enemySelectIndex = 0;
						}
						this.selectedTargetId = this.enemyIDs[this.enemySelectIndex];
						this.ResetTargetingSelection();
						return;
					}
					break;
				case BattleInterfaceController.State.AllySelection:
					if (flag)
					{
						this.partySelectIndex--;
						if (this.partySelectIndex < 0)
						{
							this.partySelectIndex = this.partyIDs.Count - 1;
						}
						this.selectedTargetId = this.partyIDs[this.partySelectIndex];
						this.ResetTargetingSelection();
						return;
					}
					if (flag2)
					{
						this.partySelectIndex++;
						if (this.partySelectIndex >= this.partyIDs.Count)
						{
							this.partySelectIndex = 0;
						}
						this.selectedTargetId = this.partyIDs[this.partySelectIndex];
						this.ResetTargetingSelection();
					}
					break;
				default:
					return;
			}
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001CE44 File Offset: 0x0001B044
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
				case BattleInterfaceController.State.SpecialSelection:
					this.SpecialSelection(b);
					return;
				case BattleInterfaceController.State.ItemSelection:
					this.ItemSelection(b);
					return;
				case BattleInterfaceController.State.EnemySelection:
				case BattleInterfaceController.State.AllySelection:
					this.TargetSelection(b);
					break;
				default:
					return;
			}
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0001CEC7 File Offset: 0x0001B0C7
		private PlayerCombatant CurrentPlayerCombatant()
		{
			return (PlayerCombatant)this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam)[this.cardBar.SelectedIndex];
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0001CEE8 File Offset: 0x0001B0E8
		private void StartTargetSelection()
		{
			if (this.selectionState.TargetingMode == TargetingMode.None)
			{
				this.CompleteTargetSelection(this.buttonBar.SelectedAction);
				return;
			}
			if (this.selectionState.TargetingMode == TargetingMode.Enemy)
			{
				this.state = BattleInterfaceController.State.EnemySelection;
				this.selectedTargetId = this.enemyIDs[this.enemySelectIndex % this.enemyIDs.Count];
			}
			else if (this.selectionState.TargetingMode == TargetingMode.AllEnemies)
			{
				this.state = BattleInterfaceController.State.EnemySelection;
				this.selectedTargetId = -1;
			}
			else if (this.selectionState.TargetingMode == TargetingMode.PartyMember)
			{
				this.state = BattleInterfaceController.State.AllySelection;
				this.selectedTargetId = this.partyIDs[this.partySelectIndex % this.partyIDs.Count];
			}
			else if (this.selectionState.TargetingMode == TargetingMode.AllPartyMembers)
			{
				this.state = BattleInterfaceController.State.AllySelection;
				this.selectedTargetId = -1;
			}
			this.buttonBar.Hide();
			this.ResetTargetingSelection();
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0001CFD4 File Offset: 0x0001B1D4
		private void TopLevelSelection(Button b)
		{
			switch (b)
			{
				case Button.A:
					switch (this.buttonBar.SelectedAction)
					{
						case ButtonBar.Action.Bash:
							this.selectionState.TargetingMode = TargetingMode.Enemy;
							this.StartTargetSelection();
							return;
						case ButtonBar.Action.Psi:
							{
								PlayerCombatant playerCombatant = this.CurrentPlayerCombatant();
								this.state = BattleInterfaceController.State.PsiTypeSelection;
								this.buttonBar.Hide();
								this.psiMenu.Show(playerCombatant.Character);
								return;
							}
						case ButtonBar.Action.Items:
							this.state = BattleInterfaceController.State.ItemSelection;
							this.buttonBar.Hide();
							return;
						case ButtonBar.Action.Talk:
							this.selectionState.TargetingMode = TargetingMode.Enemy;
							this.state = BattleInterfaceController.State.EnemySelection;
							this.buttonBar.Hide();
							this.selectedTargetId = this.enemyIDs[this.enemySelectIndex % this.enemyIDs.Count];
							this.ResetTargetingSelection();
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

		// Token: 0x06000464 RID: 1124 RVA: 0x0001D0FC File Offset: 0x0001B2FC
		private void PsiTypeSelection(Button b)
		{
			switch (b)
			{
				case Button.A:
					{
						if (!this.psiMenu.HasSelection)
						{
							this.psiMenu.Accept();
							return;
						}
						PsiLevel value = this.psiMenu.SelectedPsi.Value;
						PsiData data = PsiFile.Instance.GetData(value.PsiType);
						if ((int)data.PP[value.Level] <= this.CurrentPlayerCombatant().Stats.PP)
						{
							this.psiMenu.Hide();
							this.selectionState.Psi = value;
							this.selectionState.TargetingMode = (TargetingMode)data.Targets[value.Level];
							this.StartTargetSelection();
							return;
						}
						this.ShowTextBox("Not enough PP!", false);
						return;
					}
				case Button.B:
					if (this.psiMenu.HasSelection)
					{
						this.psiMenu.Cancel();
						return;
					}
					this.psiMenu.Hide();
					this.state = BattleInterfaceController.State.TopLevelSelection;
					this.ShowButtonBar();
					return;
				default:
					return;
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001D1F4 File Offset: 0x0001B3F4
		private void SpecialSelection(Button b)
		{
			switch (b)
			{
				case Button.A:
					break;
				case Button.B:
					this.state = BattleInterfaceController.State.TopLevelSelection;
					this.ShowButtonBar();
					break;
				default:
					return;
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001D220 File Offset: 0x0001B420
		private void ItemSelection(Button b)
		{
			switch (b)
			{
				case Button.A:
					break;
				case Button.B:
					this.state = BattleInterfaceController.State.TopLevelSelection;
					this.ShowButtonBar();
					break;
				default:
					return;
			}
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001D24C File Offset: 0x0001B44C
		private void TargetSelection(Button b)
		{
			switch (b)
			{
				case Button.A:
					this.CompleteTargetSelection(this.buttonBar.SelectedAction);
					return;
				case Button.B:
					this.selectedTargetId = -1;
					this.selectionState.TargetingMode = TargetingMode.None;
					this.ResetTargetingSelection();
					this.state = BattleInterfaceController.State.TopLevelSelection;
					this.ShowButtonBar();
					return;
				default:
					return;
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001D2A2 File Offset: 0x0001B4A2
		private void CompleteMenuUndo()
		{
			if (this.OnInteractionComplete != null)
			{
				this.selectionState.Type = SelectionState.SelectionType.Undo;
				this.OnInteractionComplete(this.selectionState);
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0001D2CC File Offset: 0x0001B4CC
		private void CompleteTargetSelection(ButtonBar.Action buttonAction)
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
				switch (this.selectionState.TargetingMode)
				{
					case TargetingMode.PartyMember:
					case TargetingMode.Enemy:
						this.selectionState.Targets = new Combatant[]
						{
						this.combatantController[this.selectedTargetId]
						};
						break;
					case TargetingMode.AllPartyMembers:
						this.selectionState.Targets = this.combatantController.GetFactionCombatants(BattleFaction.PlayerTeam);
						break;
					case TargetingMode.AllEnemies:
						this.selectionState.Targets = this.combatantController.GetFactionCombatants(BattleFaction.EnemyTeam);
						break;
				}
				this.selectionState.AttackIndex = 0;
				this.selectionState.ItemIndex = -1;
				this.state = BattleInterfaceController.State.Waiting;
				if (this.OnInteractionComplete != null)
				{
					this.OnInteractionComplete(this.selectionState);
				}
			}
			this.selectedTargetId = -1;
			this.selectionState.TargetingMode = TargetingMode.None;
			this.ResetTargetingSelection();
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0001D3F8 File Offset: 0x0001B5F8
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

		// Token: 0x0600046B RID: 1131 RVA: 0x0001D458 File Offset: 0x0001B658
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

		// Token: 0x0600046C RID: 1132 RVA: 0x0001D4B8 File Offset: 0x0001B6B8
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
			this.buttonBar.SetActions(BattleButtonBars.GetActions(character, showRun));
			this.buttonBar.Show(0);
			this.textbox.Hide();
			this.cardBar.SelectedIndex = num;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001D569 File Offset: 0x0001B769
		public void EndPlayerInteraction()
		{
			this.cardBar.SelectedIndex = -1;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001D577 File Offset: 0x0001B777
		public void SetActiveCard(int index)
		{
			this.cardBar.SelectedIndex = index;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001D585 File Offset: 0x0001B785
		public void PopCard(int index, int height)
		{
			this.cardBar.PopCard(index, height);
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001D594 File Offset: 0x0001B794
		public void SetCardSpring(int index, BattleCard.SpringMode mode, Vector2f amplitude, Vector2f speed, Vector2f decay)
		{
			this.cardBar.SetSpring(index, mode, amplitude, speed, decay);
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001D5A8 File Offset: 0x0001B7A8
		public void SetCardGroovy(int index, bool groovy)
		{
			this.cardBar.SetGroovy(index, groovy);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001D5B7 File Offset: 0x0001B7B7
		public void AddCardSpring(int index, Vector2f amplitude, Vector2f speed, Vector2f decay)
		{
			this.cardBar.AddSpring(index, amplitude, speed, decay);
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0001D5C9 File Offset: 0x0001B7C9
		public void SetCardGlow(int index, BattleCard.GlowType type)
		{
			this.cardBar.SetGlow(index, type);
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001D5D8 File Offset: 0x0001B7D8
		public void HideButtonBar()
		{
			this.buttonBar.Hide();
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x0001D5E5 File Offset: 0x0001B7E5
		public void ShowButtonBar()
		{
			this.textbox.Hide();
			this.buttonBar.Show();
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x0001D600 File Offset: 0x0001B800
		public void ShowTextBox(string message, bool useButton)
		{
			this.textbox.AutoScroll = !useButton;
			if (this.textbox.HasPrinted)
			{
				this.textbox.Enqueue(new PrintAction(PrintActionType.LineBreak, new object[0]));
			}
			TextProcessor textProcessor = new TextProcessor(message);
			this.textbox.EnqueueAll(textProcessor.Actions);
			this.textbox.Enqueue(new PrintAction(PrintActionType.Prompt, new object[0]));
			this.textbox.Show();
			this.buttonBar.Hide();
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001D685 File Offset: 0x0001B885
		public void ClearTextBox()
		{
			this.textbox.Clear();
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0001D692 File Offset: 0x0001B892
		public void HideTextBox()
		{
			this.textbox.Hide();
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0001D69F File Offset: 0x0001B89F
		public void SetLetterboxing(float letterboxing)
		{
			this.topLetterboxTargetY = (float)(-(float)((int)(14f * (1f - letterboxing))));
			this.bottomLetterboxTargetY = (float)(180L - (long)((int)(35f * letterboxing)));
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0001D6D0 File Offset: 0x0001B8D0
		public void AddEnemy(int id)
		{
			EnemyCombatant enemyCombatant = (EnemyCombatant)this.combatantController[id];
			this.enemyIDs.Add(id);
			new IndexedColorGraphic(EnemyGraphics.GetFilename(enemyCombatant.Enemy), "front", default(Vector2f), 0);
			this.AlignEnemyGraphics();
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0001D721 File Offset: 0x0001B921
		public void DoEnemyDeathAnimation(int id)
		{
			this.enemyDeathSound.Play();
			this.graphicModifiers.Add(new GraphicDeathFader(this.enemyGraphics[id], 40));
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001D74C File Offset: 0x0001B94C
		public void RemoveAllModifiers()
		{
			this.graphicModifiers.Clear();
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0001D784 File Offset: 0x0001B984
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

		// Token: 0x0600047E RID: 1150 RVA: 0x0001D830 File Offset: 0x0001BA30
		public void UpdatePlayerCard(int id, int hp, int pp, float meter)
		{
			PlayerCombatant playerCombatant = (PlayerCombatant)this.combatantController[id];
			this.cardBar.SetHP(playerCombatant.PartyIndex, hp);
			this.cardBar.SetPP(playerCombatant.PartyIndex, pp);
			this.cardBar.SetMeter(playerCombatant.PartyIndex, meter);
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001D898 File Offset: 0x0001BA98
		public void Update()
		{
			this.textbox.Update();
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

		// Token: 0x06000480 RID: 1152 RVA: 0x0001DB90 File Offset: 0x0001BD90
		public void Draw(RenderTarget target)
		{
			target.Draw(this.topLetterbox);
			target.Draw(this.bottomLetterbox);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0001DBAA File Offset: 0x0001BDAA
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001DBBC File Offset: 0x0001BDBC
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
					foreach (Graphic graphic2 in this.selectionMarkers.Values)
					{
						graphic2.Dispose();
					}
					foreach (IGraphicModifier graphicModifier in this.graphicModifiers)
					{
						if (graphicModifier is IDisposable)
						{
							((IDisposable)graphicModifier).Dispose();
						}
					}
					this.jingler.Stop();
					this.jingler.Dispose();
					this.cardBar.Dispose();
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

		// Token: 0x04000621 RID: 1569
		private const int TOP_LETTERBOX_HEIGHT = 14;

		// Token: 0x04000622 RID: 1570
		private const int BOTTOM_LETTERBOX_HEIGHT = 35;

		// Token: 0x04000623 RID: 1571
		private const float LETTERBOX_SPEED_FACTOR = 10f;

		// Token: 0x04000624 RID: 1572
		private const int ENEMY_SPACING = 10;

		// Token: 0x04000625 RID: 1573
		private const int ENEMY_DEPTH = 0;

		// Token: 0x04000626 RID: 1574
		private const int ENEMY_TRANSLATE_FRAMES = 10;

		// Token: 0x04000627 RID: 1575
		private const int ENEMY_DEATH_FRAMES = 40;

		// Token: 0x04000628 RID: 1576
		public const int ENEMY_MIDLINE = 78;

		// Token: 0x04000629 RID: 1577
		public const int ENEMY_OFFSET = 12;

		// Token: 0x0400062A RID: 1578
		private bool disposed;

		// Token: 0x0400062B RID: 1579
		private RenderPipeline pipeline;

		// Token: 0x0400062C RID: 1580
		private ActorManager actorManager;

		// Token: 0x0400062D RID: 1581
		private CombatantController combatantController;

		// Token: 0x0400062E RID: 1582
		private Shape topLetterbox;

		// Token: 0x0400062F RID: 1583
		private Shape bottomLetterbox;

		// Token: 0x04000630 RID: 1584
		private float topLetterboxY;

		// Token: 0x04000631 RID: 1585
		private float bottomLetterboxY;

		// Token: 0x04000632 RID: 1586
		private float topLetterboxTargetY;

		// Token: 0x04000633 RID: 1587
		private float bottomLetterboxTargetY;

		// Token: 0x04000634 RID: 1588
		private ButtonBar buttonBar;

		// Token: 0x04000635 RID: 1589
		private BattlePsiBox psiMenu;

		// Token: 0x04000636 RID: 1590
		private CardBar cardBar;

		// Token: 0x04000637 RID: 1591
		private Dictionary<int, IndexedColorGraphic> enemyGraphics;

		// Token: 0x04000638 RID: 1592
		private BattleTextBox textbox;

		// Token: 0x04000639 RID: 1593
		private ScreenDimmer dimmer;

		// Token: 0x0400063A RID: 1594
		private ComboAnimator comboCircle;

		// Token: 0x0400063B RID: 1595
		private int selectedTargetId;

		// Token: 0x0400063C RID: 1596
		private int enemySelectIndex;

		// Token: 0x0400063D RID: 1597
		private int partySelectIndex;

		// Token: 0x0400063E RID: 1598
		private List<int> enemyIDs;

		// Token: 0x0400063F RID: 1599
		private List<int> partyIDs;

		// Token: 0x04000640 RID: 1600
		private List<IGraphicModifier> graphicModifiers;

		// Token: 0x04000641 RID: 1601
		private List<PsiAnimator> psiAnimators;

		// Token: 0x04000642 RID: 1602
		private BattleInterfaceController.State state;

		// Token: 0x04000643 RID: 1603
		private SelectionState selectionState;

		// Token: 0x04000644 RID: 1604
		private Groovy groovy;

		// Token: 0x04000645 RID: 1605
		private CarbineSound moveBeepX;

		// Token: 0x04000646 RID: 1606
		private CarbineSound moveBeepY;

		// Token: 0x04000647 RID: 1607
		private CarbineSound selectBeep;

		// Token: 0x04000648 RID: 1608
		private CarbineSound cancelBeep;

		// Token: 0x04000649 RID: 1609
		private CarbineSound prePlayerAttack;

		// Token: 0x0400064A RID: 1610
		private CarbineSound preEnemyAttack;

		// Token: 0x0400064B RID: 1611
		private CarbineSound prePsiSound;

		// Token: 0x0400064C RID: 1612
		private CarbineSound talkSound;

		// Token: 0x0400064D RID: 1613
		private CarbineSound enemyDeathSound;

		// Token: 0x0400064E RID: 1614
		private CarbineSound smashSound;

		// Token: 0x0400064F RID: 1615
		private Dictionary<CharacterType, List<CarbineSound>> comboSoundMap;

		// Token: 0x04000650 RID: 1616
		private CarbineSound comboHitA;

		// Token: 0x04000651 RID: 1617
		private CarbineSound comboHitB;

		// Token: 0x04000652 RID: 1618
		private CarbineSound hitSound;

		// Token: 0x04000653 RID: 1619
		private CarbineSound comboSuccess;

		// Token: 0x04000654 RID: 1620
		private CarbineSound groovySound;

		// Token: 0x04000655 RID: 1621
		private CarbineSound reflectSound;

		// Token: 0x04000656 RID: 1622
		private Dictionary<int, CarbineSound> winSounds;

		// Token: 0x04000657 RID: 1623
		private YouWon youWon;

		// Token: 0x04000658 RID: 1624
		private LevelUpJingler jingler;

		// Token: 0x04000659 RID: 1625
		private List<DamageNumber> damageNumbers;

		// Token: 0x0400065A RID: 1626
		private Dictionary<Graphic, Graphic> selectionMarkers;

		// Token: 0x0400065B RID: 1627
		private bool textboxHideFlag;

		// Token: 0x0400065C RID: 1628
		private bool isUndoAllowed;

		// Token: 0x0400065D RID: 1629
		private int activeCharacter;

		// Token: 0x020000C7 RID: 199
		private enum State
		{
			// Token: 0x04000666 RID: 1638
			Waiting,
			// Token: 0x04000667 RID: 1639
			TopLevelSelection,
			// Token: 0x04000668 RID: 1640
			PsiTypeSelection,
			// Token: 0x04000669 RID: 1641
			SpecialSelection,
			// Token: 0x0400066A RID: 1642
			ItemSelection,
			// Token: 0x0400066B RID: 1643
			EnemySelection,
			// Token: 0x0400066C RID: 1644
			AllySelection
		}

		// Token: 0x020000C8 RID: 200
		// (Invoke) Token: 0x06000487 RID: 1159
		public delegate void InteractionCompletionHandler(SelectionState state);

		// Token: 0x020000C9 RID: 201
		// (Invoke) Token: 0x0600048B RID: 1163
		public delegate void TextboxCompletionHandler();

		// Token: 0x020000CA RID: 202
		// (Invoke) Token: 0x0600048F RID: 1167
		public delegate void TextTriggerHandler(int type, string[] args);
	}
}
