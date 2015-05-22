using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino;
using Rhino.Geometry;

namespace _5.Classes
{
    class Loop : Rhino_Processing//test
    {
        List<Vertex> vertex = new List<Vertex>();

        public override void Setup()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                int num1 = rnd.Next(0,200);
                int num2 = rnd.Next(0, 200);
                int edge_num = rnd.Next(1, 4);
                int[] edge_id = new int[1];
                double[] edge_distance = new double[1];
                vertex.Add(new Vertex(i, edge_num, rnd, edge_id, edge_distance, 0, 0, false,  num1, num2));
                vertex[i].MarkPoint();
            }

            for (int i = 0; i < vertex.Count; i++)
            {
                vertex[0].StartEnd();
                vertex[vertex.Count - 1].StartEnd();
                vertex[i].Neighborhood(vertex);
                vertex[i].MakeNode(vertex);
            }
            
            int min_id = 0;
            vertex[0].condition = true;

            while (vertex[vertex.Count - 1].condition != true)
            {
                for (int i = 0; i < vertex[min_id].edge_id.Length; i++)
                {
                    //RhinoApp.WriteLine(String.Format("{0}", vertex[vertex[min_id].edge_id[i]].cost));
                    if (vertex[vertex[min_id].edge_id[i]].cost == 0) //枝の先が確定している場合<=これいらないかもね。かつコストに何も入力されていない場合。
                    {
                        vertex[vertex[min_id].edge_id[i]].cost = vertex[min_id].cost + vertex[min_id].edge_distance[i];
                        vertex[vertex[min_id].edge_id[i]].former_id = min_id;
                        //RhinoApp.WriteLine(String.Format("{0}", min_id));
                    }
                    else if (vertex[vertex[min_id].edge_id[i]].cost > vertex[min_id].cost + vertex[min_id].edge_distance[i])//枝先に既にあるコストといまの経路を比較
                    {
                        vertex[vertex[min_id].edge_id[i]].cost = vertex[min_id].cost + vertex[min_id].edge_distance[i];
                        vertex[vertex[min_id].edge_id[i]].former_id = min_id;
                    }
                }

                for (int i = 0; i < vertex.Count; i++)
                {
                    if (vertex[min_id].condition == true && vertex[i].cost != 0)
                    {
                        min_id =i;
                    }
                }

                for (int i = 0; i < vertex.Count; i++)
                {
                    if (vertex[i].condition == false && vertex[i].cost != 0 && vertex[i].cost < vertex[min_id].cost)//これは０)//存在するすべてのコストよりmin_idのコストが小さかったらそれはmin_idである。
                    {
                        min_id = i;
                    }
                }
                vertex[min_id].condition = true;//上記にて決定。
            }

            int a = vertex.Count - 1;
            int b = 100;
            while (b != 0)
            {
                vertex[a].DrawShortest(vertex);
                vertex[a].Display2(doc);
                b = vertex[a].ReturnFormerId(b);
                a = b;
            }
            
            
        }
        

        public override void Draw()
        {
                for (int i = 0; i < vertex.Count; i++)
                {
                    vertex[i].Display(doc);
                } 
        }
    }
}