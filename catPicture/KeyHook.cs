using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace catPicture
{
 
    public  class KeyHook
    {
        // 安装钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        // 卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        // 继续下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        // 取得当前线程编号
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        HookProc KeyboardHookProcedure;

        static int hKeyboardHook = 0;
        public delegate bool KeyHookEventHandler(object sender,KeyHookEventArgs e);

        public event KeyHookEventHandler OnKeyChange;

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)//回调函数
        {
            if (nCode >= 0)
            {
                KeyHookEventArgs e = new KeyHookEventArgs();
                e.nCode = nCode;
                e.wParam = wParam;
                e.lParam = lParam;
                if (OnKeyChange(this, e))
                {
                    return 1;
                }
                
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam); 
        }

                // 安装钩子
        public void HookStart()
        {
            if(hKeyboardHook == 0)
            {
                // 创建HookProc实例
                KeyboardHookProcedure=new HookProc (KeyboardHookProc);
                // 设置线程钩子
                hKeyboardHook = SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                // 如果设置钩子失败
                if(hKeyboardHook == 0 )    
                {
                    HookStop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }
        // 卸载钩子
        public void HookStop()
        {
            bool retKeyboard = true;
            if(hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            } 
            if (!retKeyboard) throw new Exception("UnhookWindowsHookEx failed.");
        }


    }

    public class KeyHookEventArgs : EventArgs
    {
       public   int nCode;
       public   int wParam;
       public   IntPtr lParam;
     
    }
    //键盘消息
    public struct KeyMSG
    {
        public int vkCode; 
        public int scanCode;
        public int flags; 
        public int time; 
        public int dwExtraInfo; 
    }

}
