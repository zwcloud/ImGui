using System.IO;
using ImGui.OSImplementation;
using Typography.OpenFont;
using Xunit;
using Xunit.Abstractions;

namespace ImGui.UnitTest
{
    public class GlyphFacts
    {
        public class GlyphPointsFacts
        {
            private readonly ITestOutputHelper o;
            public GlyphPointsFacts(ITestOutputHelper output)
            {
                o = output;
            }

            private const string expectedGlyph_DroidSans_D = @"[GlyphPoints]
1276, 745
1276, 560
1180, 281
998, 94
733, 0
565, 0
199, 0
199, 1462
606, 1462
759, 1462
1007, 1370
1181, 1189
1276, 921
1079, 739
1079, 885
1013, 1098
887, 1236
703, 1303
586, 1303
385, 1303
385, 160
547, 160
811, 160
1079, 452
[EndPoints]
12
23";


            private const string expectedGlyph_msjh_D = @"[GlyphPoints]
205, 0
205, 1549
625, 1549
1442, 1549
1442, 794
1442, 432
1002, 0
609, 0
375, 1393
375, 156
611, 156
920, 156
1264, 488
1264, 789
1264, 1393
621, 1393
[EndPoints]
7
15";

            private const string expectedGlyph_msjh_啊 = @"[GlyphPoints]
1923, 1540
1782, 1540
1782, 14
1782, -23
1712, -92
1614, -104
1534, -104
1481, -104
1411, -100
1368, 10
1439, 0
1528, 0
1640, 0
1681, 21
1681, 43
1681, 1540
1231, 1540
1231, 1640
1923, 1640
1149, 1540
1149, 1501
1040, 1034
1136, 801
1136, 609
1136, 450
1016, 363
932, 363
914, 363
897, 365
852, 453
852, -102
750, -102
750, 1640
1149, 1640
1044, 1540
852, 1540
852, 465
970, 456
1036, 512
1036, 608
1036, 823
936, 1026
1011, 1319
606, 137
508, 137
508, 276
299, 276
299, 113
199, 113
199, 1550
606, 1550
508, 375
508, 1446
299, 1446
299, 375
1575, 387
1243, 387
1243, 1270
1575, 1270
1473, 492
1473, 1167
1341, 1167
1341, 492
[EndPoints]
18
33
42
50
54
58
62";
            private const string expectedGlyph_msjh_あ = @"[GlyphPoints]
1270, 991
1517, 955
1798, 678
1798, 450
1798, 31
1221, -61
1182, 31
1482, 88
1697, 305
1697, 459
1697, 609
1500, 851
1251, 885
1154, 491
973, 272
1003, 222
1055, 166
969, 98
895, 190
687, -6
517, -6
405, -6
272, 153
272, 301
272, 485
518, 805
721, 899
725, 1198
562, 1186
369, 1180
354, 1290
621, 1290
725, 1305
733, 1545
745, 1655
848, 1638
835, 1427
834, 1319
1095, 1345
1481, 1427
1491, 1321
1107, 1239
831, 1214
828, 1141
828, 1080
828, 1019
831, 946
1032, 996
1159, 993
1176, 1066
1184, 1143
1286, 1124
1281, 1070
1137, 889
995, 886
836, 838
833, 589
920, 377
1065, 585
840, 283
737, 488
727, 780
561, 689
376, 432
376, 298
376, 199
466, 96
525, 96
579, 96
778, 220
[EndPoints]
52
58
69";
            private const string expectedGlyph_msjh_아 = @"[GlyphPoints]
179, 0
179, 1510
1248, 1510
1248, 0
299, 120
1129, 120
1129, 1390
299, 1390
[EndPoints]
3
7";
            
            [Theory]
            [InlineData("DroidSans.ttf", 'D', expectedGlyph_DroidSans_D)]
            [InlineData("msjh.ttf", 'D',  expectedGlyph_msjh_D)]
            [InlineData("msjh.ttf", '啊', expectedGlyph_msjh_啊)]
            [InlineData("msjh.ttf", 'あ', expectedGlyph_msjh_あ)]
            [InlineData("msjh.ttf", '아', expectedGlyph_msjh_아)]
            public void GetGlyphPoints(string fontFile, char character, string expectedGlyph)
            {
                Typeface typeFace;
                using (var fs = Utility.ReadFile(Utility.FontDir + fontFile))
                {
                    var reader = new OpenFontReader();
                    typeFace = reader.Read(fs);
                }

                Glyph glyph = typeFace.Lookup(character);

                //this.o.WriteLine(glyph.Dump());//used to generate expected glyph data

                StringReader expectedGlyphReader = new StringReader(expectedGlyph);
                var lineGlyphPoints = expectedGlyphReader.ReadLine();
                Assert.Equal("[GlyphPoints]", lineGlyphPoints);

                var glyphPoints = glyph.GlyphPoints;
                foreach(var p in glyphPoints)
                {
                    var line = expectedGlyphReader.ReadLine();
                    Assert.NotNull(line);
                    var result = line.Split(',');
                    var x = int.Parse(result[0]);
                    var y = int.Parse(result[1]);
                    Assert.Equal(x, p.X);
                    Assert.Equal(y, p.Y);
                }
                
                var lineEndPoints = expectedGlyphReader.ReadLine();
                Assert.Equal("[EndPoints]", lineEndPoints);
                var endPoints = glyph.EndPoints;
                foreach (var end in endPoints)
                {
                    var line = expectedGlyphReader.ReadLine();
                    Assert.NotNull(line);
                    var expectedEnd = int.Parse(line);
                    Assert.Equal(expectedEnd, end);
                }
            }

        }
    }
}
