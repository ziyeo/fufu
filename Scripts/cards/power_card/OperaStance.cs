using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.Models.Powers;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class OperaStance : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new PowerVar<StrengthPower>(1m) };

    public override string PortraitPath => "res://fufu/images/card_portraits/opera_stance.png";

    public OperaStance() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature c = Owner.Creature;
        if (!c.IsPlayer) return;
        
        await PowerCmd.Apply<OperaStancePower>(choiceContext,c, DynamicVars.Strength.BaseValue, c, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1m);
    }
}