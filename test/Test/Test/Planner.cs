
namespace NPlanner.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using NPlanner.Graphplan;

    [TestClass]
    public class Planner
    {
        [TestMethod]
        public void ConjunctionionTest_Basic()
        {
            string literal = "ON (b3, Table) & ON (b2, b3) & ON (b1, b2)";
            Conjunction conj = new Conjunction(literal);

            Assert.AreEqual(3, conj.Literals.Count);

            Assert.AreEqual("ON (b3, Table)", conj.Literals[0]);
            Assert.AreEqual("ON (b2, b3)", conj.Literals[1]);
            Assert.AreEqual("ON (b1, b2)", conj.Literals[2]);

        }

        [TestMethod]
        public void ConjunctionionTest_Intersect()
        {
            string literal1 = "ON (b3, Table) & ON (b2, b3) & ON (b1, b2)";
            Conjunction conj1 = new Conjunction(literal1);

            string literal2 = "ON (b1, b2) & ON (b2, b3) & ON (b3, Table)";
            Conjunction conj2 = new Conjunction(literal2);

            Assert.IsTrue(conj1.Intersect(conj2));
            Assert.IsTrue(conj2.Intersect(conj1));

            string literal3 = "ON (b1, b3) & Clear (Table) & Clear (b1)";
            Conjunction conj3 = new Conjunction(literal3);

            Assert.IsFalse(conj1.Intersect(conj3));
            Assert.IsFalse(conj3.Intersect(conj1));
        }

        [TestMethod]
        public void ObjectTest()
        {
            Entity obj0 = new Entity("b1", "Block");
            Entity obj1 = new Entity("b2", "Block");
            Entity obj2 = new Entity("b3", "Block");
            Entity obj3 = new Entity("Table", "Block");

            Assert.AreEqual("b1", obj0.Name);
            Assert.AreEqual("Block", obj1.Type);
            Assert.AreEqual("b3", obj2.Name);
            Assert.AreEqual("Block", obj3.Type);            
        }

        [TestMethod]
        public void ParaListTest()
        {
            ParaList paraList = new ParaList("Block ?x, Block ?y, Car ?z");

            Assert.AreEqual("?x", paraList.Vars[0]);
            Assert.AreEqual("?y", paraList.Vars[1]);
            Assert.AreEqual("?z", paraList.Vars[2]);

            Assert.AreEqual("Block", paraList.Types[0]);
            Assert.AreEqual("Block", paraList.Types[1]);
            Assert.AreEqual("Car", paraList.Types[2]);

            Assert.AreEqual("Block ?x, Block ?y, Car ?z", paraList.ToString());
        }

        [TestMethod]
        public void UnifierTest()
        {
            string var = "?x, ?y, ?z";
            string val = "A B C";
            Unifier u = new Unifier (var, val);
            Assert.AreEqual("B", u.Get("?y"));
        }

        [TestMethod]
        public void OperatorTest()
        {
            EntitySet objects = new EntitySet();
            objects.addObject("b1", "Block");
            objects.addObject("b2", "Block");
            objects.addObject("b3", "Block");
            objects.addObject("Table", "Block");

            OperatorHead opMoveHead = new OperatorHead("Move", new ParaList("Block ?obj, Block ?from, Block ?to"));
            Condition opMoveCond = new Condition("?obj != ?from & ?obj != ?to & ?from != ?to & ?to != @Table");
            Conjunction opMovePre = new Conjunction("ON (?obj, ?from) & Clear (?obj) & Clear (?to)");
            Conjunction opMoveAdd = new Conjunction("ON (?obj, ?to) & Clear (?from)");
            Conjunction opMoveDel = new Conjunction("ON (?obj, ?from) & Clear (?to)");
            Operator opMove = new Operator(opMoveHead, opMoveCond, opMovePre, opMoveAdd, opMoveDel);

            opMove.GetPossibleUnifiers(objects);

            //foreach (string un in opMove.AllUnifiers)
            //{
            //    System.Console.WriteLine(un);
            //}

            // 3 个位置, 每个位置 4 种可能, 一共是 4^3=64.
            Assert.AreEqual(64, opMove.AllUnifiers.Count);


            Conjunction init = new Conjunction("ON (b1, b2) & ON (b2, Table) & ON (b3, Table) & Clear (b1) & Clear (b3) & Clear (Table)");
            List<NPlanner.Graphplan.Action> actions = opMove.GenActions(init);

            // (Block ?obj, Block ?from, Block ?to)
            // b1 b2 b3
            // b1 b2 table
            // b3 table b1
            Assert.AreEqual(3, actions.Count);
        }       


        [TestMethod]
        public void UtilTest()
        {
            string var = "?obj, ?from, ?to";
            string val = "b1 b2 b3";
            Unifier u = new Unifier(var, val);
            Conjunction conj = new Conjunction("ON (?obj, ?from) & Clear (?obj) & Clear (?to)");

            Assert.AreEqual("ON (b1, b2) & Clear (b1) & Clear (b3)", Util.Substitute(conj, u));
        }

        

        [TestMethod]
        public void GraphPlanTest_Blocks()
        {
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
        }

        [TestMethod]
        public void GraphPlanTest_Robot()
        {
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

            //foreach (string step in plan)
            //{
            //    System.Console.WriteLine(step);
            //}

            Assert.AreEqual(2, plan.Count);            

            Assert.AreEqual("ChangeColor ( green, red )", plan[0]);
            Assert.AreEqual("Paint ( sec1, red )", plan[1]);
            
        }     


        [TestMethod]
        public void GraphPlanTest_Rocket()
        {
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
            //Assert.IsTrue(res);

            res = graph.SearchGoal();
            //Assert.IsTrue(res);

            List<string> plan = graph.GetPlan();

            //foreach (string step in plan)
            //{
            //    System.Console.WriteLine(step);
            //}

            Assert.AreEqual(5, plan.Count);            

            Assert.AreEqual("load ( Rx, Lx, Ax )", plan[0]);
            Assert.AreEqual("load ( Rx, Lx, Bx )", plan[1]);
            Assert.AreEqual("move ( Rx, Lx, Px )", plan[2]);
            Assert.AreEqual("unload ( Rx, Px, Ax )", plan[3]);
            Assert.AreEqual("unload ( Rx, Px, Bx )", plan[4]);

        }
    }
}
