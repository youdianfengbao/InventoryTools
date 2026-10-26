# InventoryTools 汉化说明（Allagan Tools 中文版）

本文件记录 Allagan Tools (InventoryTools) 插件 UI 汉化的规范与术语表，供后续维护与上游同步时对照。

## 翻译规范
1. 只翻译用户可见的英文文本：窗口标题、菜单、按钮、标签、表格列头、帮助文本（tooltip）、配置项名称、聊天/通知消息。
2. 不翻译：
   - 代码标识符、变量名、类名、命名空间
   - `##` / `###` 之后的 ImGui ID 文本（如 `Settings##config` 只译前半为 `设置##config`，`##config` 原样保留）
   - 格式占位符 `{0}`、`{1}` 等，C# 插值 `{Xxx}`，转义符 `\n` `\t` 原样保留
   - 日志输出（PluginLog.*）默认不翻译
   - 参与逻辑判断的字符串：`==` 比较、`switch` 分支、字典键、JSON 序列化键、正则匹配——遇到保持原样并在汇报中注明
   - 游戏数据字段（物品名、地名等来自游戏客户端 EXD 数据，随客户端语言自动本地化）
3. UI 文本求简洁：按钮/菜单项 2-6 个字，句子符合中文习惯；中文与英文/数字之间加空格（如 `持有 3 个`）。
4. 字符串内若有引号等特殊字符，保证 C# 转义正确（半角双引号在字符串内须写成 `\"`）。
5. `InventoryTools\Localizers\` 目录下的 ContentTypeLocalizer 等文件中，映射到游戏内容类型的英文显示名改用国服官方内容名（如 Hall of Novice→新人馆、The Diadem→无名岛）。

## 术语表（国服官方译名优先）
| 英文 | 中文 |
|---|---|
| Filter / Filters | 筛选器 |
| Craft List | 制作清单 |
| Retainer | 雇员 |
| Market Board | 市场板 |
| Gil | 金币 |
| Inventory | 库存 |
| Armoury / Armoury Chest | 军械库 |
| Chocobo Saddlebag | 陆行鸟鞍袋 |
| Saddlebag | 鞍袋 |
| Glamour | 投影 |
| Armoire | 武器柜 |
| Company Chest | 部队金库 |
| Aetheryte | 以太之光 |
| Venture | 探索 |
| Character | 角色 |
| Item | 物品 |
| Quantity | 数量 |
| Source | 来源 |
| Can Be Obtained From | 获取途径 |
| Vendor | 商人 |
| Craft / Crafting | 制作 |
| Gathering | 采集 |
| Duty | 副本 |
| FATE | FATE |
| Quest | 任务 |
| Tradeable | 可交易 |
| Storage | 存储 |
| On Hand | 持有 |
| Required / Needed | 所需 |
| Missing | 缺少 |
| Highlight | 高亮 |
| Overlay | 悬浮层 |
| Tooltip | 提示 |
| Transfer | 转移 |
| Withdraw | 取出 |
| Deposit | 存入 |
| Compare | 对比 |
| Apply | 应用 |
| Save | 保存 |
| Cancel | 取消 |
| Delete | 删除 |
| Rename | 重命名 |
| Export / Import | 导出 / 导入 |
| Settings | 设置 |
| Configuration | 配置 |
| Close | 关闭 |
| Refresh | 刷新 |
| Search | 搜索 |
| Clear | 清空 |
| Wishlist | 愿望单 |
| Allagan | 亚拉戈 |
| List | 清单 |
| Shared | 共享 |
| Enabled / Disabled | 启用 / 停用 |
