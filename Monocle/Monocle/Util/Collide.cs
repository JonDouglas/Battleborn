﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    static public class Collide
    {
        #region Entity vs Entity

        static public bool Check(Entity a, Entity b)
        {
            if (a.Collider == null || b.Collider == null)
                return false;
            else
                return a != b && b.Collidable && a.Collider.Collide(b);
        }

        static public bool Check(Entity a, Entity b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Entity vs Entity Enumerable

        #region Check

        static public bool Check(Entity a, IEnumerable<Entity> b)
        {
            foreach (var e in b)
                if (Check(a, e))
                    return true;

            return false;
        }

        static public bool Check(Entity a, IEnumerable<Entity> b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        #endregion

        #region First

        static public Entity First(Entity a, IEnumerable<Entity> b)
        {
            foreach (var e in b)
                if (Check(a, e))
                    return e;

            return null;
        }

        static public Entity First(Entity a, IEnumerable<Entity> b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            Entity ret = First(a, b);
            a.Position = old;
            return ret;
        }

        #endregion

        #region All

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into)
        {
            foreach (var e in b)
                if (Check(a, e))
                    into.Add(e);

            return into;
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            List<Entity> ret = All(a, b, into);
            a.Position = old;
            return ret;
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b)
        {
            return All(a, b, new List<Entity>());
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, Vector2 at)
        {
            return All(a, b, new List<Entity>(), at);
        }

        #endregion

        #endregion

        #region Entity vs Point

        static public bool CheckPoint(Entity a, Vector2 point)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(point);
        }

        static public bool CheckPoint(Entity a, Vector2 point, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = CheckPoint(a, point);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Entity vs Line

        static public bool CheckLine(Entity a, Vector2 from, Vector2 to)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(from, to);
        }

        static public bool CheckLine(Entity a, Vector2 from, Vector2 to, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = CheckLine(a, from, to);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Entity vs Rectangle

        static public bool CheckRect(Entity a, Rectangle rect)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(rect);
        }

        static public bool CheckRect(Entity a, Rectangle rect, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = CheckRect(a, rect);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Misc

        [Flags]
        private enum PointSectors { Center = 0, Top = 1, Bottom = 2, TopLeft = 9, TopRight = 5, Left = 8, Right = 4, BottomLeft = 10, BottomRight = 6 };

        static private PointSectors GetSector(Rectangle rect, Vector2 point)
        {
            PointSectors sector = PointSectors.Center;

            if (point.X < rect.Left)
                sector |= PointSectors.Left;
            else if (point.X >= rect.Right)
                sector |= PointSectors.Right;

            if (point.Y < rect.Top)
                sector |= PointSectors.Top;
            else if (point.Y >= rect.Bottom)
                sector |= PointSectors.Bottom;

            return sector;
        }

        static public bool LineCheck(Rectangle rect, Vector2 from, Vector2 to)
        {
            PointSectors fromSector = GetSector(rect, from);
            PointSectors toSector = GetSector(rect, to);

            if (fromSector == PointSectors.Center || toSector == PointSectors.Center)
                return true;
            else if ((fromSector & toSector) != 0)
                return false;
            else
            {
                PointSectors both = fromSector | toSector;

                //Do line checks against the edges
                Vector2 edgeFrom;
                Vector2 edgeTo;

                if ((both & PointSectors.Top) != 0)
                {
                    edgeFrom = new Vector2(rect.Left, rect.Top);
                    edgeTo = new Vector2(rect.Right, rect.Top);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Bottom) != 0)
                {
                    edgeFrom = new Vector2(rect.Left, rect.Bottom);
                    edgeTo = new Vector2(rect.Right, rect.Bottom);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Left) != 0)
                {
                    edgeFrom = new Vector2(rect.Left, rect.Top);
                    edgeTo = new Vector2(rect.Left, rect.Bottom);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Right) != 0)
                {
                    edgeFrom = new Vector2(rect.Right, rect.Top);
                    edgeTo = new Vector2(rect.Right, rect.Bottom);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }
            }

            return false;
        }

        static public bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            return true;
        }

        static public bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * b;

            return true;
        }

        #endregion
    }
}
