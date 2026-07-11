using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Sora.SoraCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.ValueProps;
using Sora.SoraCode.Cards.Basic;
using Sora.SoraCode.Powers;
using Sora.SoraCode.Relics;

namespace Sora.SoraCode.Character;

public class Sora : PlaceholderCharacterModel
{
    public const string CharacterId = "Sora";
    
    private Vector2? _originalPosition;
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeSora>(),
        ModelDb.Card<StrikeSora>(),
        ModelDb.Card<StrikeSora>(),
        ModelDb.Card<StrikeSora>(),
        ModelDb.Card<SonicStrike>(),
        ModelDb.Card<DefendSora>(),
        ModelDb.Card<DefendSora>(),
        ModelDb.Card<DefendSora>(),
        ModelDb.Card<DefendSora>(),
        ModelDb.Card<DefendSora>(),
        ModelDb.Card<FullCombo>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<KingdomKey>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<SoraCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SoraRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SoraPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    
    public override CustomEnergyCounter? CustomEnergyCounter =>
	    new CustomEnergyCounter(EnergyCounterPaths, new Color(0.2f, 0.2f, 0.2f), new Color(1f, 1f, 1f));
    
    private string EnergyCounterPaths(int i)
    {
	    return i switch
	    {
		    1 => "charui/big_energy.png".ImagePath(),
		    _ => "charui/blank.png".ImagePath()
	    };
    }
    
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    
    private const string CustomVisualScenePath = "res://Sora/scenes/sora.tscn";
    public override string CustomCharacterSelectBg => "res://Sora/scenes/char_selection_bg_sora_new.tscn";
    public override string CustomIconTexturePath => "character_icon_sora.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_sora.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_sora.png".CharacterUiPath();
    public override string CharacterSelectSfx => "res://Sora/sfx/sora_select.wav";
    public override string CustomMerchantAnimPath => "res://Sora/scenes/sora_merchant.tscn";
    public override string CustomRestSiteAnimPath => "res://Sora/scenes/sora_rest_site.tscn";
    
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        // SoraAssets.EnsurePreloaded();
        return NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualScenePath);
    }
    
    public override CreatureAnimator? GenerateAnimator(MegaSprite controller) => null;
    
    public (float total, float[] impacts) PlayAnimation(Creature creature, string trigger)
    {

        if (creature == null || string.IsNullOrEmpty(trigger))
            return (0f, Array.Empty<float>());

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.Visuals == null)
            return (0f, Array.Empty<float>());

        var animPlayer = node.Visuals.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer == null)
            return (0f, Array.Empty<float>());

        var t = trigger.ToLowerInvariant();
        var mode = creature.HasPower<UltimateFormPower>() ? "ultimate" : "normal";
        
        string godotTrigger = t switch
        {

            "attack" => $"attack_{mode}",

            "block" => $"block_{mode}",

            "dash" => $"dash_{mode}",
            
            "retreat" => $"retreat_{mode}",

            "idle" or "idle_loop" => $"idle_{mode}",
            
            "hit" => $"hit_{mode}",

            "dead" or "die" => "dead",

            "cast" => $"cast_{mode}",

            _ => trigger
        };

        if (!animPlayer.HasAnimation(godotTrigger))
            return (0f, Array.Empty<float>());

        bool shouldRestartIfAlreadyPlaying =
            t == "attack" || t == "cast";


        if (shouldRestartIfAlreadyPlaying && animPlayer.CurrentAnimation == godotTrigger)
        {
            animPlayer.Seek(0, true);
        }

        else
        {
            animPlayer.Play(godotTrigger);
        }

        var anim = animPlayer.GetAnimation(godotTrigger);
        float totalLength = (float)anim.Length;

        if (godotTrigger is not ("die" or "dead"))
        {
	        if (creature.HasPower<UltimateFormPower>())
		        animPlayer.Queue("idle_ultimate");
	        else 
		        animPlayer.Queue("idle_normal");
        }

        return (totalLength, Array.Empty<float>());
    }
    
    public void DoScreenShake(ShakeStrength strength = ShakeStrength.Medium,
        ShakeDuration duration = ShakeDuration.Short)
    {
        NGame.Instance?.ScreenShake(strength, duration);
    }
    
    public Node2D PlayVfxOnTarget(Creature target, string path, string animName)
    {
        var targetNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (targetNode?.Visuals == null)
            return null;

        var scene = GD.Load<PackedScene>(path);
        var vfx = scene.Instantiate<Node2D>();

        targetNode.Visuals.AddChild(vfx);
        vfx.Position = Vector2.Zero;

        var animPlayer = vfx.GetNode<AnimationPlayer>("AnimationPlayer");

        if (animPlayer.HasAnimation(animName))
            animPlayer.Play(animName);

        return vfx;
    }
    
    private string ResolveDashAnimation(Creature creature)
    {
        return creature.HasPower<UltimateFormPower>() ? "dash_ultimate" : "dash_normal";
    }
    
    public async Task DashTo(
        Creature player,
        Creature target,
        float durationSeconds = 0.3f,
        float distance = 200f,
        bool dashBehind = false,
        string? overrideAnim = null)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(player);
        var targetNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (node == null || targetNode == null) return;

        if (!_originalPosition.HasValue)
            _originalPosition = node.GlobalPosition;

        string anim = overrideAnim ?? ResolveDashAnimation(player);
        PlayAnimation(player, anim);
		
        bool playerIsLeftOfTarget = node.GlobalPosition.X < targetNode.GlobalPosition.X;
		
        Vector2 offsetDir = playerIsLeftOfTarget ? Vector2.Left : Vector2.Right;
		
        if (dashBehind)
            offsetDir = -offsetDir;

        Vector2 targetPos = targetNode.GlobalPosition + offsetDir * distance;

        var tween = node.CreateTween();
        tween.TweenProperty(node, "global_position", targetPos, durationSeconds)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        await node.ToSignal(tween, Tween.SignalName.Finished);
    }
    
    public async Task DashToPosition(
	    Creature player,
	    Vector2 targetPos,
	    float durationSeconds = 0.3f,
	    string? overrideAnim = null)
    {
	    var node = NCombatRoom.Instance?.GetCreatureNode(player);
	    if (node == null)
		    return;

	    if (!_originalPosition.HasValue)
		    _originalPosition = node.GlobalPosition;

	    string anim = overrideAnim ?? ResolveDashAnimation(player);
	    PlayAnimation(player, anim);

	    var tween = node.CreateTween();

	    tween.TweenProperty(
			    node,
			    "global_position",
			    targetPos,
			    durationSeconds)
		    .SetTrans(Tween.TransitionType.Quad)
		    .SetEase(Tween.EaseType.Out);

	    await node.ToSignal(
		    tween,
		    Tween.SignalName.Finished);
    }
    
    public Vector2? GetPointLeftOfLeftMostEnemy(
	    IEnumerable<Creature> enemies,
	    float distance,
	    float? overrideY = null)
    {
	    var room = NCombatRoom.Instance;
	    if (room == null)
		    return null;

	    Vector2? leftMostPoint = null;

	    foreach (var enemy in enemies)
	    {
		    if (!enemy.IsAlive)
			    continue;

		    var enemyNode = room.GetCreatureNode(enemy);
		    if (enemyNode == null)
			    continue;

		    Vector2 point = enemyNode.GetBottomOfHitbox();

		    if (leftMostPoint == null || point.X < leftMostPoint.Value.X)
		    {
			    leftMostPoint = point;
		    }
	    }

	    if (leftMostPoint == null)
		    return null;

	    return new Vector2(
		    leftMostPoint.Value.X - distance,
		    overrideY ?? leftMostPoint.Value.Y
	    );
    }
    
    public async Task DashPast(
        Creature player,
        Creature target,
        string? attackAnim = null,
        float durationSeconds = 0.3f,
        float behindDistance = 200f,
        float overshoot = 0f)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(player);
        var targetNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (node == null || targetNode == null) return;

        if (!_originalPosition.HasValue)
            _originalPosition = node.GlobalPosition;

        Vector2 frontDir = (player.Side == CombatSide.Player) ? Vector2.Left : Vector2.Right;
        Vector2 behindDir = -frontDir;

        Vector2 endPos = targetNode.GlobalPosition + behindDir * (behindDistance + overshoot);

        PlayAnimation(player, attackAnim);

        var tween = node.CreateTween();
        tween.TweenProperty(node, "global_position", endPos, durationSeconds)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        await node.ToSignal(tween, Tween.SignalName.Finished);
    }
    
    public async Task DashPastSB(
	    Creature player,
	    Vector2 endPos,
	    string? attackAnim = null,
	    float durationSeconds = 0.3f)
    {
	    var node = NCombatRoom.Instance?.GetCreatureNode(player);

	    if (node == null)
		    return;

	    if (!_originalPosition.HasValue)
		    _originalPosition = node.GlobalPosition;

	    if (!string.IsNullOrEmpty(attackAnim))
		    PlayAnimation(player, attackAnim);

	    var tween = node.CreateTween();

	    tween.TweenProperty(
			    node,
			    "global_position",
			    endPos,
			    durationSeconds)
		    .SetTrans(Tween.TransitionType.Quad)
		    .SetEase(Tween.EaseType.Out);

	    await node.ToSignal(
		    tween,
		    Tween.SignalName.Finished);
    }
    
    
    public async Task Retreat(
        Creature player,
        string? animation = "retreat",
        bool goIdle = true)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(player);
        if (node == null || !_originalPosition.HasValue) return;

        if (!string.IsNullOrEmpty(animation))
            PlayAnimation(player, animation);

        var tween = node.CreateTween();
        tween.TweenProperty(node, "global_position", _originalPosition.Value, 0.3f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);

        await node.ToSignal(tween, Tween.SignalName.Finished);

        _originalPosition = null;

        var visuals = node.Visuals.GetNodeOrNull<Node2D>("Visuals");
        if (visuals != null)
            visuals.Position = Vector2.Zero;

        if (goIdle)
            PlayAnimation(player, "idle");
    }
    
    [HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
    public static class NCreatureSetTriggerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(NCreature __instance, string trigger)
        {
            if (__instance.Entity?.Player?.Character is Sora character)
            {
                character.PlayAnimation(__instance.Entity, trigger);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    public static class StartDeathAnimPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NCreature __instance, ref float __result)
        {
            if (__instance.Entity?.Player?.Character is Sora character)
            {
                AudioHelper.PlayRandomGameover();
                character.PlayAnimation(__instance.Entity, "die");

                var animPlayer = __instance.Visuals.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
                __result = animPlayer?.GetAnimation("die")?.Length ?? 1.5f;
            }
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterCombatVictory))]
    public static class SoraVictoryAnimationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(IRunState runState, CombatState? combatState)
        {

            var creatures = combatState?.Creatures?.Where(c => c.IsPlayer);

            if (creatures == null)
                return;

            foreach (var creature in creatures)
            {
                if (creature.Player?.Character is not Sora)
                    continue;

                var node = NCombatRoom.Instance?.GetCreatureNode(creature);
                var animPlayer = node?.Visuals?.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
                
                if (animPlayer == null)
                    continue;
                
                AudioHelper.PlayRandomVictory();
	            animPlayer.Play("victory");
            }
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterDamageReceived))]
	public static class SoraDamageAnimationPatch
	{
		[HarmonyPostfix]
		public static void Postfix(Creature target, DamageResult result, ValueProp props, Creature? dealer)
		{
			if (target.Player?.Character is not Sora character)
				return;


			if (dealer == null || dealer.Side != CombatSide.Enemy)
				return;


			if (props.HasFlag(ValueProp.SkipHurtAnim) || props.HasFlag(ValueProp.Unpowered))
				return;

			if (result.WasFullyBlocked && result.BlockedDamage > 0)
			{
				character.PlayAnimation(target, "block");
			}
            
			else if (result.UnblockedDamage > 0 && !target.IsDead)
			{
				character.PlayAnimation(target, "hit");
				if (target.CurrentHp < 20)
				{
					AudioHelper.PlayRandomDamagedCritical();
				}
				else if (result.UnblockedDamage < 10)
				{
					AudioHelper.PlayRandomDamaged();
				}
				else
				{
					AudioHelper.PlayRandomDamagedHigh();
				}
			}
		}
	}
    
	[HarmonyPatch(typeof(CardSelectCmd), nameof(CardSelectCmd.FromChooseACardScreen))]
	public static class CardSelectCmdFromChooseACardScreenPatch
	{
		[HarmonyPrefix]
		public static bool Prefix(
			PlayerChoiceContext context,
			IReadOnlyList<CardModel> cards,
			Player player,
			bool canSkip,
			ref Task<CardModel?> __result)
		{
			
			if (player?.Character is not Character.Sora)
			{
				return true;
			}

			
			__result = PatchedChoose(context, cards, player, canSkip);
			return false; 
		}

		private static async Task<CardModel?> PatchedChoose(
			PlayerChoiceContext context,
			IReadOnlyList<CardModel> cards,
			Player player,
			bool canSkip)
		{
			if (cards.Count > 5)
			{
				throw new ArgumentException("Only works with 5 or fewer cards", nameof(cards));
			}

			if (cards.Count == 0)
			{
				return null;
			}

			uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);

			await context.SignalPlayerChoiceBegun(PlayerChoiceOptions.None);

			CardModel? result;
			
			if (LocalContext.IsMe(player))
			{
				NPlayerHand.Instance?.CancelAllCardPlay();

				var screen = NChooseACardSelectionScreen.ShowScreen(cards, canSkip);

				if (screen == null)
				{
					await context.SignalPlayerChoiceEnded();
					return null;
				}
				
				foreach (var card in cards)
				{
					SaveManager.Instance.MarkCardAsSeen(card);
				}

				result = (await screen.CardsSelected()).FirstOrDefault();

				int index = cards.IndexOf(result);
				var choiceResult = PlayerChoiceResult.FromIndex(index);

				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, choiceResult);
			}
			else
			{
				int index = (await RunManager.Instance.PlayerChoiceSynchronizer
						.WaitForRemoteChoice(player, choiceId))
					.AsIndex();

				result = index < 0 ? null : cards[index];
			}

			await context.SignalPlayerChoiceEnded();

			return result;
		}
	}
}