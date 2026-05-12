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
public sealed class CarnivalCheer : CustomCardModel
{
    // "永久。每当你受到伤害或治疗时，获得1气氛值。升级：获得2气氛值。";
    public CarnivalCheer() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self,true) { }

    public override string PortraitPath => "res://fufu/images/card_portraits/carnival_cheer.png";

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        await PowerCmd.Apply<CarnivalCheerPower>(choiceContext,new[] { Owner.Creature }, IsUpgraded ? 3 : 2, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        // 0费保持0费，只升级效果
    }
}