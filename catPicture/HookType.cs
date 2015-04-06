using System;
using System.Collections.Generic;
using System.Text;

namespace catPicture
{
    /// <summary>
    /// Hook的类型
    /// </summary>
    public enum HookType : int
    {
        WH_JOURNALRECORD = 0,     //对发送到系统消息队列的输入消息进行监视  
        WH_JOURNALPLAYBACK = 1,   //对此前有WH_JORNALRECORD的钩子过错记录的消息
        WH_KEYBOARD = 2,          //对键盘消息进行监视
        WH_GETMESSAGE = 3,        //对发送到消息队列的消息进行监视
        WH_CALLWNDPROC = 4,       //在操作系统将消息发送到目标窗口吃力过程之前，对该消息进行监视
        WH_CBT = 5,               //接受CBT应用程序有用的消息
        WH_SYSMSGFILTER = 6,      //监视由对话框，消息框，菜单条，或滚动条中的输入事件应发的消息。该钩子过程对系统中所有的应用程序的这类消息都进行监视
        WH_MOUSE = 7,             //对鼠标消息进行监视
        WH_MSGFILTER = 8,          //应监视由对话框，消息框，菜单条，或滚动条中输入的时间引发的消息
        WH_DEBUG = 9,             //对其他钩子进行调试
        WH_SHELL = 10,            //接受对外壳应用程序有用的通知
        WH_FOREGROUNDIDLE = 11,   //当用用程序的前台线程即将进入空闲状态时调用，它有助于在空闲时间类执行优先级低的任务
        WH_CALLWNDPROCRET = 12,   //对已被目标窗口过程处理过了的消息进行监视
        WH_KEYBOARD_LL = 13,      //只能安装在NT中，对底层的键盘事件进行监视
        WH_MOUSE_LL = 14          //只能安装在NT中，对底层的鼠标输入事件进行监视
    }
}
