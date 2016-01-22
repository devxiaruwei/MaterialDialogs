using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Sino.Droid.MaterialDialogs.Internal
{
    public static class ColorPalette
    {
        public static int[] PRIMARY_COLORS = new int[]{
            Color.ParseColor("#F44336"),
            Color.ParseColor("#E91E63"),
            Color.ParseColor("#9C27B0"),
            Color.ParseColor("#673AB7"),
            Color.ParseColor("#3F51B5"),
            Color.ParseColor("#2196F3"),
            Color.ParseColor("#03A9F4"),
            Color.ParseColor("#00BCD4"),
            Color.ParseColor("#009688"),
            Color.ParseColor("#4CAF50"),
            Color.ParseColor("#8BC34A"),
            Color.ParseColor("#CDDC39"),
            Color.ParseColor("#FFEB3B"),
            Color.ParseColor("#FFC107"),
            Color.ParseColor("#FF9800"),
            Color.ParseColor("#FF5722"),
            Color.ParseColor("#795548"),
            Color.ParseColor("#9E9E9E"),
            Color.ParseColor("#607D8B")
    };

    public static int[][] PRIMARY_COLORS_SUB = new int[][]{
            new int[]{
                    Color.ParseColor("#FFEBEE"),
                    Color.ParseColor("#FFCDD2"),
                    Color.ParseColor("#EF9A9A"),
                    Color.ParseColor("#E57373"),
                    Color.ParseColor("#EF5350"),
                    Color.ParseColor("#F44336"),
                    Color.ParseColor("#E53935"),
                    Color.ParseColor("#D32F2F"),
                    Color.ParseColor("#C62828"),
                    Color.ParseColor("#B71C1C")
            },
            new int[]{
                    Color.ParseColor("#FCE4EC"),
                    Color.ParseColor("#F8BBD0"),
                    Color.ParseColor("#F48FB1"),
                    Color.ParseColor("#F06292"),
                    Color.ParseColor("#EC407A"),
                    Color.ParseColor("#E91E63"),
                    Color.ParseColor("#D81B60"),
                    Color.ParseColor("#C2185B"),
                    Color.ParseColor("#AD1457"),
                    Color.ParseColor("#880E4F")
            },
            new int[]{
                    Color.ParseColor("#F3E5F5"),
                    Color.ParseColor("#E1BEE7"),
                    Color.ParseColor("#CE93D8"),
                    Color.ParseColor("#BA68C8"),
                    Color.ParseColor("#AB47BC"),
                    Color.ParseColor("#9C27B0"),
                    Color.ParseColor("#8E24AA"),
                    Color.ParseColor("#7B1FA2"),
                    Color.ParseColor("#6A1B9A"),
                    Color.ParseColor("#4A148C")
            },
            new int[]{
                    Color.ParseColor("#EDE7F6"),
                    Color.ParseColor("#D1C4E9"),
                    Color.ParseColor("#B39DDB"),
                    Color.ParseColor("#9575CD"),
                    Color.ParseColor("#7E57C2"),
                    Color.ParseColor("#673AB7"),
                    Color.ParseColor("#5E35B1"),
                    Color.ParseColor("#512DA8"),
                    Color.ParseColor("#4527A0"),
                    Color.ParseColor("#311B92")
            },
            new int[]{
                    Color.ParseColor("#E8EAF6"),
                    Color.ParseColor("#C5CAE9"),
                    Color.ParseColor("#9FA8DA"),
                    Color.ParseColor("#7986CB"),
                    Color.ParseColor("#5C6BC0"),
                    Color.ParseColor("#3F51B5"),
                    Color.ParseColor("#3949AB"),
                    Color.ParseColor("#303F9F"),
                    Color.ParseColor("#283593"),
                    Color.ParseColor("#1A237E")
            },
            new int[]{
                    Color.ParseColor("#E3F2FD"),
                    Color.ParseColor("#BBDEFB"),
                    Color.ParseColor("#90CAF9"),
                    Color.ParseColor("#64B5F6"),
                    Color.ParseColor("#42A5F5"),
                    Color.ParseColor("#2196F3"),
                    Color.ParseColor("#1E88E5"),
                    Color.ParseColor("#1976D2"),
                    Color.ParseColor("#1565C0"),
                    Color.ParseColor("#0D47A1")
            },
            new int[]{
                    Color.ParseColor("#E1F5FE"),
                    Color.ParseColor("#B3E5FC"),
                    Color.ParseColor("#81D4FA"),
                    Color.ParseColor("#4FC3F7"),
                    Color.ParseColor("#29B6F6"),
                    Color.ParseColor("#03A9F4"),
                    Color.ParseColor("#039BE5"),
                    Color.ParseColor("#0288D1"),
                    Color.ParseColor("#0277BD"),
                    Color.ParseColor("#01579B")
            },
            new int[]{
                    Color.ParseColor("#E0F7FA"),
                    Color.ParseColor("#B2EBF2"),
                    Color.ParseColor("#80DEEA"),
                    Color.ParseColor("#4DD0E1"),
                    Color.ParseColor("#26C6DA"),
                    Color.ParseColor("#00BCD4"),
                    Color.ParseColor("#00ACC1"),
                    Color.ParseColor("#0097A7"),
                    Color.ParseColor("#00838F"),
                    Color.ParseColor("#006064")
            },
            new int[]{
                    Color.ParseColor("#E0F2F1"),
                    Color.ParseColor("#B2DFDB"),
                    Color.ParseColor("#80CBC4"),
                    Color.ParseColor("#4DB6AC"),
                    Color.ParseColor("#26A69A"),
                    Color.ParseColor("#009688"),
                    Color.ParseColor("#00897B"),
                    Color.ParseColor("#00796B"),
                    Color.ParseColor("#00695C"),
                    Color.ParseColor("#004D40")
            },
            new int[]{
                    Color.ParseColor("#E8F5E9"),
                    Color.ParseColor("#C8E6C9"),
                    Color.ParseColor("#A5D6A7"),
                    Color.ParseColor("#81C784"),
                    Color.ParseColor("#66BB6A"),
                    Color.ParseColor("#4CAF50"),
                    Color.ParseColor("#43A047"),
                    Color.ParseColor("#388E3C"),
                    Color.ParseColor("#2E7D32"),
                    Color.ParseColor("#1B5E20")
            },
            new int[]{
                    Color.ParseColor("#F1F8E9"),
                    Color.ParseColor("#DCEDC8"),
                    Color.ParseColor("#C5E1A5"),
                    Color.ParseColor("#AED581"),
                    Color.ParseColor("#9CCC65"),
                    Color.ParseColor("#8BC34A"),
                    Color.ParseColor("#7CB342"),
                    Color.ParseColor("#689F38"),
                    Color.ParseColor("#558B2F"),
                    Color.ParseColor("#33691E")
            },
            new int[]{
                    Color.ParseColor("#F9FBE7"),
                    Color.ParseColor("#F0F4C3"),
                    Color.ParseColor("#E6EE9C"),
                    Color.ParseColor("#DCE775"),
                    Color.ParseColor("#D4E157"),
                    Color.ParseColor("#CDDC39"),
                    Color.ParseColor("#C0CA33"),
                    Color.ParseColor("#AFB42B"),
                    Color.ParseColor("#9E9D24"),
                    Color.ParseColor("#827717")
            },
            new int[]{
                    Color.ParseColor("#FFFDE7"),
                    Color.ParseColor("#FFF9C4"),
                    Color.ParseColor("#FFF59D"),
                    Color.ParseColor("#FFF176"),
                    Color.ParseColor("#FFEE58"),
                    Color.ParseColor("#FFEB3B"),
                    Color.ParseColor("#FDD835"),
                    Color.ParseColor("#FBC02D"),
                    Color.ParseColor("#F9A825"),
                    Color.ParseColor("#F57F17")
            },
            new int[]{
                    Color.ParseColor("#FFF8E1"),
                    Color.ParseColor("#FFECB3"),
                    Color.ParseColor("#FFE082"),
                    Color.ParseColor("#FFD54F"),
                    Color.ParseColor("#FFCA28"),
                    Color.ParseColor("#FFC107"),
                    Color.ParseColor("#FFB300"),
                    Color.ParseColor("#FFA000"),
                    Color.ParseColor("#FF8F00"),
                    Color.ParseColor("#FF6F00")
            },
            new int[]{
                    Color.ParseColor("#FFF3E0"),
                    Color.ParseColor("#FFE0B2"),
                    Color.ParseColor("#FFCC80"),
                    Color.ParseColor("#FFB74D"),
                    Color.ParseColor("#FFA726"),
                    Color.ParseColor("#FF9800"),
                    Color.ParseColor("#FB8C00"),
                    Color.ParseColor("#F57C00"),
                    Color.ParseColor("#EF6C00"),
                    Color.ParseColor("#E65100")
            },
            new int[]{
                    Color.ParseColor("#FBE9E7"),
                    Color.ParseColor("#FFCCBC"),
                    Color.ParseColor("#FFAB91"),
                    Color.ParseColor("#FF8A65"),
                    Color.ParseColor("#FF7043"),
                    Color.ParseColor("#FF5722"),
                    Color.ParseColor("#F4511E"),
                    Color.ParseColor("#E64A19"),
                    Color.ParseColor("#D84315"),
                    Color.ParseColor("#BF360C")
            },
            new int[]{
                    Color.ParseColor("#EFEBE9"),
                    Color.ParseColor("#D7CCC8"),
                    Color.ParseColor("#BCAAA4"),
                    Color.ParseColor("#A1887F"),
                    Color.ParseColor("#8D6E63"),
                    Color.ParseColor("#795548"),
                    Color.ParseColor("#6D4C41"),
                    Color.ParseColor("#5D4037"),
                    Color.ParseColor("#4E342E"),
                    Color.ParseColor("#3E2723")
            },
            new int[]{
                    Color.ParseColor("#FAFAFA"),
                    Color.ParseColor("#F5F5F5"),
                    Color.ParseColor("#EEEEEE"),
                    Color.ParseColor("#E0E0E0"),
                    Color.ParseColor("#BDBDBD"),
                    Color.ParseColor("#9E9E9E"),
                    Color.ParseColor("#757575"),
                    Color.ParseColor("#616161"),
                    Color.ParseColor("#424242"),
                    Color.ParseColor("#212121")
            },
            new int[]{
                    Color.ParseColor("#ECEFF1"),
                    Color.ParseColor("#CFD8DC"),
                    Color.ParseColor("#B0BEC5"),
                    Color.ParseColor("#90A4AE"),
                    Color.ParseColor("#78909C"),
                    Color.ParseColor("#607D8B"),
                    Color.ParseColor("#546E7A"),
                    Color.ParseColor("#455A64"),
                    Color.ParseColor("#37474F"),
                    Color.ParseColor("#263238")
            }
    };

    public static int[] ACCENT_COLORS = new int[]{
            Color.ParseColor("#FF1744"),
            Color.ParseColor("#F50057"),
            Color.ParseColor("#D500F9"),
            Color.ParseColor("#651FFF"),
            Color.ParseColor("#3D5AFE"),
            Color.ParseColor("#2979FF"),
            Color.ParseColor("#00B0FF"),
            Color.ParseColor("#00E5FF"),
            Color.ParseColor("#1DE9B6"),
            Color.ParseColor("#00E676"),
            Color.ParseColor("#76FF03"),
            Color.ParseColor("#C6FF00"),
            Color.ParseColor("#FFEA00"),
            Color.ParseColor("#FFC400"),
            Color.ParseColor("#FF9100"),
            Color.ParseColor("#FF3D00")
    };

    public static int[][] ACCENT_COLORS_SUB = new int[][]{
            new int[]{
                    Color.ParseColor("#FF8A80"),
                    Color.ParseColor("#FF5252"),
                    Color.ParseColor("#FF1744"),
                    Color.ParseColor("#D50000")
            },
            new int[]{
                    Color.ParseColor("#FF80AB"),
                    Color.ParseColor("#FF4081"),
                    Color.ParseColor("#F50057"),
                    Color.ParseColor("#C51162")
            },
            new int[]{
                    Color.ParseColor("#EA80FC"),
                    Color.ParseColor("#E040FB"),
                    Color.ParseColor("#D500F9"),
                    Color.ParseColor("#AA00FF")
            },
            new int[]{
                    Color.ParseColor("#B388FF"),
                    Color.ParseColor("#7C4DFF"),
                    Color.ParseColor("#651FFF"),
                    Color.ParseColor("#6200EA")
            },
            new int[]{
                    Color.ParseColor("#8C9EFF"),
                    Color.ParseColor("#536DFE"),
                    Color.ParseColor("#3D5AFE"),
                    Color.ParseColor("#304FFE")
            },
            new int[]{
                    Color.ParseColor("#82B1FF"),
                    Color.ParseColor("#448AFF"),
                    Color.ParseColor("#2979FF"),
                    Color.ParseColor("#2962FF")
            },
            new int[]{
                    Color.ParseColor("#80D8FF"),
                    Color.ParseColor("#40C4FF"),
                    Color.ParseColor("#00B0FF"),
                    Color.ParseColor("#0091EA")
            },
            new int[]{
                    Color.ParseColor("#84FFFF"),
                    Color.ParseColor("#18FFFF"),
                    Color.ParseColor("#00E5FF"),
                    Color.ParseColor("#00B8D4")
            },
            new int[]{
                    Color.ParseColor("#A7FFEB"),
                    Color.ParseColor("#64FFDA"),
                    Color.ParseColor("#1DE9B6"),
                    Color.ParseColor("#00BFA5")
            },
            new int[]{
                    Color.ParseColor("#B9F6CA"),
                    Color.ParseColor("#69F0AE"),
                    Color.ParseColor("#00E676"),
                    Color.ParseColor("#00C853")
            },
            new int[]{
                    Color.ParseColor("#CCFF90"),
                    Color.ParseColor("#B2FF59"),
                    Color.ParseColor("#76FF03"),
                    Color.ParseColor("#64DD17")
            },
            new int[]{
                    Color.ParseColor("#F4FF81"),
                    Color.ParseColor("#EEFF41"),
                    Color.ParseColor("#C6FF00"),
                    Color.ParseColor("#AEEA00")
            },
            new int[]{
                    Color.ParseColor("#FFFF8D"),
                    Color.ParseColor("#FFFF00"),
                    Color.ParseColor("#FFEA00"),
                    Color.ParseColor("#FFD600")
            },
            new int[]{
                    Color.ParseColor("#FFE57F"),
                    Color.ParseColor("#FFD740"),
                    Color.ParseColor("#FFC400"),
                    Color.ParseColor("#FFAB00")
            },
            new int[]{
                    Color.ParseColor("#FFD180"),
                    Color.ParseColor("#FFAB40"),
                    Color.ParseColor("#FF9100"),
                    Color.ParseColor("#FF6D00")
            },
            new int[]{
                    Color.ParseColor("#FF9E80"),
                    Color.ParseColor("#FF6E40"),
                    Color.ParseColor("#FF3D00"),
                    Color.ParseColor("#DD2C00")
            }
    };
    }
}