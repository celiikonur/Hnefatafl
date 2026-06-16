// Sahneler arasi tasinan ayarlar. Static oldugu icin sahne degisince
// sifirlanmaz; menude secilenler oyun sahnesine bu sekilde tasinir.
public static class GameSettings
{
    // Zorluk (minimax derinligi)
    public static int SearchDepth = 3;

    // Tahta boyutu: 11 = Copenhagen, 9 = Tablut
    public static int BoardSize = 11;

    // Oyuncu saldirgan mi oynuyor? true = saldirgan, false = savunmaci
    // (Hnefatafl'da saldirgan ilk hamleyi yapar.)
    public static bool PlayerIsAttacker = true;

    // === Zorluk yardimcilari ===
    public static void SetEasy()   { SearchDepth = 1; }
    public static void SetMedium() { SearchDepth = 3; }
    public static void SetHard()   { SearchDepth = 4; }
}