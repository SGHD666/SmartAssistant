# SmartAssistant 开发准则

## 核心原则

### 1. 专业性要求
- 以专业全栈程序员的标准进行开发
- 遵循行业最佳实践
- 保持代码的专业性和一致性

### 2. 需求分析原则
- 对用户提出的要求先分析其合理性和可行性
- 发现不合理或不可行的要求及时与用户沟通
- 针对问题提供可行的替代方案
- 确保最终方案既满足用户需求又技术可行

### 3. 代码稳定性
- ⚠️ 已确认的功能代码严禁随意修改
- 如必须修改已确认的代码，需要：
  1. 提前告知并获得确认
  2. 详细说明修改原因
  3. 列出可能的影响范围
  4. 提供回滚方案

### 4. 代码完整性
- ✅ 保持已正常运行的代码不变
- 新功能开发遵循开闭原则（Open-Closed Principle）
  - 对扩展开放
  - 对修改关闭
  - 通过接口和抽象进行扩展

### 5. Bug修复准则
- 修复bug时不引入新的问题
- 全面分析问题根源
- 一次性修复相关联的所有bug
- 添加单元测试预防类似bug

### 6. 代码质量
- 保持代码可读性
  - 清晰的命名规范
  - 详细的注释说明
  - 合理的代码结构
- 编码规范
  - 使用一致的代码风格
  - 遵循C#编码规范
  - 适当的空行和缩进

### 7. 问题分析
- 全面排查问题
  - 检查相关联的代码
  - 分析潜在的影响
  - 考虑边界情况
- 提供完整的解决方案
  - 不留遗留问题
  - 确保向后兼容
  - 维护系统稳定性

## Git 工作流

### 分支策略
- `main` 分支
  - 生产环境代码
  - 只接受来自 `develop` 的合并
  - 每次合并都要打标签
- `develop` 分支
  - 开发主分支
  - 包含最新的开发特性
  - 保持可构建状态
- 功能分支 `feature/*`
  - 从 `develop` 创建
  - 完成后合并回 `develop`
  - 命名规范：`feature/功能名称`
- 修复分支 `bugfix/*`
  - 用于修复 `develop` 中的问题
  - 完成后合并回 `develop`
- 热修复分支 `hotfix/*`
  - 用于修复生产环境问题
  - 从 `main` 创建
  - 同时合并回 `main` 和 `develop`

### 提交规范
- 提交信息格式：
  ```
  <type>(<scope>): <subject>

  <body>

  <footer>
  ```
- Type 类型：
  - feat: 新功能
  - fix: 修复bug
  - docs: 文档更新
  - style: 代码格式调整
  - refactor: 重构
  - test: 测试相关
  - chore: 构建/工具链相关
- 示例：
  ```
  feat(ui): add message type indicator

  - Add icon color converter
  - Implement message type enum
  - Update UI to show different colors

  Closes #123
  ```

## 单元测试规范

### 测试原则
1. 单一职责
   - 每个测试只测试一个场景
   - 保持测试简单明了
2. 可重复性
   - 测试结果必须稳定
   - 不依赖外部环境
3. 独立性
   - 测试之间相互独立
   - 避免测试间的依赖

### 测试结构
1. AAA模式
   ```csharp
   [Fact]
   public void Method_Scenario_ExpectedResult()
   {
       // Arrange - 准备测试数据和环境
       var sut = new SystemUnderTest();
       var input = "test input";

       // Act - 执行被测试的代码
       var result = sut.Method(input);

       // Assert - 验证结果
       Assert.Equal(expected, result);
   }
   ```

2. 命名规范
   - 类名：`{被测试类}Tests`
   - 方法名：`{被测试方法}_{测试场景}_{预期结果}`

### 测试覆盖
- 代码覆盖率目标：80%以上
- 重点覆盖：
  - 业务逻辑
  - 边界条件
  - 错误处理
- 可以忽略：
  - 简单属性
  - 框架生成的代码

### Recent Updates
- **Unit Test Improvements**:
  - Enhanced mocking strategies for `ModelManager` and `AssistantController`.
  - Ensured all dependencies are properly mocked using Moq.
  - Achieved 100% pass rate for all unit tests in `MainWindowViewModelTests`.

## 开发流程

1. 新功能开发
   - 先设计后实现
   - 遵循SOLID原则
   - 编写单元测试
   - 进行代码审查

2. Bug修复
   - 完整复现问题
   - 分析根本原因
   - 设计修复方案
   - 全面回归测试

3. 代码维护
   - 定期代码审查
   - 持续重构优化
   - 更新技术文档
   - 维护版本历史

## 注意事项

⚠️ 特别强调：
1. 不随意修改已确认的功能
2. 保持代码稳定性
3. 遵循开闭原则
4. 修复bug不引入新问题
5. 保持代码可读性
6. 全面分析解决问题
7. 遵循Git工作流规范
8. 保持测试用例的质量

本文档作为开发过程中的首要参考指南，每次开始开发前必须先阅读并遵循这些准则。
