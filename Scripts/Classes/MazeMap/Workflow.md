# MazeMap 使用与结构说明

## 新建 MazeMap 流程

创建新的 Instance。
编写 Definition，定义新的 generator。Controller. 看enum，有新的就加进去

---

## 层级介绍

- **MazeMapDefinition**  
  纯数据的定义。

- **MazeMapData**  
  包含运行时的数据和 Definition，游戏中传递的就是这个数据。

- **MazeMapController**  
  在 MazeMap 阶段生成，控制 MazeMap 图像的生成。

- **MazeMapGenerator**  
  在 MazeMapData 中使用，生成 MazeMap 的数据结构。

---

## 使用示例

```csharp
IMazeMapData t = new MazeMapData(mazeMapDefinition); // 创建 MazeMapData
```
