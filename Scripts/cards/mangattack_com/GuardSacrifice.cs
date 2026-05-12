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
public class GuardSacrifice : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.AnyEnemy;
    private const bool showInLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10,ValueProp.Move),
        new HealVar(3),
        new BlockVar(0,ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/guard_sacrifice.png";

    public GuardSacrifice() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;
        if (cardPlay.Target == null) return;

        // 造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 恢复生命
        await CreatureCmd.Heal(player, DynamicVars.Heal.BaseValue);

        // ✅ 芒属性：消耗所有气氛 → 每点气氛 = 格挡+1
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int atmosphere = player.GetPowerAmount<AtmospherePower>();

            if (atmosphere > 0)
            {
                // 消耗全部气氛
                await PowerCmd.Apply<AtmospherePower>(choiceContext, player, -atmosphere, player, this);

                // 获得格挡 = 消耗的气氛数量
                await CreatureCmd.GainBlock(player, atmosphere, DynamicVars.Block.Props, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害+4
        DynamicVars.Damage.UpgradeValueBy(4);
    }

   
       // "造成{Damage}点伤害，恢复{Heal}点生命。\n芒属性时：消耗所有气氛值，每点气氛值获得1点格挡。" 
}