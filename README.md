# NPlanner
NPlanner是C#编写的图规划算法实现，支持STRIPS语言。

## 用法

### 积木世界
初始状态：

![image](https://github.com/foamliu/NPlanner/raw/master/images/blocks_init.png)

目标状态：

![image](https://github.com/foamliu/NPlanner/raw/master/images/blocks_goal.png)

生成计划：
```csharp
MoveToTable ( b1, b2 )
Move ( b2, Table, b3 )
Move ( b1, Table, b2 )
```

代码示例：

```csharp
EntitySet objects = new EntitySet();
objects.addObject("b1", "Block");
objects.addObject("b2", "Block");
objects.addObject("b3", "Block");
objects.addObject("Table", "Block");

Conjunction init = new Conjunction("ON (b1, b2) & ON (b2, Table) & ON (b3, Table) & Clear (b1) & Clear (b3) & Clear (Table)");
Conjunction goal = new Conjunction("ON (b3, Table) & ON (b2, b3) & ON (b1, b2)");

OperatorHead opMoveHead = new OperatorHead("Move", new ParaList("Block ?obj, Block ?from, Block ?to"));
Condition opMoveCond = new Condition("?obj != ?from & ?obj != ?to & ?from != ?to & ?to != Table");
Conjunction opMovePre = new Conjunction("ON (?obj, ?from) & Clear (?obj) & Clear (?to)");
Conjunction opMoveAdd = new Conjunction("ON (?obj, ?to) & Clear (?from)");
Conjunction opMoveDel = new Conjunction("ON (?obj, ?from) & Clear (?to)");
Operator opMove = new Operator(opMoveHead, opMoveCond, opMovePre, opMoveAdd, opMoveDel);

OperatorHead opMoveToTableHead = new OperatorHead("MoveToTable", new ParaList("Block ?obj, Block ?from"));
Condition opMoveToTableCond = new Condition("?obj != ?from & ?obj != Table & ?from != Table");
Conjunction opMoveToTablePre = new Conjunction("Clear (?obj) & ON (?obj, ?from)");
Conjunction opMoveToTableAdd = new Conjunction("ON (?obj, Table) & Clear (?from)");
Conjunction opMoveToTableDel = new Conjunction("ON (?obj, ?from)");
Operator opMoveToTable = new Operator(opMoveToTableHead, opMoveToTableCond, opMoveToTablePre, opMoveToTableAdd, opMoveToTableDel);

OperatorSet opSet = new OperatorSet();
opSet.Operators.Add(opMove);
opSet.Operators.Add(opMoveToTable);

GraphPlan graph = new GraphPlan(objects, init, goal, opSet);
bool res = graph.CreateGraph();
Assert.IsTrue(res);            

res = graph.SearchGoal();
Assert.IsTrue(res);    

List<string> plan = graph.GetPlan();

Assert.AreEqual(3, plan.Count);

Assert.AreEqual("MoveToTable ( b1, b2 )", plan[0]);
Assert.AreEqual("Move ( b2, Table, b3 )", plan[1]);
Assert.AreEqual("Move ( b1, Table, b2 )", plan[2]);
```

### 机器人

```csharp
EntitySet objects = new EntitySet();
objects.addObject("green", "Color");
objects.addObject("red", "Color");
objects.addObject("sec1", "Section");

Conjunction init = new Conjunction("CurrentColor (green)");
Conjunction goal = new Conjunction("Painted (sec1, red)");

OperatorHead opPaintHead = new OperatorHead("Paint", new ParaList("Section ?sec, Color ?clr"));
Condition opPaintCond = new Condition("");
Conjunction opPaintPre = new Conjunction("CurrentColor (?clr)");
Conjunction opPaintAdd = new Conjunction("Painted (?sec, ?clr)");
Conjunction opPaintDel = new Conjunction("");
Operator opPaint = new Operator(opPaintHead, opPaintCond, opPaintPre, opPaintAdd, opPaintDel);

OperatorHead opChangeColorHead = new OperatorHead("ChangeColor", new ParaList("Color ?old, Color ?new"));
Condition opChangeColorCond = new Condition("?old != ?new");
Conjunction opChangeColorPre = new Conjunction("CurrentColor (?old)");
Conjunction opChangeColorAdd = new Conjunction("CurrentColor (?new)");
Conjunction opChangeColorDel = new Conjunction("CurrentColor (?old)");
Operator opChangeColor = new Operator(opChangeColorHead, opChangeColorCond, opChangeColorPre, opChangeColorAdd, opChangeColorDel);

OperatorSet opSet = new OperatorSet();
opSet.Operators.Add(opPaint);
opSet.Operators.Add(opChangeColor);

GraphPlan graph = new GraphPlan(objects, init, goal, opSet);
bool res = graph.CreateGraph();
Assert.IsTrue(res);

res = graph.SearchGoal();
Assert.IsTrue(res);

List<string> plan = graph.GetPlan();

Assert.AreEqual(2, plan.Count);            

Assert.AreEqual("ChangeColor ( green, red )", plan[0]);
Assert.AreEqual("Paint ( sec1, red )", plan[1]);
```

### Rocket

初始状态：

![image](https://github.com/foamliu/NPlanner/raw/master/images/rocket_init.png)

目标状态：

![image](https://github.com/foamliu/NPlanner/raw/master/images/rocket_goal.png)

生成计划：
```csharp
load ( Rx, Lx, Ax )
load ( Rx, Lx, Bx )
move ( Rx, Lx, Px )
unload ( Rx, Px, Ax )
unload ( Rx, Px, Bx )
```

代码示例：

```csharp
EntitySet objects = new EntitySet();
objects.addObject("Ax", "Cargo");
objects.addObject("Bx", "Cargo");
objects.addObject("Rx", "Rocket");
objects.addObject("Lx", "Place");
objects.addObject("Px", "Place");

Conjunction init = new Conjunction("At (Ax, Lx) & At (Bx, Lx) & At (Rx, Lx) & HasFuel (Rx)");
Conjunction goal = new Conjunction("At (Ax, Px) & At (Bx, Px)");

OperatorHead opMoveHead = new OperatorHead("move", new ParaList("Rocket ?r, Place ?from, Place ?to"));
Condition opMoveCond = new Condition("?from != ?to");
Conjunction opMovePre = new Conjunction("At (?r, ?from) & HasFuel (?r)");
Conjunction opMoveAdd = new Conjunction("At (?r, ?to)");
Conjunction opMoveDel = new Conjunction("At (?r, ?from) & HasFuel (?r)");
Operator opMove = new Operator(opMoveHead, opMoveCond, opMovePre, opMoveAdd, opMoveDel);

OperatorHead opUnloadHead = new OperatorHead("unload", new ParaList("Rocket ?r, Place ?p, Cargo ?c"));
Condition opUnloadCond = new Condition("");
Conjunction opUnloadPre = new Conjunction("At (?r, ?p) & In (?c, ?r)");
Conjunction opUnloadAdd = new Conjunction("At (?c, ?p)");
Conjunction opUnloadDel = new Conjunction("In (?c, ?r)");
Operator opUnload = new Operator(opUnloadHead, opUnloadCond, opUnloadPre, opUnloadAdd, opUnloadDel);

OperatorHead opLoadHead = new OperatorHead("load", new ParaList("Rocket ?r, Place ?p, Cargo ?c"));
Condition opLoadCond = new Condition("");
Conjunction opLoadPre = new Conjunction("At (?r, ?p) & At (?c, ?p)");
Conjunction opLoadAdd = new Conjunction("In (?c, ?r)");
Conjunction opLoadDel = new Conjunction("At (?c, ?p)");
Operator opLoad = new Operator(opLoadHead, opLoadCond, opLoadPre, opLoadAdd, opLoadDel);


OperatorSet opSet = new OperatorSet();
opSet.Operators.Add(opMove);
opSet.Operators.Add(opUnload);
opSet.Operators.Add(opLoad);

GraphPlan graph = new GraphPlan(objects, init, goal, opSet);
bool res = graph.CreateGraph();

res = graph.SearchGoal();

List<string> plan = graph.GetPlan();

Assert.AreEqual(5, plan.Count);            

Assert.AreEqual("load ( Rx, Lx, Ax )", plan[0]);
Assert.AreEqual("load ( Rx, Lx, Bx )", plan[1]);
Assert.AreEqual("move ( Rx, Lx, Px )", plan[2]);
Assert.AreEqual("unload ( Rx, Px, Ax )", plan[3]);
Assert.AreEqual("unload ( Rx, Px, Bx )", plan[4]);
```