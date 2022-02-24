using System;
using System.Collections.Generic;
using System.Drawing;

namespace TowerDefence
{
    public abstract class GameObject
    {
        protected double Dist(Point pt1, Point pt2) => Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));

        protected int posX, posY;
        protected Size size;
        protected Graphics g;
        public Point pos;
        protected ObjectManager _obj;
        protected void init(ObjectManager _obj, Graphics g, int x, int y)
        {
            this._obj = _obj;
            this.g = g;
            posX = x;
            posY = y;
            pos = new Point(x, y);
        }
        protected void init(ObjectManager _obj, Graphics g, Point p)
        {
            this._obj = _obj;
            this.g = g;
            posX = p.X;
            posY = p.Y;
            pos = p;
        }

        public abstract void Render();
        public abstract void Step();
        public abstract bool CheckPos(Point p);
        public abstract void Click();
        public virtual bool Intersection(Point p) => (Math.Abs(posX - p.X) <= ((size.Width + 30) / 2)) && (Math.Abs(posY - p.Y) <= ((size.Height + 30) / 2));
        protected bool isSelected() => (_obj.selected == this);
        protected void Sel() => _obj.selected = this;
        protected void IntToPoint() => pos = new Point(posX, posY);
        protected void PointToint() { posX = pos.X; posY = pos.Y; }
    }

    public class CheckPoint : GameObject
    {
        public int number;
        protected CheckPoint() { }
        public CheckPoint(ObjectManager _obj, Graphics g, Point p, int num)
        {
            init(_obj, g, p);
            number = num;
            _obj.objs.Add(this);
        }
        public override void Render()
        {
            g.DrawLine(new Pen(Color.Green), pos, _obj.checkpoints[number + 1].pos);

        }
        public override void Click() { }
        public override void Step()
        {
        }
        public override bool CheckPos(Point p)
        {
            return false;
        }
        public override bool Intersection(Point p)
        {
            Point po = new Point((pos.X + _obj.checkpoints[number + 1].pos.X) / 2, (pos.Y + _obj.checkpoints[number + 1].pos.Y) / 2);
            Size s = new Size(Math.Abs(pos.X - _obj.checkpoints[number + 1].pos.X) + 6, Math.Abs(pos.Y - _obj.checkpoints[number + 1].pos.Y) + 6);
            return (Math.Abs(po.X - p.X) <= ((s.Width + 30) / 2)) && (Math.Abs(po.Y - p.Y) <= ((s.Height + 30) / 2));
        }

    }

    public class Fort : CheckPoint
    {

        public Fort(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            size = new Size(60, 60);
            _obj.objs.Add(this);
            _obj.checkpoints.Add(this);

        }


        public override void Click() { }
        public override void Render() => g.DrawImage(Resources.FortSprite, new Point(posX - 30, posY - 30));
        public override bool CheckPos(Point p) => (Math.Abs(p.X - posX) <= 30) && (Math.Abs(p.Y - posY) <= 30);
        public override bool Intersection(Point p) => (Math.Abs(posX - p.X) <= ((size.Width + 30) / 2)) && (Math.Abs(posY - p.Y) <= ((size.Height + 30) / 2));
    }

    public class Enemy : GameObject
    {
        protected int spd, dam, xp, maxXp, usd;
        protected Point targ;
        protected int numTarg = 1;


        public Enemy()
        {
            size = new Size(0, 0);
        }
        public Enemy(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            targ = _obj.checkpoints[numTarg].pos;
        }
        public override void Click()
        {
            Sel();
        }
        public override void Render() { }
        public override void Step()
        {
            if (Dist(targ, pos) >= spd)
            {
                int fX = targ.X - posX;
                int fY = targ.Y - posY;
                if (Math.Abs(fX) >= spd && fX != 0) posX += spd * fX / Math.Abs(fX); else posX = targ.X;
                if (Math.Abs(fY) >= spd && fY != 0) posY += spd * fY / Math.Abs(fY); else posY = targ.Y;
                IntToPoint();
                if (xp <= 0) Destroy(true);
            }
            else
            {
                numTarg++;
                targ = _obj.checkpoints[numTarg].pos;
            }
            if (_obj.fort.CheckPos(pos))
            {
                Destroy();
                _obj.XP -= dam;
            }

        }
        public override bool CheckPos(Point p) => Dist(p, pos) <= 6;
        public void Destroy(bool kill = false)
        {
            if (kill) _obj.USD += usd + (_obj.sason % 9 - 4);
            _obj.delList.Add(this);
        }
        public void Damadge(int dam) => xp -= dam;

    }

    public class Solder : Enemy
    {


        public Solder(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            targ = _obj.checkpoints[numTarg].pos;
            spd = 2; dam = 2; maxXp = 200 + (_obj.Lavle / 4); xp = maxXp; usd = 18;
            _obj.enemies.Add(this);
        }
        public override void Render()
        {
            g.DrawEllipse(new Pen(Color.Red), posX - 3, posY - 3, 6, 6);
            if (xp < maxXp)
            {
                g.DrawLine(new Pen(Color.Red, 2), pos.X - 10, pos.Y - 6, pos.X - 10 + 20, pos.Y - 6);
                g.DrawLine(new Pen(Color.Green, 2), pos.X - 10, pos.Y - 6, pos.X - 10 + (20 * xp / maxXp), pos.Y - 6);
            }
        }

    }

    public class Vert : Enemy
    {
        public Vert(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            targ = _obj.checkpoints[numTarg].pos;
            spd = 3; dam = 3; maxXp = 120 + (_obj.Lavle / 4); xp = maxXp; usd = 21;
            _obj.enemies.Add(this);
        }

        public override void Render()
        {
            g.DrawLine(new Pen(Color.FromArgb(170, 165, 255)), pos - new Size(3, 3), pos + new Size(3, 3));
            g.DrawLine(new Pen(Color.FromArgb(170, 165, 255)), pos - new Size(-3, 3), pos + new Size(-3, 3));
            if (xp < maxXp)
            {
                g.DrawLine(new Pen(Color.Red, 2), pos.X - 10, pos.Y - 6, pos.X - 10 + 20, pos.Y - 6);
                g.DrawLine(new Pen(Color.Green, 2), pos.X - 10, pos.Y - 6, pos.X - 10 + (20 * xp / maxXp), pos.Y - 6);
            }
        }
    }

    public class Waerpon : GameObject
    {
        Image sprite;
        int type, dist, damage, kd, iskd = 0, lavle = -1;
        Enemy enemy = null;
        bool isFire = false;
        public Waerpon(ObjectManager _obj, Graphics g, Point p, int type)
        {
            init(_obj, g, p);
            sprite = Data.spr[type];
            this.type = type;
            size = new Size(30, 30);
            LavleUp();
        }
        void LavleUp()
        {
            lavle++;
            _obj.USD -= Data.prise[type, lavle];
            dist = Data.dist[type, lavle];
            kd = Data.kd[type, lavle];
            damage = Data.damage[type, lavle];
            
        }

        public override void Click()
        {
            if (isSelected() && lavle < 4)
                if (_obj.USD >= Data.prise[type, lavle + 1])
                {
                    LavleUp();
                }
            Sel();
        }
        public override void Render()
        {
            if (!isFire)
                g.DrawImage(sprite, new Point(posX - 15, posY - 15));
            else
            {
                g.DrawImage(sprite, new Rectangle(pos.X - 16, pos.Y - 16, 32, 32));
                if (type == 1)
                    g.DrawLine(new Pen(Data.LaserColor[lavle]), pos, enemy.pos);
                else if (type == 2)
                    g.DrawEllipse(new Pen(Color.FromArgb(50, 234, 255, 3)), pos.X - dist, pos.Y - dist, dist * 2, dist * 2);
                else
                    g.DrawEllipse(new Pen(Color.FromArgb(150, 234, 255-(lavle*50), 255 - (lavle * 50))), enemy.pos.X - 18, enemy.pos.Y - 18, 36, 36);
            }

            if (isSelected())
            {
                g.DrawEllipse(new Pen(Color.FromArgb(50, 255, 255, 255)), pos.X - dist, pos.Y - dist, dist * 2, dist * 2);
                if(lavle<4) g.DrawString($"${Data.prise[type, lavle + 1]}", new Font("Times New Roman", 8), new SolidBrush(Color.White), pos + new Size(16, -4));
            }
            g.DrawString($"{lavle+1}", new Font("Times New Roman", 8), new SolidBrush(Color.White), pos + new Size(5, 5));




        }
        public override void Step()
        {
            isFire = false;
            iskd = Math.Max(0, iskd - 1);
            if (iskd == 0) Fire();

        }

        void Fire()
        {
            if (type == 1)
            {
                foreach (var a in _obj.enemies)
                {
                    if (Dist(a.pos, pos) <= dist)
                    {
                        isFire = true;
                        a.Damadge(damage);
                        enemy = a;
                        iskd = kd;
                        break;
                    }
                }
            }
            else if (type == 2)
            {
                foreach (var a in _obj.enemies)
                {
                    if (Dist(a.pos, pos) <= dist)
                    {
                        isFire = true;
                        a.Damadge(damage);
                        iskd = kd;
                    }
                }
            }
            else
            {
                List<Enemy> enemies = _obj.enemies.FindAll(Enemy => Dist(Enemy.pos, pos) <= dist);
                if (enemies.Count > 0)
                {
                    isFire = true;
                    enemy = enemies[enemies.Count / 2];
                    iskd = kd;
                    _obj.enemies.FindAll(Enemy => Dist(Enemy.pos, enemy.pos) <= 18).ForEach(Enemy => Enemy.Damadge(damage));
                }
            }

        }
        public override bool CheckPos(Point p) => (Math.Abs(p.X - posX) <= 15) && (Math.Abs(p.Y - posY) <= 15);


    }

    public class Button : GameObject
    {
        Image bSprite;
        int num;

        public Button(ObjectManager _obj, Graphics g, Point p, Image bSprite, int num)
        {
            init(_obj, g, p);
            _obj.objs.Add(this);
            this.bSprite = bSprite;
            this.num = num;
            size = new Size(100, 100);
        }
        public override void Step()
        {
            if (isSelected()) _obj.Rez = num;
            else if (_obj.Rez == num) _obj.Rez = 0;
        }
        public override void Render()
        {
            g.DrawImage(bSprite, pos - new Size(50, 50));
            g.DrawString($"${Data.prise[num, 0]}", new Font("Times New Roman", 14), new SolidBrush(Color.White), pos - new Size(23, 70));
            if (isSelected()) g.DrawRectangle(new Pen(Color.Red), new Rectangle(pos.X - 50, pos.Y - 50, 100, 100));

        }
        public override bool CheckPos(Point p) => (p.X >= pos.X - 50 && p.X <= pos.X + 50 && p.Y >= pos.Y - 50 && p.Y <= pos.Y + 50);
        public override void Click()
        {
            if (Data.prise[num, 0] <= _obj.USD)
                Sel();
        }
    }

    static class Data
    {
        public static Image[] spr = { new Bitmap(1, 1), Resources.PVOSprite, Resources.PressSprite, Resources.BahaSprite };
        public static int[,] prise = { { 0, 0, 0, 0, 0 }, { 100, 175, 650, 1900, 2500 }, { 175, 350, 1000, 2000, 3000 }, { 225, 400, 1350, 2250, 3500 } };
        public static int[,] dist = { { 0, 0, 0, 0, 0 }, { 65, 66, 67, 68, 70 }, { 45, 45, 45, 48, 49 }, { 80, 85, 88, 90, 90 } };
        public static int[,] damage = { { 0, 0, 0, 0, 0 }, { 20, 25, 30, 10, 20 }, { 6, 7, 8, 8, 9 }, { 80, 80, 80, 140, 140 } };
        public static int[,] kd = { { 0, 0, 0, 0, 0 }, { 15, 15, 14, 7, 6 }, { 6, 6, 5, 4, 4 }, { 40, 35, 29, 40, 35 } };
        public static Color[] LaserColor = { Color.Orange, Color.Aqua, Color.Green, Color.White, Color.MediumVioletRed };

    }

}


