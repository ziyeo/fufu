using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;


namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class WaterElementFrenzy : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool showInLibrary = true;

    // 基础易伤 2 层
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<VulnerablePower>(2m)
    };


    public override string PortraitPath => "res://fufu/images/card_portraits/water_element_frenzy.png";

    public WaterElementFrenzy() : base(energyCost, type, rarity, targetType, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || cardPlay.Target == null) return;
        
        // 给敌人施加易伤
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target!, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        // 荒属性：获得 5 元素能量
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext, player, 5, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：易伤 +1（2→3）
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}