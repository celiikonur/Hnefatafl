// Sahneler arasi tasinan ayarlar.
public static class GameSettings
{
    // Zorluk parametreleri
    public static int SearchDepth = 3;       // minimax derinligi
    public static float MistakeChance = 0.15f; // 0..1 arasi: en iyi yerine rastgele hamle olasiligi

    // Tahta boyutu: 11 = Copenhagen, 9 = Tablut
    public static int BoardSize = 11;

    // Oyuncu saldirgan mi? true = saldirgan (ilk hamle), false = savunmaci
    public static bool PlayerIsAttacker = true;

    // === Zorluk ayarlari ===
    // Kolay: sig arama + sik hata (acemi gibi)
    public static void SetEasy()
    {
        SearchDepth = 1;
        MistakeChance = 0.45f;
    }

    // Orta: orta arama + ara sira hata (keyifli mucadele)
    public static void SetMedium()
    {
        SearchDepth = 2;
        MistakeChance = 0.15f;
    }

    // Zor: derin arama + sifir hata (acimasiz)
    public static void SetHard()
    {
        SearchDepth = 4;
        MistakeChance = 0f;
    }
}