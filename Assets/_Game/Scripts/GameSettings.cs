// Sahneler arasi tasinan ayarlar.
public static class GameSettings
{
    // Zorluk parametreleri
    public static int SearchDepth = 3;
    public static float MistakeChance = 0.15f;

    // Zorluk seviyesi: 0 = Kolay, 1 = Orta, 2 = Zor
    // (Hangi Viking modelinin gosterilecegini secmek icin kullanilir.)
    public static int DifficultyLevel = 1;

    // Tahta boyutu: 11 = Copenhagen, 9 = Tablut
    public static int BoardSize = 11;

    // Oyuncu saldirgan mi?
    public static bool PlayerIsAttacker = true;

    // === Zorluk ayarlari ===
    public static void SetEasy()
    {
        SearchDepth = 1;
        MistakeChance = 0.45f;
        DifficultyLevel = 0;
    }

    public static void SetMedium()
    {
        SearchDepth = 2;
        MistakeChance = 0.15f;
        DifficultyLevel = 1;
    }

    public static void SetHard()
    {
        SearchDepth = 4;
        MistakeChance = 0f;
        DifficultyLevel = 2;
    }
}