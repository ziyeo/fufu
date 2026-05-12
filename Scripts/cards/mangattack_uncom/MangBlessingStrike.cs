using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class MangBlessingStrike : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_blessing_strike.png";

    public MangBlessingStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        if (cardPlay.Target == null) return;

        // 造成10点普通伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性时：歌者治疗 +3
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            var power = await PowerCmd.Apply<SingerHealBuffPower>(choiceContext,player, 3, player, this);
            if (power != null)
            {
                power.Duration = 1; // 持续1回合
            }

        }
    }

    protected override void OnUpgrade()
    {
        // 升级伤害+3：10 → 13
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}