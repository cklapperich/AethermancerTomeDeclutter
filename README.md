# Aethermancer Tome Declutter Mod

## Problem Statement

**Grimoire Shifted** has access to too many low-value summon skills (Lances/Totems) because:
1. It has **Wild element** (all 4 elements) - meaning it can use actions of any element
2. It has the **Summon monster type** - so it can roll summon actions

This results in 2x the number of Lance/Totem options compared to normal 2-element monsters, cluttering the level-up choices with low-value options.

**Regular Grimoire** (non-shifted) also has Wild element, but lacks the Summon type - so it can't learn Lances/Totems anyway.

---

## Target Actions

**Lances (4 total):**
| Internal Name | Display Name |
|---------------|--------------|
| SummonFireLance | Summon Fire Lance |
| SummonWaterLance | Summon Water Lance |
| SummonEarthLance | Summon Earth Lance |
| SummonWindLance | Summon Wind Lance |

**Totems (4 total):**
| Internal Name | Display Name |
|---------------|--------------|
| SummonFireTotem | Summon Fire Totem |
| SummonWaterTotem | Summon Water Totem |
| SummonEarthTotem | Summon Earth Totem |
| SummonWindTotem | Summon Wind Totem |

**Data Location:** Unity asset files (`sharedassets1.assets`, etc.) - action prefabs with `BaseAction` component

---

## Mod Options

### Option 1: Remove All Totems/Lances
Completely remove Lance and Totem actions from the level-up pool for all monsters.
- Monsters can still START with these actions (starting actions are separate)
- They just won't appear as level-up choices

### Option 2: Halve Grimoire Drop Rate
Reduce the weight of Lance/Totem actions by 50% specifically for Grimoire Shifted.
- Other monsters unaffected
- Lances/Totems still possible but less common

---

## Technical Details

### Key Files (see `references/` folder)
| File | Purpose |
|------|---------|
| `SkillPicker.cs` | Handles action rolling on level up |
| `SkillManager.cs` | Manages monster skills and learning |
| `MonsterShift.cs` | Defines shift overrides (types, elements) |
| `MonsterType.cs` | Contains action and trait pools |
| `BaseAction.cs` | Action base class with name, cost, elements |
| `EElement.cs` | Element enum (Water, Fire, Wind, Earth, Wild) |

### Level-Up Flow
1. `SkillPicker.RollThreeSkills()` - Called on level up
2. `DetermineWeightedActions()` - Builds weighted pool of learnable actions
3. `RollAction()` - Picks from pool based on weights

### Why Grimoire Shifted Gets So Many
```csharp
// SkillPicker.cs - Element matching in CanUseElement()
foreach (EElement element in action.Elements)
{
    if (Monster.SkillManager.GetElements().Contains(element)
        || Monster.SkillManager.GetElements().Contains(EElement.Wild))  // <-- Wild matches ALL
    {
        return true;
    }
}
```

Wild element monsters match ALL action elements, so they get the full pool of every element's summon skills.

### Starting Actions Are Separate
Starting actions come from `GetStartingActions()` which pulls from `StartActionsOverride` in `MonsterShift.cs` - completely separate from the level-up pool. Monsters can still start with Lances/Totems.

---

## Implementation

### Target Method
`SkillPicker.DetermineWeightedActions()` at line 581

### Name Matching
Action names can be checked via:
- `action.Name` - The localization key (e.g., "SummonFireLance")
- `action.GetName()` - The localized display name

For reliability, check the `Name` field directly (before localization):
```csharp
if (action.Name.Contains("Lance") || action.Name.Contains("Totem"))
```

### Option 1 Code: Remove All Lances/Totems
```csharp
public List<WeightedSkill> DetermineWeightedActions(bool considerLevelUpInfluence = false)
{
    weightedSkills.Clear();
    foreach (GameObject monsterType in Monster.SkillManager.GetMonsterTypes())
    {
        foreach (BaseAction action in monsterType.GetComponent<MonsterType>().Actions)
        {
            // === OPTION 1: Remove all Lances/Totems ===
            if (action.Name.Contains("Lance") || action.Name.Contains("Totem"))
            {
                continue;  // Skip entirely
            }
            // === END OPTION 1 ===

            float weightMultiplier = 1f;
            if (action == null || ContainsWeightedSkill(action) || !CanRollAction(action) || !CanUseElement(action, isMaverick: false))
            {
                continue;
            }
            if (considerLevelUpInfluence)
            {
                levelUpInfluences.Where((LevelUpInfluence x) => action.Types.Contains(x.FavouredType)).ToList().ForEach(delegate(LevelUpInfluence x)
                {
                    weightMultiplier *= x.FavouredTypeMultiplier;
                });
            }
            weightedSkills.Add(new WeightedSkill(action, weightMultiplier * action.GetSkillWeight(Monster)));
        }
    }
    return weightedSkills;
}
```

### Option 2 Code: Halve Grimoire Drop Rate
```csharp
public List<WeightedSkill> DetermineWeightedActions(bool considerLevelUpInfluence = false)
{
    weightedSkills.Clear();
    foreach (GameObject monsterType in Monster.SkillManager.GetMonsterTypes())
    {
        foreach (BaseAction action in monsterType.GetComponent<MonsterType>().Actions)
        {
            float weightMultiplier = 1f;

            // === OPTION 2: Halve rate for Grimoire Shifted ===
            // Check if this is Grimoire Shifted (Wild element + Shifted form)
            if (Monster.SkillManager.GetElements().Contains(EElement.Wild)
                && Monster.Shift == EMonsterShift.Shifted)
            {
                if (action.Name.Contains("Lance") || action.Name.Contains("Totem"))
                {
                    weightMultiplier *= 0.5f;
                }
            }
            // === END OPTION 2 ===

            if (action == null || ContainsWeightedSkill(action) || !CanRollAction(action) || !CanUseElement(action, isMaverick: false))
            {
                continue;
            }
            if (considerLevelUpInfluence)
            {
                levelUpInfluences.Where((LevelUpInfluence x) => action.Types.Contains(x.FavouredType)).ToList().ForEach(delegate(LevelUpInfluence x)
                {
                    weightMultiplier *= x.FavouredTypeMultiplier;
                });
            }
            weightedSkills.Add(new WeightedSkill(action, weightMultiplier * action.GetSkillWeight(Monster)));
        }
    }
    return weightedSkills;
}
```

---

## Key Enums

### EMonsterShift
```csharp
public enum EMonsterShift
{
    Normal,   // Regular form (Grimoire without Summon type)
    Shifted,  // Grimoire Shifted (has Summon type)
    Auto
}
```

### EElement
```csharp
public enum EElement
{
    Water,
    Fire,
    Wind,
    Earth,
    Wild,     // Matches all elements - Grimoire has this
    Neutral,
    Physical,
    Magical,
    Any,
    None
}
```

---

## File Locations

- **Game DLL**: `~/.steam/debian-installation/steamapps/common/Aethermancer/Aethermancer_Data/Managed/Assembly-CSharp.dll`
- **Asset files**: `~/.steam/debian-installation/steamapps/common/Aethermancer/Aethermancer_Data/sharedassets*.assets`
- **Decompiled source**: `~/gitrepos/Aethermancer_Decompiled/`
- **Reference files**: `./references/`

---

## Project Structure
```
AethermancerTomeDeclutter/
├── README.md                 # This file
└── references/               # Decompiled reference files
    ├── SkillPicker.cs        # Main target for patching
    ├── SkillManager.cs       # Skill management
    ├── MonsterShift.cs       # Shift overrides
    ├── MonsterType.cs        # Action pools
    ├── BaseAction.cs         # Action class
    ├── BaseSkill.cs          # Skill base class
    ├── EElement.cs           # Element enum
    ├── EMonsterShift.cs      # Shift enum
    ├── EActionType.cs        # Action type enum
    └── EActionSubType.cs     # Action subtype enum
```

---

## TODO

- [ ] Create BepInEx plugin with config option for both modes
- [ ] Test both options in-game
- [ ] Package for distribution

## Code to modify
inside DetermineWeightedActions():

  if (Monster.SkillManager.GetElements().Contains(EElement.Wild))
  {
      if (action.Name.Contains("Lance") || action.Name.Contains("Totem"))
      {
          weightMultiplier *= 0.5f;
      }
  }

