using BaseLib.Abstracts;
using BaseLib.Utils;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character.CardPool;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class BeckonShadow : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/BeckonShadow.png";

    public BeckonShadow() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        Player players = player.Player;
        bool isHuang = FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang;
        bool isMang = FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang;

        // 互斥清除
        if (isHuang)
        {
            //await PowerCmd.Remove<BasicSingerPower>(player);
            bool hasSalonBond = player.HasPower<SalonBondPower>();
            int salonBond = player.GetPowerAmount<SalonBondPower>();
            await PowerCmd.Apply<ShadowSalonPower>(choiceContext,player, 2, player, this);
            if(hasSalonBond)
                await CardPileCmd.Draw(choiceContext, salonBond, players);
        }

        if (isMang)
        {
            //await PowerCmd.Remove<ShadowSalonPower>(player);
            await PowerCmd.Apply<BasicSingerPower>(choiceContext,player, 2, player, this);
        }
    }

    protected override void OnUpgrade()
    {
         base.EnergyCost.UpgradeBy(-1);
    }
}