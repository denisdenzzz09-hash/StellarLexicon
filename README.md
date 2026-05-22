# 🚀 Space Defender – 2D Top-Down Game (Unity 6.0 LTS)

Game edukasi 2D Top-Down berbasis Unity 6.0 LTS di mana pemain harus menjawab soal untuk mengalahkan musuh alien yang menyerang. Terdiri dari 3 level dengan sistem wave, boss fight, dan save/load system.

---

## 🎮 Gameplay

- Musuh alien bergerak menuju pemain sambil membawa soal
- Pemain menjawab soal melalui input untuk mengalahkan musuh
- Setiap level terdiri dari beberapa **wave** musuh, diakhiri dengan **Boss Fight**
- Kalahkan boss untuk menyelesaikan level dan membuka level berikutnya

---

## 📁 Struktur Script Utama

| Script | Fungsi |
|---|---|
| `WaveSpawner.cs` | Spawning musuh per wave & boss, mengatur jumlah dan kecepatan musuh per level |
| `EnemyAI.cs` | Pergerakan musuh, logika jawaban benar/salah, homing ke player |
| `BossEnemy.cs` | Logika khusus boss fight |
| `GameManager.cs` | Logika utama game: HP, skor, combo, pindah scene, level complete |
| `GameUI.cs` | Semua tampilan HUD: HP bar, skor, wave text, feedback, You Win panel, Game Over |
| `PauseMenu.cs` | Pause game dengan tombol ESC, tombol resume/save/main menu |
| `SaveSystem.cs` | Baca & tulis file save dalam format JSON |
| `LoadSystem.cs` | Restore state game dari save file saat scene di-load |
| `LoadButton.cs` | Tombol "Continue" di main menu untuk load save |
| `QuestionData.cs` | ScriptableObject data soal per level |

---

## 🌊 Sistem Wave

- Setiap level punya beberapa wave musuh (konfigurasi di Inspector)
- Jumlah musuh per wave ditentukan secara random sesuai level:
  - Level 1: 3–5 musuh, kecepatan 2
  - Level 2: 5–8 musuh, kecepatan 3.5
  - Level 3: 8–12 musuh, kecepatan 5
- Setelah semua wave selesai → Boss spawn
- Boss dikalahkan → `OnLevelComplete()` dipanggil → You Win Panel muncul

---

## 🏆 Sistem You Win

Setelah boss dikalahkan:
1. Game di-freeze (`Time.timeScale = 0`)
2. Panel **YOU WIN** muncul
3. Pilihan tersedia:
   - **Next Level** – lanjut ke level berikutnya
   - **Save** – simpan progress
   - **Main Menu** – kembali ke menu utama
4. Saat You Win Panel aktif, tombol ESC (Pause) diblokir

---

## 💾 Sistem Save & Load

Data yang disimpan:
- Level aktif
- HP pemain
- Skor
- Wave terakhir
- Posisi background (untuk scrolling)
- Data semua musuh yang masih hidup (posisi, soal, kecepatan)
- Status `isLevelWon` – jika save dilakukan saat You Win Panel aktif, saat di-load langsung muncul You Win Panel kembali

File save disimpan di:
```
Application.persistentDataPath/savegame.json
```

---

## 🎯 Sistem Skor & Combo

- Jawaban benar pertama kali: **+100 poin**
- Jawaban benar setelah salah: **+50 poin**
- Setiap 5 combo berturut-turut: **+200 poin bonus**
- High score per level disimpan di `PlayerPrefs`

---

## 🗂️ Scene Setup (Build Settings)

Urutan scene yang harus didaftarkan di **File → Build Settings**:

| Index | Nama Scene |
|---|---|
| 0 | MainMenu |
| 1 | LEVEL_1 |
| 2 | LEVEL_2 |
| 3 | LEVEL_3 |

---

## ⚙️ Setup Inspector

### WaveSpawner
- `enemyPrefab` – prefab musuh biasa
- `bossPrefab` – prefab boss
- `level1Questions` / `level2Questions` / `level3Questions` – array `QuestionData`
- `totalWaves` – jumlah wave per level
- `timeBetweenWaves` – jeda antar wave (detik)
- `timeBetweenSpawns` – jeda antar spawn musuh

### GameUI
- `youWinPanel` – drag `YouWinPanel` dari Hierarchy

### Tombol di YouWinPanel
| Tombol | Fungsi |
|---|---|
| Next Level | `GameUI → OnNextLevelButton()` |
| Save | `GameUI → OnSaveButton()` |
| Main Menu | `GameUI → OnMainMenuButton()` |

---

## 🛠️ Dibuat dengan

- **Unity 6.0 LTS**
- **C#**
- **TextMeshPro** untuk UI teks
- **Unity UI (uGUI)** untuk Canvas & tombol
- **JSON** untuk sistem save

---

## 📝 Catatan Development

- `PlayerPrefs` digunakan untuk menyimpan high score dan level yang sudah di-unlock, **bukan** untuk tracking level aktif saat bermain
- Logika save dipusatkan di `GameManager.SaveGame()` agar bisa dipanggil dari `PauseMenu` maupun `GameUI` tanpa duplikasi kode
- `isLevelComplete` digunakan sebagai flag untuk memblokir pause dan mencegah wave lanjut setelah level selesai
