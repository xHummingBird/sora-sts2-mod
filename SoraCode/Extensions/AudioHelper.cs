using MegaCrit.Sts2.Core.Commands;

namespace Sora.SoraCode.Extensions;

public static class AudioHelper
{
    private static readonly Random rng = new Random();
    
    
    private static readonly string[] attackSfx =
    {
        "res://Sora/sounds/attack_1.wav",
        "res://Sora/sounds/attack_2.wav",
        "res://Sora/sounds/attack_3.wav",
        "res://Sora/sounds/attack_4.wav",
        "res://Sora/sounds/attack_5.wav",
        "res://Sora/sounds/attack_6.wav",
    };
    
    private static readonly string[] finalAttackSfx =
    {
        "res://Sora/sounds/finalhit_1.wav",
        "res://Sora/sounds/finalhit_2.wav",
        "res://Sora/sounds/finalhit_3.wav",
        "res://Sora/sounds/finalhit_4.wav",
        "res://Sora/sounds/finalhit_5.wav",
        "res://Sora/sounds/finalhit_6.wav",
        "res://Sora/sounds/finalhit_7.wav",
        "res://Sora/sounds/finalhit_8.wav",
        "res://Sora/sounds/finalhit_9.wav",
    };

    private static readonly string[] callRikuSfx =
    {
        "res://Sora/sounds/riku_1.wav",
        "res://Sora/sounds/riku_2.wav",
    };
    
    private static readonly string[] callKairiSfx =
    {
        "res://Sora/sounds/kairi_1.wav",
        "res://Sora/sounds/kairi_2.wav",
    };
    
    private static readonly string[] finalAttackSfx2 =
    {
        "res://Sora/sounds/finalhit2_1.wav",
        "res://Sora/sounds/finalhit2_2.wav",
        "res://Sora/sounds/finalhit2_3.wav",
        "res://Sora/sounds/finalhit2_4.wav",
        "res://Sora/sounds/finalhit2_6.wav",
    };
    
    private static readonly string[] formchangeSfx =
    {
        "res://Sora/sounds/formchange_1.wav",
        "res://Sora/sounds/formchange_2.wav",
    };
    
    private static readonly string[] damagedSfx =
    {
        "res://Sora/sounds/hurt_1.wav",
        "res://Sora/sounds/hurt_2.wav",
        "res://Sora/sounds/hurt_3.wav",
        "res://Sora/sounds/hurt_4.wav",
    };
    
    private static readonly string[] highDamagedSfx =
    {
        "res://Sora/sounds/hurt_medium_1.wav",
        "res://Sora/sounds/hurt_medium_2.wav",
        "res://Sora/sounds/hurt_medium_3.wav",
    };
    
    private static readonly string[] criticalDamagedSfx =
    {
        "res://Sora/sounds/hurt_high_1.wav",
        "res://Sora/sounds/hurt_high_2.wav",
        "res://Sora/sounds/hurt_high_3.wav",
    };

    private static readonly string[] fireSfx =
    {
        "res://Sora/sounds/fire_1.wav",
        "res://Sora/sounds/fire_2.wav",
    };
    
    private static readonly string[] blizzardSfx =
    {
        "res://Sora/sounds/ice_1.wav",
        "res://Sora/sounds/ice_2.wav",
    };
    
    private static readonly string[] thunderSfx =
    {
        "res://Sora/sounds/thunder_1.wav",
        "res://Sora/sounds/thunder_2.wav",
    };

    private static readonly string[] victorySfx =
    {
        "res://Sora/sounds/win_1.wav",
        "res://Sora/sounds/win_2.wav",
        "res://Sora/sounds/win_3.wav",
        "res://Sora/sounds/win_4.wav",
        "res://Sora/sounds/win_5.wav"
    };
    
    private static readonly string[] gameoverSfx =
    {
        "res://Sora/sounds/dead_1.wav",
        "res://Sora/sounds/dead_2.wav",
        "res://Sora/sounds/dead_3.wav",
    };
    
    public static void PlayRandomAttack()
    {
        PlayRandom(attackSfx);
    }
    
    public static void PlayRandomFinalAttack()
    {
        PlayRandom(finalAttackSfx);
    }
    
    public static void PlayRandomFinalAttack2()
    {
        PlayRandom(finalAttackSfx2);
    }
    
    public static void PlayRandomGameover()
    {
        PlayRandom(gameoverSfx);
    }

    public static void PlayRandomRiku()
    {
        PlayRandom(callRikuSfx);
    }

    public static void PlayRandomKairi()
    {
        PlayRandom(callKairiSfx);
    }
    
    public static void PlayRandomFormchange()
    {
        PlayRandom(formchangeSfx);
    }
    
    public static void PlayRandomDamaged()
    {
        PlayRandom(damagedSfx);
    }
    
    public static void PlayRandomDamagedHigh()
    {
        PlayRandom(highDamagedSfx);
    }
    
    public static void PlayRandomDamagedCritical()
    {
        PlayRandom(criticalDamagedSfx);
    }
    
    public static void PlayRandomFire()
    {
        PlayRandom(fireSfx);
    }
    
    public static void PlayRandomBlizzard()
    {
        PlayRandom(blizzardSfx);
    }
    
    public static void PlayRandomThunder()
    {
        PlayRandom(thunderSfx);
    }

    public static void PlayRandomVictory()
    {
        PlayRandom(victorySfx);
    }

    public static void PlayRandom(string[] pool)
    {
        int index = rng.Next(pool.Length);
        SfxCmd.Play(pool[index]);
    }
}