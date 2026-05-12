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
using MegaCrit.Sts2.Core.Entities.Players;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class SalonFeast : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override string PortraitPath => "res://fufu/images/card_portraits/salon_feast.png";

    public SalonFeast() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;
        Player players = player.Player;
        // 基础召唤数量
        int count = IsUpgraded ? 3 : 2;

        // 荒属性额外+1
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            count += 1;
        }

        // 召唤沙龙
        bool hasSalonBond = player.HasPower<SalonBondPower>();
        int salonBond = player.GetPowerAmount<SalonBondPower>();
        await PowerCmd.Apply<ShadowSalonPower>(choiceContext,player, count, player, this);
        if(hasSalonBond)
            await CardPileCmd.Draw(choiceContext, salonBond, players);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
    }
    //2.召唤 2 名沙龙；角色荒属性时额外 + 1 名；升级召唤3名沙龙
}