# Block Blast (Unity 6000.3.25f1)

Mở thư mục này bằng Unity Hub, mở một scene trống và bấm Play. `GameController` tự tạo khi scene chưa có.

## Cấu trúc
- `Assets/BlockBlast/Scripts/Core`: logic thuần (Board, Shape, Hand, ScoreSystem), không phụ thuộc scene.
- `Assets/BlockBlast/Scripts/Data/GameConfig.cs`: mọi thông số chỉnh được (bàn, điểm, combo, hình khối, art, màu chữ, bố cục).
- `Assets/BlockBlast/Resources/GameConfig.asset`: bản config đang dùng, đã gán sẵn art.
- `Assets/BlockBlast/Art`: sprite xuất từ Figma (màn chơi và màn Game Over) và font Montserrat ExtraBold.
- `Assets/BlockBlast/Scripts/View`: hiển thị (BoardView, PieceView, HudView, GameOverView).
- `Assets/BlockBlast/Scripts/Game`: GameController (vòng lặp), PointerInput, HighScoreStore.

## Luật
- Lưới 8x8, mỗi lượt 3 khối ngẫu nhiên, đặt hết 3 khối thì chia lượt mới. Không xoay khối.
- Hàng và cột đầy được xoá cùng lúc.
- Điểm mỗi lần đặt = `ô * pointsPerCell + (đường * pointsPerLine + (đường - 1) * multiLineBonus) * combo`.
- Combo +1 mỗi lần đặt có xoá đường, về 0 sau `comboBreakAfterMisses` lần đặt liên tiếp không xoá.
- Hết chỗ cho mọi khối còn lại = Game Over. Điểm cao nhất lưu bằng PlayerPrefs.

## Tuỳ chỉnh
Chọn `Resources/GameConfig` trong Project để chỉnh mọi thông số trong Inspector. Bố cục tính theo đơn vị world, 1 đơn vị = 1 ô (167 px trên Figma); khung màn hình Figma 1440x2960 là khoảng 8.6 x 17.7 đơn vị, tâm ở gốc toạ độ.
