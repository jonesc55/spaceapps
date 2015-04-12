using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        List<string> recordedJets = new List<string>();

        public bool[] robotJets = new bool[8];

        Random myrand = new Random();

        public List<Rectangle> items = new List<Rectangle>();

        public Rectangle goal = new Rectangle(100,100,10,10);

        int threshold = 70;
        int maxRealSpeed = 25;
        private double extrapower =1.6;
        private int notrealthreshold =3;
        private int maxRockets =5;
        private int goalTimer = 5;

        public double lastGoalDist = 0;

        List<Point> goalPoints = new List<Point>();
        int currentGoal = 0;

        public float robotRotation = 270;
        public float robotRotationSpeed = 0;

        private int robotX = 160;
        private double robotXSpeed = 0;

        private int robotY = 160;
        private double robotYSpeed = 0;

        public int[] robotLastSensors = new int[8];
        public int[] robotSensorsSpeed = new int[8];
        public int[] robotSensors = new int[8];

        int currentScore = 0;
        int topScore =0;

        List<Label> sensorLabels = new List<Label>();

        int recordingPosition =0;
        
        private int goalCount =0;
        private double maxGoalSpeed =3;


        public void nextGoal()
        {
            int LastGoal = currentGoal;
            while (LastGoal == currentGoal)
            {
                currentGoal = myrand.Next(goalPoints.Count - 1);
            }
            currentGoal++;
            if (currentGoal > goalPoints.Count - 1)
            {
                currentGoal = 0;
            }

            goal.Location = goalPoints.ElementAt(currentGoal);

        }

        public void resetRobot()
        {
            allOff();
            nextGoal();
           
            recordingPosition = 0;

            robotRotation = 270;
            robotRotationSpeed = 0;

            robotX = 160;
            robotXSpeed = 0;

            robotY = 160;
            robotYSpeed = 0;
        }

        public Form1()
        {
            InitializeComponent();

            for (int x = 0; x < 8; x++)
            {
                robotSensors[x] = 0;
            }

            for (int x = 0; x < 8; x++)
            {
                robotJets[x] = false;
            }

            goalPoints.Add(new Point(881, 100));
            goalPoints.Add(new Point(161, 406));
            goalPoints.Add(new Point(643, 134));
            goalPoints.Add(new Point(100, 100));
            

            sensorLabels.Add(label2);
            sensorLabels.Add(label3);
            sensorLabels.Add(label4);
            sensorLabels.Add(label5);
            sensorLabels.Add(label6);
            sensorLabels.Add(label7);
            sensorLabels.Add(label8);
            sensorLabels.Add(label9);
        }

        public void DrawRobot(Graphics g)
        {
            Rectangle robot = new Rectangle(robotX, robotY, 50, 50);

            Rectangle front = new Rectangle(robotX+45, robotY, 5, 50);

            Rectangle[] jets = new Rectangle[8];

            jets[0] = new Rectangle(robotX + 5, robotY-5, 5, 5);
            jets[1] = new Rectangle(robotX + 40, robotY-5, 5, 5);

            jets[7] = new Rectangle(robotX-5, robotY + 5, 5, 5);
            jets[6] = new Rectangle(robotX-5, robotY + 40, 5, 5);

            jets[5] = new Rectangle(robotX + 5, robotY+50, 5, 5);
            jets[4] = new Rectangle(robotX + 40, robotY+50, 5, 5);

            jets[2] = new Rectangle(robotX + 50, robotY+5, 5, 5);
            jets[3] = new Rectangle(robotX + 50, robotY+40, 5, 5);

            for(int x=0;x<8;x++)
            {
                if (robotJets[x])
                {
                    g.DrawRectangle(Pens.Red, jets[x]);
                }
                else
                {
                    g.DrawRectangle(Pens.Black, jets[x]);
                }
            }
            
            
            

            g.DrawRectangle(Pens.Green, front);
            g.DrawRectangle(Pens.Black, robot);
            
        }

        public void RotateRectangle(Graphics g, float angle)
        {
            using (Matrix m = new Matrix())
            {
                m.RotateAt(angle, new Point(robotX+25, robotY+25));
                g.Transform = m;
                
                //a robot can draw itself with no rotation in a seperate method
                DrawRobot(g);
                
                g.ResetTransform();
            }
        }




        public void drawEnvironment()
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            RotateRectangle(pictureBox1.CreateGraphics(), robotRotation);

            foreach (Rectangle thisrectangle in items)
            {
                g.FillRectangle(Brushes.Blue, thisrectangle);
            }

            g.FillRectangle(Brushes.Red, goal);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            drawEnvironment();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            robotXSpeed ++;
        }

        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public void updateSensor()
        {
            for (int x = 0; x < 8; x++)
            {
                if (robotSensors[x] < 100000)
                {
                    sensorLabels[x].Text = ""+robotSensors[x];
                }
                else
                {
                    sensorLabels[x].Text = "";
                }
            } 
        }

        void checkgoal(){
            double xdist = Math.Abs(goal.X - (robotX + 25));
            double ydist = Math.Abs(goal.Y - (robotY + 25));

            double xdistsq = xdist * xdist;
            double ydistsq = ydist * ydist;

            int maindist = Convert.ToInt32(Math.Sqrt((xdistsq) + (ydistsq)));
            if (maindist < 45)
            {
                AnimationTimer.Enabled = false;
                PhysicsTimer.Enabled = false;
                checkBox1.Checked = false;
                robotXSpeed = 0;
                robotYSpeed = 0;
                robotRotationSpeed = 0;
                robotRotation = 0;
                nextGoal();
                AnimationTimer.Enabled=true;
                PhysicsTimer.Enabled = true;
                currentScore++;
                label10.Text = "Top Score: " + topScore + " Current Score: " + currentScore;
            }
        }

        void calculateGoalAngle()
        {


            double extra = 0;

            double xdist = Math.Abs(goal.X - (robotX + 25));
            double ydist = Math.Abs(goal.Y - (robotY + 25));

            double xdistsq = xdist * xdist;
            double ydistsq = ydist * ydist;

            int maindist = Convert.ToInt32(Math.Sqrt((xdistsq) + (ydistsq)));

            double angle = 0;

            bool xneg = (goal.X - (robotX + 25)) < 0;
            bool yneg = (goal.Y - (robotY + 25)) < 0;

            if (yneg)
            {
                if (xneg)
                {
                    extra = 180;
                    angle = RadianToDegree(Math.Atan(ydist / xdist));
                }
                else
                {
                    extra = 270;
                    angle = RadianToDegree(Math.Atan(xdist / ydist));
                }
            }
            else
            {
                if (xneg)
                {
                    extra = 90;
                    angle = RadianToDegree(Math.Atan(xdist / ydist));
                }
                else
                {
                    extra = 0;
                    angle = RadianToDegree(Math.Atan(ydist / xdist));
                }
            }

            angle += extra;
            angle += (360 - robotRotation);
            if (angle > 360)
            {
                angle -= 360;
            }

            int sensor = chooseSensor(angle);

            label2.BackColor = Color.Gray;
            label3.BackColor = Color.Gray;
            label4.BackColor = Color.Gray;
            label5.BackColor = Color.Gray;
            label6.BackColor = Color.Gray;
            label7.BackColor = Color.Gray;
            label8.BackColor = Color.Gray;
            label9.BackColor = Color.Gray;

            double goalspeed = lastGoalDist - maindist;

            if (goalspeed < maxGoalSpeed)
            {
                switch (sensor)
                {
                    case 0:
                        label2.BackColor = Color.Yellow;
                        robotJets[6] = true;
                        robotJets[7] = true;
                        break;
                    case 1:
                        label3.BackColor = Color.Yellow;
                        robotJets[0] = true;
                        robotJets[7] = true;
                        break;
                    case 2:
                        label4.BackColor = Color.Yellow;
                        robotJets[0] = true;
                        robotJets[1] = true;
                        break;
                    case 3:
                        label5.BackColor = Color.Yellow;
                        robotJets[2] = true;
                        robotJets[1] = true;
                        break;
                    case 4:
                        label6.BackColor = Color.Yellow;
                        robotJets[2] = true;
                        robotJets[3] = true;
                        break;
                    case 5:
                        label7.BackColor = Color.Yellow;
                        robotJets[4] = true;
                        robotJets[3] = true;
                        break;
                    case 6:
                        label8.BackColor = Color.Yellow;
                        robotJets[4] = true;
                        robotJets[5] = true;
                        break;
                    case 7:
                        label9.BackColor = Color.Yellow;
                        robotJets[5] = true;
                        robotJets[6] = true;
                        break;
                    default:
                        allOff();
                        //robotJets[1] = !robotJets[1];
                        //robotJets[3] = robotJets[1];
                        //robotJets[5] = robotJets[1];
                        //robotJets[7] = robotJets[1];
                        break;
                }
            }
            lastGoalDist = maindist;

        }

        public void calculateSensors()
        {
            double mindist = 100000;
            for (int x = 0; x < 8; x++)
            {
                robotLastSensors[x] = robotSensors[x];

                robotSensors[x] = 1000000;
            }

            listBox1.Items.Clear();
            foreach (Rectangle thisItem in items)
            {
                double extra = 0;

                double xdist = Math.Abs(thisItem.X - (robotX + 25));
                double ydist = Math.Abs(thisItem.Y - (robotY + 25));

                double xdistsq = xdist * xdist;
                double ydistsq = ydist * ydist;

                int maindist = Convert.ToInt32(Math.Sqrt((xdistsq) + (ydistsq)));

                double angle = 0;

                bool xneg = (thisItem.X - (robotX+25)) < 0;
                bool yneg = (thisItem.Y - (robotY+25)) < 0;

                if (yneg)
                {
                    if (xneg)
                    {
                        extra = 180;
                        angle = RadianToDegree(Math.Atan(ydist / xdist));
                    }
                    else
                    {
                        extra = 270;
                        angle = RadianToDegree(Math.Atan(xdist / ydist));
                    }
                }
                else
                {
                    if (xneg)
                    {
                        extra = 90;
                        angle = RadianToDegree(Math.Atan(xdist / ydist));
                    }
                    else
                    {
                        extra = 0;
                        angle = RadianToDegree(Math.Atan(ydist / xdist));
                    }
                }

                angle += extra;
                angle += (360-robotRotation);
                if (angle > 360)
                {
                    angle -= 360;
                }
               
                listBox1.Items.Add("distance:" + maindist + " Angle:" + angle);

                int sensor = chooseSensor(angle);

                if (robotSensors[sensor] > Convert.ToInt32(maindist))
                {
                    robotSensors[sensor] = Convert.ToInt32(maindist);
                }
                if (mindist > maindist)
                {
                    mindist = maindist;
                }
            }

            for (int x = 0; x < 8; x++)
            {
                if (robotLastSensors[x] != 1000000)
                {
                    robotSensorsSpeed[x] = Math.Abs(robotLastSensors[x] - robotSensors[x]);
                }
                else
                {
                    robotSensorsSpeed[x] = 0;
                }

            }

            if (mindist < 35)
            {
                AnimationTimer.Enabled = false;
                PhysicsTimer.Enabled = false;
                checkBox1.Checked = false;
                resetRobot();
                AnimationTimer.Enabled = true;
                PhysicsTimer.Enabled = true;
                checkBox1.Checked = true;
                if (currentScore > topScore)
                {
                    topScore = currentScore;
                }
                currentScore = 0;
                label10.Text = "Top Score: "+topScore+" Current Score: "+currentScore;
            }
        }


        private void PhysicsTimer_Tick(object sender, EventArgs e)
        {
            calculateSensors();

            updateSensor();
            if (checkBox2.Checked == false)
            {
                string recordString = "";
                for (int x = 0; x < 8; x++)
                {
                    if (robotJets[x])
                    {
                        recordString += "1";
                    }
                    else
                    {
                        recordString += "0";
                    }
                    if (x < 7)
                    {
                        recordString += ",";
                    }
                }
                recordedJets.Add(recordString);
            }
            else
            {

                string jetSettingsText = File.ReadAllText("C:\\temp\\jetsetting.txt");

                if (jetSettingsText.Length == 0)
                {
                    if (recordedJets.Count > recordingPosition)
                    {
                        jetSettingsText = recordedJets[recordingPosition];
                        recordingPosition++;
                    }
                    else
                    {
                        AnimationTimer.Enabled = false;
                        PhysicsTimer.Enabled = false;
                        checkBox2.Checked = false;
                    }

                }
                if (jetSettingsText.Length > 0)
                {
                    string[] jetSettings = jetSettingsText.Split(',');
                    for (int x = 0; x < 8; x++)
                    {
                        if (jetSettings[x] == "0")
                        {
                            robotJets[x] = false;
                        }
                        else
                        {
                            robotJets[x] = true;
                        }
                    }
                }
            }
            
            if (robotJets[0] || robotJets[1])
            {
                float relativeRotation = robotRotation + 90;
                if (relativeRotation > 360)
                {
                    relativeRotation -= 360;
                }
                double xchange = 1 * Math.Cos((double)(ConvertToRadians(relativeRotation)));
                double ychange = 1 * Math.Sin((double)(ConvertToRadians(relativeRotation)));
                if (robotJets[0] && robotJets[1])
                {
                    xchange = xchange * extrapower;
                    ychange = ychange * extrapower;
                }
                robotXSpeed += xchange;
                robotYSpeed += ychange;
            }

            if (robotJets[2] || robotJets[3])
            {
                float relativeRotation = robotRotation + 180;
                if (relativeRotation > 360)
                {
                    relativeRotation -= 360;
                }

                double xchange = 1 * Math.Cos((double)ConvertToRadians(relativeRotation));
                double ychange = 1 * Math.Sin((double)ConvertToRadians(relativeRotation));
                if (robotJets[2] && robotJets[3])
                {
                    xchange = xchange * extrapower;
                    ychange = ychange * extrapower;
                }
                robotXSpeed += xchange;
                robotYSpeed += ychange;
            }

            if (robotJets[7] || robotJets[6])
            {
                double xchange = 1 * Math.Cos((double)ConvertToRadians(robotRotation));
                double ychange = 1 * Math.Sin((double)ConvertToRadians(robotRotation));
                if (robotJets[6] && robotJets[6])
                {
                    xchange = xchange * extrapower;
                    ychange = ychange * extrapower;
                }
                robotXSpeed += xchange;
                robotYSpeed += ychange;
            }

            if (robotJets[4] || robotJets[5])
            {
                float relativeRotation = robotRotation + 270;
                if (relativeRotation > 360)
                {
                    relativeRotation -= 360;
                }

                double xchange = Math.Cos((double)ConvertToRadians(relativeRotation));
                double ychange = Math.Sin((double)ConvertToRadians(relativeRotation));
                if (robotJets[4] && robotJets[5])
                {
                    xchange = xchange * extrapower;
                    ychange = ychange * extrapower;
                }
                robotXSpeed += xchange;
                robotYSpeed += ychange;
            }

            if (robotJets[1] && robotJets[3]&& robotJets[5] && robotJets[7])
            {
                robotRotationSpeed += 1;
            }


            if (robotJets[0] && robotJets[2] && robotJets[4] && robotJets[6])
            {
                robotRotationSpeed -= 1;
            }

            
            robotRotation += robotRotationSpeed;
            if (robotRotation > 360)
            {
                robotRotation = robotRotation-360;
            }
           
            robotX += (int)robotXSpeed/5;
            robotY += (int)robotYSpeed/5;

           
            if (checkBox3.Checked)
            {
                doThought();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PhysicsTimer.Enabled = checkBox1.Checked;
            AnimationTimer.Enabled = checkBox1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            robotJets[1] = !robotJets[1];
            robotJets[3] = robotJets[1];
            robotJets[5] = robotJets[1];
            robotJets[7] = robotJets[1];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            robotXSpeed --;
        }

        public void frontFire()
        {
            float ychange = (float)(Math.Cos(robotRotation));
            float xchange = (float)(Math.Sin(robotRotation));

            robotYSpeed += ychange;
            robotXSpeed += xchange;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            robotJets[0] = !robotJets[0];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            robotYSpeed++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
         
            robotJets[0] = !robotJets[0];
            robotJets[2] = robotJets[0];
            robotJets[4] = robotJets[0];
            robotJets[6] = robotJets[0];
        }

        private void button8_Click(object sender, EventArgs e)
        {
            robotJets[0] = !robotJets[0];
            robotJets[1] = robotJets[0];
        }

        private void button7_Click(object sender, EventArgs e)
        {
            robotJets[1] = !robotJets[1];
        }

        private void button10_Click(object sender, EventArgs e)
        {
            robotJets[2] = !robotJets[2];
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            robotJets[2] = !robotJets[2];
            robotJets[3] = robotJets[2];
        }

        private void button9_Click(object sender, EventArgs e)
        {
            robotJets[3] = !robotJets[3];
        }

        private void button12_Click(object sender, EventArgs e)
        {
            robotJets[4] = !robotJets[4];
        }

        private void button13_Click(object sender, EventArgs e)
        {
            robotJets[5] = !robotJets[5];
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            robotJets[6] = !robotJets[6];
        }

        private void button14_Click(object sender, EventArgs e)
        {
            robotJets[7] = !robotJets[7];
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            robotJets[4] = !robotJets[4];
            robotJets[5] = robotJets[4];
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            robotJets[6] = !robotJets[6];
            robotJets[7] = robotJets[6];
        }


        void allOff()
        {
            for (int x = 0; x < 8; x++)
            {
                robotJets[x] = false;
            }
        }

        int chooseSensor(double angle)
        {
            int sensor = -1;
            if (angle >= 0 && angle < 22.5)
            {
                sensor = 0;
            }
            if (angle >= 22.5 && angle < 67.5)
            {
                sensor = 1;
            }
            if (angle >= 67.5 && angle < 112.5)
            {
                sensor = 2;
            }
            if (angle >= 112.5 && angle < 157.5)
            {
                sensor = 3;
            }
            if (angle >= 157.5 && angle < 202.5)
            {
                sensor = 4;
            }
            if (angle >= 202.5 && angle < 247.5)
            {
                sensor = 5;
            }
            if (angle >= 247.5 && angle < 292.5)
            {
                sensor = 6;
            }
            if (angle >= 292.5 && angle < 337.5)
            {
                sensor = 7;
            }
            if (angle >= 337.5)
            {
                sensor = 0;
            }

            return sensor;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            allOff();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            allOff();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            resetRobot();
        }

       
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                items.Add(new Rectangle(e.X, e.Y, 10, 10));
            }
            else
            {
                goal = new Rectangle(e.X, e.Y, 10,10);
            }
        }

        private void doThought()
        {
            checkgoal();
            allOff();
            //look at sensors
            //decide to do something
            goalCount++;

            if (goalCount > goalTimer)
            {
                goalCount = 0;
                calculateGoalAngle();
            }

            int On = 0;
            for (int x = 0; x < 8; x++)
            {
                if (robotJets[x])
                {
                    On++;
                }
            }
            if (On > maxRockets)
            {
                allOff();
            }

            int notreal = 0;
            
                for (int x = 0; x < 8; x++)
                {
                    int randthing = robotSensorsSpeed[3];
                    double newthreshold = threshold * (Math.Sqrt(robotSensorsSpeed[x]));
                    if (robotSensors[x] < newthreshold)
                    {

                        if (robotSensorsSpeed[x] > 0)
                        {
                            if (robotSensorsSpeed[x] < maxRealSpeed)
                            {
                                int prev = x - 6;
                                if (prev < 0)
                                {
                                    prev = 8 + prev;
                                }
                                int next = prev + 1;
                                if (next == 8)
                                {
                                    next = 0;
                                }
                                robotJets[prev] = true;
                                robotJets[next] = true;


                            }
                            else
                            {
                                notreal++;
                            }
                        }
                    }
                
            }
            //if (notreal > notrealthreshold)
            if (robotRotationSpeed !=0)
            {
                allOff();
                if (robotRotationSpeed > 0)
                {
                    robotJets[0] = !robotJets[0];
                    robotJets[2] = robotJets[0];
                    robotJets[4] = robotJets[0];
                    robotJets[6] = robotJets[0];
                }
                else
                {
                    robotJets[1] = !robotJets[1];
                    robotJets[3] = robotJets[1];
                    robotJets[5] = robotJets[1];
                    robotJets[7] = robotJets[1];
                }
            }
            
           
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {

            items.Clear();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            int gap = 40;
            int x = 20;
            int y =20;
            while (x < pictureBox1.Width)
            {
                items.Add(new Rectangle(x, y, 10, 10));
                x += gap;
            }
            x = 20;
            y = pictureBox1.Height - 20;
            while (x < pictureBox1.Width)
            {
                items.Add(new Rectangle(x, y, 10, 10));
                x += gap;
            }
            x = 20;
            y = 20;
            while (y < pictureBox1.Height)
            {
                items.Add(new Rectangle(x, y, 10, 10));
                y += gap;
            }
            x = pictureBox1.Width - 20 ;
            y = 20;
            while (y < pictureBox1.Height)
            {
                items.Add(new Rectangle(x, y, 10, 10));
                y += gap;
            }

        }

    }
}
