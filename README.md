# Block Blast (Unity 6000.3.25f1)

Mở thư mục này bằng Unity Hub, mở một scene trống và bấm Play. `GameController` tự tạo khi scene chưa có.

## Cấu trúc
- `Assets/BlockBlast/Scripts/Core`: logic thuần (Board, Shape, Hand, ScoreSystem), không phụ thuộc scene.
- `Assets/BlockBlast/Scripts/Data/GameConfig.cs`: mọi thông số chỉnh được (bàn, điểm, combo, hình khối, màu, sprite, bố cục).
- `Assets/BlockBlast/Scripts/View`: hiển thị (BoardView, PieceView, HudView tạm thời).
- `Assets/BlockBlast/Scripts/Game`: GameController (vòng lặp), PointerInput, HighScoreStore.

## Luật
- Lưới 8x8, mỗi lượt 3 khối ngẫu nhiên, đặt hết 3 khối thì chia lượt mới. Không xoay khối.
- Hàng và cột đầy được xoá cùng lúc.
- Điểm mỗi lần đặt = `ô * pointsPerCell + (đường * pointsPerLine + (đường - 1) * multiLineBonus) * combo`.
- Combo +1 mỗi lần đặt có xoá đường, về 0 sau `comboBreakAfterMisses` lần đặt liên tiếp không xoá.
- Hết chỗ cho mọi khối còn lại = Game Over. Điểm cao nhất lưu bằng PlayerPrefs.

## Tuỳ chỉnh
Tạo `Assets > Create > BlockBlast > Game Config`, đặt tên `GameConfig` trong thư mục `Resources` (hoặc gán vào GameController). Gán `cellSprite` để thay ô vuông tạm bằng asset từ Figma.
