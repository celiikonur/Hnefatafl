// Sahneler arasi tasinan ayarlar. Static oldugu icin sahne degisince
// sifirlanmaz; menude secilen zorluk oyun sahnesine bu sekilde tasinir.
// (MonoBehaviour DEGIL - hicbir objeye eklenmez, sadece veri tutar.)
public static class GameSettings
{
    // Varsayilan: Orta zorluk
    public static int SearchDepth = 3;

    // Ileride buraya baska ayarlar da eklenecek:
    // board boyutu (Copenhagen/Tablut), secili karakter, ses ac/kapa vb.
    public static int BoardSize = 11;

    // Kolaylik icin zorluk isimleri
    public static void SetEasy()   { SearchDepth = 1; }
    public static void SetMedium() { SearchDepth = 3; }
    public static void SetHard()   { SearchDepth = 4; }
}