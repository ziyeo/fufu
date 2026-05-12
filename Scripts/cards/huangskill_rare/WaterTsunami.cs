using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class WaterTsunami : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VulnerablePower>(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/water_tsunami.png";

    public WaterTsunami() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;

        // 本场战斗每损失5点生命值 → 1层易伤（全体敌人）
        int lostHp = player.MaxHp - player.CurrentHp;
        int v=1;
        if (IsUpgraded)
            v += 1;

        int vulnerableStacks = lostHp / 5*v;
        
        if (vulnerableStacks > 0)
        {
           await PowerCmd.Apply<VulnerablePower>(choiceContext,cardPlay.Target!, vulnerableStacks, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }

    //3.水之海啸：耗 2 点卡牌能量，本场战斗每扣5血则 全部敌人获 1易伤；升级+1易伤。
    
}