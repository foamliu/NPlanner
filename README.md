# NPlanner
NPlanner是C#编写的图规划算法的简单粗暴的实现。

## 用法

### 积木世界
初始状态：

![image](https://github.com/foamliu/NPlanner/raw/master/images/blocks_init.png)

目标状态：
![image](https://github.com/foamliu/NPlanner/raw/master/images/blocks_goal.png)

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