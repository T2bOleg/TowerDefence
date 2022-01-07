using System;
using System.Drawing;
using System.Linq;

namespace TowerDefence
{
    public abstract class GameObject
    {
        protected double Dist(Point pt1, Point pt2) => Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));

        protected int posX, posY, wig,hei;
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
        protected void IntToPoint() => pos = new Point(posX, posY);
        protected void PointToint()  {posX = pos.X; posY = pos.Y;}
    }

    public class CheckPoint : GameObject
    {
        public int number;

        public CheckPoint(ObjectManager _obj, Graphics g, int x, int y, int num)
        {
            init(_obj, g, x, y);
            number = num;
        }
        public CheckPoint(ObjectManager _obj, Graphics g, Point p, int num)
        {
            init(_obj, g, p);
            number = num;
        }
        public override void Render()
        {
            g.DrawLine(new Pen(Color.Green), pos, _obj.checkpoints[number + 1].pos);

        }
        public override void Click(){}
        public override void Step()
        {
        }
        public override bool CheckPos(Point p)
        {
            return false;
        }
    }

    public class Fort : GameObject
    {


        public Fort(ObjectManager _obj, Graphics g, int x, int y)
        {
            init(_obj, g, x, y);
            wig = 30;
            hei = 30;
        }
        public Fort(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            wig = 30;
            hei = 30;
        }


        public override void Click() { }
        public override void Render()=>g.DrawImage(Resources.FortSprite, new Point(posX - 30, posY - 30));

        public override void Step()
        {
        }
        public override bool CheckPos(Point p) => (Math.Abs(p.X - posX) <= 30) && (Math.Abs(p.Y - posY) <= 30);
        
    }

    public class Enemy:GameObject
    {
        protected int spd = 2, dam = 2;
        protected Point targ;
        protected int numTarg = 1;
        public Enemy(ObjectManager _obj, Graphics g, int x, int y)
        {
            init(_obj, g, x, y);
            targ = _obj.checkpoints[numTarg].pos;
        }
        public Enemy()
        {
            wig = 3;
            hei = 3;
        }
        public Enemy(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            targ = _obj.checkpoints[numTarg].pos;
        }
        public override void Click() { }
        public override void Render() { }
        public override void Step()
        {
            if (Dist(targ, pos) >= spd)
            {
                int fX = targ.X - posX;
                int fY = targ.Y - posY;
                if (Math.Abs(fX) >= spd && fX != 0) posX += spd * fX/Math.Abs(fX); else posX =targ.X;
                if (Math.Abs(fY) >= spd && fY != 0) posY += spd * fY / Math.Abs(fY); else posY = targ.Y;
                IntToPoint();
            }
            else
            {                
                numTarg++;
                targ = _obj.checkpoints[numTarg].pos;
            }
            if(_obj.fort.CheckPos(pos))
            {
                _obj.delList.Add(this);
                _obj.XP -= dam;
            }

        }
        public override bool CheckPos(Point p) => Dist(p,pos)<=6;

       
    }

    public class Solder:Enemy
    {
        
        public Solder(ObjectManager _obj, Graphics g, int x, int y)
        {
            init(_obj, g, x, y);
            targ = _obj.checkpoints[numTarg].pos;
        }
        public Solder(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            targ = _obj.checkpoints[numTarg].pos;
        }
        public override void Render() => g.DrawEllipse(new Pen(Color.Red), posX - 3, posY - 3, 6, 6);
    }
    public class Vert:Enemy
    {
        public Vert(ObjectManager _obj, Graphics g, Point p)
        {
            init(_obj, g, p);
            targ = _obj.checkpoints[numTarg].pos;
            spd = 3;
            dam = 3;
        }
        public Vert(ObjectManager _obj, Graphics g, int x, int y)
        {

            init(_obj, g, x, y);
            targ = _obj.checkpoints[numTarg].pos;
            spd = 3;
            dam = 3;
        }
        public override void Render() {
            g.DrawLine(new Pen(Color.FromArgb(170, 165, 255)), pos - new Size(3, 3), pos + new Size(3, 3));
            g.DrawLine(new Pen(Color.FromArgb(170, 165, 255)), pos - new Size(-3, 3), pos + new Size(-3, 3));
        }
    }

    public class Waerpon : GameObject
    {
        Image sprite;
        public Waerpon(ObjectManager _obj, Graphics g, int x, int y, Image image)
        {
            init(_obj, g, x, y);
            sprite = image;
        }
        public Waerpon(ObjectManager _obj, Graphics g, Point p, Image image)
        {
            init(_obj, g, p);
            sprite = image;
        }
        
        public override void Click() { }
        public override void Render() => g.DrawImage(sprite, new Point(posX - 15, posY - 15));
        public override void Step()
        {
        }
        public override bool CheckPos(Point p) => (Math.Abs(p.X - posX) <= 15) && (Math.Abs(p.Y - posY) <= 15);

    }
    

    public class Button:GameObject
    {
        Image bSprite;
        int num;
        public Button(ObjectManager _obj, Graphics g, Point p,Image bSprite, int num)
        {
            init(_obj, g, p);
            this.bSprite = bSprite;
            this.num = num;
            wig = 20;
            hei = 20;
        }
        public override void Step() { }
        public override void Render()
        {
            g.DrawImage(bSprite, pos);
            if (num == _obj.Rez) g.DrawRectangle(new Pen(Color.Red), new Rectangle(pos, new Size(100, 100)));
}
        public override bool CheckPos(Point p) => (p.X >= pos.X && p.X <= pos.X + 150 && p.Y >= pos.Y && p.Y <= pos.Y + 150);
        public override void Click()
        {
            _obj.Rez = num;
        }
    }
}


