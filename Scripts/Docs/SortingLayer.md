# SortingLayer

分场景记录对象所在的SortingLayer

### MazeMap

| Id    | Layer     | Order |
| ----- | --------- | ----- |
|       |           |       |
| Tomb  | MidGround | 1000  |
| Tiles | MidGround | 500   |

### Level


| Id                   | Layer      | Order                |
| -------------------- | ---------- | -------------------- |
| LevelEndObject       | ForeGround | 200                  |
| SelectFramebox       | ForeGround | 100                  |
|                      |            |                      |
| Coin                 | ForeGround | 10                   |
| Zombies              | Zombie     | 0~30010(by Allocate) |
| Player               | MidGround  | 500                  |
| Sun(ByPlant)         | MidGround  | 360                  |
| Projectile           | MidGround  | 300~349              |
| (Obstacle)Zombies    | MidGround  | 250~299(260)         |
| Plants               | MidGround  | 200~249(210)         |
| Tile(!TileEmpty)     | MidGround  | 100~120              |
| Tile(BackGround_3~4) | BackGround | 91~99                |
| Sun(AutoFall)        | BackGround | 90                   |
| Tile(BackGround_1~2) | BackGround | 80~89                |
| BackGround           | Pa         | 50~59                |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |
|                      |            |                      |


