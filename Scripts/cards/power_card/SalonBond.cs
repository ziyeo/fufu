using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class SalonBond : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new CardsVar(1) };   
    public override string PortraitPath => "res://fufu/images/card_portraits/salon_bond.png";
    //  "永久，每召唤2名沙龙，抽1张牌。";

    public SalonBond() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await PowerCmd.Apply<SalonBondPower>(choiceContext,Owner.Creature, DynamicVars.Cards.IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}