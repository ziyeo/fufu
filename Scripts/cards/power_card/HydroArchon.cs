using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.FufuModCode.Character.CardPool;
using BaseLib.Utils;
using Fufu.Scripts.Power;


namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class HydroArchon : CustomCardModel
{
    public override string PortraitPath => "res://fufu/images/card_portraits/hydro_archon.png";
    /// <summary>
    /// 永久。回合结束获得1点元素能量。
    /// </summary>

    public HydroArchon() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature c = Owner.Creature;
        await PowerCmd.Apply<HydroArchonPower>(choiceContext,new[] { c }, 1, c, this);
    }

    protected override void OnUpgrade()
    {
     base.EnergyCost.UpgradeBy(-1);  
    }
}