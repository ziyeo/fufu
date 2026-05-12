using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.CardPools;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class WaterSilkSlash : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.AnyEnemy;
    private const bool showInLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];

    public override string PortraitPath => "res://fufu/images/card_portraits/WaterSilkSlash.png";

    public WaterSilkSlash() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;
        if (cardPlay.Target == null) return;

        // 造成基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性：命中获得 4 点气氛值
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await PowerCmd.Apply<AtmospherePower>(choiceContext,player, 4, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +3
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}