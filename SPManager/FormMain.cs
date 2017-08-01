﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SPManager
{
    public partial class FormMain : Form
    {
        //TH_RECT th_RECT = new TH_RECT();
        //TH_PlateResult th_PlateResult = new TH_PlateResult();
        //NET_DVR_PREVIEWINFO previewInfo = new NET_DVR_PREVIEWINFO();
        private bool toExit = false;
        private DateTime lastUpdateTime = DateTime.Now;
        int gcCount = 0;
        public FormMain()
        {
            InitializeComponent();
        }
        

        private void timer1_Tick(object sender, EventArgs e)
        {

            MEMORY_INFO MemInfo;
            MemInfo = new MEMORY_INFO();
            SystemUnit.GlobalMemoryStatus(ref MemInfo);
            toolRAM.Text = MemInfo.dwMemoryLoad.ToString() + "%";
        }
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            //TODO 临时返回
            //return;
            startProtectProcess();//启动保护进程
            lastMessageTime = DateTime.Now;
            addListViewHead();
            int ret = Init();
            if (ret == 0)
            {
                SPlate.SP_BeginRecog();
                timerDataProc.Enabled = true;
                return;
            }
            if ((ret & 0x01) == 0x01)
            {
                MessageBox.Show("日志启动失败");
            }
            if ((ret & 0x02) == 0x02)
            {
                MessageBox.Show("数据库连接失败");
            }
            if ((ret & 0x04) == 0x04)
            {
                MessageBox.Show("参数初始化失败");
            }
            if ((ret & 0x08) == 0x08)
            {
                MessageBox.Show("算法初始化失败");
            }
            if ((ret & 0x10) == 0x10)
            {
                MessageBox.Show("设备初始化失败");
            }
            if ((ret & 0x20) == 0x20)
            {
                MessageBox.Show("网络服务初始化失败");
            }

        }

        private void notifyIconMain_DoubleClick(object sender, EventArgs e)
        {
            
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;//使Form不在任务栏上显示
        }

       

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCloseMain_Click(object sender, EventArgs e)
        {

        }


        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitApp();
            //SPlate.SP_Close();
            //if(Global.LogServer != null)
            //Global.LogServer.Stop();
        }
    

        private void btnChangeLogLevel_Click(object sender, EventArgs e)
        {
            
            SPlate.SP_SetLogLevel(comboLogLevel.SelectedIndex + 1);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            int ret = Init();
            if (ret == 0)
            {
                SPlate.SP_BeginRecog();
                return;
            }
                
            if ((ret & 0x01) == 0x01)
            {
                MessageBox.Show("日志启动失败");
            }
            if ((ret & 0x02) == 0x02)
            {
                MessageBox.Show("数据库连接失败");
            }
            if ((ret & 0x04) == 0x04)
            {
                MessageBox.Show("参数初始化失败");
            }
            if ((ret & 0x08) == 0x08)
            {
                MessageBox.Show("算法初始化失败");
            }
            if ((ret & 0x10) == 0x10)
            {
                MessageBox.Show("设备初始化失败");
            }
            if ((ret & 0x20) == 0x20)
            {
                MessageBox.Show("网络服务初始化失败");
            }
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.toExit)
            {
                this.Visible = false;
                e.Cancel = true;
                return;
            }
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            
            if (DialogResult.Yes == (MessageBox.Show("确认退出程序？", "提示", MessageBoxButtons.YesNo)))
            {
                this.toExit = true;
                this.Close();
                
            }
        }

        private void notifyIconMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuExit.Show(Cursor.Position);
            }
        }

        private void notifyIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void timerServiceStaus_Tick(object sender, EventArgs e)
        {
            this.showMemoryInfo();
            this.showMatchRatio();
            SystemUnit.PostMessage(SystemUnit.HWND_BROADCAST, (int)WM_HEARTBEAT, 0, 0);
            gcCount++;
            if (gcCount > 60)
            {
                gcCount = 0;
                GC.Collect();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string appPath = Application.StartupPath;
            Process proc = Process.Start(appPath+Global.updateAppName);
            if (proc != null)
            {
                ExitApp();
            }
        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            FormSearchData searchData = new FormSearchData();
            searchData.Show();
        }
        
        private void timerDataProc_Tick(object sender, EventArgs e)
        {
            
            showCarList();
            
            DateTime dt = DateTime.Now;
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            SPlate.SP_TestAPI();
        }

        private void startProtectProcess()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Application.StartupPath + "//Kill.exe";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            SystemUnit.PostMessage(SystemUnit.HWND_BROADCAST, (int)WM_HEARTBEAT, 0, 0);
            //showMatchRatio();
            //int ret = SPlate.SP_InitNVR_HKCN("10.225.142.36", 8000, "admin", "sd123456");
            //GetAreaCarFromDll(1);
            //int ret = SPlate.SP_TestAPI();
            //if (ret != 0)
            //{
            //    MessageBox.Show("初始化失败，返回值：" + ret.ToString());
            //}
            //             ClsPicture pic = new ClsPicture();
            //             FileStream fs = new FileStream("C:\\lic.jpg",FileMode.Open);
            //             pic.picBufer = new byte[fs.Length];
            //             fs.Read(pic.picBufer, 0, (int)fs.Length);
            //             //pic.picPath = Global.basePicPath + dt.ToString("yyyyMMdd") + @"\\" + Global.arrayNozzleCar[index].license.Trim() + @"\\";
            //             pic.picPath = @"C:\\images" + @"\\"+DateTime.Now.ToString("yyyyMMdd") + @"\\";
            //             // Global.LogServer.Add(new LogInfo("Debug", "Main->GetCarFromDll: 车辆图片入队列,图片路径：" + pic.picPath+ pic.picName, (int)EnumLogLevel.DEBUG));
            //             pic.picName = DateTime.Now.ToString("yyyyMMdd")+ ".jpg";
            //             pic.picWidth = 1920;
            //             pic.picHeight = 1088;
            //             pic.picType = 0;
            //             //Global.picWork.Add(pic);
            //             //string path = "C:\\images\\20170606\\鲁AP607K\\";
            //             //if (!Directory.Exists(path))
            //             //{
            //             //    Directory.CreateDirectory(path);
            //             //}
            //             MemoryStream ms = new MemoryStream(pic.picBufer);
            //             Image img = Image.FromStream(ms);
            //             //Image img = Image.FromFile("C:\\lic.jpg");
            //             img.Save(pic.picPath + pic.picName);

        }
    }
}
