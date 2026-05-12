using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class DualStateMastery : CustomCardModel
{
    public override string PortraitPath => "res://fufu/images/card_portraits/dual_state_mastery.png";
   
    // "永久。荒：回合结束+2元素能量。芒：回合结束+1气氛值。";

    public DualStateMastery() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self,true) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await PowerCmd.Apply<DualStateMasteryPower>(choiceContext,new[] { Owner.Creature }, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}