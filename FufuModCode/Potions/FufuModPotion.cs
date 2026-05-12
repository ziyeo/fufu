using BaseLib.Abstracts;
using BaseLib.Utils;
using Fufu.FufuModCode.Character.PotionPool;

namespace FufuMod.FufuModCode.Potions;

[Pool(typeof(FufuPotionPool))]
public abstract class FufuModPotion : CustomPotionModel;