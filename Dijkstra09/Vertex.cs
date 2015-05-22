using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;

namespace _5.Classes
{
    class Vertex : Rhino_Processing
    {
        //////////////
        //properties//
        //////////////
        private int id;
        private int edge_num;
        private Random rnd;
        public int[] edge_id;
        public double[] edge_distance;
        public double cost;
        public int former_id;
        public bool condition;
        private double X_coordinate;
        private double Y_coordinate;

        private Point3d position;
        private Line line1, line2, line3, Shortest_line;

        private Sphere sph;

        public Brep pipe;


        /////////////////
        //constructors//
        ////////////////

        public Vertex(
            int _id,
            int _edge_num,
            Random _rnd,
            int[] _edge_id,
            double[] _edge_distance,
            double _cost,
            int _former_id,
            bool _condition,
            double _X_coordinate,
            double _Y_coordinate
            )
        {
            // set variables
            id = _id;
            edge_num = _edge_num;
            rnd = _rnd;
            edge_id = _edge_id;
            edge_distance = _edge_distance;
            cost = _cost;
            former_id = _former_id;
            condition = _condition;
            X_coordinate = _X_coordinate;
            Y_coordinate = _Y_coordinate;
        }

        ///////////
        //methods//
        ///////////

        //
        public void MarkPoint()
        {
            position = new Point3d(X_coordinate, Y_coordinate, 0);
        }

        public void Neighborhood(List<Vertex> vertex)
        {
            if (id == vertex.Count - 1)
            {
                edge_id = new int[0] { };
            }
            else if (id == vertex.Count - 2)
            {
                int num1 = vertex.Count - 1;
                edge_id = new int[1] { num1 };
            }
            else if (id == vertex.Count - 3)
            {
                if (edge_num == 1)
                {
                    int num1 = rnd.Next(id + 1, vertex.Count);

                    edge_id = new int[1] { num1 };
                }
                else if (edge_num == 2 || edge_num == 3)
                {
                    int num1 = vertex.Count - 2;
                    int num2 = vertex.Count - 1;

                    Array.Resize(ref edge_id, 2);
                    edge_id = new int[2] { num1, num2 };
                }
            }

            else
            {
                if (edge_num == 1)
                {

                    int num1 = rnd.Next(id + 1, vertex.Count);

                    edge_id = new int[1] { num1 };
                }
                else if (edge_num == 2)
                {
                    int num1 = rnd.Next(id + 1, vertex.Count - 1);
                    int num2 = rnd.Next(num1 + 1, vertex.Count);

                    Array.Resize(ref edge_id, 2);
                    edge_id = new int[2] { num1, num2 };
                }
                else if (edge_num == 3)
                {
                    int num1 = rnd.Next(id + 1, vertex.Count - 2);
                    int num2 = rnd.Next(num1 + 1, vertex.Count - 1);
                    int num3 = rnd.Next(num2 + 1, vertex.Count);

                    Array.Resize(ref edge_id, 3);
                    edge_id = new int[3] { num1, num2, num3 };
                }
            }
        }

        public void StartEnd()
        {
            sph = new Sphere(position, 3);
        }

        public void MakeNode(List<Vertex> vertex)
        {
            if (edge_id.Length == 1)
            {
                int a = edge_id[0];
                line1 = new Line(position, vertex[a].position);
                edge_distance = new double[1]{line1.Length};
            }
            else if (edge_id.Length == 2)
            {
                int a = edge_id[0];
                int b = edge_id[1];
                line1 = new Line(position, vertex[a].position);
                line2 = new Line(position, vertex[b].position);

                Array.Resize(ref edge_distance, 2);
                edge_distance = new double[2] { line1.Length, line2.Length };
            }
            else if (edge_id.Length == 3)
            {
                int a = edge_id[0];
                int b = edge_id[1];
                int c = edge_id[2];
                line1 = new Line(position, vertex[a].position);
                line2 = new Line(position, vertex[b].position);
                line3 = new Line(position, vertex[c].position);
                Array.Resize(ref edge_distance, 3);
                edge_distance = new double[3] { line1.Length, line2.Length, line3.Length };
            }
        }
       
        public void Compare(List<Vertex> vertex)
        {
            int min_id = 0;
            vertex[0].condition = true;

            while (vertex[vertex.Count - 1].condition != true)
            {
                RhinoApp.WriteLine(String.Format("{0}", min_id));
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
                RhinoApp.WriteLine(String.Format("{0}", min_id));
                vertex[min_id].condition = true;//上記にて決定。

            }
        }

        public void DrawShortest(List<Vertex> vertex)
        {
            Shortest_line = new Line(position, vertex[former_id].position);
            LineCurve crvline = new LineCurve(Shortest_line);
            pipe = Brep.CreatePipe(crvline, 10, true, PipeCapMode.Round, true, 0, 0)[0];
        }

        public int ReturnFormerId(int b)
        {
            return former_id;
        }

        //
        public void Display(RhinoDoc _doc)
        {
                _doc.Objects.AddPoint(position);
                _doc.Objects.AddSphere(sph);
                _doc.Objects.AddLine(line1);
                _doc.Objects.AddLine(line2);
                _doc.Objects.AddLine(line3);

        }
        public void Display2(RhinoDoc _doc)
        {
            _doc.Objects.AddBrep(pipe);
        }

    }
}

//各点からの距離を計測し、それを大きさに順じて並び替えることができた。
//なんか描画されるようになった。
//たまにうまくいく