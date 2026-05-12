using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.Models;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class SacrificeStrike : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInLibrary = true;

    // 关键字：消耗（Exhaust）→ 打出后直接消失
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/sacrifice_strike.png";

    public SacrificeStrike() : base(energyCost, type, rarity, targetType, shouldShowInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        if (cardPlay.Target == null) return;
        Creature player = Owner.Creature;
        Creature target = cardPlay.Target;

        // 基础伤害
        int finalDamage = DynamicVars.Damage.IntValue;

        // ----------------==== 荒属性：消耗所有气氛值，每点+1伤害 ====----------------
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int atmosphereAmount = player.GetPowerAmount<AtmospherePower>();
            
            // 每点气氛值 +1 伤害
            finalDamage += atmosphereAmount;

            // 消耗 全部 气氛值
            if (atmosphereAmount > 0)
            {
                await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -atmosphereAmount, player, this);
            }
        }

        // 造成最终伤害
        await DamageCmd.Attack(finalDamage)
            .FromCard(this)
            .Targeting(target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +4
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}