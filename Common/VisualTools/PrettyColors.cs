using Avalonia.Media;

namespace VisualTools;

public static class PrettyColors
{
    static string[] Hexs = {
        "#C71585","#3CB371","#808000","#DAA520","#00CED1","#D2691E","#2F4F4F","#5F9EA0","#6A5ACD","#808080",
        "#FF4500","#1E90FF","#32CD32","#ffba69","#6b5e6e","#ffff8e","#9dd400","#136900","#190ccb","#cccc00",
        "#cccccc","#111f6f","#ffd452","#a1006b","#ffa000","#fc7b7b","#f87600","#ffeffe","#eafafb","#9b5766",
        "#2c191b","#b700ff","#40f99b","#c5d6e8","#493c3c","#f2db74","#d64128","#feacdd","#7dbee2","#fefedd",
        "#663c3c","#12425d","#debd9f","#ac1f1f","#686e9f","#ff20c9","#17ff84","#46e8ff","#e9ff34","#a4a539",
        "#6d0000","#162857","#bbbbbb","#75b3ff","#1fadeb","#fb9cf1","#e3f8ff","#ffe5fb","#a580bb","#26594f",
        "#4dbba6","#ee4c3a","#eacb11","#422e83","#85db22","#4e5ab6","#004b75","#9a0000","#eaebed","#c75300",
        "#ffffff","#ff0000","#ff00ce","#a300ff","#0011ff","#00dfff","#4accc2","#d1ede9","#e2b63b","#f1f1f1",
        "#4c4c4c","#d7b0b1","#c29290","#991b38","#414d58","#e70f0f","#f2f0fb","#47c2cc","#f28517","#ee38af",
        "#96ceb4","#ffcc5c","#ff6f69","#ced07d","#0e9aa7","#181770","#edff37","#b3c1ff","#ffd641","#d409cc"
    };

    public static int Length => Hexs.Length;

    public static Color Get(int index)
    {
        if (index < 0 || index >= Length || !Color.TryParse(Hexs[index], out var res))
        {
            return Colors.Red;
        }

        return res;
    }
}