using System.Collections.Generic;
using NPlanner.Graphplan;

namespace NPlanner
{
    class Program
    {
        public static  void Main()
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

            foreach (string step in plan)
            {
                System.Console.WriteLine(step);
            }
            
        }
    }
}
