using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TouchPointTappingGesture
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        class TimePoint
        {
            public float X { get; private set; }
            public float Y { get; private set; }
            public float Time { get; private set; }
            public TimePoint(float x, float y, float time)
            {
                X = x;
                Y = y;
                Time = time;
            }
        }

        List<TimePoint> mousePosList = new List<TimePoint>();
        List<TimePoint> mouseVelList = new List<TimePoint>();
        List<TimePoint> mouseAccList = new List<TimePoint>();

        public Form1()
        {
            InitializeComponent();
        }

        FLib.Hooker hooker = new FLib.Hooker();

        private void Form1_Load(object sender, EventArgs e)
        {
            hooker.OnMouseHook += OnMouseHook;
            hooker.Hook();

            stopWatch.Start();
        }

        Pen pen0 = new Pen(Brushes.Black);
        Pen pen1 = new Pen(Brushes.Blue);
        Pen pen2 = new Pen(Brushes.Red);

        float getScaledIntensity(TimePoint pt)
        {
            const float ratioY = 100.0f;
            
            float val = (float)Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y);
            float logVal = (float)Math.Log10(val + 1);

            return logVal * ratioY;
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            float current = stopWatch.ElapsedMilliseconds * 0.001f;

            if (mousePosList.Count >= 2)
            {
                e.Graphics.DrawLines(
                    pen0,
                    mousePosList.Select(pt =>
                        new PointF(
                            e.ClipRectangle.Right - (current - pt.Time) * 100.0f,
                            getScaledIntensity(pt))
                    ).ToArray());
            }

            if (mouseVelList.Count >= 2)
            {
                e.Graphics.DrawLines(
                    pen1,
                    mouseVelList.Where(pt => pt != null)
                        .Select(pt =>
                        new PointF(
                            e.ClipRectangle.Right - (current - pt.Time) * 100.0f,
                            getScaledIntensity(pt))
                    ).ToArray());
            }
            if (mouseAccList.Count >= 2)
            {
                e.Graphics.DrawLines(
                    pen2,
                    mouseVelList.Where(pt => pt != null)
                        .Select(pt =>
                        new PointF(
                            e.ClipRectangle.Right - (current - pt.Time) * 100.0f,
                            getScaledIntensity(pt))
                    ).ToArray());

                foreach (var pt in mouseVelList.Where(pt => pt != null)
                        .Select(pt =>
                        new PointF(
                            e.ClipRectangle.Right - (current - pt.Time) * 100.0f,
                            getScaledIntensity(pt))
                    ))
                {
                    e.Graphics.FillRectangle(Brushes.Red, new RectangleF(pt.X - 3, pt.Y - 3, 6, 6));
                }
            }

            foreach (var time in detecttimeList)
            {
                float detectX = e.ClipRectangle.Right - (current - time) * 100.0f;
                e.Graphics.DrawLine(pen1, new PointF(detectX, 0), new PointF(detectX, e.ClipRectangle.Height));
            }


        }

        TimePoint getCurrentPointInfo()
        {
            float time = stopWatch.ElapsedMilliseconds * 0.001f;

            if (mousePosList.Count >= 1)
            {
                float lastTime = mousePosList[mousePosList.Count - 1].Time;
                if (time - lastTime <= 1e-2f)
                {
                    return null;
                }
            }

            Point curPt = Cursor.Position;
            return new TimePoint(curPt.X, curPt.Y, time);
        }

        void addMousePointInfo(TimePoint pt)
        {
            if (pt == null)
            {
                return;
            }

            float time = pt.Time;

            mousePosList.Add(pt);

            if (mousePosList.Count >= 2)
            {
                float dx = mousePosList[mousePosList.Count - 1].X - mousePosList[mousePosList.Count - 2].X;
                float dy = mousePosList[mousePosList.Count - 1].Y - mousePosList[mousePosList.Count - 2].Y;
                float dt = (mousePosList[mousePosList.Count - 1].Time - mousePosList[mousePosList.Count - 2].Time);
                mouseVelList.Add(new TimePoint(dx / dt, dy / dt, time));
            }

            if (mouseVelList.Count >= 2)
            {
                float dx = mouseVelList[mouseVelList.Count - 1].X - mouseVelList[mouseVelList.Count - 2].X;
                float dy = mouseVelList[mouseVelList.Count - 1].Y - mouseVelList[mouseVelList.Count - 2].Y;
                float dt = (mouseVelList[mouseVelList.Count - 1].Time - mouseVelList[mouseVelList.Count - 2].Time);
                mouseAccList.Add(new TimePoint(dx / dt, dy / dt, time));
            }

            while (mouseAccList.Count > 1000)
            {
                mousePosList.RemoveAt(0);
                mouseVelList.RemoveAt(0);
                mouseAccList.RemoveAt(0);
            }
        }

        bool OnMouseHook(int code, FLib.WM message, IntPtr state, FLib.Hooker hooker)
        {
            switch (message)
            {
                case FLib.WM.MOUSEMOVE:
                    float time = stopWatch.ElapsedMilliseconds * 0.001f;

                    if (mousePosList.Count >= 1)
                    {
                        float lastTime = mousePosList[mousePosList.Count - 1].Time;
                        if (time - lastTime <= 1e-2f)
                        {
                            break;
                        }
                    }

                    addMousePointInfo(getCurrentPointInfo());
                    
                    if (checkGesture_dirty())
                    {
                        click();
                    }

                    break;
            }

            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            addMousePointInfo(getCurrentPointInfo());

            if (checkGesture_dirty())
            {
                click();
            }

            canvas.Invalidate();
        }

        List<float> detecttimeList = new List<float>();
        float lastDetectTime = 0;

        private bool checkGesture_dirty()
        {
            if (mouseAccList.Count < 16)
            {
                labelGesture.Text = "";
                return false;
            }

            float time = mouseAccList[mouseAccList.Count - 1].Time;

            if (time - lastDetectTime < 0.3f)
            {
                labelGesture.Text = "";
                return false;
            }


            labelGesture.Text = "";

            const int lookFrameOffset0 = -9;
            const int lookFrameOffset1 = -6;
            const int lookFrameOffset2 = -2;

            for (int i = mouseAccList.Count + lookFrameOffset0; i < mouseAccList.Count + lookFrameOffset1; i++)
            {
                // 全部小さい
                if (getScaledIntensity(mouseAccList[i]) < 100)
                {
                }
                else
                {
                    return false;
                }
            }

            float newDetecttTime = time;
            bool anyHighIntensity = false;
            for (int i = mouseAccList.Count + lookFrameOffset1; i < mouseAccList.Count + lookFrameOffset2; i++)
            {
                // 最低ひとつが大きい
                if (getScaledIntensity(mouseAccList[i]) >= 30)
                {
                    anyHighIntensity = true;
                    newDetecttTime = mouseAccList[i].Time;
                }
            }

            if (false == anyHighIntensity)
            {
                return false;
            }

            for (int i = mouseAccList.Count + lookFrameOffset2; i < mouseAccList.Count; i++)
            {
                // 全部小さい
                if (getScaledIntensity(mouseAccList[i]) < 100)
                {
                }
                else
                {
                    return false;
                }
            }

            labelGesture.Text = "Tapped!";

            lastDetectTime = newDetecttTime;
            
            detecttimeList.Add(newDetecttTime);
            while (detecttimeList.Count > 100)
            {
                detecttimeList.RemoveAt(0);
            }

            return true;
        }


        void click()
        {
            if (checkBoxEnableClick.Checked)
            {
                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                mouse_event((UInt32)MouseEventFlags.LEFTDOWN, (uint)x, (uint)y, 0, IntPtr.Zero);
                mouse_event((UInt32)MouseEventFlags.LEFTUP, (uint)x, (uint)y, 0, IntPtr.Zero);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }
    }
}
