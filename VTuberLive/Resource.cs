//我单推的虚拟管人们的数据
//试验用,可能需要自动化脚本用来自动生成和编辑
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

internal class VtuberData
{
    public VtuberData(){
    }

    public void Initalize()
    {
        VtuberList = new Dictionary<string, string>();
        for (int i = 0;i < Members.Count;i++)
        {
            VtuberList.Add(Members[i],ChannelURL[i]);
        }
    }

    public Dictionary<string, string> VtuberList { get; set; }

    private static List<string> Members = new List<string>{
        "月の美兔",
        "静凛",
        "樋口楓",
        "千草はな",
        "周防パトラ",
        };
    private static List<string> ChannelURL = new List<string>{
        "https://www.youtube.com/channel/UCD-miitqNY3nyukJ4Fnf4_A",
        "https://www.youtube.com/channel/UC6oDys1BGgBsIC3WhG1BovQ",
        "https://www.youtube.com/channel/UCsg-YqdqQ-KFF0LNk23BY4A",
        "https://www.youtube.com/channel/UCn14Z641OthNps7vppBvZFA",
        "https://www.youtube.com/channel/UCeLzT-7b2PBcunJplmWtoDg",
        };

}