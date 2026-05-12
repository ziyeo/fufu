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
public class AtmosphereCallEnergy : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/atmosphere_call_energy.png";

    public AtmosphereCallEnergy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) 
            return;

        // 获得 4 气氛值
        await PowerCmd.Apply<AtmospherePower>(choiceContext,
            creature, 
            4,
            creature, 
            this
        );

        // 荒属性额外 +1 气氛
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, 1, creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：消耗 1 → 0
        base.EnergyCost.UpgradeBy(-1);
    }
    //.气氛唤能：耗 1 点卡牌能量，获 4 气氛值；角色荒属性时额外获 1 气氛值；升级0能量
}