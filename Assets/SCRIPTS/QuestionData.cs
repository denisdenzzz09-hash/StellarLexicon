using UnityEngine;

// Cara buat: klik kanan di Project → Create → StellarLexicon → Question Data
[CreateAssetMenu(fileName = "NewQuestion", menuName = "StellarLexicon/Question Data")]
public class QuestionData : ScriptableObject
{
    [Header("Soal")]
    [Tooltip("Kalimat Bahasa Inggris yang muncul di atas alien")]
    public string englishSentence;

    [Header("Jawaban")]
    [Tooltip("Terjemahan ideal (ditampilkan saat alien mati)")]
    public string referenceAnswer;

    [Tooltip("Kata kunci wajib ada di jawaban pemain (2-3 kata)")]
    public string[] keywords;
}