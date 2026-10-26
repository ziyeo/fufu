using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class JoyfulStrike : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/joyful_strike.png";

    public JoyfulStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        if (cardPlay.Target == null) return;

        // 造成基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性效果：气氛≥5 → 扣5气氛 → 回3血
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int currentAtm = creature.GetPowerAmount<AtmospherePower>();
            if (currentAtm >= 5)
            {
                await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, -5, creature, this);
                
                // ✅ 官方标准治疗（完全匹配你给的官方Heal方法）
                await CreatureCmd.Heal(creature, 3m);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}