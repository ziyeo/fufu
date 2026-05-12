using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class WaterChant : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override string PortraitPath => "res://fufu/images/card_portraits/water_chant.png";

    public WaterChant() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;

        // 本场战斗每损失2血 → +1气氛
        int lostHp = player.MaxHp - player.CurrentHp;
        int atmosphereGain = lostHp / 2;

        if (atmosphereGain > 0)
        {
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, atmosphereGain, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：1费 → 0费
        base.EnergyCost.UpgradeBy(-1);
    }

    //2.众水之颂：耗1点卡牌能量，本场战斗每扣2血则 +1气氛值。升级0能量
}