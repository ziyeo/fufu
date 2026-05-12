using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class DimLightSummon : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override string PortraitPath => "res://fufu/images/card_portraits/dim_light_summon.png";

    public DimLightSummon() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        Player players = player.Player;
        // 召唤 2 名基础沙龙
        bool hasSalonBond = player.HasPower<SalonBondPower>();
        int salonBond = player.GetPowerAmount<SalonBondPower>();
        await PowerCmd.Apply<ShadowSalonPower>(choiceContext,player, 2, player, this);
        if(hasSalonBond)
            await CardPileCmd.Draw(choiceContext, salonBond, players);

        // 荒属性时获得元素能量
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,
                player, 
                IsUpgraded ? 2 : 1, 
                player, 
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
    }
    //1.微光召唤：耗 1 点卡牌能量，召唤 2名基础沙龙；角色荒属性时额外获 1 元素；升级2元素。
}